using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Data;

using Irisa.Logger;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;

namespace DCP
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly SqlServerDataManager _linkDBpcsDataManager;
        private readonly DataManager _staticDataManager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, DCPScadaPoint> _scadaPoints;
        private readonly Dictionary<string, DCPScadaPoint> _scadaPointsHelper;

        public Repository(ILogger logger, DataManager staticDataManager, DataManager historicalDataManager, SqlServerDataManager linkDBpcsDataManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _staticDataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _historicalDataManager = historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            _linkDBpcsDataManager = linkDBpcsDataManager ?? throw new ArgumentNullException(nameof(linkDBpcsDataManager));
            _scadaPoints = new Dictionary<Guid, DCPScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, DCPScadaPoint>();
        }

        public bool Build()
        {
            var isBuild = false;

            try
            {
                GetInputScadaPoints();
                isBuild = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return isBuild;
        }
        private static string GetEndStringCommand()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "app.";
               // return string.Empty;

            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "APP_";
            }

            return string.Empty;
        }

        private void GetInputScadaPoints()
        {
            try
            {
                var dataTable = _staticDataManager.GetRecord($"SELECT Name, NetworkPath, DirectionType from {GetEndStringCommand()}DCP_PARAMS");

                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = row["DirectionType"].ToString();
                    if (name == "MAC_DS")
                        System.Diagnostics.Debugger.Break();
                    var id = GetGuid(networkPath);
                    var scadaPoint = new DCPScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType));

                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }
                }
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading  scada point.", LogLevels.Error, ex);
            }
        }

        public DataTable GetEAFSPARAMS()
        {
            DataTable dataTable = null;

            try
            {
                string sql = $"select * from {GetEndStringCommand()}EFS_PARAMS";
                dataTable = _staticDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public DataTable GetFromMasterDB(string sql)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _staticDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public DataTable GetFromHistoricalDB(string sql)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _historicalDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            return dataTable;
        }

        public DataTable GetFromLinkDB(string sql)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _linkDBpcsDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            return dataTable;
        }

        public int GetMAxFKeyEndedFurnace(int oldMAxFKey)
        {
            try
            {
                string sql = $"SELECT F_Key, Furnace from dbo.T_EndedFurnace_Backup WHERE F_Key>'" + oldMAxFKey.ToString() + "' ORDER BY F_Key DESC";
                var dataTable = _linkDBpcsDataManager.GetRecord(sql);
                DataRow dr = dataTable.Rows[0];
                return Convert.ToInt32(dr["F_Key"].ToString());
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return 0;
        }

        public float GetPowerSumationFromT_EAFsPower(string sql)
        {
            try
            {
                var dataTable = _linkDBpcsDataManager.GetRecord(sql);
                DataRow dr = dataTable.Rows[0];
                return Convert.ToSingle(dr["Sumation"].ToString());
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return -1;
        }

        public bool WriteTimeNowToFurnace(int I)
        {
            var sql = "INSERT INTO dbo.T_FURNACE + (Start, FurnaceNumber) values('" +
                        DateTime.Now.ToString(("yyyy-MMMM-dd HH:mm:ss")) +
                        "', " + I.ToString() + ")";

            try
            {
                var rowAffetced = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffetced > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool UpdateFurnace(int I, string time1, string TotalConsumption)
        {
            var sql = "UPDATE dbo.T_FURNACE SET EndTime='" + time1 +
                "',ConsumedEnergy='" + TotalConsumption +
                "' WHERE F_Key = (SELECT MAX(F_Key) FROM dbo.T_FURNACE where FurnaceNumber = " + I.ToString() + ") AND " +
                " FurnaceNumber = " + I.ToString();
            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool ClearFromFurnace(int I)
        {
            var sql = "DELETE FROM dbo.T_FURNACE WHERE ENDTIME IS NULL and dbo.T_FURNACE.furnacenumber = " + I.ToString();

            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool UpdateEAFSComnsumption(int I, string eafConsumption)
        {
            var sql = "UPDATE dbo.T_EAFsEnergyConsumption SET [Consumed Energy Per Heat] = '" + eafConsumption +
                "' WHERE Furnace='" + (I + 1).ToString() + "'";

            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public DCPScadaPoint GetScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public DCPScadaPoint GetScadaPoint(string name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public DigitalStatus GetDigitalStatusByScadaName(string name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var scadaPoint))
                return (DigitalStatus)scadaPoint.Value;
            else
                throw new InvalidOperationException($"Scada point with name {name} can not found");
        }

        public DataTable GetEECTELEGRAM()
        {
            DataTable dataTable = null;

            try
            {
                string sql = $"Select * FROM {GetEndStringCommand()}EEC_TELEGRAMS WHERE TELDATETIME = (SELECT MAX(TELDATETIME) FROM {GetEndStringCommand()}EEC_TELEGRAMS)";
                dataTable = _historicalDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public bool UpdateEECTELEGRAM(string atime, string telDate)
        {
            String sql = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                sql = $"Update app.EEC_TELEGRAMS Set SentTime='" + atime + "' Where TELDATETIME = '" + telDate + "'";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {                
                sql = $"Update APP_EEC_TELEGRAMS Set SentTime=" + $"TO_DATE('{atime}', 'mm/dd/yyyy HH:MI:SS AM')" + $" Where TELDATETIME = TO_DATE('{telDate}','mm/dd/yyyy HH:MI:SS AM')";
            }

            try
            {
                var rowAffected = _historicalDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool InsertTELEGRAM(string sql)
        {
            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        // on PU10 Link Server
        public bool UpdateEAFGroupRequest()
        {
            try
            {
                var sql = "Update dbo.T_EAFGroupRequest Set ResponseDateTime = '" + DateTime.Now.ToString() + "' Where RequestDateTime = (Select MAX(RequestDateTime) From dbo.T_EAFGroupRequest)";

                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public DataTable GetVMABEAFSGRPNUM()
        {
            DataTable dataTable = null;

            try
            {
                string sql = "SELECT * FROM dbo.T_EAFGroup WHERE TelDateTime = (SELECT MAX(TelDateTime) FROM dbo.T_EAFGroup)";
                dataTable = _linkDBpcsDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public DataTable GetEAFGROUPREQUEST()
        {
            DataTable dataTable = null;

            try
            {
                string sql = "Select * From dbo.T_EAFGroupRequest Where RequestDateTime = (Select MAX(RequestDateTime) From dbo.T_EAFGroupRequest)";
                dataTable = _linkDBpcsDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public bool ModifyOnLinkDB(string sql)
        {
            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool ModifyOnStaticDB(string sql)
        {
            try
            {
                var rowAffected = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public bool ModifyOnHistoricalDB(string sql)
        {
            try
            {
                var rowAffected = _historicalDataManager.ExecuteNonQuery(sql);
                if (rowAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return false;
        }

        public Guid GetGuid(String networkpath)
        {
            string sql = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                sql = "SELECT * FROM dbo.NodesFullPath where FullPath = '" + networkpath + "'";

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                sql = "SELECT * FROM NodesFullPath where TO_CHAR(FullPath) = '" + networkpath + "'";

            try
            {
                var dataTable = _staticDataManager.GetRecord(sql);
                Guid id = Guid.Empty;
                if (dataTable != null && dataTable.Rows.Count == 1)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        id = Guid.Parse(row["GUID"].ToString());
                    }
                    return id;
                }
                else if (dataTable.Rows.Count > 1)
                {
                    _logger.WriteEntry("Error More Guid found for " + networkpath, LogLevels.Error);
                    return Guid.Empty;
                }
                else
                {
                    _logger.WriteEntry("Error in loading Guid for " + networkpath, LogLevels.Error);
                    return Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading Guid for " + networkpath, LogLevels.Error, ex);
                return Guid.Empty;
            }
        }
    }
}