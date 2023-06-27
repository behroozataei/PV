using Irisa.Logger;
using System;
using System.Collections.Generic;

namespace EEC
{
    internal sealed class EECCycleValidator
    {
        private readonly ILogger _logger;
        private readonly List<EECCycle> _EECCycles;

        public EECCycleValidator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _EECCycles = new List<EECCycle>(15);
            for (int i = 0; i < 15; i++)
                _EECCycles.Add(new EECCycle(i + 1, DateTime.MinValue));
        }

        public int GetEECCycleNo()
        {
            var cycleNo = System.DateTime.UtcNow.Minute % 15;

            if (cycleNo == 0)
            {
                var fullCycleTag = _EECCycles[14].CycleValue > DateTime.MinValue;
                _logger.WriteEntry($"In 0-Cycle, FullCycle check= {fullCycleTag.ToString()}, {DateTime.UtcNow.Minute.ToString()}", LogLevels.Info);
            }

            _EECCycles[cycleNo].CycleValue = DateTime.UtcNow;

            if (cycleNo > 0)
            {
                var result = IsContiniuousCycles(cycleNo);
                if (!result)
                    _logger.WriteEntry("Function is not running continuously!", LogLevels.Warn);
            }

            _logger.WriteEntry($"Cycle Number is: {cycleNo}", LogLevels.Info);
            return cycleNo;
        }

        private bool IsContiniuousCycles(int nCurCycle)
        {
            var result = true;
            var CurrHour = 0;
            var PrevHour = 0;
            var PrevMin = 0;
            var CurrMin = 0;

            // Check continuosely in activation times
            var tempForEndVar = nCurCycle;
            for (int i = 1; i <= tempForEndVar; i++)
            {
                PrevHour = _EECCycles[i - 1].CycleValue.Hour;
                PrevMin = _EECCycles[i - 1].CycleValue.Minute;

                CurrHour = _EECCycles[i].CycleValue.Hour;
                CurrMin = _EECCycles[i].CycleValue.Minute;

                if ((PrevHour != CurrHour) || ((PrevMin + 1) != CurrMin))
                    result = false;
            }

            return result;
        }
    }
}