using COMMON;
using Irisa.DataLayer;
using Irisa.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SDK_Template
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, SDK_Template_ScadaPoint> _scadaPoints;
        private readonly Dictionary<string, SDK_Template_ScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RedisConnectorHelper;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, RedisUtils RedisConnectorHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, SDK_Template_ScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, SDK_Template_ScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }

        public bool Build()
        {
            try
            {
                if (GetInputScadaPoints(_dataManager))
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


        private bool GetInputScadaPoints(DataManager dataManager)
        {
            _logger.WriteEntry("Loading Data from Database", LogLevels.Info);
            string command = "";
            command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from APP_MAB_PARAMS";
            DataTable dataTable = null;
            SDK_TEMP_PARAMS_Str sdk_temp_params = new SDK_TEMP_PARAMS_Str();

            try
            {
                dataTable = dataManager.GetRecord(command);
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
            foreach (DataRow row in dataTable.Rows)
            {
                //var id = Guid.Parse(row["MeasurementId"].ToString());
                var name = row["Name"].ToString();
                var networkPath = row["NetworkPath"].ToString();
                var pointDirectionType = row["DirectionType"].ToString();
                var scadaType = Convert.ToString(row["SCADAType"]);
                var id = GetGuid(networkPath);

                sdk_temp_params.Name = name;
                sdk_temp_params.NetworkPath = networkPath;
                sdk_temp_params.DirectionType = pointDirectionType;
                sdk_temp_params.ScadaType = scadaType;
                sdk_temp_params.ID = id;

                try
                {
                    if (RedisUtils.IsConnected)
                        _RedisConnectorHelper.DataBase.StringSet(RedisKeyPattern.SDK_TEMPLATE + networkPath, JsonConvert.SerializeObject(sdk_temp_params));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);



                    var scadaPoint = new SDK_Template_ScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }

                }
                catch (Irisa.DataLayer.DataException ex)
                {
                    _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading " + networkPath.ToString(), LogLevels.Error, ex);
                    return false;
                }
            }
            return true;

        }

        private bool GetInputScadaPointsFromRedis()
        {
            
            _logger.WriteEntry("Loading Data from Cache", LogLevels.Info);
            IEnumerable<SDK_TEMP_PARAMS_Str> dataTable = null;
            try
            {
                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.SDK_TEMPLATE);
                if (keys.Length!=0)
                    dataTable = _RedisConnectorHelper.StringGet<SDK_TEMP_PARAMS_Str>(keys);
            }
            catch
            {
                _logger.WriteEntry("Error in loading Cache Keys " , LogLevels.Error);
                return false;

            }
           


            foreach (SDK_TEMP_PARAMS_Str row in dataTable)
            {
                var name = row.Name;
                var networkPath = row.NetworkPath;
                var pointDirectionType = row.DirectionType;
                var scadaType = row.ScadaType;
                var id = Guid.Parse(row.ID.ToString());

                try
                {
                    var scadaPoint = new SDK_Template_ScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
                    if (!_scadaPoints.ContainsKey(id))
                    {
                        _scadaPoints.Add(id, scadaPoint);
                        _scadaPointsHelper.Add(name, scadaPoint);
                    }
                }
                catch (Irisa.DataLayer.DataException ex)
                {
                    _logger.WriteEntry(ex.ToString(), LogLevels.Error, ex);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading from Cache" + networkPath.ToString(), LogLevels.Error, ex);
                    return false;
                }
            }
            return true;
        }

        public SDK_Template_ScadaPoint GetScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public SDK_Template_ScadaPoint GetScadaPoint(String name)
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