using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;

namespace MAB
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

        public bool SendMAB(MABScadaPoint MAB, MABScadaPoint MAB1, MABScadaPoint MAB2, MABScadaPoint MAB3, MABScadaPoint MAB4,
            MABScadaPoint MAB5, MABScadaPoint MAB6, MABScadaPoint MAB7)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB.Id.ToString(), Value = (MAB.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB1.Id.ToString(), Value = (MAB1.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB2.Id.ToString(), Value = (MAB2.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB3.Id.ToString(), Value = (MAB3.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB4.Id.ToString(), Value = (MAB4.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB5.Id.ToString(), Value = (MAB5.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB6.Id.ToString(), Value = (MAB6.Value) });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = MAB7.Id.ToString(), Value = (MAB7.Value) });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("MAB", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Command is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendEAFGroups(MABScadaPoint EAF1_Group, MABScadaPoint EAF2_Group, MABScadaPoint EAF3_Group, MABScadaPoint EAF4_Group,
                                    MABScadaPoint EAF5_Group, MABScadaPoint EAF6_Group, MABScadaPoint EAF7_Group, MABScadaPoint EAF8_Group)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF1_Group.Id.ToString(), Value = (float)EAF1_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF2_Group.Id.ToString(), Value = (float)EAF2_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF3_Group.Id.ToString(), Value = (float)EAF3_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF4_Group.Id.ToString(), Value = (float)EAF4_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF5_Group.Id.ToString(), Value = (float)EAF5_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF6_Group.Id.ToString(), Value = (float)EAF6_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF7_Group.Id.ToString(), Value = (float)EAF7_Group.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = EAF8_Group.Id.ToString(), Value = (float)EAF8_Group.Value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("MAB", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Command is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        public bool SendTransBusbars(MABScadaPoint T1AN_BB, MABScadaPoint T2AN_BB, MABScadaPoint T3AN_BB, MABScadaPoint T5AN_BB, MABScadaPoint T7AN_BB, MABScadaPoint T8AN_BB)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T1AN_BB.Id.ToString(), Value = (float)T1AN_BB.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T2AN_BB.Id.ToString(), Value = (float)T2AN_BB.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T3AN_BB.Id.ToString(), Value = (float)T3AN_BB.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T5AN_BB.Id.ToString(), Value = (float)T5AN_BB.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T7AN_BB.Id.ToString(), Value = (float)T7AN_BB.Value });
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = T8AN_BB.Id.ToString(), Value = (float)T8AN_BB.Value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("MAB", reply.Log, LogLevels.Warn);
                else
                    executed = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry($"Command is not executed, {ex.Message}", LogLevels.Error);
            }

            return executed;
        }

        // WriteDigital: It maybe causes SendAlarm, but no everytime.
        public bool WriteDigital(MABScadaPoint ascadaPoint, int ev, string alarmText)
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
                    new CalculatedValueItem() { Console = "MAB", ElementId = ascadaPoint.Id.ToString(), Value = (float)ev, CauseOfStatusChange = alarmText });

                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("MAB", reply.Log, LogLevels.Error);
                else
                    executed = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry($"Write Digital for {ascadaPoint.Name} is not executed, {excep.Message}", LogLevels.Error, excep);
            }

            return executed;
        }

        public bool WriteAnalog(MABScadaPoint scadaPoint, float value)
        {
            var executed = false;

            var applyCalculatedValue = new ApplyCalculatedValueRequest();
            applyCalculatedValue.Items.Add(
                new CalculatedValueItem() { Console = "MAB", ElementId = scadaPoint.Id.ToString(), Value = value });

            try
            {
                var reply = _commandService.ApplyCalculatedValue(applyCalculatedValue);

                if (reply.Executed == false)
                    _logger.WriteEntry("MAB", reply.Log, LogLevels.Warn);
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