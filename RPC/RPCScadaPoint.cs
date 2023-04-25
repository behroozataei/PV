using Irisa.Common;

using System;
using System.Collections.Generic;
using System.Text;

namespace RPC
{
    public sealed class RPCScadaPoint
    {
        public RPCScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
            PointDirectionType = pointDirectionType;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Quality { get; set; }
        public PointDirectionType PointDirectionType { get; }
        public float Value { get; set; }
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
