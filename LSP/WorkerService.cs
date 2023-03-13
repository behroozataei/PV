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
using System.Threading;
using System.Threading.Tasks;

namespace LSP
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly StoreLogs _storeLogs;
        private readonly CpsRpcService _rpcService;
        private readonly Repository _repository;
        private readonly DataManager _staticDataManager;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private readonly LSPManager _lspManager;
        private readonly RedisUtils _RedisConnectorHelper;
        private readonly IConfiguration _config;

        public WorkerService(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetService<IConfiguration>();
            _logger = serviceProvider.GetService<ILogger>();

            _staticDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(_config["OracleServicename"], _config["OracleDatabaseAddress"], _config["OracleStaticUser"], _config["OracleStaticPassword"]);
            _storeLogs = new StoreLogs(_staticDataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");

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
            //_RedisConnectorHelper.ConnectionFailed += _mainRTDBmanager_ConnectionFailed;
            //_RedisConnectorHelper.ConnectionRestored += _mainRTDBmanager_ConnectionRestored;
            //_RedisConnectorHelper.ErrorMessage += _mainRTDBmanager_ErrorMessage;

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer);
            _repository = new Repository(_logger, _config, _RedisConnectorHelper);
            _lspManager = new LSPManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, _lspManager, _rpcService, _cpsRuntimeDataBuffer);
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

        //private  void _mainRTDBmanager_ErrorMessage(object sender, StackExchange.Redis.RedisErrorEventArgs e)
        //{
        //    if (!RedisUtils.IsConnected)
        //        _logger.WriteEntry($"Redis Error Message: {e.Message}, RedisConnection: {RedisUtils.IsConnected}", LogLevels.Warn);
        //}

        //private void _mainRTDBmanager_ConnectionRestored(object sender, StackExchange.Redis.ConnectionFailedEventArgs e)
        //{
        //    if (!RedisUtils.IsConnected)
        //        _logger.WriteEntry($"Redis Connection Restored , RedisConnection: {RedisUtils.IsConnected}", LogLevels.Warn);
        //}

        //private void _mainRTDBmanager_ConnectionFailed(object sender, StackExchange.Redis.ConnectionFailedEventArgs e)
        //{
        //    if (!RedisUtils.IsConnected)
        //    {
        //        _logger.WriteEntry($"Redis Connection Failed, RedisConnection: {RedisUtils.IsConnected}", LogLevels.Error);
        //        while (!RedisUtils.IsConnected)
        //        {
        //            _logger.WriteEntry(">>>>> Waiting for Redis Connection", LogLevels.Info);
        //            CallConnection();
        //            Thread.Sleep(5000);

        //        };
        //    }

        //}

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            CallConnection();
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();
            _logger.WriteEntry("Start of running LSP ... ***************************************", LogLevels.Info);
                       
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


            _rpcService.StateChanged += OnRpcStateChanged;
            _runtimeDataReceiver.Start();
            _lspManager.CheckCPSStatus();  //waiting for connection
            _lspManager.Initialize();
            _logger.WriteEntry("End of preparing data for LSP ... ***************************************", LogLevels.Info);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop LSP", LogLevels.Info);
            _storeLogs.Stop();
            _staticDataManager.Close();

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

        private void OnRpcStateChanged(object sender, GrpcStateChangeEventArgs e)
        {
            _logger.WriteEntry("RpcClientManager_StateChanged ... " + e.State.ToString(), LogLevels.Info);

            if (e.State == GrpcCommunicationState.Connect)
            {
                _logger.WriteEntry("CPS is going to Connect", LogLevels.Info);

                Task.Run(() =>
                {
                    Thread.Sleep(3000);
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
    }
}
