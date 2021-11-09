using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

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
        IDatabase _cache;
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
            if (RedisUtils.IsConnected)
            {
                _logger.WriteEntry("Connected to Redis Cache", LogLevels.Info);
                _cache = _RedisConnectorHelper.DataBase;
                if (_RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_CheckPoints).Length != 0 &&
                    _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_PARAMS).Length != 0 )

                {
                    LoadfromCache = true;
                }
                else
                {
                    LoadfromCache = false;
                }
            }
            else
            {
                _logger.WriteEntry("Redis Connaction Failed.", LogLevels.Error);
            }
            try
            {
                FetchCheckPoints();
                FetchScadaPoints();

                isBuild = true;
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

        private static string GetEndStringCommand()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //return "app.";
                return "APP_";
                //return string.Empty;


            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
               return "APP_";

            }

            return string.Empty;
        }

        private void FetchCheckPoints()
        {
            if (LoadfromCache)
            {
                _logger.WriteEntry("Loading OCP_Checkpoint Data from Cache", LogLevels.Info);

                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_CheckPoints);
                var dataTable_cache = _RedisConnectorHelper.StringGet<OCP_CHECKPOINTS_Object>(keys);

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
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error load data from Cache database", LogLevels.Error, ex);
                }
            }
            else
            {
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
                                                                                $"FROM {GetEndStringCommand()}OCP_CheckPoints";

                var dataTable = _dataManager.GetRecord(sql);


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
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry("Error in loading " + checkPoint.NetworkPath, LogLevels.Error, ex);
                    }


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

        private void FetchScadaPoints()
        {
            try
            {
                if (LoadfromCache)
                {
                    _logger.WriteEntry("Loading LSP_PARAMS Data from Cache", LogLevels.Info);

                    var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OCP_PARAMS);
                    var dataTable_cache = _RedisConnectorHelper.StringGet<OCP_PARAMS_Object>(keys);

                    foreach (OCP_PARAMS_Object row in dataTable_cache)
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
                else
                {
                    OCP_PARAMS_Object _ocp_param = new OCP_PARAMS_Object();
                    var dataTable = _dataManager.GetRecord($"SELECT * FROM {GetEndStringCommand()}OCP_PARAMS");
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

                        _ocp_param.FUNCTIONNAME = row["FUNCTIONNAME"].ToString();
                        _ocp_param.NAME = name;
                        _ocp_param.DIRECTIONTYPE = scadaPoint.DirectionType;
                        _ocp_param.NETWORKPATH = networkPath;
                        _ocp_param.SCADATYPE = scadaPoint.SCADAType;
                        _ocp_param.ID = id.ToString();
                        if (RedisUtils.IsConnected)
                            _cache.StringSet(RedisKeyPattern.OCP_PARAMS + networkPath, JsonConvert.SerializeObject(_ocp_param));


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
            }


            catch (Irisa.DataLayer.DataException ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
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
        public const string EEC_SFSCEAFSPRIORITY = "APP:EEC_SFSCEAFSPRIORITY:";
    }
    class OCP_PARAMS_Object
    {
        public string ID;
        public string FUNCTIONNAME;
        public string NAME;
        public string DIRECTIONTYPE;
        public string NETWORKPATH;
        public string SCADATYPE;
    }
    class OCP_CHECKPOINTS_Object
    {
        public int OCPSHEDPOINT_ID;
        public string NAME;
        public string NETWORKPATH;
        public string DECISIONTABLE;
        public string CHECKOVERLOAD;
        public string DESCRIPTION;
        public string SHEDTYPE;
        public string CATEGORY;
        public float NOMINALVALUE;
        public string LIMITPERCENT;
        public string VOLTAGEENOM;
        public string VOLTAGEDENOM;
        public string POWERNUM;
        public string POWERDENOM;
        public string CHECKPOINT_NETWORKPATH;
        public string Measurement_Id;
        public string IT_Id;
        public string AllowedActivePower_Id;
        public string Average_Id;
        public string Sample_Id;
        public string QualityErr_Id;
    };
}