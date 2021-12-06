using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

using COM;
using Irisa.Logger;
using Irisa.DataLayer;

namespace OCP
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, OCPCheckPoint> _checkPoints;
        private readonly Dictionary<string, OCPCheckPoint> _checkPointHelper;

        private readonly Dictionary<Guid, OCPScadaPoint> _scadaPoints;
        private readonly Dictionary<string, OCPScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;

        private bool LoadfromCache = false;
        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _checkPoints = new Dictionary<Guid, OCPCheckPoint>();
            _checkPointHelper = new Dictionary<string, OCPCheckPoint>();

            _scadaPoints = new Dictionary<Guid, OCPScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, OCPScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }

        public bool Build()
        {
            try
            {
                if (FetchCheckPoints())
                {
                    if (FetchScadaPoints())
                        isBuild = true;
                }
                else if (FetchCheckPointsfromRedis())
                {
                    if (FetchScadaPointsfromRedis())
                        isBuild = true;
                }
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return isBuild;
        }

        private bool FetchCheckPoints()
        {
            _logger.WriteEntry("Loading OCP_Checkpoint Data from Database", LogLevels.Info);
            DataTable dataTable = new DataTable();
            var sql = "SELECT OCPSHEDPOINT_ID, " +
                                                                                    "NAME, " +
                                                                                    "NETWORKPATH, " +
                                                                                    "DECISIONTABLE, " +
                                                                                    "CHECKOVERLOAD, " +
                                                                                    "DESCRIPTION, " +
                                                                                    "SHEDTYPE, " +
                                                                                    "CATEGORY, " +
                                                                                    "NOMINALVALUE, " +
                                                                                    "LIMITPERCENT, " +
                                                                                    "VoltageEnom, " +
                                                                                    "VOLTAGEDENOM, " +
                                                                                    "POWERNUM, " +
                                                                                    "POWERDENOM, " +
                                                                                    "CHECKPOINT_NETWORKPATH " +
                                                                            $"FROM APP_OCP_CheckPoints";
            try
            {

                dataTable = _dataManager.GetRecord(sql);
            }

            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading from database", LogLevels.Error, ex);
                return false;
            }


            foreach (DataRow row in dataTable.Rows)
            {
                var checkPoint = new OCPCheckPoint();

                //checkPoint.MeasurementId = Guid.Parse(Convert.ToString(row["GUID"]));
                checkPoint.Name = Convert.ToString(row["NAME"]);
                checkPoint.NetworkPath = Convert.ToString(row["NetworkPath"]);
                checkPoint.DecisionTable = Convert.ToInt32(row["DECISIONTABLE"]);
                checkPoint.CheckOverload = Convert.ToChar(row["CHECKOVERLOAD"]);
                checkPoint.ShedType = Convert.ToString(row["ShedType"]);
                checkPoint.Category = Convert.ToString(row["CATEGORY"]);
                checkPoint.NominalValue = Convert.ToSingle(row["NominalValue"]);
                checkPoint.LIMITPERCENT = Convert.ToSingle(row["LIMITPERCENT"]);
                //checkPoint.IT_GUID = Guid.Parse(Convert.ToString(row["IT_GUID"]));
                //checkPoint.ALLOWEDACTIVEPOWER_GUID = Guid.Parse(Convert.ToString(row["ALLOWEDACTIVEPOWER_GUID"]));
                //checkPoint.SAMPLE_GUID = Guid.Parse(Convert.ToString(row["SAMPLE_GUID"]));
                //checkPoint.AVERAGE_GUID = Guid.Parse(Convert.ToString(row["AVERAGE_GUID"]));

                checkPoint.MeasurementId = GetGuid(checkPoint.NetworkPath);
                checkPoint.IT_GUID = GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/IT");
                checkPoint.ALLOWEDACTIVEPOWER_GUID = GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/AllowedActivePower");
                checkPoint.SAMPLE_GUID = GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/Sample");
                checkPoint.AVERAGE_GUID = GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/Average");


                checkPoint.AverageQuality = OCPCheckPointQuality.Invalid;
                checkPoint.SubstitutionCounter = 0;
                checkPoint.Overload.Value = 0;
                checkPoint.OverloadFlag = false;
                checkPoint.ResetIT = false;
                checkPoint.FourValueFlag = false;
                checkPoint.Quality1 = OCPCheckPointQuality.Invalid;
                checkPoint.Quality2 = OCPCheckPointQuality.Invalid;
                checkPoint.Quality3 = OCPCheckPointQuality.Invalid;
                checkPoint.Quality4 = OCPCheckPointQuality.Invalid;
                checkPoint.Quality5 = OCPCheckPointQuality.Invalid;
                checkPoint.QualityErrorId = GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/QualityError");
                try
                {
                    if (!_checkPoints.ContainsKey(checkPoint.MeasurementId))
                    {
                        _checkPoints.Add(checkPoint.MeasurementId, checkPoint);
                        _checkPointHelper.Add(checkPoint.Name, checkPoint);
                    }
                    else
                    {
                        _logger.WriteEntry("Error in loading CheckPoint, " + "GUID: " + checkPoint.MeasurementId + "  NetworkPath: " + checkPoint.NetworkPath, LogLevels.Error);
                    }
                }
                catch (Irisa.DataLayer.DataException ex)
                {
                    _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                    return false;
                }
            }
            return true;
        }

        private bool FetchCheckPointsfromRedis()
        {
            _logger.WriteEntry("Loading OCP_Checkpoint Data from Cache", LogLevels.Info);

            var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_CheckPoints);
            var dataTable_cache = _RedisConnectorHelper.StringGet<OCP_CHECKPOINTS_Str>(keys);

            try
            {
                foreach (var row in dataTable_cache)
                {
                    var checkPoint = new OCPCheckPoint();

                    checkPoint.Name = row.NAME;
                    checkPoint.NetworkPath = row.NETWORKPATH;
                    checkPoint.DecisionTable = Convert.ToInt32(row.DECISIONTABLE);
                    checkPoint.CheckOverload = Convert.ToChar(row.CHECKOVERLOAD);
                    checkPoint.ShedType = row.SHEDTYPE;
                    checkPoint.Category = row.CATEGORY;
                    checkPoint.NominalValue = Convert.ToSingle(row.NOMINALVALUE);
                    checkPoint.LIMITPERCENT = Convert.ToSingle(row.LIMITPERCENT);

                    checkPoint.MeasurementId = Guid.Parse(Convert.ToString(row.Measurement_Id));
                    checkPoint.IT_GUID = Guid.Parse(Convert.ToString(row.IT_Id));
                    checkPoint.ALLOWEDACTIVEPOWER_GUID = Guid.Parse(Convert.ToString(row.AllowedActivePower_Id));
                    checkPoint.SAMPLE_GUID = Guid.Parse(Convert.ToString(row.Sample_Id));
                    checkPoint.AVERAGE_GUID = Guid.Parse(Convert.ToString(row.Average_Id));
                    checkPoint.QualityErrorId = Guid.Parse(Convert.ToString(row.QualityErr_Id));


                    checkPoint.AverageQuality = OCPCheckPointQuality.Invalid;
                    checkPoint.SubstitutionCounter = 0;
                    checkPoint.Overload.Value = 0;
                    checkPoint.OverloadFlag = false;
                    checkPoint.ResetIT = false;
                    checkPoint.FourValueFlag = false;
                    checkPoint.Quality1 = OCPCheckPointQuality.Invalid;
                    checkPoint.Quality2 = OCPCheckPointQuality.Invalid;
                    checkPoint.Quality3 = OCPCheckPointQuality.Invalid;
                    checkPoint.Quality4 = OCPCheckPointQuality.Invalid;
                    checkPoint.Quality5 = OCPCheckPointQuality.Invalid;

                    try
                    {
                        if (!_checkPoints.ContainsKey(checkPoint.MeasurementId))
                        {
                            _checkPoints.Add(checkPoint.MeasurementId, checkPoint);
                            _checkPointHelper.Add(checkPoint.Name, checkPoint);
                        }
                        else
                        {
                            _logger.WriteEntry("Error in loading CheckPoint, " + "GUID: " + checkPoint.MeasurementId + "  NetworkPath: " + checkPoint.NetworkPath, LogLevels.Error);
                        }
                    }
                    catch (Irisa.DataLayer.DataException ex)
                    {
                        _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error load data from Cache database", LogLevels.Error, ex);
                return false;
            }
            return true;
        }

        public OCPCheckPoint GetCheckPoint(Guid measurementId)
        {
            if (_checkPoints.TryGetValue(measurementId, out var checkPoint))
                return checkPoint;
            else
                return null;
        }

        public OCPCheckPoint GetCheckPoint(String name)
        {
            if (_checkPointHelper.TryGetValue(name, out var checkPoint))
                return checkPoint;
            else
                return null;
        }

        public IEnumerable<OCPCheckPoint> GetCheckPoints()
        {
            return _checkPoints.Values;
        }

        public OCPScadaPoint GetOCPScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public OCPScadaPoint GetOCPScadaPoint(String name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var oCPParam))
                return oCPParam;
            else
                return null;
        }

        public IEnumerable<OCPScadaPoint> GetOCPScadaPoints()
        {
            return _scadaPoints.Values;
        }

        private bool FetchScadaPoints()
        {
            try
            {

                OCP_PARAMS_Str ocp_param = new OCP_PARAMS_Str();
                var dataTable = _dataManager.GetRecord($"SELECT * FROM APP_OCP_PARAMS");
                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var name = row["Name"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
                    //var pointDirectionType = row["DirectionType"].ToString();
                    var id = GetGuid(networkPath);
                    var scadaPoint = new OCPScadaPoint(id, name, networkPath);
                    scadaPoint.DirectionType = Convert.ToString(row["DirectionType"]);
                    scadaPoint.SCADAType = Convert.ToString(row["SCADAType"]);

                    ocp_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                    ocp_param.NAME = name;
                    ocp_param.DIRECTIONTYPE = scadaPoint.DirectionType;
                    ocp_param.NETWORKPATH = networkPath;
                    ocp_param.SCADATYPE = scadaPoint.SCADAType;
                    ocp_param.ID = id.ToString();
                    if (RedisUtils.IsConnected)
                        _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.OCP_PARAMS + networkPath, JsonConvert.SerializeObject(ocp_param));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);


                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }
                    else
                    {
                        _logger.WriteEntry("Error in loading scadaPoint, " + "GUID: " + id + "  NetworkPath: " + networkPath, LogLevels.Error);
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

        private bool FetchScadaPointsfromRedis()
        {
            _logger.WriteEntry("Loading LSP_PARAMS Data from Cache", LogLevels.Info);

            var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_PARAMS);
            var dataTable_cache = _RedisConnectorHelper.StringGet<OCP_PARAMS_Str>(keys);
            try
            {
                foreach (OCP_PARAMS_Str row in dataTable_cache)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    var name = row.NAME;
                    var networkPath = row.NETWORKPATH;

                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new OCPScadaPoint(id, name, networkPath);
                        scadaPoint.DirectionType = row.DIRECTIONTYPE;
                        scadaPoint.SCADAType = row.SCADATYPE;

                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            _logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
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