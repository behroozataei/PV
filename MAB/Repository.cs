using COMMON;
using Irisa.DataLayer;
using Irisa.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MAB
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, MABScadaPoint> _scadaPoints;
        private readonly Dictionary<string, MABScadaPoint> _scadaPointsHelper;
        private readonly RedisUtils _RTDBManager;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager, RedisUtils RTDBManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, MABScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, MABScadaPoint>();
            _RTDBManager = RTDBManager ?? throw new ArgumentNullException(nameof(RTDBManager));
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

            MAB_PARAMS_Str mab_param = new MAB_PARAMS_Str();
            try
            {
                dataTable = dataManager.GetRecord(command);
                if (dataTable != null)
                {
                    if (!_RTDBManager.DelKeys(RedisKeyPattern.MAB_PARAMS))
                        _logger.WriteEntry("Error: Delete APP_MAB_PARAMS from Redis", LogLevels.Error);
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
            foreach (DataRow row in dataTable.Rows)
            {
                //var id = Guid.Parse(row["MeasurementId"].ToString());
                var name = row["Name"].ToString();
                var networkPath = row["NetworkPath"].ToString();
                var pointDirectionType = row["DirectionType"].ToString();
                var scadaType = Convert.ToString(row["SCADAType"]);
                var id = GetGuid(networkPath);

                mab_param.Name = name;
                mab_param.NetworkPath = networkPath;
                mab_param.DirectionType = pointDirectionType;
                mab_param.ScadaType = scadaType;
                mab_param.ID = id;

                try
                {
                    if (_RTDBManager.IsConnected)
                        _RTDBManager.RedisConn.Set(RedisKeyPattern.MAB_PARAMS + networkPath, JsonConvert.SerializeObject(mab_param));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);



                    var scadaPoint = new MABScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
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
            try
            {

                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.MAB_PARAMS);
                var dataTable = _RTDBManager.StringGet<MAB_PARAMS_Str>(keys);


                foreach (MAB_PARAMS_Str row in dataTable)
                {
                    var name = row.Name;
                    var networkPath = row.NetworkPath;
                    var pointDirectionType = row.DirectionType;
                    var scadaType = row.ScadaType;
                    var id = Guid.Parse(row.ID.ToString());


                    var scadaPoint = new MABScadaPoint(id, name, networkPath, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointDirectionType), scadaType);
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
                _logger.WriteEntry("Error in loading from Cache", LogLevels.Error, ex);
                return false;
            }
            return true;
        }

        public MABScadaPoint GetScadaPoint(Guid measurementId)
        {
            if (_scadaPoints.TryGetValue(measurementId, out var scadaPoint))
                return scadaPoint;
            else
                return null;
        }

        public MABScadaPoint GetScadaPoint(String name)
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