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
using COMMON;
using Irisa.DataLayer.SqlServer;

namespace DCIS
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly SqlServerDataManager _linkDBpcsDataManager;
        private readonly DataManager _staticDataManager;
        private readonly DataManager _historicalDataManager;
        private readonly StoreLogs _storeLogs;
        private readonly CpsRpcService _rpcService;
        private readonly Repository _repository;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private readonly RuntimeDataReceiver _runtimeDataReceiver;
        private readonly DCISManager _dcisManager;
        private readonly IConfiguration _config;

        public WorkerService(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetService<IConfiguration>();
            _logger = serviceProvider.GetService<ILogger>();


            _staticDataManager = new SqlServerDataManager(_config["SQLServerNameOfStaticDataDatabase"], _config["SQLServerDatabaseAddress"], _config["SQLServerUser"], _config["SQLServerPassword"]);
            //_staticDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(_config["OracleServicename"], _config["OracleDatabaseAddress"], _config["OracleStaticUser"], _config["OracleStaticPassword"]);
            _historicalDataManager = new SqlServerDataManager(_config["SQLServerNameOfHistoricalDatabase"], _config["SQLServerDatabaseAddress"], _config["SQLServerUser"], _config["SQLServerPassword"]);
           //_historicalDataManager = new Irisa.DataLayer.Oracle.OracleDataManager(_config["OracleServicename"], _config["OracleDatabaseAddress"], _config["OracleHISUser"], _config["OracleHISPassword"]);
            _storeLogs = new StoreLogs(_staticDataManager, _logger, "[HIS].[HIS_LOGS_INSERT]");
            //_storeLogs = new StoreLogs(_staticDataManager, _logger, "SCADA.\"HIS_HisLogs_Insert\"");
            

            var historyDataRequest = new HistoryDataRequest
            {
                RequireMeasurements = true,
                RequireMarker = true,
                RequireScadaEvent = false,
                RequireEquipment = false,
                RequireConnectivityNode = false,
            };

         


            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer);

            _repository = new Repository(_logger, _staticDataManager, _historicalDataManager);
            _dcisManager = new DCISManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_dcisManager, _rpcService, _cpsRuntimeDataBuffer);

        }





        public override Task StartAsync(CancellationToken cancellationToken)
        {
           
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();
            _logger.WriteEntry("Start of running DCIS ... ***************************************", LogLevels.Info);

            while (!COMMON.Connection.PingHost(_config["CpsIpAddress"], 10000))
            {
                _logger.WriteEntry(">>>>> Waiting for CPS Connection", LogLevels.Info);
                Thread.Sleep(5000);
            }
            _logger.WriteEntry(">>>>> Connected to CPS", LogLevels.Info);

                      


            _logger.WriteEntry("Loading data from database is started.", LogLevels.Info);

            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading data from database is completed", LogLevels.Info);

            _dcisManager.StartCyclicOperation();
            //_rpcManager.Build();

            _rpcService.StateChanged += RpcStateChanged;
            _runtimeDataReceiver.Start();
           // _rpcManager.CheckCPSStatus();


            var taskWaiting = Task.Delay(3000, cancellationToken);
            taskWaiting.ContinueWith((t) =>
            {
               // _rpcManager.InitializeRPC();

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            _logger.WriteEntry("End of preparing data for DCIS ... ***************************************", LogLevels.Info);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop DCIS", LogLevels.Info);
            _storeLogs.Stop();
            _staticDataManager.Close();
            _historicalDataManager.Close();

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
                    Thread.Sleep(3000);
                   // GlobalData.CPSStatus = true;
                });
            }

            if (e.State == GrpcCommunicationState.Disconnect)
            {
              //  GlobalData.CPSStatus = false;
                _logger.WriteEntry("CPS is going to Disconnect", LogLevels.Info);
            }
            if (e.State == GrpcCommunicationState.Connecting)
            {
               // GlobalData.CPSStatus = false;
                _logger.WriteEntry("CPS is going to Connecting", LogLevels.Info);
            }

        }
    }
}
