using Irisa.DataLayer;
using Irisa.Logger;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Irisa.DataLayer.Oracle;

namespace SRC_FEED_DETECTION
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly DataManager _dataManager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, ScadaPoint> _scadaPoints;
        private readonly Dictionary<string, ScadaPoint> _scadaPointsHelper;
       

        private bool isBuild = false;

        public Repository(ILogger logger, IConfiguration configuration, DataManager staticDataManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _scadaPoints = new Dictionary<Guid, ScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, ScadaPoint>();
            _historicalDataManager = new OracleDataManager(configuration["OracleServicename"], configuration["OracleDatabaseAddress"], configuration["OracleHISUser"], configuration["OracleHISPassword"]);


        }

        public bool Build()
        {
            try
            {
                if (GetInputScadaPoints(_dataManager))
                    isBuild = true;

                
            }
            
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                isBuild = false;
            }

            return isBuild;
        }


        private bool GetInputScadaPoints(DataManager dataManager)
        {
            _logger.WriteEntry("Loading ScadaData Point Information", LogLevels.Info);
            try
            {
                // ScadaPoint Information : Name, NetworkPath, PointDirectionType, ScadaType,Guid
                var dataTable = _dataManager.GetRecord("SELECT * FROM  SCADA.APP_PV_PARAMS");
                if (dataTable != null)
                {
                    // if (!_RTDBManager.DelKeys(RedisKeyPattern.EEC_PARAMS))
                    //     _logger.WriteEntry("Error: Delete APP_EEC_PARAMS from Redis", LogLevels.Error);
                }


                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var name = row["NAME"].ToString();
                    var networkPath = row["NETWORKPATH"].ToString();
                    var pointDirectionType = row["DIRECTIONTYPE"].ToString();
                    var scadatype = row["SCADATYPE"].ToString();
                    //if (name == "PMAX1")
                    //    System.Diagnostics.Debug.Print("PAMX1");
                    var id = GetGuid(networkPath);
                    var test = (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType);



                    var scadaPoint = new ScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadatype);

                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading " , LogLevels.Error, ex);
                return false;
            }

            //List<Tuple<string, string, string, string, Guid>> _pointsParams = new List<Tuple<string, string, string, string, Guid>>();
            //_pointsParams.Add(Tuple.Create("_1MinEnergy", "Network/Substations/63KV STATION/63KV/FL01/ENERGY_SUB_1", "Input", "EnergyMeasurement", Guid.Empty));
            //_pointsParams.Add(Tuple.Create("DailyEnergy", "Network/Substations/63KV STATION/63KV/FL01/ENERGY_daily", "Output", "EnergyMeasurement", Guid.Empty));
            //_pointsParams.Add(Tuple.Create("PerviousDayEnergy", "Network/Substations/63KV STATION/63KV/FL01/ENERGY_yesterday", "Output", "EnergyMeasurement", Guid.Empty));
            //_pointsParams.Add(Tuple.Create("TotalEnergy", "Network/Substations/63KV STATION/63KV/FL01/ENERGY_total", "Output", "EnergyMeasurement", Guid.Empty));


            //foreach (var pointParam in _pointsParams)
            //{
                

            //    try
            //    {
            //        var id = GetGuid(pointParam.Item2);
                    
            //        var scadaPoint = new ScadaPoint(id, pointParam.Item1, pointParam.Item2, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointParam.Item3), pointParam.Item4);
            //        if (!_scadaPoints.ContainsKey(pointParam.Item5))
            //        {
            //            _scadaPoints.Add(id, scadaPoint);
            //            _scadaPointsHelper.Add(pointParam.Item1, scadaPoint);
            //        }

            //    }
                
            //    catch (Exception ex)
            //    {
            //        _logger.WriteEntry("Error in loading " + pointParam.Item2, LogLevels.Error, ex);
            //        return false;
            //    }
            //}
            return true;

        }

        

        public ScadaPoint GetScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public ScadaPoint GetScadaPoint(String name)
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

        public DigitalSingleStatusOnOff DigitalSingleStatusOnOffByScadaName(string name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var scadaPoint))
                return (DigitalSingleStatusOnOff)scadaPoint.Value;
            else
                throw new InvalidOperationException($"Scada point with name {name} can not found");
        }

        public bool ModifyOnHistoricalDB(string sql)
        {
            try
            {
                var RowAffected = _historicalDataManager.ExecuteNonQuery(sql);
                if (RowAffected > 0)
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

        public bool ModifyOnHistoricalDB(string StoredProcedure, IDbDataParameter[] dbDataParameter)
        {
            try
            {
                var RowAffected = _historicalDataManager.ExecuteNonQuery(StoredProcedure, CommandType.StoredProcedure, dbDataParameter);
                // if (RowAffected > 0)
                return true;
                // else
                //     return false;
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

        public DataTable GetFromHistoricalDB(string sql)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _historicalDataManager.GetRecord(sql);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            return dataTable;
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
           
            try
            {
                IDbDataParameter[] parameters = new IDbDataParameter[1];
                parameters[0] = _dataManager.CreateParameter("networkpath", networkpath);
                var dataTable = _dataManager.GetRecord("APP_GUID_SELECT", CommandType.StoredProcedure, parameters);
                //string SQL = " SELECT * FROM SCADAMASTER.NODESFULLPATH n WHERE TO_CHAR(n.FULLPATH) = networkpath";
               // var dataTable = _dataManager.GetRecord("APP_GUID_SELECT", CommandType.StoredProcedure, parameters);
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