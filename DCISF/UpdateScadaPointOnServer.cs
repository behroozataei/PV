using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;

namespace DCIS
{
    public class UpdateScadaPointOnServer
    {
        private readonly ILogger _logger;
        private readonly ICpsCommandService _cpsCommandService;

        public UpdateScadaPointOnServer(ILogger logger, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cpsCommandService = cpsCommandService ?? throw new ArgumentNullException(nameof(cpsCommandService));
        }

        public bool WriteSCADAPoint(ArchivedPoint scadaPoint, float value)
        {
            var executed = false;

            try
            {
                if (scadaPoint is null)
                {
                    _logger.WriteEntry("Input SCADApoint is null", LogLevels.Error);
                    return false;
                }
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "DCIS", ElementId = scadaPoint.AccumulatorMeasurementId.ToString(), Value = value });

                var reply = _cpsCommandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("DCIS", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for {scadaPoint.MeterName} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(ArchivedPoint ascadaPoint, SinglePointStatus ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (ascadaPoint is null)
                {
                    _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                    return executed;
                }

                // TODO: alarmText is not using now.
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "DCIS", ElementId = ascadaPoint.AnalogMeasurementId.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _cpsCommandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry(reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.MeterName} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }
    }
}