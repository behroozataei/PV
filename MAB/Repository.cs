using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Irisa.Logger;
using Irisa.DataLayer;

namespace MAB
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly Dictionary<Guid, MABScadaPoint> _scadaPoints;
        private readonly Dictionary<string, MABScadaPoint> _scadaPointsHelper;

        public Repository(ILogger logger, DataManager staticDataManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _scadaPoints = new Dictionary<Guid, MABScadaPoint>();
            _scadaPointsHelper = new Dictionary<string, MABScadaPoint>();
        }

        public bool Build()
        {
            var isBuild = false;

            try
            {
                GetInputScadaPoints(_dataManager);
                isBuild = true;
            }
            catch(Irisa.DataLayer.DataException ex)
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
                return "app.";
               // return string.Empty;

            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "APP_";

            }

            return string.Empty;
        }

        private void GetInputScadaPoints(DataManager dataManager)
        {
            string command = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from app.MAB_Measurements";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                command = $"SELECT  Name, NetworkPath, DirectionType, SCADAType from APP_MAB_PARAMS";

            var dataTable = dataManager.GetRecord(command);

            foreach (DataRow row in dataTable.Rows)
            {
                //var id = Guid.Parse(row["MeasurementId"].ToString());
                var name = row["Name"].ToString();
                var networkPath = row["NetworkPath"].ToString();
                var pointDirectionType = row["DirectionType"].ToString();
                var scadaType = Convert.ToString(row["SCADAType"]);
                var id = GetGuid(networkPath);
               

                try
                {
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
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry("Error in loading " + networkPath.ToString(), LogLevels.Error, ex);
                }
            }
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
        public Guid GetGuid(String networkpath)
        {
            string sql = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                sql = "SELECT * FROM dbo.NodesFullPath where FullPath = '" + networkpath + "'";

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
    
}