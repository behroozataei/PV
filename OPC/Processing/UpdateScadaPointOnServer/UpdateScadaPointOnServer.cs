using System;

using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;

namespace OPC
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

        public bool WriteSCADAPoint(ScadaPoint aSCADAPoint)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "OPC", ElementId = aSCADAPoint.Id.ToString(), Value = aSCADAPoint.Value });

            try
            {
                var reply = _cpsCommandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("DC.OPC", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Command is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(ScadaPoint ascadaPoint, SinglePointStatus ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (ascadaPoint is null)
                {
                    _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                    return executed;
                }

                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "DC.OPC", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _cpsCommandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry(reply.Log, LogLevels.Error);
                else
                {
                    executed = true;
                    _logger.WriteEntry("Send Alarm for " + ascadaPoint.NetworkPath + " ; '" + alarmText + "'", LogLevels.Info);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }
    }
}
