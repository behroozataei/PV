﻿using COMMON;
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
        private readonly RedisUtils _RTDBManager;
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

            RedisUtils.SetRedisUtilsParams(0, _config["RedisKeySentinel1"], _config["RedisKeySentinel2"], _config["RedisKeySentinel3"], _config["RedisKeySentinel4"], _config["RedisKeySentinel5"], _config["RedisPassword"], _config["RedisServiceName"],
                                                      _config["RedisConName1"], _config["RedisConName2"], _config["RedisConName3"], _config["RedisConName4"], _config["RedisConName5"], _config["IsSentinel"]);
            _RTDBManager = RedisUtils.GetRedisUtils();

            _cpsRuntimeDataBuffer = new BlockingCollection<CpsRuntimeData>();
            _rpcService = new CpsRpcService(_config["CpsIpAddress"], 10000, historyDataRequest, _cpsRuntimeDataBuffer, GetRpcSslCredentials());
            _repository = new Repository(_logger, _staticDataManager, _RTDBManager);
            _opcManager = new OPCManager(_logger, _repository, _rpcService.CommandService);
            _runtimeDataReceiver = new RuntimeDataReceiver(_logger, _repository, _opcManager, _rpcService, _cpsRuntimeDataBuffer);
        }

        private void CallConnection()
        {
            try
            {
                _RTDBManager.RedisUtils_Connect();
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
            while (!_RTDBManager.IsConnected)
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
            Task RedisConMon = new Task(() => RedisConnctionMonitoring("OPC"));
            RedisConMon.Start();


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
        private RpcSslCredentials? GetRpcSslCredentials()
        {
            var dataTable = _staticDataManager.GetRecord("APP_GRPC_CERTIFICATION_SELECT", CommandType.StoredProcedure);

            if (dataTable.Rows.Count > 0)
            {
                var rootCertificate = Convert.ToString(dataTable.Rows[0]["ROOT_CERTIFICATE"]);
                var clientCertificateChain = Convert.ToString(dataTable.Rows[0]["CLIENT_CERTIFICATE_CHAIN"]);
                var clientPrivateKey = Convert.ToString(dataTable.Rows[0]["CLIENT_PRIVATE_KEY"]);

                return new RpcSslCredentials(rootCertificate, clientCertificateChain, clientPrivateKey);
            }
            return null;
        }
        private void RedisConnctionMonitoring(String AppName)
        {
            while (true)
            {
                try
                {
                    if (_RTDBManager.CheckConnection("APP:" + AppName))
                        ;
                    //_logger.WriteEntry("RedisConnctionMonitoring: Redis Connection Ok", LogLevels.Info);
                    else
                    {
                        _logger.WriteEntry("RedisConnctionMonitoring: Redis not Connected", LogLevels.Error);
                        _RTDBManager.RedisConn.Dispose();
                        CallConnection();
                    }
                }
                catch
                {
                    _RTDBManager.RedisConn.Dispose();
                    CallConnection();
                    _logger.WriteEntry("RedisConnctionMonitoring: Redis Connection Error", LogLevels.Error);
                }
                Thread.Sleep(5000);
            }
        }
    }
}
