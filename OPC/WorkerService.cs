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

namespace OPC
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly StoreLogs _storeLogs;
        private readonly CpsRpcService _rpcService;
        private readonly Repository _repository;
        private readonly OPCManager _opcManager;
        private readonly DataManager _staticDataManager;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
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
            

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer);
            _repository = new Repository(_logger, _staticDataManager, _RedisConnectorHelper);
            _opcManager = new OPCManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, _opcManager, _rpcService, _cpsRuntimeDataBuffer);
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
            _logger.WriteEntry("Start of running OPC ... ***************************************", LogLevels.Info);

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

            _runtimeDataReceiver.Start();
            Task.Factory.StartNew(() =>
            {
                _opcManager.RunClient();
            }, TaskCreationOptions.LongRunning);
            _logger.WriteEntry("End of preparing data for OPC ... ***************************************", LogLevels.Info);


            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop OPC", LogLevels.Info);
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
    }
}
