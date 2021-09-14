using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;
using System.Linq;

using Irisa.Logger;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;

namespace OPC
{
    internal class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly OPCRepository _OPCRepository;
        private readonly Dictionary<Guid, ScadaPoint> _scadaPoints;
        private readonly Dictionary<string, ScadaPoint> _scadaPointsHelper;
        private readonly Dictionary<string, Guid> _entityMapper;

        public Repository(ILogger logger, DataManager databaseQuery)
        {
            _OPCRepository = new OPCRepository();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = databaseQuery ?? throw new ArgumentNullException(nameof(databaseQuery));
            _entityMapper = new Dictionary<string, Guid>();
            _scadaPoints = new Dictionary<Guid, ScadaPoint>();
        }
        public bool Build()
        {
            var isBuild = false;

            try
            {
                PopulateOPCTags(_dataManager);
                PopulateOPCParams(_dataManager);
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
                return "app.";
                //return string.Empty;

            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "APP_";

            }

            return string.Empty;
        }
        private void PopulateOPCTags(DataManager dbQuery)
        {
            string sql = $"SELECT ScadaTagName, KeepServerTagName, Description, MessageConfiguration, TagType, NetworkPath  FROM {GetEndStringCommand()}OPCMEASUREMENT";
            var dataTable = dbQuery.GetRecord(sql);                

            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    _OPCRepository.OPCTags.Add
                      (
                        new Tag()
                        {
                            ScadaName = row["ScadaTagName"].ToString(),
                            OPCName = row["KeepServerTagName"].ToString(),
                            Description = row["Description"].ToString(),
                            MessageConfiguration = (int)row["MessageConfiguration"],
                            //MeasurementId = Guid.Parse(row["GUID"].ToString()),
                            MeasurementId = GetGuid(row["NetworkPath"].ToString()),
                            Type = row["TagType"].ToString() == "DMODigitalMeasurement" ? Type.Digital : Type.Analog
                        }
                      ) ; 
                    //_entityMapper.Add(row["KeepServerTagName"].ToString(), Guid.Parse(row["GUID"].ToString()));
                    _entityMapper.Add(row["KeepServerTagName"].ToString(), GetGuid(row["NetworkPath"].ToString()));
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                }
            }

        }
        private void PopulateOPCParams(DataManager dbQuery)
        {
            string sql = $"SELECT Name, IP, Port FROM {GetEndStringCommand()}OPC_PARAMS";
            var dataTable = dbQuery.GetRecord(sql);

            DataRow row = dataTable.AsEnumerable().FirstOrDefault();
            _OPCRepository.Connection.Name = row["Name"].ToString();
            _OPCRepository.Connection.IP = row["IP"].ToString();
            _OPCRepository.Connection.Port = row["Port"].ToString();
        }

        public Guid GetMeasurementID(string opcTagName)
        {
            if (_entityMapper.TryGetValue(opcTagName, out var guid))
                return guid;
            else
                throw new InvalidOperationException($"No Guid associated with {opcTagName}");
        }

        public IList<Tag> GetTags()
        {
            return _OPCRepository.OPCTags;
        }

        public OPCRepository GetOPCConnectionParams()
        {
            return _OPCRepository;
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

        Guid GetGuid(String networkpath)
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