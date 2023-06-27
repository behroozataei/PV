using COMMON;
using Irisa.Common.Utils;
using Irisa.DataLayer;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EEC
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private DataManager _dataManager;
        private StoreLogs _storeLogs;
        private CpsRpcService _rpcService;
        private Repository _repository;
        private EECManager _eecManager;
        private BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private RuntimeDataReceiver _runtimeDataReceiver;
        private RedisUtils _RedisConnectorHelper;
        private IConfiguration _config;

        public WorkerService(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetService<IConfiguration>();
            _logger = serviceProvider.GetService<ILogger>();

            _dataManager = new Irisa.DataLayer.Oracle.OracleDataManager(_config["OracleServicename"], _config["OracleDatabaseAddress"], _config["OracleStaticUser"], _config["OracleStaticPassword"]);
            _storeLogs = new StoreLogs(_dataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");
            var historyDataRequest = new HistoryDataRequest
            {
                RequireMeasurements = true,
                RequireMarker = true,
                RequireScadaEvent = false,
                RequireEquipment = false,
                RequireConnectivityNode = false,
            };

            _RedisConnectorHelper = new RedisUtils(0, _config["RedisKeySentinel1"], _config["RedisKeySentinel2"], _config["RedisKeySentinel3"], _config["RedisKeySentinel4"], _config["RedisKeySentinel5"], _config["RedisPassword"], _config["RedisServiceName"],
                                                     _config["RedisConName1"], _config["RedisConName2"], _config["RedisConName3"], _config["RedisConName4"], _config["RedisConName5"], _config["IsSentinel"]);
            

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer, GetRpcSslCredentials());
            _repository = new Repository(_logger, _config, _RedisConnectorHelper);
            _eecManager = new EECManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_eecManager.RuntimeDataProcessing, _rpcService, _cpsRuntimeDataBuffer);

        }

        private void CallConnection()
        {
            try
            {
                RedisUtils.RedisUtils_Connect();
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Redis Connection Error {ex}", LogLevels.Error);
            }
        }

        

        public override Task StartAsync(CancellationToken cancellationToken)
        {

             CallConnection();
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();
            _logger.WriteEntry("Start of running EEC ... ***************************************", LogLevels.Info);
            //_logger.WriteEntry("Start of running EEC.", LogLevels.Info);

           

            while (!COMMON.Connection.PingHost(_config["CpsIpAddress"], 10000))
            {
                _logger.WriteEntry(">>>>> Waiting for CPS Connection", LogLevels.Info);
                Thread.Sleep(5000);
            }
            _logger.WriteEntry(">>>>> Connected to CPS", LogLevels.Info);


            _logger.WriteEntry("Check Redis Connection", LogLevels.Info);
            while (!RedisUtils.IsConnected)
            {
                _logger.WriteEntry(">>>>> Waiting for Redis Connection", LogLevels.Info);
                CallConnection();
                Thread.Sleep(5000);

            }

            _logger.WriteEntry("Loading data from database/redis is started.", LogLevels.Info);
            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database/redis is completed", LogLevels.Info);

            _rpcService.StateChanged += RpcStateChanged;
            _runtimeDataReceiver.Start();

            _eecManager.CheckCPSStatus();

            _eecManager.StartCyclicOperation();

            _logger.WriteEntry("End of preparing data for EEC ... ***************************************", LogLevels.Info);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop EEC", LogLevels.Info);
            _storeLogs.Stop();
            _dataManager.Close();

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private static void OnLogReceived(object sender, LogInfoEventArgs e)
        {
            if (e.Level == LogLevels.Info)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (e.Level == LogLevels.Warn)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (e.Level == LogLevels.Error || e.Level == LogLevels.Critical)
                Console.ForegroundColor = ConsoleColor.Red;

            if (string.IsNullOrEmpty(e.Exception))
                Console.WriteLine($"{e.TimeStamp.ToIranStandardTime()} ==>   {e.Message}");
            else
                Console.WriteLine($"{e.TimeStamp.ToIranStandardTime()} ==>   \n\tCall site: {e.CallSite} \n\t{e.Message}");

            Console.ResetColor();
        }

        private void RpcStateChanged(object sender, GrpcStateChangeEventArgs e)
        {
            if (e.State == GrpcCommunicationState.Connect)
            {
                _logger.WriteEntry("CPS is going to Connect", LogLevels.Info);
                Task.Run(() =>
                {
                    _eecManager.ReinitializeCurrentFromNewAfterCPSStartToWork();
                    GlobalData.CPSStatus = true;
                });
            }

            if (e.State == GrpcCommunicationState.Disconnect)
            {

                GlobalData.CPSStatus = false;
                _logger.WriteEntry("CPS is going to Disconnect", LogLevels.Info);
            }

            if (e.State == GrpcCommunicationState.Connecting)
            {

                GlobalData.CPSStatus = false;
                _logger.WriteEntry("CPS is going to Connecting", LogLevels.Info);
            }
        }
        private RpcSslCredentials? GetRpcSslCredentials()
        {
            var dataTable = _dataManager.GetRecord("APP_GRPC_CERTIFICATION_SELECT", CommandType.StoredProcedure);

            if (dataTable.Rows.Count > 0)
            {
                var rootCertificate = Convert.ToString(dataTable.Rows[0]["ROOT_CERTIFICATE"]);
                var clientCertificateChain = Convert.ToString(dataTable.Rows[0]["CLIENT_CERTIFICATE_CHAIN"]);
                var clientPrivateKey = Convert.ToString(dataTable.Rows[0]["CLIENT_PRIVATE_KEY"]);

                return new RpcSslCredentials(rootCertificate, clientCertificateChain, clientPrivateKey);
            }
            return null;
        }
    }
}
