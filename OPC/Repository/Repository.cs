using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;
using StackExchange.Redis;
using System.Linq;
using Newtonsoft.Json;

using COM;
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
        private readonly RedisUtils _RedisConnectorHelper;

        private bool LoadfromRedis = false;
        IDatabase _cache;
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
            if (RedisUtils.IsConnected)
            {
                _logger.WriteEntry("Connected to Redis Cache", LogLevels.Info);
                _cache = _RedisConnectorHelper.DataBase;
                if (_RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OPCMeasurement).Length != 0 &&
                    _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OPC_Params).Length != 0)
                {
                    LoadfromRedis = true;
                }
                else
                {
                    LoadfromRedis = false;
                }
            }
            else
            {
                _logger.WriteEntry("Redis Connaction Failed.", LogLevels.Error);
            }

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
        
        private void PopulateOPCTags(DataManager dbQuery)
        {
            if (LoadfromRedis)
            {
                _logger.WriteEntry("Loading Data from Cache", LogLevels.Info);

                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OPCMeasurement);
                var dataTable = _RedisConnectorHelper.StringGet<OPC_MEAS_Str>(keys);

                foreach (OPC_MEAS_Str row in dataTable)
                {
                    try
                    {
                        _OPCRepository.OPCTags.Add
                          (
                            new Tag()
                            {
                                ScadaName = row.ScadaTagName.ToString(),
                                OPCName = row.KeepServerTagName.ToString(),
                                Description = row.Description.ToString(),
                                MessageConfiguration = (int)row.MessageConfiguration,
                                MeasurementId = Guid.Parse(row.ID.ToString()),
                                Type = (OPC.Type)row.TagType
                            }
                          ) ;
                        //_entityMapper.Add(row["KeepServerTagName"].ToString(), Guid.Parse(row["GUID"].ToString()));
                        _entityMapper.Add(row.KeepServerTagName.ToString(), GetGuid(row.NetworkPath.ToString()));
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                    }
                }
            }
            else
            {
                string sql = $"SELECT ScadaTagName, KeepServerTagName, Description, MessageConfiguration, TagType, NetworkPath  FROM APP_OPC_MEASUREMENT";
                var dataTable = dbQuery.GetRecord(sql);
                OPC_MEAS_Str opc_meas = new OPC_MEAS_Str();

                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        opc_meas.ScadaTagName = row["ScadaTagName"].ToString();
                        opc_meas.KeepServerTagName = row["KeepServerTagName"].ToString();
                        opc_meas.NetworkPath = row["NetworkPath"].ToString();
                        opc_meas.Description = row["Description"].ToString();
                        opc_meas.MessageConfiguration = (int)row["MessageConfiguration"];
                        opc_meas.ID= GetGuid(row["NetworkPath"].ToString());
                        opc_meas.TagType =(int) (row["TagType"].ToString() == "DMODigitalMeasurement" ? Type.Digital: Type.Analog);
                        if (RedisUtils.IsConnected)
                            _cache.StringSet(RedisKeyPattern.OPCMeasurement + opc_meas.NetworkPath, JsonConvert.SerializeObject(opc_meas));

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
                        //_entityMapper.Add(row["KeepServerTagName"].ToString(), Guid.Parse(row["GUID"].ToString()));
                        _entityMapper.Add(row["KeepServerTagName"].ToString(), GetGuid(row["NetworkPath"].ToString()));

                        
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                    }
                }
            }

        }
        private void PopulateOPCParams(DataManager dbQuery)
        {
            string sql = $"SELECT * FROM APP_OPC_PARAMS";
            DataTable dataTable = new DataTable();
            if (LoadfromRedis)
            {
                var keys = _RedisConnectorHelper.GetKeys(pattern: RedisKeyPattern.OPC_Params);
                var paramTable = _RedisConnectorHelper.StringGet<OPC_PARAM_Str>(keys);
                var row = paramTable.First();
                _OPCRepository.Connection.Name = row.Name;
                _OPCRepository.Connection.IP = row.IP;
                _OPCRepository.Connection.Port = row.Port;
            }
            else
            {
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
                    _cache.StringSet(RedisKeyPattern.OPC_Params, JsonConvert.SerializeObject(opc_param));
            }
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
    
    
}