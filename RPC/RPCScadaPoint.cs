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
}
