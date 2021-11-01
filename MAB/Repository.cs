using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using StackExchange.Redis;
using Newtonsoft.Json;

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
            var isBuild = false;
            if (RedisUtils.IsConnected)
            {
                _logger.WriteEntry("Redis Connacted ", LogLevels.Info);
                _cache = _RedisConnectorHelper.DataBase;
                if (_RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.MAB_Measurements).Length != 0)
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
                _logger.WriteEntry("Redis Connaction Fauiled.", LogLevels.Error);
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
                return "app.";
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
                _logger.WriteEntry("Loading Data from Cash", LogLevels.Info);

                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.MAB_Measurements);
                var rows = _RedisConnectorHelper.StringGet<MABScadaPoint>(keys);

                try
                {
                    foreach (var _mabscadapoint in rows)
                    {

                        _scadaPoints.Add(_mabscadapoint.Id, _mabscadapoint);
                        _scadaPointsHelper.Add(_mabscadapoint.Name, _mabscadapoint);
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error load data from cash database", LogLevels.Error, ex);
                }
            }
            else
            {
                _logger.WriteEntry("Loading Data from Database", LogLevels.Info);
                string command = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from app.MAB_Measurements";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from APP_MAB_PARAMS";

                var dataTable = dataManager.GetRecord(command);

                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["MeasurementId"].ToString());
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = row["DirectionType"].ToString();
                    var scadaType = Convert.ToString(row["SCADAType"]);
                    var id = GetGuid(networkPath);


                    try
                    {
                        var scadaPoint = new MABScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                            if (RedisUtils.IsConnected)
                                _cache.StringSet(RedisKeyPattern.MAB_Measurements + networkPath, JsonConvert.SerializeObject(scadaPoint));
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
            string sql = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                sql = "SELECT * FROM dbo.NodesFullPath where FullPath = '" + networkpath + "'";

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
        public const string MAB_Measurements = "APP:MAB_Measurements:";
        public const string DCIS_PARAMS = "DCIS_PARAMS:";
        public const string DCP_PARAMS = "DCP_PARAMS:";
        public const string EEC_EAFSPriority = "EEC_EAFSPriority:";
        public const string EEC_PARAMS = "EEC_PARAMS:";
        public const string LSP_DECTCOMB = "LSP_DECTCOMB:";
        public const string LSP_DECTITEMS = "LSP_DECTITEMS:";
        public const string LSP_DECTLIST = "LSP_DECTLIST:";
        public const string LSP_DECTPRIOLS = "LSP_DECTPRIOLS:";
        public const string LSP_PARAMS = "LSP_PARAMS:";
        public const string LSP_PRIORITYITEMS = "LSP_PRIORITYITEMS:";
        public const string LSP_PRIORITYLIST = "LSP_PRIORITYLIST:";
        public const string OCP_CheckPoints = "OCP_CheckPoints:";
        public const string OCP_PARAMS = "OCP_PARAMS:";
        public const string OPCMeasurement = "OPCMeasurement:";
        public const string OPC_Params = "OPC_Params:";
    }

}