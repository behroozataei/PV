using Irisa.Common;
using System;

namespace DCIS
{
    public class ArchivedPoint
    {
        public ArchivedPoint(int meterProxyId, string meterName, Guid accumulatorMeasurementId, string accumulatorNetworkPath,
            Guid analogMeasurementId, string analogNetworkPath)
        {
            MeterProxyId = meterProxyId;
            MeterName = meterName;
            AnalogMeasurementId = analogMeasurementId;
            AnalogNetworkPath = analogNetworkPath;
            AccumulatorMeasurementId = accumulatorMeasurementId;
            AccumulatorNetworkPath = accumulatorNetworkPath;
        }

        public int MeterProxyId { get; }
        public string MeterName { get; }
        public Guid AnalogMeasurementId { get; set; }
        public string AnalogNetworkPath { get; }
        public Guid AccumulatorMeasurementId { get; }
        public string AccumulatorNetworkPath { get; }
    }
    public enum SinglePointStatus
    {
        Disappear = 0,
        Appear = 1
    }

    public struct SampleData
    {
        public DateTime dateTime { get; set; }
        public float value { get; set; }
        public QualityCodes qualityCode { get; set; }
    }
}
