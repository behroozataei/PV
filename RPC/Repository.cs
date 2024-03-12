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
using System.Collections;

namespace RPC
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, RPCScadaPoint> _scadaPoints;
        private readonly Dictionary<string, RPCScadaPoint> _scadaPointsHelper;
        public Dictionary<string, ACCScadaPoint> _accScadaPoints;
        private readonly RedisUtils _RTDBManager;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, DataManager historicalDataManager, RedisUtils RTDBManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _historicalDataManager = historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            _scadaPoints = new Dictionary<Guid, RPCScadaPoint>();
            _accScadaPoints = new Dictionary<string, ACCScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, RPCScadaPoint>();
            _RTDBManager = RTDBManager ?? throw new ArgumentNullException(nameof(RTDBManager));
        }

        public bool Build()
        {
            try
            {
                if (GetInputScadaPoints())
                    isBuild = true;

                else if (GetInputScadaPointsFromRedis())
                    isBuild = true;
                BuildAccList();
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
                var dataTable = _dataManager.GetRecord("FUNCTIONS.APP_RPC_PARAMS_SELECT", CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.RPC_PARAMS))
                        _logger.WriteEntry("Error: Delete APP_RPC_PARAMS from Redis", LogLevels.Error);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = "Input";
                    var id = Guid.Parse(row["GUID"].ToString());
                    var scadaType = row["SCADATYPE"].ToString();


                    RPC_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                    RPC_param.NAME = name;
                    RPC_param.DIRECTIONTYPE = row["DIRECTIONTYPE"].ToString();
                    RPC_param.NETWORKPATH = networkPath;
                    RPC_param.SCADATYPE = row["SCADATYPE"].ToString();
                    RPC_param.ID = id.ToString();
                    RPC_param.SNAME = row["SNAME"].ToString(); ;

                    if (_RTDBManager.CheckConnection("APP:RPC"))
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.RPC_PARAMS + networkPath, JsonConvert.SerializeObject(RPC_param));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);

                    var scadaPoint = new RPCScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);

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
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.RPC_PARAMS);
                var dataTable_cache = _RTDBManager.StringGet<RPC_PARAMS_Str>(keys);
                foreach (RPC_PARAMS_Str row in dataTable_cache)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    var name = row.NAME;
                    var networkPath = row.NETWORKPATH;
                    var pointDirectionType = "Input";
                    var scadaType = row.SCADATYPE;

                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new RPCScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);

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

        void BuildAccList()
        {
            var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.RPC_PARAMS);
            var dataTable_cache = _RTDBManager.StringGet<RPC_PARAMS_Str>(keys);
            foreach (RPC_PARAMS_Str row in dataTable_cache)
            {
                if(row.SCADATYPE.ToLower() == "accumulatorcalc")
                {
                    var accScadaPoint = new ACCScadaPoint();
                    accScadaPoint.sPCScadaPoint = GetRPCScadaPoint(row.SNAME);
                    accScadaPoint.aPCScadaPoint = GetRPCScadaPoint(row.NAME);
                    _accScadaPoints.Add(row.SNAME,accScadaPoint);
                }

            }

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
            return _RTDBManager;

        }

        public bool TryGetHISAverageinIntervalTime(RPCScadaPoint scadaPoint, IntervalTime duration, out float value)
        {
            value = 0;
            if (scadaPoint == null)
            {
                _logger.WriteEntry($"scadaPoint {scadaPoint.Name} is NULL to get average in intervalTime ", LogLevels.Error);
                return false;
            }
            Guid analogMeasurementId = scadaPoint.Id;
            try
            {
                Queue<SampleData> archData = new Queue<SampleData>();
                float firstvalue = 0;
                GetHISFirstDatainIntervalTime(analogMeasurementId, duration, out firstvalue);
                archData.Enqueue(new SampleData
                {
                    dateTime = duration.StartTime,
                    qualityCode = Irisa.Common.QualityCodes.None,
                    value = firstvalue
                });

                //var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP >  to_timestamp('{duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') AND TIMESTAMP <=  to_timestamp('{duration.EndTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') ORDER BY  TIMESTAMP ASC ";
                //var archiveDataTable = _historicalDataManager.GetRecord(command);

                IDbDataParameter[] parameters = new IDbDataParameter[3];
                parameters[0] = _dataManager.CreateParameter("analogMeasurementId", analogMeasurementId.ToString().ToUpper());
                parameters[1] = _dataManager.CreateParameter("StartTime", duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                parameters[2] = _dataManager.CreateParameter("EndTime", duration.EndTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));

                var archiveDataTable = _dataManager.GetRecord("FUNCTIONS.APP_RPC_GET_AVERAGE_IN_INTERVAL_TIME_SELECT", CommandType.StoredProcedure, parameters);


                float lastsampledValue = firstvalue;

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
                        lastsampledValue = Convert.ToSingle(row["VALUE"]);
                    }
                }

                archData.Enqueue(new SampleData
                {
                    value = lastsampledValue,
                    qualityCode = Irisa.Common.QualityCodes.None,
                    dateTime = duration.EndTime
                });

                int Samplecount = 0;
                //foreach (var sample in archData)
                //{
                //    Samplecount++;
                //    _logger.WriteEntry($"{scadaPoint.Name} sample[{Samplecount}] =  {sample.dateTime.ToString("yyyy-MM-dd HH:mm:ss.ff")} , {sample.value}", LogLevels.Info);

                //}



                var preSampled = archData.Dequeue();
                float sumvalue = 0;
                while (archData.Count > 0)
                {
                    var NextSampled = archData.Dequeue();
                    var deltatime = NextSampled.dateTime - preSampled.dateTime;
                    sumvalue += preSampled.value * (float)deltatime.TotalMilliseconds;
                    preSampled = NextSampled;
                }
                value = sumvalue / (float)(duration.EndTime - duration.StartTime).TotalMilliseconds;
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
            double Minutes_Step = 10.0;
            try
               {
            //    var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE " +
            //                  $"MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND " +
            //                  $"TIMESTAMP <=   to_timestamp('{duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') AND " +
            //                  $"TIMESTAMP >=   to_timestamp('{duration.StartTime.AddMinutes(-1 * Minutes_Step).ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') ORDER BY TIMESTAMP DESC";

            //    var archiveDataTable = _historicalDataManager.GetRecord(command);


                IDbDataParameter[] parameters = new IDbDataParameter[3];
                parameters[0] = _dataManager.CreateParameter("analogMeasurementId", analogMeasurementId.ToString().ToUpper());
                parameters[1] = _dataManager.CreateParameter("StartTime", duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                parameters[2] = _dataManager.CreateParameter("EndTime", duration.StartTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));

                var archiveDataTable = _dataManager.GetRecord("FUNCTIONS.APP_RPC_FIRST_DATA_IN_INTERVAL_TIME_SELECT", CommandType.StoredProcedure, parameters);

                if (archiveDataTable.Rows.Count == 0)
                {

                    int Count = 0;
                    do
                    {
                        Count++;
                        //{
                        //    command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE " +
                        //              $"MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND " +
                        //              $"TIMESTAMP <=   to_timestamp('{duration.StartTime.AddMinutes(-1 * Minutes_Step * (Count - 1)).ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') AND " +
                        //              $"TIMESTAMP >=   to_timestamp('{duration.StartTime.AddMinutes(-1 * Minutes_Step * Count).ToString("yyyy-MM-dd HH:mm:ss.ff")}','yyyy-MM-dd HH24:mi:ss.ff') ORDER BY TIMESTAMP DESC";
                        //}
                        //archiveDataTable = _historicalDataManager.GetRecord(command);

                        parameters[0] = _dataManager.CreateParameter("analogMeasurementId", analogMeasurementId.ToString().ToUpper());
                        parameters[1] = _dataManager.CreateParameter("StartTime", duration.StartTime.AddMinutes(-1 * Minutes_Step * (Count - 1)).ToString("yyyy-MM-dd HH:mm:ss.ff"));
                        parameters[2] = _dataManager.CreateParameter("EndTime", duration.StartTime.AddMinutes(-1 * Minutes_Step * Count).ToString("yyyy-MM-dd HH:mm:ss.ff"));
                        archiveDataTable = _dataManager.GetRecord("FUNCTIONS.APP_RPC_FIRST_DATA_IN_INTERVAL_TIME_SELECT", CommandType.StoredProcedure, parameters);
                    } while (archiveDataTable.Rows.Count == 0 && Count <10);
                }

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

        Guid GetGuid(String networkpath)
        {
            if (isBuild)
            {
                var res = _scadaPoints.FirstOrDefault(n => n.Value.NetworkPath.Equals(networkpath)).Key;
                if (res != Guid.Empty)
                    return res;
                else
                    _logger.WriteEntry("The GUID could not read from Repository for Network   " + networkpath, LogLevels.Error);
            }

            try
            {

                IDbDataParameter[] parameters = new IDbDataParameter[1];
                parameters[0] = _dataManager.CreateParameter("networkpath", networkpath);
                var dataTable = _dataManager.GetRecord("FUNCTIONS.APP_GUID_SELECT", CommandType.StoredProcedure, parameters);
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
        public void PrintRepository()
        {

            foreach (var SP1 in _scadaPointsHelper)
            {
                var RPC_SP = SP1.Value;
                Console.WriteLine(RPC_SP.Name + "\t\t...." + RPC_SP.Id);
            }

        }
        public Dictionary<string ,ACCScadaPoint> accScadaPoint
        {
            get => _accScadaPoints;
        }

        public ACCScadaPoint GetAccScadaPoint(string name)
        {
            if (_accScadaPoints.TryGetValue(name, out var accscadaPoint))
                return accscadaPoint;
            else
                return null;
        }
    }


}