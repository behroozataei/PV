using Irisa.Common;

using System;
using System.Collections.Generic;
using System.Text;

namespace RPC
{
    public sealed class RPCScadaPoint
    {
        public RPCScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType, string scadaType)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
            PointDirectionType = pointDirectionType;
            ScadaType = scadaType;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Quality { get; set; }
        public PointDirectionType PointDirectionType { get; }
        public float Value { get; set; }
        public string ScadaType { get; }

    }
    public class ACCScadaPoint
    {
        public RPCScadaPoint sPCScadaPoint { get; set; }
        public RPCScadaPoint aPCScadaPoint { get; set; }
        public double Energy = 0.0;
        public double Value_t1, Value_t2;
        public DateTime t1 = new DateTime(1900, 1, 1), t2 = new DateTime(1900, 1, 1);

    }

    public class IntervalTime
    {
        public IntervalTime(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }


    public enum PointDirectionType
    {
        Input,
        Output
    }

    public enum DigitalDoubleStatus
    {
        Intransit = 0,
        Open = 1,
        Close = 2,
        Disturb = 3
    }

    public enum DigitalSingleStatus
    {
        Close = 0,
        Open = 1
    }
    public enum DigitalSingleStatusOnOff
    {
        Off = 0,
        On = 1
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
