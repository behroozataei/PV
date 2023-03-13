using System;

namespace DCIS
{
    public sealed class ScadaPoint
    {
        public ScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType, String scadatype)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
            PointDirectionType = pointDirectionType;
            ScadaType = scadatype;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Quality { get; set; }
        public PointDirectionType PointDirectionType { get; }
        public String  ScadaType { get; }
        public float Value { get; set; }
    }
    

    public enum PointDirectionType
    {
        input,
        output
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
}
