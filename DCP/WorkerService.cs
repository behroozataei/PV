using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;
using Irisa.Common.Utils;

namespace DCP
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly SqlServerDataManager _linkDBpcsDataManager;
        private readonly DataManager _staticDataManager;
        private readonly DataManager _historicalDataManager;
        private readonly StoreLogs _storeLogs;
        private readonly RpcService _rpcService;
        private readonly Repository _repository;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly DCPManager _dcpManager;

        public WorkerService(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            _logger = serviceProvider.GetService<ILogger>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _staticDataManager = new SqlServerDataManager(config["SQLServerNameOfStaticDataDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
                _historicalDataManager = new SqlServerDataManager(config["SQLServerNameOfHistoricalDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
                _storeLogs = new StoreLogs(_staticDataManager, _logger, "[HIS].[HIS_LOGS_INSERT]");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _staticDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
                _historicalDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleHISUser"], config["OracleHISPassword"]);
                _storeLogs = new StoreLogs(_staticDataManager, _logger, "HIS_HisLogs_INSERT");
            }

            _linkDBpcsDataManager = new SqlServerDataManager(config["PCSLinkDatabaseName"], config["PCSLinkDatabaseAddress"], config["PCSLinkUser"], config["PCSLinkPassword"]);
           

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new RpcService(config["CpsIpAddress"], 10000, _cpsRuntimeDataBuffer);

            _repository = new Repository(_logger, _staticDataManager, _historicalDataManager, _linkDBpcsDataManager);
            _dcpManager = new DCPManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_dcpManager, _rpcService, _cpsRuntimeDataBuffer);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
           
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();

            _logger.WriteEntry("Start of running DCP.", LogLevels.Info);
            _logger.WriteEntry("Loading data from database is started.", LogLevels.Info);

            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database is completed", LogLevels.Info);

            _runtimeDataReceiver.Start();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop DCP", LogLevels.Info);
            _storeLogs.Stop();
            _staticDataManager.Close();
            _historicalDataManager.Close();
            _linkDBpcsDataManager.Close();

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
            else if (e.Level == LogLevels.Error || e.Level == LogLevels.Critical || e.Level == LogLevels.Warn)
                Console.ForegroundColor = ConsoleColor.Red;

            if (string.IsNullOrEmpty(e.Exception))
                Console.WriteLine($"{e.TimeStamp.ToLocalFullDateAndTimeString()} ==>   {e.Message}");
            else
                Console.WriteLine($"{e.TimeStamp.ToLocalFullDateAndTimeString()} ==>   \n\tCall site: {e.CallSite} \n\t{e.Message}");

            Console.ResetColor();
        }
    }
}