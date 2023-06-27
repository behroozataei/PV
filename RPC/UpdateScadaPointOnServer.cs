using Google.Protobuf.WellKnownTypes;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;


namespace RPC
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

        
        public bool SendAlarm(RPCScadaPoint ascadaPoint, SinglePointStatus ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (ascadaPoint is null)
                {
                    _logger.WriteEntry($"ScadaPoint {ascadaPoint.Name} is NULL", LogLevels.Warn);
                    return executed;
                }

                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "RPC", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry(reply.Log, LogLevels.Error);
                else
                {
                    executed = true;
                    //      _logger.WriteEntry("Send Alarm for " + ascadaPoint.NetworkPath + " ; '" + alarmText + "'", LogLevels.Info);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(Guid id, SinglePointStatus ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (id == Guid.Empty)
                {
                    _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                    return executed;
                }

                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "RPC", ElementId = id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry(reply.Log, LogLevels.Error);
                else
                {
                    executed = true;
                    //      _logger.WriteEntry("Send Alarm for " + ascadaPoint.NetworkPath + " ; '" + alarmText + "'", LogLevels.Info);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Send Alarm for {id} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteAnalog(RPCScadaPoint scadaPoint, float value)
        {

            var executed = false;
            if (scadaPoint is null)
            {
                _logger.WriteEntry($"ScadaPoint {scadaPoint.Name} is NULL to WriteAnalog Value ", LogLevels.Warn);
                return executed;
            }
           
            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "RPC", ElementId = scadaPoint.Id.ToString(), Value = value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("RPC", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Avergae of {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteDigital(RPCScadaPoint scadaPoint, SinglePointStatus Status )
        {

            var executed = false;
            if (scadaPoint is null)
            {
                _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                return executed;
            }

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "RPC", ElementId = scadaPoint.Id.ToString(), Value = (float) Status });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("RPC", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for  {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(RPCScadaPoint ascadaPoint, DigitalSingleStatus ev, string alarmText)
        {
            bool executed = false;

            try
            {
                if (ascadaPoint is null)
                {
                    _logger.WriteEntry($"ScadaPoint  { ascadaPoint.Name} is NULL to Send Alarm", LogLevels.Warn);
                    return executed;
                }

                // TODO: alarmText where to go? Final check only
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "RPC", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("RPC", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        public bool ApplyMarkerCommand(RPCScadaPoint ascadaPoint)
        {
            try
            {
                var markerModifierRequset = new MarkerModifierRequset()
                {
                    Console = "RPC",
                    User = "mscfunction",
                };

                var newmarker = new MarkerData()
                {
                    ElementId = ascadaPoint.Id.ToString(),
                    MarkerType = 2,     // Blocked = 2
                    IsOwnership = true,
                    IsSetMarker = false,
                    LastChanged = Timestamp.FromDateTime(DateTime.UtcNow),
                    User = "mscfunction",
                    OwnershipLevel = 0
                };
                markerModifierRequset.Markers.Add(newmarker);

                var response = _commandService.MarkerModifierCommand(markerModifierRequset);
                if (!response.Executed)
                {
                    _logger.WriteEntry($"Error in appling 'Remove Blocked Makrer' for {ascadaPoint.NetworkPath}", LogLevels.Error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.ToString(), LogLevels.Error);
                return false;
            }
        }



    }
}
