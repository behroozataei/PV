using System;

namespace DCIS
{
    public class ShiftTimeInfo
    {
        internal ShiftTimeInfo(int shiftWorkId, TimeSpan shiftStartTime, TimeSpan shiftEndTime, TimeSpan calculationStartTime)
        {
            ShiftWorkId = shiftWorkId;
            ShiftStartTime = shiftStartTime;
            ShiftEndTime = shiftEndTime;
            CalculationStartTime = calculationStartTime;
        }

        public int ShiftWorkId { get; }
        public TimeSpan ShiftStartTime { get; }
        public TimeSpan ShiftEndTime { get; }
        public TimeSpan CalculationStartTime { get; }
    }
}