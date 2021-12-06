using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using COM;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using Irisa.DataLayer;
using Irisa.DataLayer.SqlServer;
using Irisa.Common.Utils;

namespace MAB
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
        private readonly MABManager _mabManager;
        private readonly RedisUtils _RedisConnectorHelper;
        public WorkerService(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            _logger = serviceProvider.GetService<ILogger>();
            _RedisConnectorHelper = new RedisUtils(0);

            
            _dataManager = new Irisa.DataLayer.Oracle.OracleDataManager(config["OracleServicename"], config["OracleDatabaseAddress"], config["OracleStaticUser"], config["OracleStaticPassword"]);
            _storeLogs = new StoreLogs(_dataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");

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
            _mabManager = new MABManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_mabManager, _rpcService, _cpsRuntimeDataBuffer);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();

            _logger.WriteEntry("Start of running MAB.", LogLevels.Info);
            _logger.WriteEntry("Loading data from database/redis is started.", LogLevels.Info);

            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database/redis is completed", LogLevels.Info);

            _mabManager.Build();

            _rpcService.StateChanged += RpcStateChanged;
            _runtimeDataReceiver.Start();
            _mabManager.CheckCPSStatus();


            var taskWaiting = Task.Delay(3000, cancellationToken);
            taskWaiting.ContinueWith((t) =>
            {
                _mabManager.InitializeMAB();

            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop MAB", LogLevels.Info);
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
                _mabManager.ClearEAFGroupsLocal();
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
                _mabManager.ClearEAFGroupsLocal();
                _logger.WriteEntry("CPS is going to Disconnect", LogLevels.Info);
            }
            if (e.State == GrpcCommunicationState.Connecting)
            {
                GlobalData.CPSStatus = false;
                _mabManager.ClearEAFGroupsLocal();
                _logger.WriteEntry("CPS is going to Connecting", LogLevels.Info);
            }

        }
    }
}
