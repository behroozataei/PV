using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;

namespace LSP
{
    internal sealed class ChangeControlStateOnServer
    {
        private readonly ILogger _logger;
        private readonly ICpsCommandService _scadaCommand;

        internal ChangeControlStateOnServer(ILogger logger, ICpsCommandService scadaCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scadaCommand = scadaCommand ?? throw new ArgumentNullException(nameof(scadaCommand));
        }

        internal bool SendCommand(Guid digitalMeasuremntId, float value)
        {
            var executed = false;

            var controlStateRequest = new ControlStateRequest
            {
                MeasurementId = digitalMeasuremntId.ToString(),
                Console = "LSP",
                User = "LSP",
                Value = value,
                Force = true
            };

            try
            {
                var reply = _scadaCommand.ChangeStateCommand(controlStateRequest);

                if (reply.Executed == false)
                    _logger.WriteEntry(reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry("Change command state is not executed.", LogLevels.Error);
            }

            return executed;
        }
    }
}
