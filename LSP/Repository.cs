using COMMON;
using Irisa.DataLayer;
using Irisa.DataLayer.Oracle;
using Irisa.DataLayer.SqlServer;
using Irisa.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LSP
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _staticDataManager;
        private readonly SqlServerDataManager _linkDBpcsDataManager;
        private readonly DataManager _historicalDataManager;
        private readonly Dictionary<Guid, OCPCheckPoint> _checkPoints;
        private readonly Dictionary<string, OCPCheckPoint> _checkPointHelper;
        private readonly Dictionary<short, OCPCheckPoint> _checkPointHelperNumber;

        private readonly Dictionary<Guid, LSPScadaPoint> _scadaPoints;
        private readonly Dictionary<string, LSPScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RTDBManager;

        private bool isBuild = false;

        public Repository(ILogger logger, IConfiguration config, RedisUtils RTDBManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            _staticDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
            _historicalDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleHISUser"], config["OracleHISPassword"]);

            _linkDBpcsDataManager = new SqlServerDataManager(config["PCSLinkDatabaseName"], config["PCSLinkDatabaseAddress"], config["PCSLinkUser"], config["PCSLinkPassword"]);
            _checkPoints = new Dictionary<Guid, OCPCheckPoint>();
            _checkPointHelper = new Dictionary<string, OCPCheckPoint>();
            _checkPointHelperNumber = new Dictionary<short, OCPCheckPoint>();

            _scadaPoints = new Dictionary<Guid, LSPScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, LSPScadaPoint>();
            _RTDBManager = RTDBManager ?? throw new ArgumentNullException(nameof(RTDBManager));
        }


        public bool Build()
        {

            try
            {
                // Important note: Always CheckPoints should be loaded before ScadaPoints
                if (FetchCheckPoints())
                {
                    if (FetchScadaPoints())
                    {
                        if (BuildCashe())
                            isBuild = true;

                    }
                }
                else if (FetchCheckPointsfromRedis())
                {
                    if (FetchScadaPointsfromRedis())
                    {
                        isBuild = true;
                    }
                }

                _staticDataManager.Close();
                _linkDBpcsDataManager.Close();


            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                isBuild = false;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                isBuild = false;
            }

            return isBuild;
        }

        private bool FetchCheckPoints()
        {

            _logger.WriteEntry("Loading Data from Database", LogLevels.Info);
            DataTable dataTable = new DataTable();

            try
            {
                //dataTable = _staticDataManager.GetRecord("SELECT OCPSHEDPOINT_ID, " +
                //                                                                       //  "GUID, " +
                //                                                                       "NAME, " +
                //                                                                       "NETWORKPATH, " +
                //                                                                       "DECISIONTABLE, " +
                //                                                                       "CHECKOVERLOAD," +
                //                                                                       " DESCRIPTION, " +
                //                                                                       "SHEDTYPE, " +
                //                                                                       "CATEGORY, " +
                //                                                                       "NOMINALVALUE, " +
                //                                                                       "LIMITPERCENT, " +
                //                                                                       "VOLTAGEENOM, " +
                //                                                                       "VOLTAGEDENOM, " +
                //                                                                       "POWERNUM, " +
                //                                                                       "POWERDENOM, " +
                //                                                                       //"IT_GUID, " +
                //                                                                       //"ALLOWEDACTIVEPOWER_GUID, " +
                //                                                                       //"SAMPLE_GUID, " +
                //                                                                       //"AVERAGE_GUID, " +
                //                                                                       "CHECKPOINT_NETWORKPATH " +
                //                                                               $"FROM  APP_OCP_CHECKPOINTS");
                dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_CHECKPOINTS_SELECT", CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.OCP_CheckPoints))
                        _logger.WriteEntry("Error: Delete APP_OCP_CheckPoints", LogLevels.Error);
                }
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading Data from database ", LogLevels.Error, ex);
                return false;
            }

            OCP_CHECKPOINTS_Str ocp_checkpoint_obj = new OCP_CHECKPOINTS_Str();

            foreach (DataRow row in dataTable.Rows)
            {
                var checkPoint = new OCPCheckPoint();

                checkPoint.CheckPointNumber = Convert.ToInt32(row["OCPSHEDPOINT_ID"]);
                //checkPoint.MeasurementId = Guid.Parse(Convert.ToString(row["GUID"]));
                checkPoint.Name = Convert.ToString(row["NAME"]);
                checkPoint.NetworkPath = Convert.ToString(row["NetworkPath"]);
                checkPoint.DecisionTable = Convert.ToInt32(row["DECISIONTABLE"]);
                checkPoint.CheckOverload = Convert.ToChar(row["CHECKOVERLOAD"]);
                checkPoint.ShedType = Convert.ToString(row["ShedType"]);
                checkPoint.Category = Convert.ToString(row["CATEGORY"]);
                checkPoint.NominalValue = Convert.ToSingle(row["NominalValue"]);
                checkPoint.LimitPercent = Convert.ToSingle(row["LIMITPERCENT"]);

                checkPoint.VoltageEnom = Convert.ToSingle(row["VoltageEnom"]);
                checkPoint.VoltageDenom = Convert.ToSingle(row["VoltageDenom"]);

                checkPoint.AverageQuality = CheckPointQuality.Invalid;
                checkPoint.SubstitutionCounter = 0;
                checkPoint.OverloadIT.Value = 0;
                checkPoint.OverloadFlag = false;
                checkPoint.ResetIT = false;
                checkPoint.FourValueFlag = false;
                checkPoint.Quality1 = CheckPointQuality.Invalid;
                checkPoint.Quality2 = CheckPointQuality.Invalid;
                checkPoint.Quality3 = CheckPointQuality.Invalid;
                checkPoint.Quality4 = CheckPointQuality.Invalid;
                checkPoint.Quality5 = CheckPointQuality.Invalid;
                checkPoint.MeasurementId = GetGuid(checkPoint.NetworkPath);


                var scadaPoint1 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/IT"), "IT", checkPoint.NetworkPath + "/IT");
                scadaPoint1.DirectionType = "INPUT";
                scadaPoint1.SCADAType = "AnalogMeasurement";
                checkPoint.OverloadIT = scadaPoint1;

                var scadaPoint2 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/AllowedActivePower"), "AAP", checkPoint.NetworkPath + "/AllowedActivePower");
                scadaPoint2.DirectionType = "INPUT";
                scadaPoint2.SCADAType = "AnalogMeasurement";
                checkPoint.ActivePower = scadaPoint2;

                var scadaPoint3 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/Average"), "AVERAGE", checkPoint.NetworkPath + "/Average");
                scadaPoint3.DirectionType = "INPUT";
                scadaPoint3.SCADAType = "AnalogMeasurement";
                checkPoint.Average = scadaPoint3;

                var scadaPoint4 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/Sample"), "Sample", checkPoint.NetworkPath + "/Sample");
                scadaPoint4.DirectionType = "INPUT";
                scadaPoint4.SCADAType = "AnalogMeasurement";
                checkPoint.Sample = scadaPoint4;

                var scadaPoint5 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/QualityError"), "QualityError", checkPoint.NetworkPath + "/QualityError");
                scadaPoint5.DirectionType = "INPUT";
                scadaPoint5.SCADAType = "AnalogMeasurement";
                checkPoint.QulityError = scadaPoint5;

                ocp_checkpoint_obj.OCPSHEDPOINT_ID = Convert.ToInt32(row["OCPSHEDPOINT_ID"]);
                ocp_checkpoint_obj.NAME = Convert.ToString(row["NAME"]);
                ocp_checkpoint_obj.NETWORKPATH = Convert.ToString(row["NetworkPath"]);
                ocp_checkpoint_obj.DECISIONTABLE = Convert.ToString(row["DECISIONTABLE"]);
                ocp_checkpoint_obj.CHECKOVERLOAD = Convert.ToString(row["CHECKOVERLOAD"]);
                ocp_checkpoint_obj.DESCRIPTION = Convert.ToString(row["DESCRIPTION"]);
                ocp_checkpoint_obj.SHEDTYPE = Convert.ToString(row["ShedType"]);
                ocp_checkpoint_obj.CATEGORY = Convert.ToString(row["CATEGORY"]);
                ocp_checkpoint_obj.NOMINALVALUE = Convert.ToSingle(row["NominalValue"]);
                ocp_checkpoint_obj.LIMITPERCENT = Convert.ToString(row["LIMITPERCENT"]);
                ocp_checkpoint_obj.VOLTAGEENOM = Convert.ToString(row["VoltageEnom"]);
                ocp_checkpoint_obj.VOLTAGEDENOM = Convert.ToString(row["VoltageDenom"]);
                ocp_checkpoint_obj.POWERNUM = Convert.ToString(row["POWERNUM"]);
                ocp_checkpoint_obj.POWERDENOM = Convert.ToString(row["POWERDENOM"]);
                ocp_checkpoint_obj.CHECKPOINT_NETWORKPATH = Convert.ToString(row["CHECKPOINT_NETWORKPATH"]);
                ocp_checkpoint_obj.Measurement_Id = checkPoint.MeasurementId.ToString();
                ocp_checkpoint_obj.IT_Id = scadaPoint1.Id.ToString();
                ocp_checkpoint_obj.AllowedActivePower_Id = scadaPoint2.Id.ToString();
                ocp_checkpoint_obj.Average_Id = scadaPoint3.Id.ToString();
                ocp_checkpoint_obj.Sample_Id = scadaPoint4.Id.ToString();
                ocp_checkpoint_obj.QualityErr_Id = scadaPoint5.Id.ToString();


                try
                {
                    if (_RTDBManager.IsConnected)
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.OCP_CheckPoints + ocp_checkpoint_obj.OCPSHEDPOINT_ID, JsonConvert.SerializeObject(ocp_checkpoint_obj));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);


                    if (checkPoint.MeasurementId != Guid.Empty)
                    {
                        if (!_checkPoints.ContainsKey(checkPoint.MeasurementId))
                        {
                            _checkPoints.Add(checkPoint.MeasurementId, checkPoint);
                            _checkPointHelper.Add(checkPoint.Name, checkPoint);
                            _checkPointHelperNumber.Add(Convert.ToInt16(checkPoint.CheckPointNumber), checkPoint);
                        }
                        else
                        {
                            _logger.WriteEntry("Error in loading CheckPoint, " + "GUID: " + checkPoint.MeasurementId + "  NetworkPath: " + checkPoint.NetworkPath, LogLevels.Error);
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
                    _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                    return false;
                }
            }
            return true;

        }

        private bool FetchCheckPointsfromRedis()
        {
            _logger.WriteEntry("Loading OCP_Checkpoint Data from Cache", LogLevels.Info);

            try
            {
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.OCP_CheckPoints);
                var dataTable_cache = _RTDBManager.StringGet<OCP_CHECKPOINTS_Str>(keys);
                foreach (var row in dataTable_cache)
                {
                    var checkPoint = new OCPCheckPoint();

                    checkPoint.CheckPointNumber = Convert.ToInt32(row.OCPSHEDPOINT_ID);
                    checkPoint.Name = Convert.ToString(row.NAME);
                    checkPoint.NetworkPath = Convert.ToString(row.NETWORKPATH);
                    checkPoint.DecisionTable = Convert.ToInt32(row.DECISIONTABLE);
                    checkPoint.CheckOverload = Convert.ToChar(row.CHECKOVERLOAD);
                    checkPoint.ShedType = Convert.ToString(row.SHEDTYPE);
                    checkPoint.Category = Convert.ToString(row.CATEGORY);
                    checkPoint.NominalValue = Convert.ToSingle(row.NOMINALVALUE);
                    checkPoint.LimitPercent = Convert.ToSingle(row.LIMITPERCENT);

                    checkPoint.VoltageEnom = Convert.ToSingle(row.VOLTAGEENOM);
                    checkPoint.VoltageDenom = Convert.ToSingle(row.VOLTAGEDENOM);

                    checkPoint.AverageQuality = CheckPointQuality.Invalid;
                    checkPoint.SubstitutionCounter = 0;
                    checkPoint.OverloadIT.Value = 0;
                    checkPoint.OverloadFlag = false;
                    checkPoint.ResetIT = false;
                    checkPoint.FourValueFlag = false;
                    checkPoint.Quality1 = CheckPointQuality.Invalid;
                    checkPoint.Quality2 = CheckPointQuality.Invalid;
                    checkPoint.Quality3 = CheckPointQuality.Invalid;
                    checkPoint.Quality4 = CheckPointQuality.Invalid;
                    checkPoint.Quality5 = CheckPointQuality.Invalid;
                    checkPoint.MeasurementId = Guid.Parse(Convert.ToString(row.Measurement_Id));

                    var scadaPoint1 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row.IT_Id)), "IT", checkPoint.NetworkPath + "/IT");
                    scadaPoint1.DirectionType = "INPUT";
                    scadaPoint1.SCADAType = "AnalogMeasurement";
                    checkPoint.OverloadIT = scadaPoint1;

                    var scadaPoint2 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row.AllowedActivePower_Id)), "AAP", checkPoint.NetworkPath + "/AllowedActivePower");
                    scadaPoint2.DirectionType = "INPUT";
                    scadaPoint2.SCADAType = "AnalogMeasurement";
                    checkPoint.ActivePower = scadaPoint2;

                    var scadaPoint3 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row.Average_Id)), "AVERAGE", checkPoint.NetworkPath + "/Average");
                    scadaPoint3.DirectionType = "INPUT";
                    scadaPoint3.SCADAType = "AnalogMeasurement";
                    checkPoint.Average = scadaPoint3;

                    try
                    {
                        if (checkPoint.MeasurementId != Guid.Empty)
                        {
                            if (!_checkPoints.ContainsKey(checkPoint.MeasurementId))
                            {
                                _checkPoints.Add(checkPoint.MeasurementId, checkPoint);
                                _checkPointHelper.Add(checkPoint.Name, checkPoint);
                                _checkPointHelperNumber.Add(Convert.ToInt16(checkPoint.CheckPointNumber), checkPoint);
                            }
                            else
                            {
                                _logger.WriteEntry("Error in loading CheckPoint, " + "GUID: " + checkPoint.MeasurementId + "  NetworkPath: " + checkPoint.NetworkPath, LogLevels.Error);
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

        public OCPCheckPoint GetCheckPoint(int checkPointNumber)
        {
            if (_checkPointHelperNumber.TryGetValue(Convert.ToInt16(checkPointNumber), out var checkPoint))
                return checkPoint;
            else
                return null;
        }

        public IEnumerable<OCPCheckPoint> GetCheckPoints()
        {
            return _checkPoints.Values;
        }

        public LSPScadaPoint GetLSPScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public LSPScadaPoint GetLSPScadaPoint(String name)
        {
            if (_scadaPointsHelper.TryGetValue(name, out var oCPParam))
                return oCPParam;
            else
                return null;
        }

        public IEnumerable<LSPScadaPoint> GetLSPScadaPoints()
        {
            return _scadaPoints.Values;
        }

        private bool FetchScadaPoints()
        {
            var fetchedData = false;

            fetchedData = FetchLspParam();
            if (fetchedData == false) return false;

            fetchedData = FetchDectItems();
            if (fetchedData == false) return false;

            fetchedData = FetchPriorityItems();
            if (fetchedData == false) return false;

            fetchedData = FetchEecEafsPriorityListItems();
            if (fetchedData == false) return false;
            return true;
        }

        private bool FetchScadaPointsfromRedis()
        {
            var fetchedData = false;

            fetchedData = FetchLspParamfromRedis();
            if (fetchedData == false) return false;

            fetchedData = FetchDectItemsfromRedis();
            if (fetchedData == false) return false;

            fetchedData = FetchPriorityItemsfromRedis();
            if (fetchedData == false) return false;

            fetchedData = FetchEecEafsPriorityListItemsfromRedis();
            if (fetchedData == false) return false;
            return true;
        }

        private bool FetchLspParam()
        {
            try
            {
                LSP_PARAMS_Str lsp_param = new LSP_PARAMS_Str();

                //var dataTable = _staticDataManager.GetRecord($"SELECT * FROM APP_LSP_PARAMS");
                var dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_PARAMS_SELECT", CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_PARAMS))
                        _logger.WriteEntry("Error: Delete APP_LSP_PARAMS from Redis", LogLevels.Error);
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var id = Guid.Parse(row["GUID"].ToString());
                    var name = row["NAME"].ToString();
                    var networkPath = row["NetworkPath"].ToString();

                    lsp_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                    lsp_param.NAME = name;
                    lsp_param.DESCRIPTION = row["DESCRIPTION"].ToString();
                    lsp_param.DIRECTIONTYPE = row["DIRECTIONTYPE"].ToString();
                    lsp_param.NETWORKPATH = networkPath;
                    lsp_param.SCADATYPE = row["SCADATYPE"].ToString();
                    lsp_param.ID = id.ToString();
                    if (_RTDBManager.IsConnected)
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_PARAMS + networkPath, JsonConvert.SerializeObject(lsp_param));



                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new LSPScadaPoint(id, name, networkPath);
                        scadaPoint.DirectionType = Convert.ToString(row["DirectionType"]);
                        scadaPoint.SCADAType = Convert.ToString(row["SCADAType"]);

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

        private bool FetchLspParamfromRedis()
        {
            _logger.WriteEntry("Loading LSP_PARAMS Data from Cache", LogLevels.Info);

            try
            {
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.LSP_PARAMS);
                var dataTable_cache = _RTDBManager.StringGet<LSP_PARAMS_Str>(keys);
                foreach (LSP_PARAMS_Str row in dataTable_cache)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    var name = row.NAME;
                    var networkPath = row.NETWORKPATH;

                    if (id != Guid.Empty)
                    {
                        var scadaPoint = new LSPScadaPoint(id, name, networkPath);
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

        private bool FetchDectItems()
        {
            try
            {

                LSP_DECTITEMS_Str lsp_dectitems = new LSP_DECTITEMS_Str();
                //var dataTable = _staticDataManager.GetRecord($"SELECT * FROM APP_LSP_DECTITEMS");
                var dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_DECTITEMS_SELECT",CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_DECTITEMS))
                        _logger.WriteEntry("Error: Delete APP_LSP_DECTITEMS from Redis", LogLevels.Error);
                }


                foreach (DataRow row in dataTable.Rows)
                {
                    var id = GetGuid(row["NetworkPath"].ToString());
                    //if (Guid.TryParse(row["GUID"].ToString(), out var id))
                    if (id != Guid.Empty)
                    {
                        var name = row["Name"].ToString();
                        var networkPath = row["NetworkPath"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        lsp_dectitems.NAME = name;
                        lsp_dectitems.NETWORKPATH = networkPath;
                        lsp_dectitems.ID = id.ToString();
                        lsp_dectitems.DECTNO = Convert.ToInt32(row["DECTNO"]);
                        lsp_dectitems.DECTITEMNO = Convert.ToInt32(row["DECTITEMNO"]);
                        if (_RTDBManager.IsConnected)
                            _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_DECTITEMS + row["DECTNO"].ToString() + "-" + row["DECTITEMNO"].ToString(), JsonConvert.SerializeObject(lsp_dectitems));

                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            // _logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
                        }
                    }
                }

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

        private bool FetchDectItemsfromRedis()
        {
            _logger.WriteEntry("Loading LSP_DECTITEMS Data from Cache", LogLevels.Info);
            
            try
            {
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.LSP_DECTITEMS);
                var dataTable = _RTDBManager.StringGet<LSP_DECTITEMS_Str>(keys);
                foreach (LSP_DECTITEMS_Str row in dataTable)
                {
                    var id = Guid.Parse((row.ID).ToString());
                    //if (Guid.TryParse(row["GUID"].ToString(), out var id))
                    if (id != Guid.Empty)
                    {
                        var name = row.NAME;
                        var networkPath = row.NETWORKPATH;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            // _logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
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

        private bool FetchPriorityItems()
        {
            try
            {
                LSP_PRIORITYITEMS_Str lsp_priorityitems = new LSP_PRIORITYITEMS_Str();
                //var dataTable = _staticDataManager.GetRecord($"SELECT * FROM APP_LSP_PRIORITYITEMS ORDER BY PRIORITYLISTNO, ITEMNO");
                var dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_PRIORITY_ITEMS_SELECT", CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_PRIORITYITEMS))
                        _logger.WriteEntry("Error: Delete APP_LSP_PRIORITYITEMS from Redis", LogLevels.Error);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    var id_curr = GetGuid(row["NETWORKPATH_CURR"].ToString());
                    var id_cb = GetGuid(row["NETWORKPATH_ITEM"].ToString());
                    Guid id_cb_partner = Guid.Empty;
                    if (row["ADDRESSPARTNER"].ToString() != "NULL")
                        id_cb_partner = GetGuid(row["ADDRESSPARTNER"].ToString());

                    lsp_priorityitems.ID_CURR = id_curr.ToString();
                    lsp_priorityitems.ID_CB = id_cb.ToString();
                    lsp_priorityitems.ID_CB_PARTNER = id_cb_partner.ToString();
                    lsp_priorityitems.PRIORITYLISTNO = Convert.ToInt32(row["PRIORITYLISTNO"].ToString());
                    lsp_priorityitems.ITEMNO = Convert.ToInt32(row["ITEMNO"].ToString());
                    lsp_priorityitems.NETWORKPATH_CURR = row["NETWORKPATH_CURR"].ToString();
                    lsp_priorityitems.NETWORKPATH_ITEM = row["NETWORKPATH_ITEM"].ToString();
                    lsp_priorityitems.DESCRIPTION = row["DESCRIPTION"].ToString();
                    lsp_priorityitems.HASPARTNER = row["HASPARTNER"].ToString();
                    lsp_priorityitems.ADDRESSPARTNER = row["ADDRESSPARTNER"].ToString();

                    if (_RTDBManager.IsConnected)
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_PRIORITYITEMS + row["PRIORITYLISTNO"].ToString() + "-" + row["ITEMNO"].ToString(), JsonConvert.SerializeObject(lsp_priorityitems));

                    //if (Guid.TryParse(row["GUID_CURR"].ToString(), out var id))
                    if (id_curr != Guid.Empty)
                    {
                        var name = row["NETWORKPATH_CURR"].ToString();
                        var networkPath = row["NETWORKPATH_CURR"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_curr, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "AnalogMeasurement";

                        if (!_scadaPoints.ContainsKey(id_curr))
                        {
                            _scadaPoints.Add(id_curr, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }

                    //if (Guid.TryParse(row["GUID_ITEM"].ToString(), out id_cb))
                    if (id_cb != Guid.Empty)
                    {
                        var name = row["NETWORKPATH_ITEM"].ToString();
                        var networkPath = row["NETWORKPATH_ITEM"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_cb, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_cb))
                        {
                            _scadaPoints.Add(id_cb, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
                        }
                    }
                }


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

        private bool FetchPriorityItemsfromRedis()
        {
            _logger.WriteEntry("Loading LSP_PRIORITYITEMS Data from Cache", LogLevels.Info);

            try
            {
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.LSP_PRIORITYITEMS);
                var dataTable = _RTDBManager.StringGet<LSP_PRIORITYITEMS_Str>(keys);
                foreach (LSP_PRIORITYITEMS_Str row in dataTable)
                {
                    var id_curr = Guid.Parse((row.ID_CURR).ToString());
                    var id_cb = Guid.Parse((row.ID_CB).ToString());


                    //if (Guid.TryParse(row["GUID_CURR"].ToString(), out var id))
                    if (id_curr != Guid.Empty)
                    {
                        var name = row.NETWORKPATH_CURR;
                        var networkPath = row.NETWORKPATH_CURR;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_curr, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "AnalogMeasurement";

                        if (!_scadaPoints.ContainsKey(id_curr))
                        {
                            _scadaPoints.Add(id_curr, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }

                    //if (Guid.TryParse(row["GUID_ITEM"].ToString(), out id_cb))
                    if (id_cb != Guid.Empty)
                    {
                        var name = row.NETWORKPATH_ITEM;
                        var networkPath = row.NETWORKPATH_ITEM;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_cb, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_cb))
                        {
                            _scadaPoints.Add(id_cb, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
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
            return true; ;


        }

        private bool FetchEecEafsPriorityListItems()
        {
            try
            {

                EEC_EAFSPRIORITY_Str eec_eafpriority = new EEC_EAFSPRIORITY_Str();
                //var dataTable = _staticDataManager.GetRecord($"SELECT * FROM APP_EEC_EAFSPRIORITY ORDER BY Furnace");
                var dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_EEC_EAFS_PRIORITY_SELECT", CommandType.StoredProcedure);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.EEC_EAFSPriority))
                        _logger.WriteEntry("Error: Delete APP_EEC_EAFSPRIORITY from Redis", LogLevels.Error);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    var id_CB = GetGuid(row["CB_NETWORKPATH"].ToString());
                    var id_CT = GetGuid(row["CT_NETWORKPATH"].ToString());
                    var id_CB_Partner = GetGuid(row["PARTNERADDRESS"].ToString());

                    eec_eafpriority.ID_CB = id_CB.ToString();
                    eec_eafpriority.ID_CT = id_CT.ToString();
                    eec_eafpriority.ID_CB_PARTNER = id_CB_Partner.ToString();
                    eec_eafpriority.CB_NETWORKPATH = row["CB_NETWORKPATH"].ToString();
                    eec_eafpriority.CT_NETWORKPATH = row["CT_NETWORKPATH"].ToString();
                    eec_eafpriority.HASPARTNER = row["HASPARTNER"].ToString();
                    eec_eafpriority.PARTNERADDRESS = row["PARTNERADDRESS"].ToString();
                    eec_eafpriority.FURNACE = row["FURNACE"].ToString();
                    if (_RTDBManager.IsConnected)
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.EEC_EAFSPriority + row["CB_NETWORKPATH"].ToString(), JsonConvert.SerializeObject(eec_eafpriority));




                    //if (Guid.TryParse(row["CB_GUID"].ToString(), out var id))
                    if (id_CB != Guid.Empty)
                    {
                        var name = row["CB_NETWORKPATH"].ToString();
                        var networkPath = row["CB_NETWORKPATH"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CB, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CB))
                        {
                            _scadaPoints.Add(id_CB, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }
                    //if (Guid.TryParse(row["CT_GUID"].ToString(), out id))
                    if (id_CT != Guid.Empty)
                    {
                        var name = row["CT_NETWORKPATH"].ToString();
                        var networkPath = row["CT_NETWORKPATH"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CT, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "AnalogMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CT))
                        {
                            _scadaPoints.Add(id_CT, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
                        }
                    }
                    //if (Guid.TryParse(row["PARTNER_GUID"].ToString(), out id))
                    if (id_CB_Partner != Guid.Empty)
                    {
                        var name = row["PARTNERADDRESS"].ToString();
                        var networkPath = row["PARTNERADDRESS"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CB_Partner, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CB_Partner))
                        {
                            _scadaPoints.Add(id_CB_Partner, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
                        }
                    }

                }

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

        private bool FetchEecEafsPriorityListItemsfromRedis()
        {
            _logger.WriteEntry("Loading EEC_EAFSPRIORITY Data from Cache", LogLevels.Info);
            
            try
            {
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.EEC_EAFSPriority);
                var dataTable = _RTDBManager.StringGet<EEC_EAFSPRIORITY_Str>(keys);
                foreach (EEC_EAFSPRIORITY_Str row in dataTable)
                {
                    var id_CB = Guid.Parse((row.ID_CB).ToString());
                    var id_CT = Guid.Parse((row.ID_CT).ToString());
                    var id_CB_Partner = Guid.Parse((row.ID_CB_PARTNER).ToString());

                    if (id_CB != Guid.Empty)
                    {
                        var name = row.CB_NETWORKPATH;
                        var networkPath = row.CB_NETWORKPATH;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CB, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CB))
                        {
                            _scadaPoints.Add(id_CB, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                    }
                    if (id_CT != Guid.Empty)
                    {
                        var name = row.CT_NETWORKPATH;
                        var networkPath = row.CT_NETWORKPATH;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CT, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "AnalogMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CT))
                        {
                            _scadaPoints.Add(id_CT, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
                        }
                    }
                    if (id_CB_Partner != Guid.Empty)
                    {
                        var name = row.PARTNERADDRESS;
                        var networkPath = row.PARTNERADDRESS;
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id_CB_Partner, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id_CB_Partner))
                        {
                            _scadaPoints.Add(id_CB_Partner, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
                        }
                        else
                        {
                            ;
                            //_logger.WriteEntry(networkPath + " already exist in repository", LogLevels.Error);
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

        public bool BuildCashe()
        {
            try
            {
                if (_RTDBManager.IsConnected)
                {
                    DataTable dataTable = null;

                    //dataTable = _staticDataManager.GetRecord($"SELECT * from APP_LSP_DECTCOMB");
                    dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_DECTCOMB_SELECT", CommandType.StoredProcedure);
                    if (dataTable != null)
                    {
                        if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_DECTCOMB))
                            _logger.WriteEntry("Error: Delete APP_LSP_DECTCOMB from Redis", LogLevels.Error);
                    }
                    //var ff1 = JsonConvert.SerializeObject(dataTable);
                    //var ff2 = JsonConvert.DeserializeObject<DataTable>(ff1);
                    _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_DECTCOMB, JsonConvert.SerializeObject(dataTable));

                    //------------------------------------------------------------------------------
                    //dataTable = _staticDataManager.GetRecord($"SELECT * from APP_LSP_DECTLIST");
                    dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_DECTLIST_SELECT", CommandType.StoredProcedure);
                    if (dataTable != null)
                    {
                        if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_DECTLIST))
                            _logger.WriteEntry("Error: Delete APP_LSP_DECTLIST from Redis", LogLevels.Error);
                    }
                    _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_DECTLIST, JsonConvert.SerializeObject(dataTable));

                    //-------------------------------------------------------------------------------
                    //dataTable = _staticDataManager.GetRecord($"SELECT * from APP_LSP_DECTPRIOLS");
                    dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_DECTPRIOLS_SELECT", CommandType.StoredProcedure);

                    if (dataTable != null)
                    {
                        if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_DECTPRIOLS))
                            _logger.WriteEntry("Error: Delete APP_LSP_DECTPRIOLS from Redis", LogLevels.Error);
                    }
                    _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_DECTPRIOLS, JsonConvert.SerializeObject(dataTable));

                    //-------------------------------------------------------------------------------
                    //dataTable = _staticDataManager.GetRecord($"SELECT * from APP_LSP_PRIORITYLIST");
                    dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_LSP_PRIORITY_LIST_SELECT", CommandType.StoredProcedure);
                    if (dataTable != null)
                    {
                        if (!_RTDBManager.DelKeys(RedisKeyPattern.LSP_PRIORITYLIST))
                            _logger.WriteEntry("Error: Delete APP_LSP_PRIORITYLIST from Redis", LogLevels.Error);
                    }
                    _RTDBManager.RedisConn.Set(RedisKeyPattern.LSP_PRIORITYLIST, JsonConvert.SerializeObject(dataTable));

                }


                //foreach (DataRow dr in dataTable.Rows)
                //{
                //    if (RedisUtils.IsConnected)
                //        _cache.StringSet(RedisKeyPattern.LSP_DECTCOMB + dr["DECTNO"].ToString() +"-"+ dr["COMBINATIONNO"].ToString() + "-" + dr["DECTITEMNO"].ToString(), Convert_DataRowToJson(dr));
                //}

                //dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTLIST");
                //foreach (DataRow dr in dataTable.Rows)
                //{
                //    if (RedisUtils.IsConnected)
                //        _cache.StringSet(RedisKeyPattern.LSP_DECTLIST + dr["DECTNO"].ToString(), Convert_DataRowToJson(dr));
                //}

                //dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTPRIOLS");
                //foreach (DataRow dr in dataTable.Rows)
                //{
                //    if (RedisUtils.IsConnected)
                //        _cache.StringSet(RedisKeyPattern.LSP_DECTPRIOLS + dr["DECTNO"].ToString()+"-"+ dr["COMBINATIONNO"].ToString(), Convert_DataRowToJson(dr));
                //}

                //dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_PRIORITYLIST");
                //foreach (DataRow dr in dataTable.Rows)
                //{
                //    if (RedisUtils.IsConnected)
                //        _cache.StringSet(RedisKeyPattern.LSP_PRIORITYLIST + dr["PRIORITYLISTNO"].ToString()+"-"+ dr["NITEMS"].ToString(), Convert_DataRowToJson(dr));
                //}

                return true;
            }


            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }

        }

        public static string Convert_DataRowToJson(DataRow datarow)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in datarow.Table.Columns)
            {
                dict.Add(col.ColumnName, datarow[col]);
            }
            return JsonConvert.SerializeObject(datarow);
        }

        public DataTable FetchDecisionTables()
        {
            DataTable dataTable = null;
            try
            {
                //if(LoadfromCache)
                dataTable = JsonConvert.DeserializeObject<DataTable>(_RTDBManager.RedisConn.Get(RedisKeyPattern.LSP_DECTLIST));
                //else
                //    dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTLIST ORDER BY DECTNO");

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public IEnumerable<LSP_DECTITEMS_Str> FetchItems(byte decisionTableNo)
        {
            DataTable dataTable = null;
            IEnumerable<LSP_DECTITEMS_Str> row = null;

            try
            {
                //if (LoadfromCache)
                //{
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.LSP_DECTITEMS);
                row = _RTDBManager.StringGet<LSP_DECTITEMS_Str>(keys).ToArray();
                //}
                //else
                //    dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTITEMS WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY DECTITEMNO ");

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return row;
        }

        public DataTable FetchCombinations(byte decisionTableNo)
        {
            DataTable dataTable = null;

            try
            {
                //if (LoadfromCache)
                dataTable = JsonConvert.DeserializeObject<DataTable>(_RTDBManager.RedisConn.Get(RedisKeyPattern.LSP_DECTCOMB));
                //else
                //dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTCOMB WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY COMBINATIONNO, DECTITEMNO");

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public DataTable FetchPriorityListsNoForCombinations(byte decisionTableNo)
        {
            DataTable dataTable = null;

            try
            {
                //if (LoadfromCache)
                dataTable = JsonConvert.DeserializeObject<DataTable>(_RTDBManager.RedisConn.Get(RedisKeyPattern.LSP_DECTPRIOLS));
                //else
                //{
                //    string strSQL = $"SELECT * FROM {GetEndStringCommand()}LSP_DECTPRIOLS WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY COMBINATIONNO";
                //    dataTable = _staticDataManager.GetRecord(strSQL);
                //}

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public IEnumerable<LSP_PRIORITYITEMS_Str> FetchBreakersToShed(byte priorityListNo)
        {
            DataTable dataTable = null;
            IEnumerable<LSP_PRIORITYITEMS_Str> row = null;

            try
            {
                //if (LoadfromCache)
                //{
                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.LSP_PRIORITYITEMS);
                row = _RTDBManager.StringGet<LSP_PRIORITYITEMS_Str>(keys).ToArray();
                //}
                //else
                //{
                //    string strSQL = $"SELECT * FROM {GetEndStringCommand()}LSP_PRIORITYITEMS WHERE PRIORITYLISTNO = " + priorityListNo.ToString() + " ORDER BY ITEMNO ";
                //    dataTable = _staticDataManager.GetRecord(strSQL);
                //}
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return row;
        }

        public DataTable FetchEAFsGroup(string sqlQuery)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _staticDataManager.GetRecord(sqlQuery);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public float GetTANSecondaryActivePower(byte Index)
        {
            float result = 0;
            try
            {
                string strTemp = "";

                // TODO: check
                // 2016.02.17 Adding 8 for T8AN
                if ((Index != 1) && (Index != 2) && (Index != 3) && (Index != 5) && (Index != 8) && (Index != 7))
                {
                    _logger.WriteEntry("Index of Transformer in GetTANSecondaryActivePower is incorrect", LogLevels.Error);
                    return 0;
                }

                strTemp = "T" + Index.ToString().Trim() + "AN_SEC_P";

                _scadaPointsHelper.TryGetValue(strTemp, out var scadaPoint);
                if (scadaPoint is null)
                {
                    _logger.WriteEntry("The value could not read from SCADA", LogLevels.Error);
                }
                else
                {
                    result = (float)scadaPoint.Value;
                }
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return result;
        }

        public int GetTANBusbarPosition(byte Index)
        {
            // TODO:
            //get
            //{
            //	// Default value in error cases
            //	byte result = (byte)Breaker_Status.bClose;
            //	try
            //	{
            //		result = Convert.ToByte(getValuebyName("MAC_DS"));

            //	}
            //	catch (System.Exception excep)
            //	{
            //		GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLSPParameters..MAC_DS()", excep.Message);
            //	}

            //	return result;
            //}
            int result = 0;

            /*
            try
            {
                string strValue = "";
                string strTemp = "";
                string strNetorwrkPath = "";
                bool aIsValid = false;

                result = 0;
                //' KAJI T8AN Definition, Adding (Index <> 8)
                if ((Index != 1) && (Index != 2) && (Index != 3) && (Index != 5) && (Index != 7) && (Index != 8))
                {
                    GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLSPParameters..TANBusbarBPsition()", "Index of Transformer is incorrect");
                    return result;
                }

                strTemp = "T" + Index.ToString().Trim() + "AN-BB";

                strNetorwrkPath = FindNetWorkPath(strTemp);
                if (!m_CSCADAInterface.ReadData(strNetorwrkPath, ref strValue, ref aIsValid))
                {
                    GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLSPParameters..TANBusbarBPsition()", "The value could not read from SCADA");
                }
                else
                {
                    if (Convert.ToInt32(strValue) > 2 || Convert.ToInt32(strValue) < 0)
                    {
                        GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLSPParameters..TANBusbarBPsition()", "The value is not valid");
                    }
                    else
                    {
                        result = Convert.ToInt32(Convert.ToInt32(strValue));
                    }
                }
            }
            catch (System.Exception excep)
            {
                GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLSPParameters..TANBusbarBPsition()", excep.Message);
            }
            */
            return result;
        }

        public DataTable FetchPriorityLists()
        {
            DataTable dataTable = null;

            try
            {
                dataTable = JsonConvert.DeserializeObject<DataTable>(_RTDBManager.RedisConn.Get(RedisKeyPattern.LSP_PRIORITYLIST));
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public FetchEAFSPriority_Str[] FetchEAFSPriority(string grpNumber, string FurnaceStatus, List<string> Exception)
        {
            FetchEAFSPriority_Str[] dataTable = null;
            try
            {
                var EEC_SFSCEAFSPRIORITY_KEYs = _RTDBManager.GetKeys(pattern: RedisKeyPattern.EEC_SFSCEAFSPRIORITY);
                if (EEC_SFSCEAFSPRIORITY_KEYs.Length == 0)
                {
                    _logger.WriteEntry("Error in running get Keys from EEC_SFSCEAFSPRIORITY cache", LogLevels.Error);
                    return dataTable;
                }

                var EEC_EAFSPriority_KEYs = _RTDBManager.GetKeys(pattern: RedisKeyPattern.EEC_EAFSPriority);
                if (EEC_EAFSPriority_KEYs.Length == 0)
                {
                    _logger.WriteEntry("Error in running get Keys from EEC_EAFSPriority cache", LogLevels.Error);
                    return dataTable;
                }

                EEC_SFSCEAFSPRIORITY_Str[] eec_sfscprio_rows = new EEC_SFSCEAFSPRIORITY_Str[8];
                EEC_EAFSPRIORITY_Str[] eec_eafprio_rows = new EEC_EAFSPRIORITY_Str[8];

                var _eec_sfscprio_table = _RTDBManager.StringGet<EEC_SFSCEAFSPRIORITY_Str>(EEC_SFSCEAFSPRIORITY_KEYs).ToArray();
                var _eec_eafprio_table = _RTDBManager.StringGet<EEC_EAFSPRIORITY_Str>(EEC_EAFSPriority_KEYs).ToArray();

                //for (int fur = 0; fur < 8; fur++)
                //{
                //    eec_sfscprio_rows[fur] = _eec_sfscprio_table.Where(n => n.FURNACE == (fur + 1).ToString()).First();
                //    eec_eafprio_rows[fur] = _eec_eafprio_table.Where(n => n.FURNACE == (fur + 1).ToString()).First();
                //}

                var dataTable_1 = from ee in _eec_eafprio_table
                                  join es in _eec_sfscprio_table on ee.FURNACE.ToString() equals es.FURNACE.ToString()
                                  select new FetchEAFSPriority_Str
                                  {
                                      CB_NETWORKPATH = ee.CB_NETWORKPATH.ToString(),
                                      CT_NETWORKPATH = ee.CT_NETWORKPATH.ToString(),
                                      HASPARTNER = ee.HASPARTNER.ToString(),
                                      PARTNERADDRESS = ee.PARTNERADDRESS.ToString(),
                                      FURNACE = ee.FURNACE.ToString(),
                                      CONSUMED_ENERGY_PER_HEAT = es.CONSUMED_ENERGY_PER_HEAT.ToString(),
                                      STATUS_OF_FURNACE = es.STATUS_OF_FURNACE.ToString(),
                                      GROUPNUM = es.GROUPNUM.ToString(),
                                      ID_CB = ee.ID_CB,
                                      ID_CT = ee.ID_CT,
                                      ID_CB_PARTNER = ee.ID_CB_PARTNER
                                  };
                var dataTable_2 = dataTable_1;
                if (grpNumber != "")
                    dataTable_2 = dataTable_1.Where(n => n.GROUPNUM == grpNumber).ToArray();
                var dataTable_3 = dataTable_2.Where(n => n.STATUS_OF_FURNACE == FurnaceStatus).OrderBy(n => (Convert.ToDecimal(n.CONSUMED_ENERGY_PER_HEAT))).ToArray();
                dataTable = dataTable_3.Where(n => !Exception.Any(a => n.CB_NETWORKPATH.Contains(a))).ToArray();

            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public DataTable FetchReducedPower(int furnaceIndex)
        {
            DataTable dataTable = null;

            try
            {
                string StringSql = "SELECT TELDATETIME, FURNACE" +
                                    furnaceIndex +
                                    " FROM  [PU10_PCS].[dbo].[T_EAFsPower_Backup] " +
                                    "WHERE TELDATETIME = (SELECT MAX(TELDATETIME) " +
                                    "FROM [PU10_PCS].[dbo].[T_EAFsPower_Backup] WHERE FURNACE" +
                                    furnaceIndex + " > 0)";
                dataTable = _linkDBpcsDataManager.GetRecord(StringSql);
                //dataTable = _databaseQuery.Execute(DatabaseSource.StaticData, StringSql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

            return dataTable;
        }

        public SFSC_EAFSPOWER_Str GetFromHistoricalCache()
        {
            SFSC_EAFSPOWER_Str dataTable = null;

            try
            {
                if (_RTDBManager.GetKeys(pattern: RedisKeyPattern.SFSC_EAFSPOWER).Length == 0)
                    return dataTable;

                dataTable = JsonConvert.DeserializeObject<SFSC_EAFSPOWER_Str>(_RTDBManager.RedisConn.Get(RedisKeyPattern.SFSC_EAFSPOWER));

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
                // TODO:
                dataTable = _linkDBpcsDataManager.GetRecord(sql);
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
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
                var isDMLOkay = _linkDBpcsDataManager.ExecuteNonQuery(sql);
                if (isDMLOkay > 0)
                    return true;
                else
                    return false;
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
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
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
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
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
                var isDMLOkay = _historicalDataManager.ExecuteNonQuery(sql);
                if (isDMLOkay > 0)
                    return true;
                else
                    return false;
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
        public RedisUtils GetRedisUtiles()
        {
            return _RTDBManager;

        }
        public DataManager Get_historicalDataManager()
        {
            return _historicalDataManager;
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
                parameters[0] = _staticDataManager.CreateParameter("networkpath", networkpath);
                var dataTable = _staticDataManager.GetRecord("FUNCTIONS.APP_GUID_SELECT", CommandType.StoredProcedure, parameters);
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

        //public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        //{
        //    DataTable dt = new DataTable();
        //    PropertyInfo[] columns = null;

        //    if (Linqlist == null) return dt;

        //    foreach (T Record in Linqlist)
        //    {

        //        if (columns == null)
        //        {
        //            columns = ((Type)Record.GetType()).GetProperties();
        //            foreach (PropertyInfo GetProperty in columns)
        //            {
        //                Type colType = GetProperty.PropertyType;

        //                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
        //                       == typeof(Nullable<>)))
        //                {
        //                    colType = colType.GetGenericArguments()[0];
        //                }

        //                dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
        //            }
        //        }

        //        DataRow dr = dt.NewRow();

        //        foreach (PropertyInfo pinfo in columns)
        //        {
        //            dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
        //                   (Record, null);
        //        }

        //        dt.Rows.Add(dr);
        //    }
        //    return dt;
        //}



    }



}