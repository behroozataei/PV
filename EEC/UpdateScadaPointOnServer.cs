using Google.Protobuf.WellKnownTypes;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Threading.Tasks;

namespace EEC
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

        public bool SendOneMinuteEnergyCALCValues(EECScadaPoint PMAXG, EECScadaPoint EBSum, EECScadaPoint EFSum,
                                        EECScadaPoint EAV_Sum, EECScadaPoint ER, EECScadaPoint EBMAX,
                                        EECScadaPoint RESTIME, EECScadaPoint PC, EECScadaPoint PB)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = PMAXG.Id.ToString(), Value = PMAXG.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EBSum.Id.ToString(), Value = EBSum.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EFSum.Id.ToString(), Value = EFSum.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EAV_Sum.Id.ToString(), Value = EAV_Sum.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = ER.Id.ToString(), Value = ER.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EBMAX.Id.ToString(), Value = EBMAX.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = RESTIME.Id.ToString(), Value = RESTIME.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = PC.Id.ToString(), Value = PC.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = PB.Id.ToString(), Value = PB.Value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Commands of one minute energy is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool Send15MinuteEnergy(EECScadaPoint EAV, EECScadaPoint EFSum, EECScadaPoint EBSum, EECScadaPoint ER_Cycle,
           EECScadaPoint ECONEAF_EREAF, EECScadaPoint EPURCH_EC, EECScadaPoint EREAF_ECONEAF, EECScadaPoint EnergyResEnd)
        {
            var executed = false;


            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EAV.Id.ToString(), Value = EAV.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EFSum.Id.ToString(), Value = EFSum.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EBSum.Id.ToString(), Value = EBSum.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = ER_Cycle.Id.ToString(), Value = ER_Cycle.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = ECONEAF_EREAF.Id.ToString(), Value = ECONEAF_EREAF.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EPURCH_EC.Id.ToString(), Value = EPURCH_EC.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EREAF_ECONEAF.Id.ToString(), Value = EREAF_ECONEAF.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = EnergyResEnd.Id.ToString(), Value = EnergyResEnd.Value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Warn);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Commands of 15 minute energy is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteResEnegy(EECScadaPoint scadaPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = scadaPoint.Id.ToString(), Value = value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for ResEnegy {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteEECTelegram(EECScadaPoint scadaPoint, float value)
        {
            var executed = false;
            try
            {
                var applyCalculatedValue = new ApplyCalculatedValueRequest();

                if (scadaPoint is null)
                {
                    _logger.WriteEntry($"Null value is passed for {scadaPoint.Name}", LogLevels.Error);
                    return executed;
                }
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "EEC", ElementId = scadaPoint.Id.ToString(), Value = value });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for EECTelegram {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteAnalog(EECScadaPoint scadaPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "EEC", ElementId = scadaPoint.Id.ToString(), Value = value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Avergae of {scadaPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(EECScadaPoint ascadaPoint, DigitalSingleStatus ev, string alarmText)
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
                    new CalculatedValueItem() { Console = "EEC", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("EEC", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        public bool ApplyMarkerCommand(EECScadaPoint ascadaPoint)
        {
            try
            {
                var markerModifierRequset = new MarkerModifierRequset()
                {
                    Console = "EEC",
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