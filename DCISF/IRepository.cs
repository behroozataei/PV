
using System;
using System.Collections.Generic;
using Irisa.Common;

namespace DCIS
{
    public interface IRepository
    {
        ArchivedPoint? GetHisPoint(int metereId);
        IEnumerable<ShiftTimeInfo> GetShiftTimeInfoList();
        IEnumerable<int> GetMeterIdList();
        bool TryGetArchiveFromExactDateTime(Guid accumulatorMeasurementId, CalculationTimePerShift duration,  Queue<SampleData> accData);
        bool GetFirstData(Guid analogMeasurementId, CalculationTimePerShift duration, out float value);
    }
}
