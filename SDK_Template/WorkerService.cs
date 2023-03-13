using COMMON;
using Irisa.Common.Utils;
using Irisa.DataLayer;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SDK_Template
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly DataManager _dataManager;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private readonly StoreLogs _storeLogs;
        private readonly CpsRpcService _rpcService;
        private readonly Repository _repository;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly SDK_Template_Manager _sdk_template_manager;
        private readonly RedisUtils _RedisConnectorHelper;

        public WorkerService(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            _logger = serviceProvider.GetService<ILogger>();

            _RedisConnectorHelper.ConnectionFailed += _RedisConnectorHelper_ConnectionFailed;
            _RedisConnectorHelper.RedisUtils_Connect(0, config["RedisKeySentinel1"], config["RedisKeySentinel2"], config["RedisKeySentinel3"], config["RedisKeySentinel4"], config["RedisKeySentinel5"], config["RedisPassword"], config["RedisServiceName"]);

            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    _dataManager = new SqlServerDataManager(config["SQLServerNameOfStaticDataDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
            //    _storeLogs = new StoreLogs(_dataManager, _logger, "[HIS].[HIS_LOGS_INSERT]");

            //}
            //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _dataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
                _storeLogs = new StoreLogs(_dataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");
            }

            //_dataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
            //_storeLogs = new StoreLogs(_dataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");
            var historyDataRequest = new HistoryDataRequest
            {
                RequireMeasurements = true,
                RequireMarker = true,
                RequireScadaEvent = false,
                RequireEquipment = false,
                RequireConnectivityNode = false,
            };

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer);
            _repository = new Repository(_logger, _dataManager, _RedisConnectorHelper);
            _sdk_template_manager = new SDK_Template_Manager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_sdk_template_manager, _rpcService, _cpsRuntimeDataBuffer);
        }
        private void _RedisConnectorHelper_ConnectionFailed(object sender, StackExchange.Redis.ConnectionFailedEventArgs e)
        {
            _logger.WriteEntry("Redis Connection Failed", LogLevels.Error);

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();

            _logger.WriteEntry("Start of running SDK_Template.", LogLevels.Info);


            _logger.WriteEntry("Loading data from database/redis is started.", LogLevels.Info);

            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database/redis is completed", LogLevels.Info);
            

            _runtimeDataReceiver.Start();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop SDK_Template", LogLevels.Info);
            _storeLogs.Stop();
            _dataManager.Close();

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            _sdk_template_manager.Function3();
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
