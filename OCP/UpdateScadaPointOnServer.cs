using System;

using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;

namespace OCP
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

        public bool WriteAAP(OCPCheckPoint checkPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "OCP", ElementId = checkPoint.ALLOWEDACTIVEPOWER_GUID.ToString(), Value = value });

            _logger.WriteEntry($"Write data for AAP of {checkPoint.Name} = {value}", LogLevels.Info);

            try
            {
                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("OCP", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Allowed Active Power of {checkPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteSample(OCPCheckPoint checkPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "OCP", ElementId = checkPoint.SAMPLE_GUID.ToString(), Value = value });

           // _logger.WriteEntry($"Write data for Sample Value of {checkPoint.Name} = {value}", LogLevels.Info);

            try
            {
                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("OCP", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Allowed Active Power of {checkPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteIT(OCPCheckPoint checkPoint, float value)
        {
            var executed = false;

            //_logger.WriteEntry($"Write data for IT of {checkPoint.Name} = {value}", LogLevels.Info);
            try
            {
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "OCP", ElementId = checkPoint.IT_GUID.ToString(), Value = value });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("OCP", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Overload of {checkPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool WriteAverage(OCPCheckPoint checkPoint, float value)
        {
            var executed = false;
            //_logger.WriteEntry($"Write data for Average of {checkPoint.Name} = {value}", LogLevels.Info);

            try
            {
                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "OCP", ElementId = checkPoint.AVERAGE_GUID.ToString(), Value = value });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("OCP", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Write data for Avergae of {checkPoint.Name} is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendAlarm(OCPScadaPoint ascadaPoint, SinglePointStatus ev, string alarmText)
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
                    new CalculatedValueItem() { Console = "OCP", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

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
                if (id==Guid.Empty)
                {
                    _logger.WriteEntry("ScadaPoint is NULL", LogLevels.Warn);
                    return executed;
                }

                var applyCalculatedValue = new ApplyCalculatedValueRequest();
                applyCalculatedValue.Items.Add(
                    new CalculatedValueItem() { Console = "OCP", ElementId=id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

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

        public bool SendOverloadToLSP(OCPScadaPoint ocpSCADAPoint, SinglePointStatus value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "OCP", ElementId = ocpSCADAPoint.Id.ToString(), Value = (float)value });

            try
            {
                var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("OCP", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Command is not executed in SendOverloadToLSP, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        //public bool SendLSPActivated(OCPScadaPoint ocpSCADAPoint, EventStatus value)
        //{
        //    var executed = false;

        //    var applyCalculatedValue = new ApplyCalculatedValueRequest();
        //    applyCalculatedValue.Items.Add(
        //        new CalculatedValueItem() { Console = "OCP", ElementId = ocpSCADAPoint.Id.ToString(), Value = (float)value });

        //    try
        //    {
        //        var reply = _scadaCommand.ApplyCalculatedValue(applyCalculatedValue);

        //        if (reply.Executed == false)
        //            _logger.WriteEntry("OCP", reply.Log, LogLevels.Error);
        //        else
        //            executed = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.WriteEntry($"Command is not executed in SendLSPActivated, {ex.Message}", LogLevels.Error);
        //    }

        //    return executed;
        //}

        /* private void UpdateSourceCommandIT( OCPCheckPoint checkPoint, ref MeasurementSourceCommand command)
         {
             command.ElementId = checkPoint.IT_GUID;
             return ;
         }
         private void UpdateSourceCommandAAP(OCPCheckPoint checkPoint, ref MeasurementSourceCommand command)
         {
             command.ElementId = checkPoint.ALLOWEDACTIVEPOWER_GUID;
             return;
         }
         private void UpdateSourceCommandAverage(OCPCheckPoint checkPoint, ref MeasurementSourceCommand command)
         {
             command.ElementId = checkPoint.AVERAGE_GUID;
             return;
         }*/

    }
}
