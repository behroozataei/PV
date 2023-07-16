using System;

namespace DCIS
{
    public class CalculationTimePerShift
    {
        public CalculationTimePerShift(int shiftWorkId, DateTime shiftStartTime, DateTime shiftEndTime, DateTime calculationStartTime)
        {
            ShiftWorkId = shiftWorkId;
            ShiftStartTime = shiftStartTime;
            ShiftEndTime = shiftEndTime;
            CalculationStartTime = calculationStartTime;
        }

        public int ShiftWorkId { get; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public DateTime CalculationStartTime { get; }
    }
}