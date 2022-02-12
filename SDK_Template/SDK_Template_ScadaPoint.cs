using System;

namespace SDK_Template
{
    public sealed class SDK_Template_ScadaPoint
    {
        public SDK_Template_ScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType, string scadaType)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
            PointDirectionType = pointDirectionType;
            SCADAType = scadaType;
            Value = 0;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Qulity { get; set; }
        public PointDirectionType PointDirectionType { get; }
        public float Value { get; set; }

        public string SCADAType { get; set; }
    }

    public enum PointDirectionType
    {
        Input,
        Output
    }

    public enum DigitalStatus
    {
        Intransit = 0,
        Open = 1,
        Close = 2,
        Disturb = 3
    }
}