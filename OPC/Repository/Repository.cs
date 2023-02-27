using COM;
using Irisa.DataLayer;
using Irisa.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        private readonly RedisUtils _RedisConnectorHelper;

        private bool isBuild = false;

        public Repository(ILogger logger, DataManager databaseQuery, RedisUtils RedisConnectorHelper)
        {
            _OPCRepository = new OPCRepository();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataManager = databaseQuery ?? throw new ArgumentNullException(nameof(databaseQuery));
            _entityMapper = new Dictionary<string, Guid>();
            _scadaPoints = new Dictionary<Guid, ScadaPoint>();
            _RedisConnectorHelper = RedisConnectorHelper ?? throw new ArgumentNullException(nameof(RedisConnectorHelper));
        }
        public bool Build()
        {

            try
            {
                if (PopulateOPCTags(_dataManager))
                {
                    if (PopulateOPCParams(_dataManager))
                        isBuild = true;
                }
                else if (PupulateOPCTagsfromRedis())
                {
                    if (PopulateOPCParamsfromRedis())
                        isBuild = true;
                }
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

        private bool PopulateOPCTags(DataManager dbQuery)
        {
            _logger.WriteEntry("Loading Data from database", LogLevels.Info);
            DataTable dataTable = null;
            string sql = $"SELECT ScadaTagName, KeepServerTagName, Description, MessageConfiguration, TagType, NetworkPath  FROM APP_OPC_MEASUREMENT";
            try
            {
                dataTable = dbQuery.GetRecord(sql);
                OPC_MEAS_Str opc_meas = new OPC_MEAS_Str();

                foreach (DataRow row in dataTable.Rows)
                {

                    opc_meas.ScadaTagName = row["ScadaTagName"].ToString();
                    opc_meas.KeepServerTagName = row["KeepServerTagName"].ToString();
                    opc_meas.NetworkPath = row["NetworkPath"].ToString();
                    opc_meas.Description = row["Description"].ToString();
                    opc_meas.MessageConfiguration = (int)row["MessageConfiguration"];
                    opc_meas.ID = GetGuid(row["NetworkPath"].ToString());
                    opc_meas.TagType = (int)(row["TagType"].ToString() == "DMODigitalMeasurement" ? Type.Digital : Type.Analog);
                    if (RedisUtils.IsConnected)
                        RedisUtils.RedisConnection1.Set(RedisKeyPattern.OPCMeasurement + opc_meas.NetworkPath, JsonConvert.SerializeObject(opc_meas));
                    else
                        _logger.WriteEntry("Redis Connection Error", LogLevels.Error);

                    _OPCRepository.OPCTags.Add
                      (
                        new Tag()
                        {
                            ScadaName = row["ScadaTagName"].ToString(),
                            OPCName = row["KeepServerTagName"].ToString(),
                            Description = row["Description"].ToString(),
                            MessageConfiguration = (int)row["MessageConfiguration"],
                            MeasurementId = opc_meas.ID,
                            Type = row["TagType"].ToString() == "DMODigitalMeasurement" ? Type.Digital : Type.Analog
                        }
                      );
                    _entityMapper.Add(row["KeepServerTagName"].ToString(), opc_meas.ID);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }

            return true;

        }

        private bool PupulateOPCTagsfromRedis()
        {
            _logger.WriteEntry("Loading Data from Cache", LogLevels.Info);

            var keys = RedisUtils.GetKeys(pattern: RedisKeyPattern.OPCMeasurement);
            var dataTable = RedisUtils.StringGet<OPC_MEAS_Str>(keys);

            foreach (OPC_MEAS_Str row in dataTable)
            {
                try
                {
                    Tag tag = new Tag();
                    tag.ScadaName = row.ScadaTagName.ToString();
                    tag.OPCName = row.KeepServerTagName.ToString();
                    tag.Description = row.Description.ToString();
                    tag.MessageConfiguration = (int)row.MessageConfiguration;
                    tag.MeasurementId = Guid.Parse(row.ID.ToString());
                    tag.Type = (OPC.Type)row.TagType;

                    _OPCRepository.OPCTags.Add(tag);

                    //_entityMapper.Add(row["KeepServerTagName"].ToString(), Guid.Parse(row["GUID"].ToString()));
                    _entityMapper.Add(row.KeepServerTagName.ToString(), tag.MeasurementId);
                }
                catch (Exception ex)
                {
                    _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                    return false;
                }
            }
            return true;
        }

        private bool PopulateOPCParams(DataManager dbQuery)
        {
            try
            {
                string sql = $"SELECT * FROM APP_OPC_PARAMS";
                DataTable dataTable = new DataTable();

                dataTable = dbQuery.GetRecord(sql);

                DataRow row = dataTable.AsEnumerable().FirstOrDefault();
                _OPCRepository.Connection.Name = row["Name"].ToString();
                _OPCRepository.Connection.IP = row["IP"].ToString();
                _OPCRepository.Connection.Port = row["Port"].ToString();

                OPC_PARAM_Str opc_param = new OPC_PARAM_Str();
                opc_param.Name = _OPCRepository.Connection.Name;
                opc_param.IP = _OPCRepository.Connection.IP;
                opc_param.Port = _OPCRepository.Connection.Port;
                opc_param.Description = row["Description"].ToString();

                if (RedisUtils.IsConnected)
                    RedisUtils.RedisConnection1.Set(RedisKeyPattern.OPC_Params, JsonConvert.SerializeObject(opc_param));
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
            return true;
        }

        private bool PopulateOPCParamsfromRedis()
        {
            try
            {
                var keys = RedisUtils.GetKeys(pattern: RedisKeyPattern.OPC_Params);
                var paramTable = RedisUtils.StringGet<OPC_PARAM_Str>(keys);
                var row = paramTable.First();
                _OPCRepository.Connection.Name = row.Name;
                _OPCRepository.Connection.IP = row.IP;
                _OPCRepository.Connection.Port = row.Port;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
            return true;

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