using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;

namespace SRC_FEED_DETECTION
{
    public class UpdateScadaPointOnServer
    {
        private readonly ILogger _logger;
        private readonly ICpsCommandService _commandService;

        public UpdateScadaPointOnServer(ILogger logger, ICpsCommandService commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

       

        // WriteDigital: It maybe causes SendAlarm, but no everytime.
        public bool WriteDigital(ScadaPoint ascadaPoint, int ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (ascadaPoint is null)
                {
                    _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                    return executed;
                }

                // TODO: alarmText where to go? Final check only
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "SFD", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("SFD", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Write Digital for {ascadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        public bool WriteAnalog(ScadaPoint scadaPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "SFD", ElementId = scadaPoint.Id.ToString(), Value = value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("SFD", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Avergae of {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }
    }
}