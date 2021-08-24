using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using Irisa.Logger;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;
using Irisa.DataLayer.Oracle;

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

        public Repository(ILogger logger, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _staticDataManager = new SqlServerDataManager(config["SQLServerNameOfStaticDataDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
                //_staticDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);

                _historicalDataManager = new SqlServerDataManager(config["SQLServerNameOfHistoricalDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
                //_historicalDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleHISUser"], config["OracleHISPassword"]);

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                _staticDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
                _historicalDataManager = new OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleHISUser"], config["OracleHISPassword"]);
            }
            _linkDBpcsDataManager = new SqlServerDataManager(config["PCSLinkDatabaseName"], config["PCSLinkDatabaseAddress"], config["PCSLinkUser"], config["PCSLinkPassword"]);
            _checkPoints = new Dictionary<Guid, OCPCheckPoint>();
            _checkPointHelper = new Dictionary<string, OCPCheckPoint>();
            _checkPointHelperNumber = new Dictionary<short, OCPCheckPoint>();

            _scadaPoints = new Dictionary<Guid, LSPScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, LSPScadaPoint>();
        }

        private static string GetEndStringCommand()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "app.";
                //return string.Empty;

            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                return "APP_";

            }

            return string.Empty;
        }

        public bool Build()
        {
            var isBuild = false;

            try
            {
                // Important note: Always CheckPoints should be loaded before ScadaPoints
                FetchCheckPoints();
                FetchScadaPoints();

                _staticDataManager.Close();
                _linkDBpcsDataManager.Close();

                isBuild = true;
            }
            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return isBuild;
        }

        private void FetchCheckPoints()
        {
            var dataTable = _staticDataManager.GetRecord("SELECT OCPSHEDPOINT_ID, " +
                                                                                  //  "GUID, " +
                                                                                    "NAME, " +
                                                                                    "NETWORKPATH, " +
                                                                                    "DECISIONTABLE, " +
                                                                                    "CHECKOVERLOAD," +
                                                                                    " DESCRIPTION, " +
                                                                                    "SHEDTYPE, " +
                                                                                    "CATEGORY, " +
                                                                                    "NOMINALVALUE, " +
                                                                                    "LIMITPERCENT, " +
                                                                                    "VOLTAGEENOM, " +
                                                                                    "VOLTAGEDENOM, " +
                                                                                    "POWERNUM, " +
                                                                                    "POWERDENOM, " +
                                                                                    //"IT_GUID, " +
                                                                                    //"ALLOWEDACTIVEPOWER_GUID, " +
                                                                                    //"SAMPLE_GUID, " +
                                                                                    //"AVERAGE_GUID, " +
                                                                                    "CHECKPOINT_NETWORKPATH " +
                                                                            $"FROM  {GetEndStringCommand()}OCP_CHECKPOINTS");

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

                //if(checkPoint.NetworkPath.Contains("RED-KH"))
                //{
                //    System.Diagnostics.Debug.WriteLine(checkPoint.NetworkPath);
                //}

                //checkPoint.IT_GUID = Guid.Parse(Convert.ToString(row["IT_GUID"]));
                //checkPoint.ALLOWEDACTIVEPOWER_GUID = Guid.Parse(Convert.ToString(row["ALLOWEDACTIVEPOWER_GUID"]));
                //checkPoint.SAMPLE_GUID = Guid.Parse(Convert.ToString(row["SAMPLE_GUID"]));
                //checkPoint.AVERAGE_GUID = Guid.Parse(Convert.ToString(row["AVERAGE_GUID"]));

                //var scadaPoint1 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row["IT_GUID"])), "IT", checkPoint.NetworkPath + "/IT");
                var scadaPoint1 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/IT"), "IT", checkPoint.NetworkPath + "/IT");
                scadaPoint1.DirectionType = "INPUT";
                scadaPoint1.SCADAType = "AnalogMeasurement";
                checkPoint.OverloadIT = scadaPoint1;

                //var scadaPoint2 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row["ALLOWEDACTIVEPOWER_GUID"])), "AAP", checkPoint.NetworkPath + "/AAP");
                var scadaPoint2 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/AllowedActivePower"), "AAP", checkPoint.NetworkPath + "/AllowedActivePower");
                scadaPoint2.DirectionType = "INPUT";
                scadaPoint2.SCADAType = "AnalogMeasurement";
                checkPoint.ActivePower = scadaPoint2;

                //var scadaPoint3 = new LSPScadaPoint(Guid.Parse(Convert.ToString(row["AVERAGE_GUID"])), "AVERAGE", checkPoint.NetworkPath + "/AVERAGE");
                var scadaPoint3 = new LSPScadaPoint(GetGuid(Convert.ToString(row["CHECKPOINT_NETWORKPATH"]) + "/Average"), "AVERAGE", checkPoint.NetworkPath + "/Average");
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
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                }

                
            }
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

        private void FetchScadaPoints()
        {
            var fetchedData = false;

            fetchedData = FetchLspParam();
            if (fetchedData == false) return;

            fetchedData = FetchPriorityListItems();
            if (fetchedData == false) return;

            fetchedData = FetchShedpointListItems();
            if (fetchedData == false) return;

            FetchEecEafsPriorityListItems();
        }

        private bool FetchLspParam()
        {
            try
            {
                var dataTable = _staticDataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}LSP_PARAMS");
                foreach (DataRow row in dataTable.Rows)
                {
                    //var id = Guid.Parse(row["GUID"].ToString());
                    var id = GetGuid(row["NetworkPath"].ToString());
                    var name = row["NAME"].ToString();
                    var networkPath = row["NetworkPath"].ToString();
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

        private bool FetchPriorityListItems()
        {
            try
            {
                var dataTable = _staticDataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}LSP_DECTITEMS");

                foreach (DataRow row in dataTable.Rows)
                {
                    var id = GetGuid(row["NetworkPath"].ToString());
                    //if (Guid.TryParse(row["GUID"].ToString(), out var id))
                    if(id!=Guid.Empty)
                    {
                        var name = row["Name"].ToString();
                        var networkPath = row["NetworkPath"].ToString();
                        var pointDirectionType = "INPUT";
                        var scadaPoint = new LSPScadaPoint(id, name, networkPath);
                        scadaPoint.DirectionType = pointDirectionType;
                        scadaPoint.SCADAType = "DigitalMeasurement";

                        if (!_scadaPoints.ContainsKey(id))
                        {
                            _scadaPoints.Add(id, scadaPoint);
                            _scadaPointsHelper.Add(name, scadaPoint);
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

        private bool FetchShedpointListItems()
        {
            try
            {
                var dataTable = _staticDataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}LSP_PRIORITYITEMS ORDER BY PRIORITYLISTNO, ITEMNO");

                foreach (DataRow row in dataTable.Rows)
                {
                    var id_curr = GetGuid(row["NETWORKPATH_CURR"].ToString());
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

                    var id_cb = GetGuid(row["NETWORKPATH_ITEM"].ToString());
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

        private bool FetchEecEafsPriorityListItems()
        {
            try
            {
                var dataTable = _staticDataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}EEC_EAFSPRIORITY ORDER BY Furnace");

                foreach (DataRow row in dataTable.Rows)
                {
                    var id_CB = GetGuid(row["CB_NETWORKPATH"].ToString());
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

                    var id_CT = GetGuid(row["CT_NETWORKPATH"].ToString());
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
                    }

                    var id_CB_Partner = GetGuid(row["PARTNERADDRESS"].ToString());
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

        public DataTable FetchDecisionTables()
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTLIST ORDER BY DECTNO");

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

        public DataTable FetchItems(byte decisionTableNo)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTITEMS WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY DECTITEMNO ");

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

        public DataTable FetchCombinations(byte decisionTableNo)
        {
            DataTable dataTable = null;

            try
            {
                dataTable = _staticDataManager.GetRecord($"SELECT * from {GetEndStringCommand()}LSP_DECTCOMB WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY COMBINATIONNO, DECTITEMNO");

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
                string strSQL = $"SELECT * FROM {GetEndStringCommand()}LSP_DECTPRIOLS WHERE DECTNO = " + decisionTableNo.ToString() + " ORDER BY COMBINATIONNO";
                dataTable = _staticDataManager.GetRecord(strSQL);

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

        public DataTable FetchBreakersToShed(byte priorityListNo)
        {
            DataTable dataTable = null;

            try
            {
                string strSQL = $"SELECT * FROM {GetEndStringCommand()}LSP_PRIORITYITEMS WHERE PRIORITYLISTNO = " + priorityListNo.ToString() + " ORDER BY ITEMNO ";
                dataTable = _staticDataManager.GetRecord(strSQL);
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
                dataTable = _staticDataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}LSP_PRIORITYLIST ORDER BY PRIORITYLISTNO");

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

        public DataTable FetchEAFSPriority(string grpNumber, string FurnaceStatus, string strSQLException)
        {
            DataTable dataTable = null;

            try
            {
                string selectsql = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    selectsql = "select ee.CB_NETWORKPATH, ee.CT_NETWORKPATH, ee.HASPARTNER, ee.PARTNERADDRESS, " +
                   "ee.FURNACE, es.CONSUMED_ENERGY_PER_HEAT, es.STATUS_OF_FURNACE, es.FURNACE, " +
                   "es.GROUPNUM from IrisaHistorical.app.EEC_SFSCEAFSPriority es, app.EEC_EAFSPriority ee " +
                   "where ee.FURNACE = es.FURNACE ";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    selectsql = "select ee.CB_NETWORKPATH, ee.CT_NETWORKPATH, ee.HASPARTNER, ee.PARTNERADDRESS, " +
                  "ee.FURNACE, es.CONSUMED_ENERGY_PER_HEAT, es.STATUS_OF_FURNACE, es.FURNACE, " +
                  "es.GROUPNUM from SCADAHIS.APP_EEC_SFSCEAFSPRIORITY es, SCADA.APP_EEC_EAFSPRIORITY ee " +
                  "WHERE ee.FURNACE = es.FURNACE ";

                }

                    //  1399-10-03
                    //ToDO :
                    // why does for Line Overloaded (Line 914 or 915)  not considered grpnumber  ?
                    string selgrpNumber = (grpNumber != "") ? "AND GROUPNUM = '" + grpNumber + "'" : "";

                string StringSql = selectsql + selgrpNumber + " AND STATUS_OF_FURNACE = '" + FurnaceStatus + "' " + strSQLException + " ORDER BY CAST( CONSUMED_ENERGY_PER_HEAT AS FLOAT) ASC";
                //string StringSql = "SELECT * FROM app.EEC_EAFSPriority WHERE GROUPNUM = " + grpNumber + " AND STATUS_OF_FURNACE = '" + FurnaceStatus + "' " + strSQLException + " ORDER BY CONSUMED_ENERGY_PER_HEAT ASC";

                dataTable = _staticDataManager.GetRecord(StringSql);
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