using Google.Protobuf.Collections;
using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LSP
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
                var measurementGUID = Guid.Parse(measurement.MeasurementId);

                if (measurement.MeasurementType == MeasurementDataType.Digital)
                {
                    var checkPoint = _repository.GetCheckPoint(measurementGUID);
                    if (checkPoint != null)
                    {
                        checkPoint.Value = measurement.Value;
                        checkPoint.Quality = (QualityCodes)measurement.QualityCodes;

                    }
                   

                    var lspScadaPoint = _repository.GetLSPScadaPoint(measurementGUID);
                    if (lspScadaPoint != null)
                    {
                        lspScadaPoint.Value = measurement.Value;
                        lspScadaPoint.Quality = (QualityCodes)measurement.QualityCodes;
                        if (measurement.Acknowledged)
                        {
                            if (lspScadaPoint.Name == "LSPActivated" ||
                                lspScadaPoint.Name == "OverloadReset" ||
                                lspScadaPoint.Name == "PCSPowerLimit" ||
                                lspScadaPoint.Name == "WrongNetworkStatus")
                                _dataProcessing.AlarmAcked_Processing(lspScadaPoint);
                        }

                    }

                    _dataProcessing.SCADAEventRaised(measurement);
                    
                }
                else if (measurement.MeasurementType == MeasurementDataType.Analog)
                {
                    var checkPoint = _repository.GetCheckPoint(measurementGUID);
                    if (checkPoint != null)
                    {
                        // TODO: Mr. Khanbazi check
                        if (measurement.Value > 0)
                        {
                            checkPoint.Value = measurement.Value;
                            checkPoint.Quality = (QualityCodes)measurement.QualityCodes;
                        }
                        //else
                        //    _logger.WriteEntry("OnRawDataReceived()..checkPoint with 0 is received! " + checkPoint.NetworkPath + " ; " + measurement.Value, LogLevels.Warn);

                        //_logger.WriteEntry("OnRawDataReceived()..checkPoint " + checkPoint.NetworkPath + ";" + measurement.Value, LogLevels.Info);
                    }

                    var lspScadaPoint = _repository.GetLSPScadaPoint(measurementGUID);
                    //_logger.WriteEntry("OnRawDataReceived()..Analog................: " + measurement.MeasurementId, LogLevels.Info);
                    if (lspScadaPoint != null)
                    {
                        lspScadaPoint.Value = measurement.Value;
                        lspScadaPoint.Quality = (QualityCodes)measurement.QualityCodes;

                        //_logger.WriteEntry("OnRawDataReceived()..Analog..LSPScadaPoint: " + lspScadaPoint.NetworkPath + ";" + measurement.Value, LogLevels.Info);

                        // 2022.03.07 A.K, B.A      Add new SCADAPoint for checking OVERLCOND from OCP, OVERLCONDA
                        if (lspScadaPoint.NetworkPath.Contains("OVERLCONDA"))
                            _dataProcessing.SCADAEventRaised(measurement);
                    }

                    // Updating values for ancealary points of CheckPoints:
                    foreach (var ancPoint in _repository.GetCheckPoints())
                    {
                        if (ancPoint.Average.Id == measurementGUID)
                        {
                            ancPoint.Average.Value = measurement.Value;
                            ancPoint.Average.Quality = (QualityCodes)measurement.QualityCodes;

                            //_logger.WriteEntry("OnRawDataReceived()..ancPoint.Average " + ancPoint.Average.NetworkPath + ";" + measurement.Value, LogLevels.Info);
                        }

                        if (ancPoint.OverloadIT.Id == measurementGUID)
                        {
                            ancPoint.OverloadIT.Value = measurement.Value;
                            ancPoint.OverloadIT.Quality = (QualityCodes)measurement.QualityCodes;

                            //_logger.WriteEntry("OnRawDataReceived()..ancPoint.OverloadIT " + ancPoint.OverloadIT.NetworkPath + ";" + measurement.Value, LogLevels.Info);
                        }

                        if (ancPoint.ActivePower.Id == measurementGUID)
                        {
                            ancPoint.ActivePower.Value = measurement.Value;
                            ancPoint.ActivePower.Quality = (QualityCodes)measurement.QualityCodes;

                            //_logger.WriteEntry("OnRawDataReceived()..ancPoint.ActivePower " + ancPoint.ActivePower.NetworkPath + ";" + measurement.Value, LogLevels.Info);
                        }
                    }
                }
                else
                {
                    var scadaPoint = _repository.GetCheckPoint(measurementGUID);
                    if (scadaPoint != null)
                    {
                        scadaPoint.Value = measurement.Value;
                        scadaPoint.Quality = (QualityCodes)measurement.QualityCodes;
                    }

                    var lspScadaPoint = _repository.GetLSPScadaPoint(measurementGUID);
                    if (lspScadaPoint != null)
                    {
                        lspScadaPoint.Value = measurement.Value;
                        lspScadaPoint.Quality = (QualityCodes)measurement.QualityCodes;
                    }
                }
            }
        }
    }
}