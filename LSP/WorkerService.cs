using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

namespace LSP
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly StoreLogs _storeLogs;
        private readonly RpcService _rpcService;
        private readonly Repository _repository;
        private readonly DataManager _staticDataManager;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private readonly LSPManager _lspManager;

        public WorkerService(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            _logger = serviceProvider.GetService<ILogger>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _staticDataManager = new SqlServerDataManager(config["SQLServerNameOfStaticDataDatabase"], config["SQLServerDatabaseAddress"], config["SQLServerUser"], config["SQLServerPassword"]);
                //_staticDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);

                _storeLogs = new StoreLogs(_staticDataManager, _logger, "[HIS].[HIS_LOGS_INSERT]");

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _staticDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
                _storeLogs = new StoreLogs(_staticDataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");
            }


            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new RpcService(config["CpsIpAddress"], 10000, _cpsRuntimeDataBuffer);
            _repository = new Repository(_logger, config);
            _lspManager = new LSPManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, _lspManager, _rpcService, _cpsRuntimeDataBuffer);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();

            _logger.WriteEntry("Start of running LSP.", LogLevels.Info);
            _logger.WriteEntry("Loading data from database is started.", LogLevels.Info);

            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database is completed", LogLevels.Info);

            _rpcService.StateChanged += OnRpcStateChanged;
            _runtimeDataReceiver.Start();
            
            _lspManager.Initialize();

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
                });
            }

            if (e.State == GrpcCommunicationState.Disconnect)
            {
                _logger.WriteEntry("CPS is going to Disconnect", LogLevels.Info);
            }

            if (e.State == GrpcCommunicationState.Connecting)
            {
                _logger.WriteEntry("CPS is going to Connecting", LogLevels.Info);
            }
        }
    }
}
