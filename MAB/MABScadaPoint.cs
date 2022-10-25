using System;

namespace MAB
{
    public sealed class MABScadaPoint
    {
        public MABScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType, string scadaType)
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

        internal bool IsTransient()
        {
           if (((SCADAType == "DigitalMeasurement")||(SCADAType == "DigitalMeasurement_RPC")) && (((DigitalStatus)Value) == DigitalStatus.Intransit))
                return true;    
           else
                return false;
        }

        internal bool IsOpen()
        {
            if (((SCADAType == "DigitalMeasurement") || (SCADAType == "DigitalMeasurement_RPC")) && (((DigitalStatus)Value) == DigitalStatus.Open))
                return true;
            else
                return false;
        }

        internal bool IsClose()
        {
            if (((SCADAType == "DigitalMeasurement") || (SCADAType == "DigitalMeasurement_RPC")) && (((DigitalStatus)Value) == DigitalStatus.Close))
                return true;
            else
                return false;
        }
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

    public enum DigitalSingleStatusOnOff
    {
        Off = 0,
        On = 1
    }

}