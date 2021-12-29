using Google.Protobuf.Collections;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace EEC
{
    internal sealed class RuntimeDataReceiver
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly IProcessing _dataProcessing;
        private readonly CpsRpcService _rpcService;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private bool _isWorking;

        internal RuntimeDataReceiver(ILogger logger, IRepository repository, IProcessing dataProcessing,
            CpsRpcService rpcService, BlockingCollection<CpsRuntimeData> cpsRuntimeDataBuffer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataProcessing = dataProcessing ?? throw new ArgumentNullException(nameof(dataProcessing));
            _rpcService = rpcService ?? throw new ArgumentNullException(nameof(rpcService));
            _cpsRuntimeDataBuffer = cpsRuntimeDataBuffer ?? throw new ArgumentNullException(nameof(cpsRuntimeDataBuffer));
        }

        public void Start()
        {
            _isWorking = true;
            _rpcService.ConnectAsync();
            TakeDataAsync();
        }

        public void Stop()
        {
            _isWorking = false;
            _rpcService.ShutdownAsync();
        }

        private Task TakeDataAsync()
        {
            return Task.Run(() =>
            {
                while (_isWorking)
                {
                    var runtimeData = _cpsRuntimeDataBuffer.Take();
                    if (runtimeData == null) continue;

                    try
                    {
                        ProcessRuntimeData(runtimeData.Measurements);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry(ex.Message, LogLevels.Error);
                    }
                }
            });
        }

        private void ProcessRuntimeData(RepeatedField<MeasurementData> measurements)
        {
            foreach (var measurement in measurements)
            {
                var scadaPoint = _repository.GetScadaPoint(Guid.Parse(measurement.MeasurementId));
                if (scadaPoint != null)
                {
                    scadaPoint.Value = measurement.Value;
                    scadaPoint.Quality = measurement.QualityCodes;

                    _dataProcessing.Check_Apply_EECConst(measurement);
                }
            }
        }
    }
}