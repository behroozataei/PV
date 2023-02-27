using System;
using System.Data;

namespace UpgradeHelpers.DB
{

    /// <summary>
    /// It simulates a VB6 Field, contains the Value and FieldMetadata
    /// </summary>
    public class FieldHelper
    {

        // The Field Helper can have two modes: 1. Bound to a RecordsetHelper with the properties rs and columnReference or as metadata with the property dataColumn:
        protected internal enum FieldHelperMode
        {
            RecordsetBound = 0,
            Metadata = 1
        }

        protected internal FieldHelperMode mode;

        // 1 - Bound mode properties
        protected internal RecordSetHelper rs;
        protected internal object columnReference;
        // 2 - Metadata mode properties
        protected internal DataColumn dataColumn;

        /// <summary>
        /// Constructor for recordset-bound mode
        /// </summary>
        /// <param name="rs">The recordset for this Field.</param>
        /// <param name="columnReference">The column index or column string to get the Field in the recordset.</param>
        public FieldHelper(RecordSetHelper rs, object columnReference)
        {
            this.rs = rs;
            this.columnReference = columnReference;
            this.dataColumn = null;
            mode = FieldHelperMode.RecordsetBound;
        }

        /// <summary>
        /// Constructor for metadata mode
        /// </summary>
        /// <param name="dataColumn">The DataColumn object that contains the field metadata.</param>
        public FieldHelper(DataColumn dataColumn)
        {
            this.rs = null;
            this.columnReference = null;
            this.dataColumn = dataColumn;
            mode = FieldHelperMode.Metadata;
        }

        /// <summary>
        /// Constructor for metadata mode with no parameters
        /// </summary>
        public FieldHelper()
            : this (new DataColumn())
        {
        }

        /// <summary>
        /// Value for this Field
        /// </summary>
        public virtual object Value
        {
            get
            {
                if (mode == FieldHelperMode.Metadata)
                    throw new Exception("Invalid operation on a Field that is not bound to a Table");
                return rs[columnReference];
            }
            set
            {
                if (mode == FieldHelperMode.Metadata)
                    throw new Exception("Invalid operation on a Field that is not bound to a Table");
                rs[columnReference] = value;
            }
        }

        /// <summary>
        /// Metadata for this Field
        /// </summary>
        public virtual DataColumn FieldMetadata
        {
            get
            {
                if (mode == FieldHelperMode.Metadata)
                    return dataColumn; 
                else
                    return rs.GetFieldMetadata(columnReference);
            }
        }
        
        /// <summary>
        /// Returns a string representation of the field's value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
	    {
	        if (!rs.EOF)
	        {
	            return Convert.ToString(this.Value);
	        }
	        else
	        {
	            return string.Empty;
	        }
        }

    }
}
