using System;

namespace EEC
{
    internal sealed class EECCycle
    {
        internal EECCycle(int cycleId, DateTime cycleValue)
        {
            CycleId = cycleId;
            CycleValue = cycleValue;
        }

        public int CycleId { get; set; }
        public DateTime CycleValue { get; set; }
    }
}
