using System;

namespace EEC
{
    public sealed class EECScadaPoint
    {
        public EECScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType, String scadatype)
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
    public sealed class EECEAFPoint
    {
        public EECEAFPoint(int furnaceNumber, string groupNumber, string statusOfFurnace, string ConsumedEnergyPerHeat)
        {
            FurnaceNumber = furnaceNumber;
            GROUPNUM = groupNumber;
            STATUS_OF_FURNACE = statusOfFurnace;
            CONSUMED_ENERGY_PER_HEAT = Convert.ToSingle(ConsumedEnergyPerHeat);
        }
        public int FurnaceNumber { get; }
        public string GROUPNUM { get; }
        public string STATUS_OF_FURNACE { get; }
        public float CONSUMED_ENERGY_PER_HEAT { get; }

        //public Guid CB_GUID { get; }
        //public Guid CT_GUID { get; }
        //public string CB_NETWORKPATH { get; }
        //public string CT_NETWORKPATH { get; }
        //public string HASPARTNER { get; }
        //public string PARTNERADDRESS { get; }
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
