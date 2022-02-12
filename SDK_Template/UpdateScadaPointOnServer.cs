using Google.Protobuf.WellKnownTypes;
using Irisa.Common.Utils;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Threading.Tasks;

namespace SDK_Template
{
    public class UpdateScadaPointOnServer
    {
        private readonly ILogger _logger;
        private readonly ICpsCommandService _scadaCommand;

        public UpdateScadaPointOnServer(ILogger logger, ICpsCommandService scadaCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scadaCommand = scadaCommand ?? throw new ArgumentNullException(nameof(scadaCommand));
        }

        public bool WriteSample(SDK_Template_ScadaPoint checkPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "SDK_TEMPLATE", ElementId = checkPoint.Id.ToString(), Value = value });

            
            try
            {
                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("SDK_TEMPLATE", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Allowed Active Power of {checkPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendCommand(SDK_Template_ScadaPoint scadaPoint, int value)
        {
            try
            {
                if (scadaPoint is null)
                {
                    _logger.WriteEntry("Error: Scadapoint for sending command is NULL!", LogLevels.Error);
                    return false;
                }

                _logger.WriteEntry($"Send Command for {scadaPoint.Name} is received with {value}", LogLevels.Info);

                var controlRequest = new ControlStateRequest();
                controlRequest.Console = "SDK_TEMPLATE";
                controlRequest.Force = true;
                controlRequest.MeasurementId = scadaPoint.Id.ToString();

                controlRequest.User = "mscfunction";    // It is a constant value with "mscfunction"
                controlRequest.Value = value;

                
                for (int cntr = 0; cntr < 3; cntr++)
                {
                    _logger.WriteEntry($"SendCommand for Item: {scadaPoint.NetworkPath}  ; cntr = {cntr} ; {DateTime.UtcNow.ToLocalFullDateAndTimeString()}", LogLevels.Info);
                    var reply = _scadaCommand.ChangeStateCommand(controlRequest, 2);
                    if (reply.Executed)
                    {
                        _logger.WriteEntry("SDK_TEMPLATE Command for " + $"{scadaPoint.NetworkPath} is received with {value} " + reply.Log, LogLevels.Info);
                        break;
                    }
                    else
                    {
                        _logger.WriteEntry(reply.Log, LogLevels.Error);
                       
                    }
                }

                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }
    }
}
