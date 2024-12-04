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

namespace SRC_FEED_DETECTION
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
        private readonly Source_Feed_detection_Manager _source_Feed_Detect_Manager;
       
        private readonly IConfiguration _config;
        public WorkerService(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetService<IConfiguration>();
            _logger = serviceProvider.GetService<ILogger>();
            var test = _config["OracleServicename"];

            _dataManager = new Irisa.DataLayer.Oracle.OracleDataManager(_config["OracleServicename"], _config["OracleDatabaseAddress"], _config["OracleStaticUser"], _config["OracleStaticPassword"]);
            _storeLogs = new StoreLogs(_dataManager, _logger, "SCADAHIS.\"HIS_HisLogs_Insert\"");

            var historyDataRequest = new HistoryDataRequest
            {
                RequireMeasurements = true,
                RequireMarker = true,
                RequireScadaEvent = false,
                RequireEquipment = false,
                RequireConnectivityNode = false,
            };

           

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer, GetRpcSslCredentials());
            _repository = new Repository(_logger, _dataManager);
            _source_Feed_Detect_Manager = new Source_Feed_detection_Manager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, (IProcessing)_source_Feed_Detect_Manager, _rpcService, _cpsRuntimeDataBuffer);
        }

               


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogReceived += OnLogReceived;
            _storeLogs.Start();
            _logger.WriteEntry("Start of running SRC_FEED_DETECTION ... ***************************************", LogLevels.Info);

           
            _logger.WriteEntry(">>>>> Connected to CPS", LogLevels.Info);

           
          
            if (_repository.Build() == false)
                return Task.FromException<Exception>(new Exception("Create repository is failed"));
            else
                _logger.WriteEntry("Loading scada point information is completed", LogLevels.Info);

           // _source_Feed_Detect_Manager.Build();

            _rpcService.StateChanged += RpcStateChanged;
            _runtimeDataReceiver.Start();
            _source_Feed_Detect_Manager.CheckCPSStatus();


            
            _logger.WriteEntry("End of preparing data for SFD ... ***************************************", LogLevels.Info);
           

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeDataReceiver.Stop();

            _logger.WriteEntry("Stop SFD", LogLevels.Info);
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
                    Thread.Sleep(3000);
                    _source_Feed_Detect_Manager.SetCPSStatus(true);
                });
            }

            if (e.State == GrpcCommunicationState.Disconnect)
            {
                _source_Feed_Detect_Manager.SetCPSStatus(false);
                _logger.WriteEntry("CPS is going to Disconnect", LogLevels.Info);
            }
            if (e.State == GrpcCommunicationState.Connecting)
            {
                _source_Feed_Detect_Manager.SetCPSStatus(false);
                _logger.WriteEntry("CPS is going to Connecting", LogLevels.Info);
            }

        }

        private RpcSslCredentials? GetRpcSslCredentials()
        {
            var dataTable = _dataManager.GetRecord("APP_GRPC_CERTIFICATION_SELECT", CommandType.StoredProcedure);
            string rootCertificate="";
            string clientCertificateChain = "";
            string clientPrivateKey = "";

            if (dataTable.Rows.Count > 0)
            {
                 rootCertificate = Convert.ToString(dataTable.Rows[0]["ROOT_CERTIFICATE"]);
                 clientCertificateChain = Convert.ToString(dataTable.Rows[0]["CLIENT_CERTIFICATE_CHAIN"]);
                 clientPrivateKey = Convert.ToString(dataTable.Rows[0]["CLIENT_PRIVATE_KEY"]);
                 return new RpcSslCredentials(rootCertificate, clientCertificateChain, clientPrivateKey);
            }
           

            return null;
        }
       

    }
}
