using COM;
using Irisa.DataLayer;
using Irisa.DataLayer.Oracle;
using Irisa.Logger;
using Irisa.Common.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RPC
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, RPCScadaPoint> _scadaPoints;
        private readonly Dictionary<string, RPCScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, RPCScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, RPCScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }

        public bool Build()
        {

            try
            {
                if (GetInputScadaPoints())
                {
                    isBuild = true;
                }
                else if (GetInputScadaPointsfromRedis())
                {
                    isBuild = true;
                }

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }


            return isBuild;
        }


        private bool GetInputScadaPoints()
        {
            try
            {

                RPC_PARAMS_Str RPC_param = new RPC_PARAMS_Str();
                var dataTable = _dataManager.GetRecord($"SELECT * FROM APP_RPC_PARAMS");

                foreach (DataRow row in dataTable.Rows)
                {
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = "Input";
                    var id = GetGuid(networkPath);

                    RPC_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                    RPC_param.NAME = name;
                    RPC_param.DIRECTIONTYPE = row["DIRECTIONTYPE"].ToString();
                    RPC_param.NETWORKPATH = networkPath;
                    RPC_param.SCADATYPE = row["SCADATYPE"].ToString();

                    RPC_param.ID = id.ToString();
                    if (RedisUtils.IsConnected)
                        RedisUtils.RedisConn.Set(RedisKeyPattern.RPC_PARAMS + networkPath, JsonConvert.SerializeObject(RPC_param));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);

                    var scadaPoint = new RPCScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType));

                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }
                }

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
            return true;
        }

        private bool GetInputScadaPointsfromRedis()
        {
            _logger.WriteEntry("Loading RPC_PARAMS Data from Cache", LogLevels.Info);

            

            try
            {
                var keys = RedisUtils.GetKeys(pattern: RedisKeyPattern.RPC_PARAMS);
                var dataTable_cache = RedisUtils.StringGet<RPC_PARAMS_Str>(keys);
                foreach (RPC_PARAMS_Str row in dataTable_cache)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    var name = row.NAME;
                    var networkPath = row.NETWORKPATH;
                    var pointDirectionType = "Input";

                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new RPCScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType));

                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }

                    }

                }
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
            return true;

        }


        public RPCScadaPoint GetRPCScadaPoint(Guid guid)
        {
            if (_scadaPoints.TryGetValue(guid, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public RPCScadaPoint GetRPCScadaPoint(string name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

       
        public RedisUtils GetRedisUtiles()
        {
            return _RedisConnectorHelper;

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
            string sql = "SELECT * FROM NodesFullPath where TO_CHAR(FullPath) = '" + networkpath + "'";

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


}