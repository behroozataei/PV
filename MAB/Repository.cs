using System;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using StackExchange.Redis;
using System.Linq;


using Irisa.Logger;
using Irisa.DataLayer;

namespace MAB
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, MABScadaPoint> _scadaPoints;
        private readonly Dictionary<string, MABScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;

        private bool LoadfromRedis = false;
        IDatabase _cache;
        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, MABScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, MABScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }

        public bool Build()
        {
            
            if (RedisUtils.IsConnected)
            {
                _logger.WriteEntry("Connected to Redis Cache", LogLevels.Info);
                _cache = _RedisConnectorHelper.DataBase;
                if (_RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.MAB_PARAMS).Length != 0)
                {
                    LoadfromRedis = true;
                }
                else
                {
                    LoadfromRedis = false;
                }
            }
            else
            {
                _logger.WriteEntry("Redis Connaction Failed.", LogLevels.Error);
            }

            try
            {
                GetInputScadaPoints(_dataManager);
                isBuild = true;
            }
            catch(Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            
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
                //return "app.";
                return "APP_";
                // return string.Empty;

            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "APP_";

            }

            return string.Empty;
        }

        private void GetInputScadaPoints(DataManager dataManager)
        {
            if (LoadfromRedis)
            {
                _logger.WriteEntry("Loading Data from Cache", LogLevels.Info);

                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.MAB_PARAMS);
                var dataTable = _RedisConnectorHelper.StringGet<MAB_PARAMS_Object>(keys);

                foreach (MAB_PARAMS_Object row in dataTable)
                {
                    var name = row.Name;
                    var networkPath = row.NetworkPath;
                    var pointDirectionType = row.DirectionType;
                    var scadaType = row.ScadaType;
                    var id = Guid.Parse(row.ID.ToString());
                    
                    try
                    {
                        var scadaPoint = new MABScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }
                    catch (Irisa.DataLayer.DataException ex)
                    {
                        _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry("Error in loading from Cache" + networkPath.ToString(), LogLevels.Error, ex);
                    }
                }
            }
            else
            {
                _logger.WriteEntry("Loading Data from Database", LogLevels.Info);
                string command = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    //command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from app.MAB_Measurements";
                    command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from APP_MAB_PARAMS";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from APP_MAB_PARAMS";

                MAB_PARAMS_Object _mab_param = new MAB_PARAMS_Object();
                var dataTable = dataManager.GetRecord(command);
                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["MeasurementId"].ToString());
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = row["DirectionType"].ToString();
                    var scadaType = Convert.ToString(row["SCADAType"]);
                    var id = GetGuid(networkPath);

                    _mab_param.Name = name;
                    _mab_param.NetworkPath = networkPath;
                    _mab_param.DirectionType = pointDirectionType;
                    _mab_param.ScadaType = scadaType;
                    _mab_param.ID = id;


                    if (RedisUtils.IsConnected)
                        _cache.StringSet(RedisKeyPattern.MAB_PARAMS + networkPath, JsonConvert.SerializeObject(_mab_param));
                    
                    try
                    {
                        var scadaPoint = new MABScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }
                    catch (Irisa.DataLayer.DataException ex)
                    {
                        _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry("Error in loading " + networkPath.ToString(), LogLevels.Error, ex);
                    }
                }
            }
        }

        public MABScadaPoint GetScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public MABScadaPoint GetScadaPoint(String name)
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
        public Guid GetGuid(String networkpath)
        {
            if (isBuild)
            {
                var res = _scadaPoints.FirstOrDefault(n => n.Value.NetworkPath.Equals(networkpath)).Key;
                if (res != Guid.Empty)
                    return res;
                else
                    _logger.WriteEntry("The GUID could not read from Repository for Network   " + networkpath, LogLevels.Error);
            }
            string sql = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //sql = "SELECT * FROM dbo.NodesFullPath where FullPath = '" + networkpath + "'";
                sql = "SELECT * FROM NodesFullPath where TO_CHAR(FullPath) = '" + networkpath + "'";

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                sql = "SELECT * FROM NodesFullPath where TO_CHAR(FullPath) = '" + networkpath + "'";

            try
            {
                var dataTable = _dataManager.GetRecord(sql);
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

    static class RedisKeyPattern
    {
        public const string MAB_PARAMS = "APP:MAB_PARAMS:";
        public const string DCIS_PARAMS = "APP:DCIS_PARAMS:";
        public const string DCP_PARAMS = "APP:DCP_PARAMS:";
        public const string EEC_EAFSPriority = "APP:EEC_EAFSPriority:";
        public const string EEC_PARAMS = "APP:EEC_PARAMS:";
        public const string LSP_DECTCOMB = "APP:LSP_DECTCOMB:";
        public const string LSP_DECTITEMS = "APP:LSP_DECTITEMS:";
        public const string LSP_DECTLIST = "APP:LSP_DECTLIST:";
        public const string LSP_DECTPRIOLS = "APP:LSP_DECTPRIOLS:";
        public const string LSP_PARAMS = "APP:LSP_PARAMS:";
        public const string LSP_PRIORITYITEMS = "APP:LSP_PRIORITYITEMS:";
        public const string LSP_PRIORITYLIST = "APP:LSP_PRIORITYLIST:";
        public const string OCP_CheckPoints = "APP:OCP_CheckPoints:";
        public const string OCP_PARAMS = "APP:OCP_PARAMS:";
        public const string OPCMeasurement = "APP:OPCMeasurement:";
        public const string OPC_Params = "APP:OPC_Params:";
    }
     class MAB_PARAMS_Object
     {
        public string Name ;
        public string NetworkPath;
        public string DirectionType;
        public string ScadaType;
        public Guid   ID;
    };

}