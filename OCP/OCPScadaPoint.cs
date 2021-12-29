using System;

namespace OCP
{
    enum eParamDirectionType
    {
        INPUT, OUTPUT
    }

    enum eSCADAType
    {
        DigitalMeasurement,
        AnalogMeasurement,
        AccomulatorMeasurement
    }

    public enum SinglePointStatus
    {
        Disappear = 0,
        Appear = 1
    }

    public sealed class OCPScadaPoint
    {
        public OCPScadaPoint(Guid id, string name, string networkPath)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public string DirectionType { get; set; }   // values from eParamDirectionType
        public string SCADAType { get; set; }       // values from eSCADAType
        public float Value { get; set; }
        public OCPCheckPointQuality Quality { get; set; }

    }
}
