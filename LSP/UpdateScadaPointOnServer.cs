using Google.Protobuf.WellKnownTypes;
using Irisa.Common.Utils;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Threading.Tasks;

namespace LSP
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

        public bool WriteAnalog(LSPScadaPoint scadaPoint, float value)
        {
            var executed = false;

            try
            {
                if (scadaPoint == null)
                {
                    _logger.WriteEntry("Error: Input scadaPoint is null", LogLevels.Error);
                    return executed;
                }
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "LSP", ElementId = scadaPoint.Id.ToString(), Value = value });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("LSP", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Write data for Avergae of {scadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        // WriteDigital: It maybe causes SendAlarm, but no everytime.
        public bool WriteDigital(LSPScadaPoint ascadaPoint, SinglePointStatus ev, string alarmText)
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
                    new CalculatedValueItem() { Console = "LSP", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("LSP", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Send Alarm for {ascadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        public bool SendCommandTestRetry()
        {
            _logger.WriteEntry("SendCommandTestRetry 22222222222222222 ", LogLevels.Warn);
            try
            {
                var controls = new System.Collections.ObjectModel.Collection<Tuple<string, string>>
                {
                    //new Tuple<string, string>("63ad911c-ceca-446c-92c9-f1582f0adcf1", "Network/Substations/EFS/6.6kV/C.54/CB/STATE"),
                    //new Tuple<string, string>("7e4974bb-bd45-4142-96eb-73f4170e6664", "Network/Substations/EFS/6.6kV/C.55/CB/STATE"),
                    //new Tuple<string, string>("c09ace81-fc50-4420-8182-98ff8cd22f66", "Network/Substations/EFS/6.6kV/C.52/CB/STATE"),
                    //new Tuple<string, string>("b94f5db0-59aa-4cdf-b91f-2b65d0a797b0", "Network/Substations/EFS/6.6kV/C.53/CB/STATE")
                    //new Tuple<string, string>("699002d5-575a-4ba2-bd7b-efc3cbd54cab", "Network/Substations/MIS1/6.6kV/C.07/CB/STATE"),
                    //new Tuple<string, string>("989bf3d8-2d76-47ba-9b0a-0dd944b90d5c", "Network/Substations/MIS1/6.6kV/C.08/CB/STATE"),
                    //new Tuple<string, string>("dd1bc017-4b2b-47da-ba39-f810b9001737", "Network/Substations/MIS1/6.6kV/C.11/CB/STATE"),
                    new Tuple<string, string>("8655CC5B-9AC1-4CFF-A9B5-5BFE96BB69FA", "Network/Substations/MIS1/6.6kV/C.16/CB/STATE")
                    //new Tuple<string, string>("dbe1303f-410d-4a52-836a-2dd81e868f55", "Network/Substations/MIS2/63KV/MO1/DS2/STATE")

                };

                foreach (var commandItem in controls)
                {
                    ControlStateRequest controlRequest = new ControlStateRequest
                    {
                        MeasurementId = commandItem.Item1,
                        Console = "LSPTEST",
                        Force = true,
                        User = "mscfunction",    // It is a constant value with "mscfunction"
                        Value = (int)Breaker_Status.bClose
                    };

                    for (int cntr = 0; cntr < 3; cntr++)
                    {
                        _logger.WriteEntry($"SendCommand for Item: { commandItem.Item2}  ; cntr = {cntr} ; {DateTime.UtcNow.ToLocalFullDateAndTimeString()}", LogLevels.Info);



                        var reply = _scadaCommand.ChangeStateCommand(controlRequest, 2);
                        if (reply.Executed)
                        {
                            _logger.WriteEntry("SendCommand Exectuted", LogLevels.Info);
                            break;
                        }
                        else
                        {
                            _logger.WriteEntry(reply.Log, LogLevels.Info);
                            //System.Threading.Thread.Sleep(500);
                        }
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


        public bool SendCommandTest(LSPScadaPoint scadaPoint, int value)
        {
            try
            {
                _logger.WriteEntry($"Send Command for {scadaPoint.Name} is received with {value}", LogLevels.Info);

                var controls = new System.Collections.ObjectModel.Collection<Tuple<string, string>>
                {
                    //new Tuple<string, string>("387b29ca-f0d6-47a2-839b-dea431364ab6", "Network/Substations/MIS1/63KV/ML8/CB/STATE"),
                    //new Tuple<string, string>("9a98b7ac-8cd8-4b97-89a4-09ac1996372d", "Network/Substations/MIS1/63KV/M87/CB/STATE"),
                    new Tuple<string, string>("ddad2847-6117-4a6c-9b41-f4743f33ad36", "Network/Substations/OXH/6.6kV/C.05/CB/STATE"),
                    new Tuple<string, string>("c825508e-5f87-4773-96ef-23126801784f", "Network/Substations/OXH/6.6kV/C.17/CB/STATE"),
                };

                foreach (var commandItem in controls)
                {
                    ControlStateRequest controlRequest = new ControlStateRequest
                    {
                        MeasurementId = commandItem.Item1,
                        Console = "LSPTEST",
                        Force = true,
                        User = "mscfunction",    // It is a constant value with "mscfunction"
                        Value = (int)Breaker_Status.BOpen
                    };

                    _ = SendCommandAsync(controlRequest);
                }
                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }

        public async Task<bool> SendCommandAsync(ControlStateRequest controlRequest)
        {
            try
            {
                _logger.WriteEntry($"Send Command for {controlRequest.MeasurementId} is received with {controlRequest.Value}", LogLevels.Info);

                var reply = await _scadaCommand.ChangeStateCommandAsync(controlRequest, 5);

                if (reply.Executed == false)
                {
                    _logger.WriteEntry("LSPTEST " + reply.Log, LogLevels.Warn);
                    return false;
                }
                else
                    _logger.WriteEntry("LSP Command for " + $"Send Command for {controlRequest.MeasurementId} is received with {controlRequest.Value} " + reply.Log, LogLevels.Info);

                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }


        public bool SendCommandTest2(String scadaPoint, ControlStateRequest controlRequest)
        {
            try
            {
                //_logger.WriteEntry($"Send Command for {scadaPoint} is received with {controlRequest.Value}", LogLevels.Info);


                //var reply = _scadaCommand.ChangeStateCommandAsync(controlRequest, 12);

                //if (reply.Result.Executed == false)
                //    _logger.WriteEntry("LSPTEST " +reply.Result.Log, LogLevels.Warn);
                //else
                //    _logger.WriteEntry("LSP Command for " + $"Send Command for {scadaPoint} is received with {controlRequest.Value} " + reply.Result.Log, LogLevels.Info);

                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }

        public bool SendCommand(LSPScadaPoint scadaPoint, int value)
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
                controlRequest.Console = "LSP";
                controlRequest.Force = true;
                controlRequest.MeasurementId = scadaPoint.Id.ToString();

                controlRequest.User = "mscfunction";    // It is a constant value with "mscfunction"
                controlRequest.Value = value;

                // For "TEST ONLY" these lines will be uncommented!
                //_logger.WriteEntry($"Send Command is bypassed because of DEVELOPEMENT !!!", LogLevels.Info);
                //return true;

                // For "REAL COMMAND ONLY" these lines will be executed!
                for (int cntr = 0; cntr < 3; cntr++)
                {
                    _logger.WriteEntry($"SendCommand for Item: {scadaPoint.NetworkPath}  ; cntr = {cntr} ; {DateTime.UtcNow.ToIranStandardTime()}", LogLevels.Info);
                    var reply = _scadaCommand.ChangeStateCommand(controlRequest, 2);
                    if (reply.Executed)
                    {
                        _logger.WriteEntry("LSP Command for " + $"{scadaPoint.NetworkPath} is received with {value} " + reply.Log, LogLevels.Info);
                        break;
                    }
                    else
                    {
                        _logger.WriteEntry(reply.Log, LogLevels.Error);
                        //System.Threading.Thread.Sleep(500);
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

        public bool SendCommandSFSC(LSPScadaPoint scadaPoint, int value)
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
                controlRequest.Console = "LSPSFSC";
                controlRequest.Force = true;
                controlRequest.MeasurementId = scadaPoint.Id.ToString();

                controlRequest.User = "mscfunction";    // It is a constant value with "mscfunction"
                controlRequest.Value = value;

                // for test only these lines will be uncommented!
                //_logger.WriteEntry($"send command is bypassed because of developement !!!", LogLevels.Info);
                //return true;

                // For "REAL COMMAND ONLY" these lines will be executed!
                for (int cntr = 0; cntr < 3; cntr++)
                {
                    _logger.WriteEntry($"SendCommand for Item: {scadaPoint.NetworkPath}  ; cntr = {cntr} ; {DateTime.UtcNow.ToIranStandardTime()}", LogLevels.Info);
                    var reply = _scadaCommand.ChangeStateCommand(controlRequest, 2);
                    if (reply.Executed)
                    {
                        _logger.WriteEntry("LSP SFSC Command for " + $"{scadaPoint.NetworkPath} is received with {value} " + reply.Log, LogLevels.Info);
                        break;
                    }
                    else
                    {
                        _logger.WriteEntry(reply.Log, LogLevels.Error);
                        //System.Threading.Thread.Sleep(500);
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

        public bool SendAlarm(LSPScadaPoint ascadaPoint, SinglePointStatus ev, string alarmText)
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
                    new CalculatedValueItem() { Console = "LSP", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

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

        /*private void UpdateSourceCommandAverage(LSPScadaPoint scadaPoint, ref MeasurementSourceCommand command)
        {
            command.ElementId = scadaPoint.Id;
            return;
        }*/

        public bool ApplyMarkerCommand(LSPScadaPoint ascadaPoint)
        {
            try
            {
                var markerModifierRequset = new MarkerModifierRequset()
                {
                    Console = "LSP",
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

                var response = _scadaCommand.MarkerModifierCommand(markerModifierRequset);
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
