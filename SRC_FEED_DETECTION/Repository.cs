using Irisa.DataLayer;
using Irisa.Logger;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SRC_FEED_DETECTION
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, ScadaPoint> _scadaPoints;
        private readonly Dictionary<string, ScadaPoint> _scadaPointsHelper;
       

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager staticDataManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, ScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, ScadaPoint>();
           
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
            // ScadaPoint Information : Name, NetworkPath, PointDirectionType, ScadaType,Guid
            List<Tuple<string, string, string, string, Guid>> _pointsParams = new List<Tuple<string, string, string, string, Guid>>();
           
            _pointsParams.Add(Tuple.Create("FL01_CB", "Network/Substations/63KV STATION/63KV/Virtual Source Bay 1/CB/STATE", "Output", "DigitalMeasurment", Guid.Empty));
            _pointsParams.Add(Tuple.Create("FL02_CB", "Network/Substations/63KV STATION/63KV/Virtual Source Bay 2/CB/STATE", "Output", "DigitalMeasurment", Guid.Empty));
            _pointsParams.Add(Tuple.Create("FL01_VT", "Network/Substations/63KV STATION/63KV/FL01/VT", "Input", "AnalogMeasurment", Guid.Empty));
            _pointsParams.Add(Tuple.Create("FL02_VT", "Network/Substations/63KV STATION/63KV/FL02/VT", "Input", "AnalogMeasurment", Guid.Empty));
            
                
            foreach (var pointParam in _pointsParams)
            {
                

                try
                {
                    
                    var scadaPoint = new ScadaPoint(pointParam.Item5, pointParam.Item1, pointParam.Item2, (PointDirectionType)Enum.Parse(typeof(PointDirectionType), pointParam.Item3), pointParam.Item4);
                    if (!_scadaPoints.ContainsKey(pointParam.Item5))
                    {
                        _scadaPoints.Add(pointParam.Item5, scadaPoint);
                        _scadaPointsHelper.Add(pointParam.Item1, scadaPoint);
                    }

                }
                
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading " + pointParam.Item2, LogLevels.Error, ex);
                    return false;
                }
            }
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
    }


}