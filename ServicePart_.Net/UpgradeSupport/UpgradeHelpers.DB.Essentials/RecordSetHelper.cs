using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace UpgradeHelpers.DB
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class RecordSetHelper : DataSet, ISerializable
    {

        #region Fields Declaration

        protected const string TITLE_DIALOG_RecordSetError = "RecordSet error";
        protected const string NotAllowedOperation = "Operation is not allowed when the object is closed.";

        protected DbConnection _activeConnection;

        /// <summary> Auto increment column name </summary>
        protected string _autoIncrementCol = String.Empty;

        protected bool _cachingAdapter;

        /// <summary>
        /// Contains the FieldsHelper associated to this RecordSetHelper
        /// </summary>
        protected FieldsHelper _fields = null;

        /// <summary> Actual Connection State </summary>
        protected ConnectionState _connectionStateAtEntry = ConnectionState.Closed;

        /// <summary> Connection String </summary>
        protected String _connectionString = string.Empty;

        protected Dictionary<KeyValuePair<DbConnection, string>, DbDataAdapter> _dataAdaptersCached = new Dictionary<KeyValuePair<DbConnection, string>, DbDataAdapter>();

        /// <summary> Holds default values for each column </summary>
        protected List<KeyValuePair<bool, object>> _defaultValues;

        /// <summary> Internal variable added to indicate that the recordset is disconnected </summary>
        protected bool _disconnected;

        /// <summary> Edit Mode </summary>
        protected EditModeEnum _editMode = EditModeEnum.EditNone;

        /// <summary> Is filtered? </summary>
        protected bool _filtered;

        /// <summary> True when no changes have been made to the table </summary>
        protected bool _firstChange = true;

        /// <summary> True when the helper is deserialized </summary>
        protected bool _isDeserialized;

        protected bool _requiresDefaultValues;

        protected bool _isDefaultSerializationInProgress;

        /// <summary> Has auto increment columns </summary>
        protected bool _hasAutoincrementCols;

        /// <summary> Operation finished state </summary>
        protected bool _operationFinished;

        /// <summary> String for delete query </summary>
        protected String _sqlDeleteQuery = string.Empty;

        /// <summary> String for insert query </summary>
        protected String _sqlInsertQuery = string.Empty;

        /// <summary> String for update query </summary>
        protected String _sqlUpdateQuery = string.Empty;

        /// <summary>
        ///     actual filter object
        /// </summary>
        protected object filter;

        private const long NTEXT_MAX_LENGTH = 1073741823; // 2^30, max length of nText column type in MSSQL

        /// <summary>
        ///     Flag that indicates if the current recordset is a cloned one
        /// </summary>
        protected bool _isClone;

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public RecordSetHelper()
            : base()
        {
            LoadSchema = true;
            SchemaTable = new DataTable();
            SchemaTable.Columns.CollectionChanged += SchemaTable_CollectionChanged;
        }

        /// <summary>
        /// Add/remove columns from the main DataTable when columns are added or removed from the SchemaTable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SchemaTable_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            if (!IsSchemaBeingLoaded && e.Action == System.ComponentModel.CollectionChangeAction.Add || e.Action == System.ComponentModel.CollectionChangeAction.Remove)
            {
                DataTable dataTable = GetTable();
                DataColumn column = e.Element as DataColumn;

                if (e.Action == System.ComponentModel.CollectionChangeAction.Add && column != null && !dataTable.Columns.Contains(column.ColumnName))
                {
                    dataTable.Columns.Add(column.ColumnName, column.DataType);
                }
                else if (e.Action == System.ComponentModel.CollectionChangeAction.Remove && column != null && dataTable.Columns.Contains(column.ColumnName))
                {
                    dataTable.Columns.Remove(column.ColumnName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSetName"></param>
        public RecordSetHelper(string dataSetName) : base(dataSetName) 
        { 
        }

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RecordSetHelper(SerializationInfo info, StreamingContext context)
        {
            AdoNetHelper.DeserializeDataSet(this, (byte[])info.GetValue("_", typeof(byte[])));
            _opened = info.GetBoolean("Opened");
            _dbType = (DatabaseType)info.GetUInt16("DatabaseType");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_", AdoNetHelper.SerializeDataSet(this));
            info.AddValue("Opened", _opened);
            info.AddValue("DatabaseType", _dbType);
        }
        #endregion


        /// <summary>
        /// Internal variable added to indicate that the recordset is disconnected
        /// </summary>
        public bool Disconnected
        {
            get { return _disconnected; }
            set { _disconnected = value; }
        }

        protected abstract bool isBatchEnabled();

        /// <summary>
        /// Sets a new value for a specific index column.
        /// </summary>
        /// <param name="columnIndex">Index of the column to be updated.</param>
        /// <param name="value">New value for column.</param>
        public virtual void SetNewValue(int columnIndex, object value)
        {
            CurrentRow[columnIndex] = value;
        }

        /// <summary>
        /// Indexer to access a field by index or column name (resolved by reflection)
        /// </summary>
        public object this[object column]
        {
            get
            {
                if (DBUtils.IsNumericType(column.GetType()))
                    return this[Convert.ToInt32(column)];
                else if (Type.GetTypeCode(column.GetType()) == TypeCode.String)
                    return this[Convert.ToString(column)];
                else
                    throw new Exception("The key used to access a field could not be recognized as either of a number (index access) or a string (column name access)");
            }
            set
            {
                if (DBUtils.IsNumericType(column.GetType()))
                    this[Convert.ToInt32(column)] = value;
                else if (Type.GetTypeCode(column.GetType()) == TypeCode.String)
                    this[Convert.ToString(column)] = value;
                else
                    throw new Exception("The key used to access a field could not be recognized as either of a number (index access) or a string (column name access)");
            }
        }

        /// <summary>
        /// Indexer to access a field by index
        /// </summary>
        public object this[int columnIndex]
        {
            get
            {
                return CurrentRow[columnIndex];
            }
            set
            {
                SetNewValue(columnIndex, value);
                if (!isBatchEnabled() || _editMode != EditModeEnum.EditNone)
                {
                    _editMode = EditModeEnum.EditInProgress;
                }
                _firstChange = !_firstChange && _firstChange;
            }
        }

        /// <summary>
        /// Indexer to access a field by column name
        /// </summary>
        public object this[String columnName]
        {
            get
            {
                return CurrentRow[columnName];
            }
            set
            {
                int columnIndex = GetColumnIndexByName(columnName);
                if (columnIndex > -1)
                {
                    SetNewValue(columnIndex, value);
                }
                else
                {
                    throw new Exception(string.Format("Column {0} not found", columnName));
                }

                //end of base class definition
                if (!isBatchEnabled() || _editMode != EditModeEnum.EditNone)
                {
                    _editMode = EditModeEnum.EditInProgress;
                }
                _firstChange = !_firstChange && _firstChange;
            }
        }

        /// <summary>
        /// Returns a FieldsHelper representing the collection of fields from this RecordsetHelper
        /// </summary>
        public virtual FieldsHelper Fields
        {
            get
            {
                if (_fields == null)
                    _fields = new FieldsHelper(this);
                return _fields;
            }
        }

        /// <summary>
        ///     Gets a DataColumnCollection object that contains the information of all columns on the RecordsetHelper.
        /// </summary>
        public virtual DataColumnCollection FieldsMetadata
        {
            get
            {
                if (Tables.Count <= 0)
                {
                    Tables.Add();
                }
                DataTable table = SchemaTable ?? GetTable();
                return table.Columns;
            }
        }

        /// <summary>
        ///     Gets a DataColumn object that contains the information of the specified column
        /// </summary>
        public virtual DataColumn GetFieldMetadata(object columnReference)
        {
            if (DBUtils.IsNumericType(columnReference.GetType()))
                return FieldsMetadata[Convert.ToInt32(columnReference)];
            else
                return FieldsMetadata[Convert.ToString(columnReference)];
        }

        EventHandler _beforeMove = null;
        /// <summary>
        /// 
        /// </summary>
        public virtual EventHandler BeforeMove
        {
            get
            {
                return _beforeMove;
            }
            set
            {
                _beforeMove = value;
            }
        }
        private EventHandler _newRecord = null;
        /// <summary>
        /// Fires event when a new record is been created.
        /// </summary>
        public virtual EventHandler NewRecord
        {
            get
            {
                return _newRecord;
            }
            set
            {
                _newRecord = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract RecordSetHelper CreateInstance();
        /// <summary>
        /// open state
        /// </summary>
        internal bool _opened = false;
        /// <summary>
        /// Indicates if this RecordsetHelper have been open.
        /// </summary>
        public bool Opened
        {
            get
            {
                return _opened;
            }
            set
            {
                _opened = value;
            }
        }
        /// <summary>
        /// string for select query
        /// </summary>
        internal String _sqlSelectQuery = string.Empty;
        /// <summary>
        /// Gets or sets the SQL query used for select operations in this RecordsetHelper.
        /// </summary>
        public String SqlQuery
        {
            get
            {
                return _sqlSelectQuery;
            }
            set
            {
                _sqlSelectQuery = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract DbConnection ActiveConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public abstract string ConnectionString { get; set; }
        internal DbProviderFactory _providerFactory;
        /// <summary>
        /// Gets or sets the DBProviderFactory to be use by this object.
        /// </summary>
        public DbProviderFactory ProviderFactory
        {
            get
            {
                return _providerFactory;
            }
            set
            {
                _providerFactory = value;
            }
        }
        /// <summary>
        ///     Infers the command type from an sql string getting the schema metadata from the database.
        /// </summary>
        /// <param name="sql">The sql string to be analyzed</param>
        public CommandType getCommandType(String sql)
        {
            List<DbParameter> parameters;
            return getCommandType(sql, out parameters);
        }

        protected abstract CommandType getCommandType(String sqlCommand, out List<DbParameter> parameters);

        private EventHandler _afterMove;
        /// <summary>
        /// Handler for AfterMove Event
        /// </summary>
        public EventHandler AfterMove
        {
            get
            {
                return _afterMove;
            }
            set
            {
                _afterMove = value;
            }
        }
        private EventHandler _afterQuery;
        /// <summary>
        /// Handler for After Query Event
        /// </summary>
        public EventHandler AfterQuery
        {
            get
            {
                return _afterQuery;
            }
            set
            {
                _afterQuery = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// 
        /// </summary>
        public abstract int RecordCount { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract void MoveFirst();
        /// <summary>
        /// 
        /// </summary>
        public abstract bool BOF { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract void MovePrevious();

        /// <summary>
        /// Moves the position of the current record
        /// </summary>
        /// <param name="records">The number of records that the current record position moves</param>
        public abstract void Move(int records);

        /// <summary>
        /// Performs the move action after setting a filter
        /// </summary>
        protected abstract void MoveAfterFilter();

        /// <summary>
        ///     is end of file
        /// </summary>
        internal bool _eof = true;
        /// <summary>
        ///     Gets a bool value indicating if the current record is the last one in the RecordsetHelper object.
        /// </summary>
        public bool EOF
        {
            get
            {
                return _eof;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract void AddNew();
        /// <summary>
        /// 
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// 
        /// </summary>
        public abstract void MoveLast();
        /// <summary>
        /// 
        /// </summary>
        public abstract void MoveNext();
        /// <summary>
        /// 
        /// </summary>
        public abstract bool CanMovePrevious { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract void Requery();
        /// <summary>
        /// actual index
        /// </summary>
        internal int _index = -1;
        /// <summary>
        /// Gets or Sets the current Record position inside the RecordsetHelper.
        /// </summary>
        public int CurrentPosition
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract DataRow CurrentRow { get; }

        /// <summary>
        /// Pointer to CurrentRecordset when there are multiple recordset request or response;
        /// </summary>
        public abstract int CurrentRecordSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Open();
        /// <summary>
        /// 
        /// </summary>
        public abstract bool CanMoveNext { get; }

        /// <summary>
        ///     actual object source
        /// </summary>
        internal Object _source;
        /// <summary>
        ///     Sets or gets the source to obtain the necessary queries. Can be DBCommand or String.
        /// </summary>
        public Object Source
        {
            get
            {
                return _source;
            }
            set
            {
                DbCommand command = value as DbCommand;
                if (command != null)
                {
                    if (command.Connection != null && ActiveConnection != command.Connection)
                    {
                        ActiveConnection = command.Connection;
                    }
                    _activeCommand = command;
                }
                else
                {
                    string s = value as string;
                    if (s != null)
                    {
                        List<DbParameter> parameters;
                        CommandType commandType = getCommandType(s, out parameters);
                        _activeCommand = CreateCommand(s, commandType, parameters);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type for the Source property");
                    }
                }
                _source = value;
            }
        }
        private bool _allowEdit = true;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool AllowEdit
        {
            get
            {
                return _allowEdit;
            }
            set
            {
                _allowEdit = value;
            }
        }

        private bool _allowNew = true;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool AllowNew
        {
            get
            {
                return _allowNew;
            }
            set
            {
                _allowNew = value;
            }
        }

        private bool _allowDelete = true;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool AllowDelete
        {
            get
            {
                return _allowDelete;
            }
            set
            {
                _allowDelete = value;
            }
        }

        /// <summary>
        /// Clone a command
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        protected DbCommand CloneCommand(DbCommand dbCommand)
        {
            if (dbCommand != null)
            {
                DbCommand res = ProviderFactory.CreateCommand();
                if (res != null)
                {
                    res.CommandText = dbCommand.CommandText;
                    res.CommandTimeout = dbCommand.CommandTimeout;
                    res.CommandType = dbCommand.CommandType;
                    res.Connection = dbCommand.Connection;
                    res.Transaction = dbCommand.Transaction;

                    foreach (DbParameter param in dbCommand.Parameters)
                    {
                        DbParameter newParam = res.CreateParameter();
                        newParam.DbType = param.DbType;
                        newParam.Direction = param.Direction;
                        newParam.ParameterName = param.ParameterName;
                        newParam.Size = param.Size;
                        newParam.SourceColumn = param.SourceColumn;
                        newParam.SourceColumnNullMapping = param.SourceColumnNullMapping;
                        newParam.SourceVersion = param.SourceVersion;
                        newParam.Value = param.Value;

                        res.Parameters.Add(newParam);
                    }

                    return res;
                }
            }
            return null;
        }

        /// <summary>
        ///  Send to DB query to compute.
        /// </summary>
        /// <param name="expression">The query to compute</param>
        /// <returns>The value computed</returns>
        protected object ComputeValue(string expression)
        {
            object result;
            using (DbCommand cmd = ActiveConnection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select " + expression;
                cmd.Transaction = TransactionManager.GetCurrentTransaction(ActiveConnection);
                result = cmd.ExecuteScalar();
            }
            return result;
        }

        /// <summary>
        /// Creates a DBCommand object using de provided parameters.
        /// </summary>
        /// <param name="commandText">A string containing the SQL query.</param>
        /// <param name="commandType">The desire type for the command.</param>
        /// <returns>A new DBCommand object containing the SLQ code received has parameter.</returns>
        public DbCommand CreateCommand(String commandText, CommandType commandType)
        {
            return CreateCommand(commandText, commandType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected abstract DbCommand CreateCommand(String commandText, CommandType commandType, List<DbParameter> parameters);

        /// <summary>
        /// Creates a Dbparameter obtaining the information from a DataColumn object.
        /// </summary>
        /// <param name="paramName">The name for the parameter.</param>
        /// <param name="dColumn">The DataColumn object to extract the information from.</param>
        /// <returns>A new DBParameter object containing the desired configuration.</returns>
        protected DbParameter CreateParameterFromColumn(string paramName, DataColumn dColumn)
        {
            DbParameter parameter = ProviderFactory.CreateParameter();
            if (parameter != null)
            {
                parameter.ParameterName = paramName;
                parameter.DbType = GetDBType(dColumn.DataType);
                parameter.SourceColumn = dColumn.ColumnName;
                parameter.SourceVersion = DataRowVersion.Current;
                return parameter;
            }
            return null;
        }

        private DatabaseType _dbType;
        /// <summary>
        /// Gets or sets the DatabaseType being use by this object. 
        /// </summary>
        public virtual DatabaseType DatabaseType
        {
            get
            {
                return _dbType;
            }
            set
            {
                _dbType = value;
            }
        }

        private bool _disableEventsWhileDeleting = false;
        /// <summary>
        /// This flag is used to stop the propagation of events while performing a delete.
        /// It was found that deleting a DataRow raised several events on the binding source
        /// and these events update the current row which must remain the same until the update logic is executed
        /// </summary>
        public bool disableEventsWhileDeleting
        {
            get
            {
                return _disableEventsWhileDeleting;
            }
            set
            {
                _disableEventsWhileDeleting = value;
            }
        }

        /// <summary>
        ///     Gets or sets the row value at �ColumnName� index.
        /// </summary>
        /// <param name="columnReference">Reference to the desired column.</param>
        /// <returns>The value at the given index.</returns>
        public FieldHelper GetField(object columnReference)
        {
            return new FieldHelper(this, columnReference);
        }
        
        /// <summary>
        /// Looks for a column with the given name and returns the column index
        /// or -1 if not found
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetColumnIndexByName(String columnName)
        {
            if (UsingView)
            {
                return _currentView.Table.Columns.IndexOf(columnName);
            }
            if (Tables.Count > 0)
            {
                return Tables[0].Columns.IndexOf(columnName);
            }
            return -1;
        }

        /// <summary>
        /// current view
        /// </summary>
        private DataView _currentView;

        internal DataView CurrentView
        {
            get 
            {
                if (_currentView == null && Tables.Count > 0)
                {
                    _currentView = Tables[0].DefaultView;
                }
                return _currentView;
            }
            set
            {
                _currentView = value;
            }
        }

        /// <summary>
        /// Property used to determine if the data needs to be get from a dataview or the table directly
        /// </summary>
        protected abstract bool UsingView
        {
            get;
        }

        /// <summary>
        /// Gets a value that indicates whether the named column contains a null value.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>true if the column contains a null value; otherwise, false.</returns>
        public bool IsNull(string columnName)
        {
            if (CurrentRow == null)
            {
                throw new InvalidOperationException("No current row selected.");
            }
            return CurrentRow.IsNull(columnName);
        }

        /// <summary>
        /// This is property is used when the record set uses more than one table,
        /// uses CommandBuilder, primary keys or, any other metadata information.
        /// </summary>
        public static bool LoadSchema
        {
            get; 
            set;
        }

        /// <summary>
        /// DataTable to hold the schema separated from the DataTable that contains data in order to avoid validations to happen as rows are edited/added.
        /// </summary>
        public DataTable SchemaTable
        {
            get;
            set;
        }

        /// <summary>
        /// Used to signal to load only the schema and not fill any data, useful to retrieve meta information
        /// </summary>
        public bool LoadSchemaOnly
        {
            get; set;
        }

        protected bool IsSchemaBeingLoaded 
        {
            get; set; 
        }

        /// <summary>
        /// Sets the AfterMove EventHandler.
        /// </summary>
        internal void OnAfterMove()
        {
            if (AfterMove != null)
            {
                AfterMove(this, new EventArgs());
            }
        }

        /// <summary>
        ///     Sets the AfterQuery eventHandler.
        /// </summary>
        internal void OnAfterQuery()
        {
            if (AfterQuery != null)
            {
                AfterQuery(this, new EventArgs());
            }
        }

        /// <summary>
        /// active command
        /// </summary>
        internal DbCommand _activeCommand;

        /// <summary>
        /// Returns a copy of the current ActiveCommand of this RecordsetHelper.
        /// </summary>
        /// <returns>A copy of the current ActiveCommand.</returns>
        public DbCommand CopySourceCommand()
        {
            if (_opened)
            {
                DbCommand result = ActiveConnection.CreateCommand();
                result.CommandText = _activeCommand.CommandText;
                result.CommandType = _activeCommand.CommandType;
                DbParameter[] paramArray = new DbParameter[_activeCommand.Parameters.Count];
                _activeCommand.Parameters.CopyTo(paramArray, 0);
                result.Parameters.AddRange(paramArray);
                return result;
            }
            throw new InvalidOperationException("The recordSet has to be opened to perform this operation");
        }

        /// <summary>
        /// new row state
        /// </summary>
        internal bool _newRow;
        /// <summary>
        /// Cancels any changes made to the current or new row of a ADORecordsetHelper object.
        /// </summary>
        internal void DoCancelUpdate()
        {
            DataRow theRow = CurrentRow;
            if (theRow.RowState != DataRowState.Unchanged)
            {
                theRow.RejectChanges();
                if (theRow.RowState == DataRowState.Detached && RecordCount > 0)
                {
                    _index = 0;
                }
            }
            _newRow = false;
        }

        /// <summary>
        /// Analyzes an SQL Query and obtain the name of the table.
        /// </summary>
        /// <param name="sqlSelectQuery">The SQL query containing the name of the table.</param>
        /// <param name="useParam"> When use the first table name in the query, by default is false.</param>
        /// <returns>The SQL query containing the name of the table.</returns>
        protected string getTableName(string sqlSelectQuery, bool useParam)
        {
            String query = _activeCommand.CommandText;
            if (!string.IsNullOrEmpty(query))
            {
                if (_activeCommand.CommandType == CommandType.Text)
                {
                    Match mtch;
                    if (useParam)
                        mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"FROM\s+([^ ,]+)(?:\s*,\s*([^ ,]+))*\s+", RegexOptions.IgnoreCase);
                    else
                        mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+(\w+)[ \t]*.*$", RegexOptions.IgnoreCase);

                    if (mtch != Match.Empty)
                    {
                        return mtch.Groups[1].Value.Trim();
                    }
                    if (useParam)
                    {
                        mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+(\w+)[ \t]*.*$", RegexOptions.IgnoreCase);
                        if (mtch != Match.Empty)
                            return mtch.Groups[1].Value.Trim();
                    }
                }
                else if (_activeCommand.CommandType == CommandType.TableDirect)
                {
                    return query;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Analyzes an SQL Query and obtain the name of the table.
        /// </summary>
        /// <param name="sqlSelectQuery">The SQL query containing the name of the table.</param>
        /// 
        /// <returns>The SQL query containing the name of the table.</returns>
        protected string getTableName(string sqlSelectQuery)
        {
            return getTableName(sqlSelectQuery, false);
        }

        /// <summary>
        ///     Sets the Filter to by applied to the this ADORecordsetHelper. (valid objects are: string, DataViewRowState and DataRow[]).
        /// </summary>
        public Object Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                if (Opened)
                {
                    if (filter == null)
                    {
                        CurrentView.RowFilter = null;
                        CurrentView.RowStateFilter = DataViewRowState.CurrentRows;
                        _filtered = false;
                    }
                    else if (filter is string)
                    {
                        SetFilter(filter as string);
                        _filtered = !string.IsNullOrEmpty(filter as string);
                    }
                    else if (filter is DataViewRowState)
                    {
                        SetFilter((DataViewRowState)filter);
                        _filtered = true;
                    }
                    else
                    {
                        throw new ArgumentException("Filter value not allowed");
                    }

                    //First reset the index and eof
                    //When the user filters, the current row goes to the
                    //first row if there is one.
                    //Also there might be no rows at all.
                    _index = -1;
                    _eof = IsEof();
                    if (RecordCount > 0)
                    {
                        MoveAfterFilter();
                    }
                }
            }
        }

        /// <summary>
        ///     Determines if we should be at the end of file (EOF) based on the current index.
        /// </summary>
        /// <returns>Returns true if based on the index variable EOF is true; otherwise false.</returns>
        protected bool IsEof()
        {
            bool isEof = _index < 0;
            if (UsingView)
            {
                isEof = (_index < 0) || (_index >= CurrentView.Count);
            }
            else if (Tables.Count > 0)
            {
                isEof = (_index >= Tables[CurrentRecordSet].Rows.Count);
            }
            return isEof;
        }

        /// <summary>
        ///     Sets the filter for the RecordsetHelper.
        /// </summary>
        /// <param name="filter">The filter to apply to this RecordsetHelper.</param>
        protected void SetFilter(String filter)
        {
            try
            {
                CurrentView.RowFilter = filter;
            }
            catch (EvaluateException)
            {
            }
        }

        /// <summary>
        ///     Sets the filter for the RecordsetHelper.
        /// </summary>
        /// <param name="filter">The filter to apply to this RecordsetHelper.</param>
        protected virtual void SetFilter(DataViewRowState filter)
        {
            CurrentView.RowStateFilter = filter;
        }

        /// <summary>
        ///     OleDb Row Updated event
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">Row updated event args</param>
        protected void RecordSetHelper_RowUpdatedOleDb(object sender, OleDbRowUpdatedEventArgs e)
        {
            //This behavior depends on the database we are interacting with
            if (e.StatementType == StatementType.Insert && e.Status == UpdateStatus.Continue)
            {
                string tablename = getTableName(_activeCommand.CommandText, false);
                Dictionary<String, String> identities = null;
                if (!LoadSchema && !LoadSchemaOnly)
                {
                    identities = IdentityColumnsManager.GetIndentityInformation(tablename);
                }
                else if (DatabaseType != DB.DatabaseType.Oracle)
                {
                    identities = new Dictionary<string, string>();
                    foreach (DataColumn col in SchemaTable.Columns)
                    {
                        if (col.AutoIncrement)
                        {
                            identities.Add(col.ColumnName, String.Empty);
                        }
                    }
                }

                if (identities != null)
                {
                    DbCommand oCmd = e.Command.Connection.CreateCommand();
                    oCmd.Transaction = e.Command.Transaction;
                    object lastIdentity = null;
                    foreach (KeyValuePair<String, String> identityInfo in identities)
                    {
                        switch (DatabaseType)
                        {
                            case DatabaseType.Oracle:
                                oCmd.CommandText = "Select " + identityInfo.Value + ".Currval from dual";
                                break;
                            case DatabaseType.SQLServer:
                                oCmd.CommandText = "SELECT @@IDENTITY";
                                break;
                            case DatabaseType.Access:
                                oCmd.CommandText = "SELECT @@IDENTITY";
                                break;
                        }
                        lastIdentity = oCmd.ExecuteScalar();
                        this.LastIdentity = Convert.IsDBNull(lastIdentity) ? -1 : Convert.ToInt32(lastIdentity);
                        e.Row[identityInfo.Key] = lastIdentity;
                    }
                    e.Row.AcceptChanges();
                }
            }
        }

        private int _lastIdentity;
        /// <summary>
        /// Returns the last identity value inserted into an identity column
        /// </summary>
        public int LastIdentity
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the schema of the RecordSetHelper. The schema is loaded in a separate table to avoid validations to happen as rows are added/edited.
        /// </summary>
        protected void FillSchema(DbDataAdapter dbAdapter)
        {
            try
            {
                IsSchemaBeingLoaded = true;
                dbAdapter.FillSchema(SchemaTable, SchemaType.Source);
                IsSchemaBeingLoaded = false;
            }
            catch
            {
            } //ignore errors as we can't get schema info with calls to stored procedures
        }

        /// <summary>
        ///     Creates an update command using the information contained in the RecordsetHelper.
        /// </summary>
        /// <returns>A DBCommand object containing an update command.</returns>
        protected DbCommand CreateUpdateCommandFromMetaData()
        {
            int i = 0, j = 0;
            DbCommand result = null;
            String tableName = getTableName(_activeCommand.CommandText, false);
            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    string updatePart = "";
                    string wherePart = "";
                    string sql = "";
                    List<DbParameter> listGeneral = new List<DbParameter>();
                    List<DbParameter> listWhere = new List<DbParameter>();

                    DataTable table = SchemaTable ?? GetTable();
                    foreach (DataColumn dColumn in table.Columns)
                    {
                        if (table.PrimaryKey != null && !(Array.Exists(table.PrimaryKey, delegate(DataColumn col)
                                {
                                    return col.ColumnName.Equals(dColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase);
                                }) || dColumn.ReadOnly || dColumn.AutoIncrement))
                        {
                            if (updatePart.Length > 0)
                            {
                                updatePart += " , ";
                            }

                            updatePart += string.Format("{0} = ?", AddSQLDelimiter(dColumn.ColumnName));
                            listGeneral.Add(CreateParameterFromColumn("p" + (++i), dColumn));
                        }
                        // Filter binary columns for wherePart
                        if (dColumn.DataType == typeof(Byte[]))
                            continue;
                        if (wherePart.Length > 0)
                        {
                            wherePart += " AND ";
                        }
                        DbParameter param;
                        if (dColumn.AllowDBNull)
                        {
                            wherePart += "((? = 1 AND " + AddSQLDelimiter(dColumn.ColumnName) + " IS NULL) OR (" + AddSQLDelimiter(dColumn.ColumnName);

                            if (IsNextDataType(dColumn))
                            {
                                wherePart += " Like ?))";
                            }
                            else
                            {
                                wherePart += " = ?))";
                            }

                            param = CreateParameterFromColumn("q" + (++j), dColumn);
                            param.DbType = DbType.Int32;
                            param.SourceVersion = DataRowVersion.Original;
                            param.SourceColumnNullMapping = true;
                            param.Value = 1;
                            listWhere.Add(param);
                            param = CreateParameterFromColumn("q" + (++j), dColumn);
                            param.SourceVersion = DataRowVersion.Original;
                            listWhere.Add(param);
                        }
                        else
                        {
                            wherePart += "(" + AddSQLDelimiter(dColumn.ColumnName) + " = ?)";
                            param = CreateParameterFromColumn("q" + (++j), dColumn);
                            param.SourceVersion = DataRowVersion.Original;
                            listWhere.Add(param);
                        }
                    }
                    listGeneral.AddRange(listWhere);
                    sql = "UPDATE " + AddSQLDelimiter(tableName) + " SET " + updatePart + " WHERE " + wherePart;
                    result = ProviderFactory.CreateCommand();
                    if (result != null)
                    {
                        result.CommandText = sql;
                        listGeneral.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
                        result.Connection = _activeCommand.Connection;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        ///     Creates a delete command using the information contained in the RecordsetHelper.
        /// </summary>
        /// <returns>A DBCommand object containing a delete command.</returns>
        protected DbCommand CreateDeleteCommandFromMetaData()
        {
            DbCommand result = null;
            String tableName = getTableName(_activeCommand.CommandText, false);
            int j = 0;
            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    string wherePart = "";
                    List<DbParameter> listGeneral = new List<DbParameter>();

                    DataTable table = SchemaTable ?? GetTable();
                    foreach (DataColumn dColumn in table.Columns)
                    {
                        if (wherePart.Length > 0)
                        {
                            wherePart += " AND ";
                        }

                        DbParameter pInfo;
                        if (dColumn.AllowDBNull)
                        {
                            wherePart += "((? = 1 AND " + AddSQLDelimiter(dColumn.ColumnName) + " IS NULL) OR (" + AddSQLDelimiter(dColumn.ColumnName) + " = ?))";

                            pInfo = CreateParameterFromColumn("p" + (++j), dColumn);
                            pInfo.DbType = DbType.Int32;
                            pInfo.SourceVersion = DataRowVersion.Original;
                            pInfo.SourceColumnNullMapping = true;
                            pInfo.Value = 1;
                            listGeneral.Add(pInfo);

                            pInfo = CreateParameterFromColumn("p" + (++j), dColumn);
                            pInfo.SourceVersion = DataRowVersion.Original;
                            listGeneral.Add(pInfo);
                        }
                        else
                        {
                            wherePart += "(" + AddSQLDelimiter(dColumn.ColumnName) + " = ?)";
                            pInfo = CreateParameterFromColumn("q" + (++j), dColumn);
                            pInfo.SourceVersion = DataRowVersion.Original;
                            listGeneral.Add(pInfo);
                        }
                    }
                    string sql = "DELETE FROM " + AddSQLDelimiter(tableName) + " WHERE (" + wherePart + ")";
                    result = ProviderFactory.CreateCommand();
                    if (result != null)
                    {
                        result.CommandText = sql;
                        listGeneral.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
                        result.Connection = _activeCommand.Connection;
                    }
                }
            }
            catch
            {
            }
            return result;
        }


        /// <summary>
        ///     Creates an insert command using the information contained in the RecordsetHelper.
        /// </summary>
        /// <returns>A DBCommand object containing an insert command.</returns>
        protected DbCommand CreateInsertCommandFromMetaData()
        {
            DbCommand result = null;
            int i = 0;
            String tableName = getTableName(_activeCommand.CommandText, false);
            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    string fieldsPart = "";
                    string valuesPart = "";
                    string sql;
                    DataTable table = SchemaTable ?? GetTable();
                    foreach (DataColumn dColumn in table.Columns)
                    {
                        if (!dColumn.ReadOnly)
                        {
                            if (fieldsPart.Length > 0)
                            {
                                fieldsPart += ", ";
                                valuesPart += ", ";
                            }

                            fieldsPart += AddSQLDelimiter(dColumn.ColumnName);
                            valuesPart += "?";
                            parameters.Add(CreateParameterFromColumn("p" + (++i), dColumn));
                        }
                    }
                    sql = "INSERT INTO " + AddSQLDelimiter(tableName) + " (" + fieldsPart + ") VALUES (" + valuesPart + ")";
                    result = ProviderFactory.CreateCommand();
                    if (result != null)
                    {
                        result.CommandText = sql;
                        parameters.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
                        result.Connection = _activeCommand.Connection;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        ///     Returns the table according to the status View/Table of the RecordSet
        /// </summary>
        public DataTable GetTable()
        {
            return (UsingView ? CurrentView.Table : Tables[CurrentRecordSet]);
        }

        /// <summary>
        /// To know if the type of the column is nText database type, the type has to be mapped to string
        /// and the max length of the type has to be NTEXT_MAX_LENGTH
        /// </summary>
        /// <param name="dColumn">The dataColumn that needs to be questioned</param>
        /// <returns>True if the type of dColumn is nText database type, false otherwise</returns>
        private bool IsNextDataType(DataColumn dColumn)
        {
            return (dColumn.DataType.Equals(typeof(string)) && dColumn.MaxLength == NTEXT_MAX_LENGTH);
        }

        private string AddSQLDelimiter(string identifier)
        {
            return String.Format("[{0}]", identifier);
        }

        /// <summary>
        ///     Clone a source ADORecordSetHelper through a target ADORecordSetHelper
        /// </summary>
        /// <param name="source">The source ADORecordSetHelper</param>
        /// <param name="target">The target ADORecordSetHelper</param>
        protected void CloneIt(RecordSetHelper source, RecordSetHelper target)
        {
            if (CurrentRow != null)
            {
                CurrentRow.EndEdit();
            }
            target.DatabaseType = source.DatabaseType;
            target.ProviderFactory = source.ProviderFactory;
            target._opened = true;
            target._isClone = true;
            target.ActiveConnection = source.ActiveConnection;
            target._activeCommand = source._activeCommand;
            target.CurrentRecordSet = source.CurrentRecordSet;
            if (source.Tables.Count > 0)
            {
                foreach (DataTable sourceTable in Tables)
                {
                    target.Tables.Add(sourceTable.Copy());
                }
                target.CurrentView = target.Tables[source.CurrentRecordSet].DefaultView;
            }
            target.Filter = source.Filter;
            if (target.CurrentView != null && target.CurrentView.Count > 0)
            {
                target._index = 0;
                target._eof = false;
            }
        }


        #region static methods

        /// <summary>
        ///     Converts from System.Type to DbType.
        /// </summary>
        /// <param name="type">The System.Type to be converted.</param>
        /// <returns>The equivalent DBType.</returns>
        public static DbType GetDBType(Type type)
        {
            DbType result = DbType.String;
            switch (type.Name)
            {
                case "Byte":
                    result = DbType.Byte;
                    break;
                case "Byte[]":
                    result = DbType.Binary;
                    break;
                case "Boolean":
                    result = DbType.Boolean;
                    break;
                case "DateTime":
                    result = DbType.DateTime;
                    break;
                case "Decimal":
                    result = DbType.Decimal;
                    break;
                case "Double":
                    result = DbType.Double;
                    break;
                case "Guid":
                    result = DbType.Guid;
                    break;
                case "Int16":
                    result = DbType.Int16;
                    break;
                case "Int32":
                    result = DbType.Int32;
                    break;
                case "Int64":
                    result = DbType.Int64;
                    break;
                case "Object":
                    result = DbType.Object;
                    break;
                case "SByte":
                    result = DbType.SByte;
                    break;
                case "Single":
                    result = DbType.Single;
                    break;
                case "String":
                    result = DbType.String;
                    break;
                case "UInt16":
                    result = DbType.UInt16;
                    break;
                case "UInt32":
                    result = DbType.UInt32;
                    break;
                case "UInt64":
                    result = DbType.UInt64;
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Turns the DB type string to corresponding CLR type string.
        /// </summary>
        /// <param name="strDbType"></param>
        /// <returns></returns>
        public static DbType MapToDbType(string strDbType)
        {
            switch (strDbType)
            {
                case "xml":
                    return DbType.Xml;

                case "nvarchar":
                case "varchar":
                case "sysname":
                case "nchar":
                case "char":
                case "ntext":
                case "text":
                    return DbType.String;

                case "int":
                    return DbType.Int32;

                case "bigint":
                    return DbType.Int64;

                case "bit":
                    return DbType.Boolean;

                case "long":
                    return DbType.Int32;

                case "real":
                case "float":
                    return DbType.Double;

                case "datetime":
                case "datetime2":
                case "date":
                    return DbType.DateTime;

                case "datetimeoffset":
                    return DbType.DateTimeOffset;

                case "time":
                case "timespan":
                    return DbType.Time;

                case "tinyint":
                    return DbType.Byte;

                case "smallint":
                    return DbType.Int16;

                case "uniqueidentifier":
                    return DbType.Guid;

                case "numeric":
                case "decimal":
                    return DbType.Decimal;

                case "binary":
                case "image":
                case "varbinary":
                    return DbType.Binary;

                case "sql_variant":
                    return DbType.Object;
            }
            throw new ArgumentException("Given DB type does not map to any known types.", "strDbType");
        }

        /// <summary>
        ///     Verifies if a parameter with the provided name exists on the command received, otherwise a new parameter using the specified name.
        /// </summary>
        /// <param name="command">The command object to look into.</param>
        /// <param name="name">The name of the parameter to look for.</param>
        /// <returns>The parameter named with �name�.</returns>
        public static DbParameter commandParameterBinding(DbCommand command, string name)
        {
            if (!command.Parameters.Contains(name))
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = name;
                command.Parameters.Add(parameter);
            }
            return command.Parameters[name];
        }

        #endregion

    }
}
