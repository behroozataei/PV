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

namespace EEC
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly DataManager sqlDataMnager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, EECScadaPoint> _scadaPoints;
        private readonly Dictionary<string, EECScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;


        private bool isBuild = false;

        public Repository(ILogger logger, IConfiguration configuration, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));

            sqlDataMnager = new OracleDataManager(_configuration["OracleServicename"], _configuration["OracleDatabaseAddress"], _configuration["OracleStaticUser"], _configuration["OracleStaticPassword"]);
            _historicalDataManager = new OracleDataManager(configuration["OracleServicename"], configuration["OracleDatabaseAddress"], configuration["OracleHISUser"], configuration["OracleHISPassword"]);

            _scadaPoints = new Dictionary<Guid, EECScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, EECScadaPoint>();

        }

        public bool Build()
        {

            try
            {
                if (GetInputScadaPoints())
                {
                    isBuild = true;
                    BuildCashe();
                }
                else if (GetInputScadaPointsfromRedis())
                {
                    isBuild = true;
                    BuildCashe();
                }

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }


            return isBuild;
        }

        private bool BuildCashe()
        {
            try
            {
                EEC_SFSCEAFSPRIORITY_Str[] eec_sfsceafsproi = new EEC_SFSCEAFSPRIORITY_Str[8];
                for (int furnace = 0; furnace < 8; furnace++)
                {
                    eec_sfsceafsproi[furnace] = new EEC_SFSCEAFSPRIORITY_Str();
                    eec_sfsceafsproi[furnace].FURNACE = (furnace + 1).ToString();
                    eec_sfsceafsproi[furnace].GROUPNUM = "1";
                    eec_sfsceafsproi[furnace].CONSUMED_ENERGY_PER_HEAT = "0";
                    eec_sfsceafsproi[furnace].REASON = "";
                    eec_sfsceafsproi[furnace].STATUS_OF_FURNACE = "OFF";

                    _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + $"{ furnace + 1}", JsonConvert.SerializeObject(eec_sfsceafsproi[furnace]));
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

        private bool GetInputScadaPoints()
        {
            try
            {

                EEC_PARAMS_Str eec_param = new EEC_PARAMS_Str();
                var dataTable = sqlDataMnager.GetRecord($"SELECT * FROM APP_EEC_PARAMS");

                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    var pointDirectionType = "Input";
                    //if (name == "PMAX1")
                    //    System.Diagnostics.Debug.Print("PAMX1");
                    var id = GetGuid(networkPath);

                    eec_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                    eec_param.NAME = name;
                    eec_param.DESCRIPTION = row["DESCRIPTION"].ToString();
                    eec_param.DIRECTIONTYPE = row["DIRECTIONTYPE"].ToString();
                    eec_param.NETWORKPATH = networkPath;
                    eec_param.SCADATYPE = row["SCADATYPE"].ToString();
                    eec_param.TYPE = row["TYPE"].ToString();

                    eec_param.ID = id.ToString();
                    if (RedisUtils.IsConnected)
                        _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.EEC_PARAMS + networkPath, JsonConvert.SerializeObject(eec_param));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);

                    var scadaPoint = new EECScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType));

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
            _logger.WriteEntry("Loading EEC_PARAMS Data from Cache", LogLevels.Info);

            var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.EEC_PARAMS);
            var dataTable_cache = _RedisConnectorHelper.StringGet<EEC_PARAMS_Str>(keys);

            try
            {
                foreach (EEC_PARAMS_Str row in dataTable_cache)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    var name = row.NAME;
                    var networkPath = row.NETWORKPATH;
                    var pointDirectionType = "Input";

                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new EECScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType));

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


        public EECScadaPoint GetScadaPoint(Guid guid)
        {
            if (_scadaPoints.TryGetValue(guid, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public EECScadaPoint GetScadaPoint(string name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public bool SendEECTelegramToDC(float RESTIME, float ER_Cycle, float PSend, float PSend1, float PSend2, float m_EnergyResEnd)
        {
            String Datatime = DateTime.Now.ToString("yyyy-MMMM-dd HH:mm:ss");
            String strSQL = $"INSERT INTO APP_EEC_TELEGRAMS" +
                "(TelDateTime, SentTime, ResidualTime, ResidualEnergy, MaxOverload1, MaxOverload2, ResidualEnergyEnd) " +
                "VALUES (" +
                $"TO_DATE('{Datatime}', 'yyyy-mm-dd HH24:mi:ss')" + "," +
                $"TO_DATE('1900-01-01 00:00:00','yyyy-mm-dd HH24:mi:ss')" + ",'" +
                RESTIME.ToString() + "', '" +
                ER_Cycle.ToString() + "', '" +
                PSend1.ToString() + "', '" +
                PSend2.ToString() + "', '" +
                m_EnergyResEnd.ToString() + "')";


            EEC_TELEGRAM_Str eec_telegram = new EEC_TELEGRAM_Str();
            //1401.03.24 IranTime
            eec_telegram.TELDATETIME = DateTime.UtcNow.ToIranDateTime();
            eec_telegram.SENTTIME = DateTime.Parse("1900-01-01 00:00:00");
            eec_telegram.RESIDUALTIME = RESTIME;
            eec_telegram.RESIDUALENERGY = ER_Cycle;
            eec_telegram.MAXOVERLOAD1 = PSend1;
            eec_telegram.MAXOVERLOAD2 = PSend2;
            eec_telegram.RESIDUALENERGYEND = m_EnergyResEnd;



            try
            {
                if (RedisUtils.IsConnected)
                    _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.EEC_TELEGRAM, JsonConvert.SerializeObject(eec_telegram));
                //var RowAffected = _historicalDataManager.ExecuteNonQuery(strSQL);

                //if (RowAffected > 0)
                //    return true;
                //else
                //    return false;
                return true;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
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

        public bool ModifyOnHistoricalCache(float[] _BusbarPowers, float[] _FurnacePowers)
        {
            try
            {
                SFSC_EAFPOWER_Str sfsc_eafpower = new SFSC_EAFPOWER_Str();
                //1401.03.24 IranTime
                sfsc_eafpower.TELDATETIME = DateTime.UtcNow.ToIranDateTime();
                sfsc_eafpower.SUMATION = _BusbarPowers[0] + _BusbarPowers[1];
                sfsc_eafpower.POWERGRP1 = _BusbarPowers[0];
                sfsc_eafpower.POWERGRP2 = _BusbarPowers[1];
                sfsc_eafpower.FURNACE1 = _FurnacePowers[0];
                sfsc_eafpower.FURNACE2 = _FurnacePowers[1];
                sfsc_eafpower.FURNACE3 = _FurnacePowers[2];
                sfsc_eafpower.FURNACE4 = _FurnacePowers[3];
                sfsc_eafpower.FURNACE5 = _FurnacePowers[4];
                sfsc_eafpower.FURNACE6 = _FurnacePowers[5];
                sfsc_eafpower.FURNACE7 = _FurnacePowers[6];
                sfsc_eafpower.FURNACE8 = _FurnacePowers[7];
                if (RedisUtils.IsConnected)
                    _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.SFSC_EAFSPOWER, JsonConvert.SerializeObject(sfsc_eafpower));
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return true;
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
                var dataTable = sqlDataMnager.GetRecord(sql);
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
