using COMMON;
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
using Irisa.Common;

namespace RPC
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, RPCScadaPoint> _scadaPoints;
        private readonly Dictionary<string, RPCScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, DataManager historicalDataManager, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _historicalDataManager = historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            _scadaPoints = new Dictionary<Guid, RPCScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, RPCScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }

       public bool Build()
        {
            try
            {
                if (GetInputScadaPoints())
                    isBuild = true;

                else if (GetInputScadaPointsFromRedis())
                    isBuild = true;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                isBuild = false;

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                isBuild = false;
            }

            return isBuild;
        }


        private bool GetInputScadaPoints()
        {
            try
            {
                _logger.WriteEntry("Loading Data from Database", LogLevels.Info);
                RPC_PARAMS_Str RPC_param = new RPC_PARAMS_Str();
                var dataTable = _dataManager.GetRecord($"SELECT * from APP_RPC_PARAMS"); ;
                if (dataTable != null)
                {
                    if (!RedisUtils.DelKeys(RedisKeyPattern.RPC_PARAMS))
                        _logger.WriteEntry("Error: Delete APP_RPC_PARAMS from Redis", LogLevels.Error);
                }

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

                    if (RedisUtils.CheckConnection())
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

        private bool GetInputScadaPointsFromRedis()
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

        public bool TryGetHISAverageinIntervalTime(RPCScadaPoint scadaPoint, IntervalTime duration, out float value)
        {
            value = 0;
            if (scadaPoint == null)
            {
                _logger.WriteEntry("scadaPoint is NULL to get average in intervalTime ", LogLevels.Error);
                return false;
            }
            Guid analogMeasurementId = scadaPoint.Id;
            try
            {
                Queue<SampleData> archData = new Queue<SampleData>();
                float firstvalue = 0;
                GetHISFirstDatainIntervalTime(analogMeasurementId, duration, out firstvalue);
                archData.Enqueue(new SampleData { dateTime = duration.StartTime, qualityCode = Irisa.Common.QualityCodes.None, value = firstvalue });

                var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP >=  '{duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}' AND TIMESTAMP <=  '{duration.EndTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}' ORDER BY  TIMESTAMP ASC ";
                var archiveDataTable = _historicalDataManager.GetRecord(command);

                if (archiveDataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in archiveDataTable.Rows)
                    {
                        archData.Enqueue(new SampleData
                        {
                            value = Convert.ToSingle(row["VALUE"]),
                            qualityCode = (QualityCodes)Convert.ToInt16(row["QUALITY"]),
                            dateTime = Convert.ToDateTime(row["TIMESTAMP"])
                        });
                    }
                }
                var preSampled = archData.Dequeue();
                float sumvalue = 0;
                while (archData.Count > 0)
                {
                    var NextSampled = archData.Dequeue();
                    var deltatime = NextSampled.dateTime - preSampled.dateTime;
                    sumvalue += preSampled.value * (float)deltatime.TotalMilliseconds;
                    preSampled = NextSampled;
                }
                value = sumvalue/(float)(duration.EndTime - duration.StartTime).TotalMilliseconds;
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex is Irisa.DataLayer.DataException ? ex.ToString() : ex.Message, LogLevels.Error);
                return false;
            }
        }

        public bool GetHISFirstDatainIntervalTime(Guid analogMeasurementId, IntervalTime duration, out float value)
        {
            try
            {
                var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE " +
                              $"TIMESTAMP = (SELECT MAX(TIMESTAMP) FROM HISANALOGS  WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP <=  '{duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}') AND " +
                              $"MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}'";

                var archiveDataTable = _historicalDataManager.GetRecord(command);

                value = 0;
                if (archiveDataTable.Rows.Count > 0)
                {
                    DataRow row = archiveDataTable.Rows[0];
                    value = Convert.ToSingle(row["VALUE"]);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                value = 0;
                _logger.WriteEntry(ex is Irisa.DataLayer.DataException ? ex.ToString() : ex.Message, LogLevels.Error);
                return false;
            }
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
        public void  PrintRepository()
        {
           
            foreach ( var SP1 in _scadaPointsHelper)
            {
                var RPC_SP = SP1.Value;
                    Console.WriteLine(RPC_SP.Name +"\t\t...."+ RPC_SP.Id);
            }

        }
    }


}