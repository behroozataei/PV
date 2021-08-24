using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Google.Protobuf.Collections;

using Irisa.Logger;
using Irisa.Common;
using Irisa.Message;
using Irisa.Message.CPS;

namespace OCP
{
    internal sealed class RuntimeDataReceiver
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly IProcessing _dataProcessing;
        private readonly RpcService _rpcService;
        private readonly BlockingCollection<CpsRuntimeData> _cpsRuntimeDataBuffer;
        private bool _isWorking;

        internal RuntimeDataReceiver(ILogger logger, IRepository repository, IProcessing dataProcessing,
            RpcService rpcService, BlockingCollection<CpsRuntimeData> cpsRuntimeDataBuffer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataProcessing = dataProcessing ?? throw new ArgumentNullException(nameof(dataProcessing));
            _rpcService = rpcService ?? throw new ArgumentNullException(nameof(rpcService));
            _cpsRuntimeDataBuffer = cpsRuntimeDataBuffer ?? throw new ArgumentNullException(nameof(cpsRuntimeDataBuffer));
        }

        public void Start()
        {
            var historyDataRequest = new HistoryDataRequest
            {
                RequireMeasurements = true,
                RequireMarker = true,
                RequireScadaEvent = false,
                RequireEquipment = false,
                RequireConnectivityNode = false,
            };

            _isWorking = true;
            _rpcService.ConnectAsync(historyDataRequest);
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
                        ProcessRuntimeData(runtimeData.Events);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteEntry(ex.Message, LogLevels.Error);
                    }
                }
            });
        }

        private void ProcessRuntimeData(RepeatedField<ScadaEventData> events)
        {
            foreach(var eventItem in events)
            {
                string strguid = "059d74d9-9d2a-4059-b3ab-a559992b9e03"; //MIS2-63-FCB-TRIP
                
                 if (eventItem.ElementId == strguid || eventItem.ElementId == strguid.ToUpper())

                if (eventItem.EventStatus ==  3) //3 = Acknowledge Event
                {
                        ;
                 
                    //System.Diagnostics.Debugger.Break();

                }
            }

        }
        private void ProcessRuntimeData(RepeatedField<MeasurementData> measurements)
        {
            foreach (var measurement in measurements)
            {

                var checkPoint = _repository.GetCheckPoint(Guid.Parse(measurement.MeasurementId));
                if (checkPoint != null)
                {
                    //#if DEBUG
                    //if (checkPoint.Name == "CP16_CSM_T1B-C")
                    //{
                    //    measurement.Value = 420;
                    //    measurement.QualityCodes = (int)QualityCodes.LocalOutOfRange;

                    //    _logger.WriteEntry(checkPoint.Name + " " + checkPoint.Value.ToString(), LogLevels.Info);
                    //}
                    //#endif

                    checkPoint.Value = measurement.Value;
                    checkPoint.QualityCodes = measurement.QualityCodes;
                    // 2021.04.24 A.K and B.A, added these lines:
                    if (((QualityCodes)measurement.QualityCodes != QualityCodes.None) && GlobalData.CPSStatus == true)
                    {
                        _logger.WriteEntry("Quality Error : " + "QualityCode = " + (QualityCodes)measurement.QualityCodes + " ; Value = " + measurement.Value.ToString() + " ; Network Path = " + checkPoint.NetworkPath.ToString(), LogLevels.Warn);
                        _dataProcessing.QualityError(checkPoint, (QualityCodes)measurement.QualityCodes, SinglePointStatus.Appear);

                    }
                    else if (((QualityCodes)checkPoint.QualityCodes_Old != QualityCodes.None) && GlobalData.CPSStatus == true)
                        _dataProcessing.QualityError(checkPoint, (QualityCodes)measurement.QualityCodes, SinglePointStatus.Disappear);

                    checkPoint.Quality = OCPQualityConvertor.GetCheckPointQuality((QualityCodes)measurement.QualityCodes);
                    checkPoint.QualityCodes_Old = checkPoint.QualityCodes;

                    //#if DEBUG                                        
                        //if(( checkPoint.Name == "CP23_MIS_T3AN_MV3") || (checkPoint.Name == "CP36_MIS_T5AN"))
                        //    _logger.WriteEntry(checkPoint.Name + " " + checkPoint.Value.ToString(), LogLevels.Info);
                    //#endif
                }

                var ocpScadaPoint = _repository.GetOCPScadaPoint(Guid.Parse(measurement.MeasurementId));
                if (ocpScadaPoint != null)
                {
                    
                    ocpScadaPoint.Value = measurement.Value;
                    // 2021.04.24 A.K and B.A, added these lines:
                    if ((QualityCodes)measurement.QualityCodes != QualityCodes.None) 
                    {
                        _logger.WriteEntry("Quality warning : " + "QualityCode = " + (QualityCodes)measurement.QualityCodes + " ; Value = " + measurement.Value.ToString() + " ; Network Path = " + ocpScadaPoint.NetworkPath.ToString(), LogLevels.Warn);
                    }


                    ocpScadaPoint.Quality = OCPQualityConvertor.GetCheckPointQuality((QualityCodes)measurement.QualityCodes);
                }
            }
        }
    }
}