//*************************************************************************************
// @author   Ali.A.Kaji
// @version  1.0
//
// Development Environment       : MS-Visual Basic 6.0
// Name of the Application       : LSP_Service_App.vbp
// Creation/Modification History :
//
// Ali.A.Kaji       20-Feb-2008       Created
//
// Overview of Application       :
//
//
//***************************************************************************************
using Irisa.Common;
using System;

namespace LSP
{
    public sealed class OCPCheckPoint
    {
        public int CheckPointNumber { get; set; }
        public Guid MeasurementId { get; set; }
        public string Name { get; set; }
        public string NetworkPath { get; set; }
        public int DecisionTable { get; set; }
        public string ShedType { get; set; }
        public string Category { get; set; }
        public char CheckOverload { get; set; }
        public int SubstitutionCounter { get; set; }
        public float Value { get; set; }
        public QualityCodes Quality { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public float Value3 { get; set; }
        public float Value4 { get; set; }
        public float Value5 { get; set; }
        public float LimitPercent { get; set; }
        public float NominalValue { get; set; }

        public float VoltageEnom { get; set; }
        public float VoltageDenom { get; set; }

        public LSPScadaPoint OverloadIT { get; set; }
        public LSPScadaPoint ActivePower { get; set; }
        public LSPScadaPoint Average { get; set; }
        public LSPScadaPoint Sample { get; set; }
        public LSPScadaPoint QulityError { get; set; }
        public CheckPointQuality Quality1 { get; set; }
        public CheckPointQuality Quality2 { get; set; }
        public CheckPointQuality Quality3 { get; set; }
        public CheckPointQuality Quality4 { get; set; }
        public CheckPointQuality Quality5 { get; set; }
        public CheckPointQuality ActivePowerQuality { get; set; }
        public CheckPointQuality AverageQuality { get; set; }
        public bool OverloadFlag { get; set; }
        public bool ResetIT { get; set; }
        public bool FourValueFlag { get; set; }
        //public Guid IT_GUID { get; set; }
        //public Guid ALLOWEDACTIVEPOWER_GUID { get; set; }
        //public Guid SAMPLE_GUID { get; set; }
        //public Guid AVERAGE_GUID { get; set; }

        public OCPCheckPoint primeSideBigTans;

        public OCPCheckPoint SecondSideBigTans;

        public OCPCheckPoint()
        {
            OverloadIT = new LSPScadaPoint(MeasurementId, Name, NetworkPath);
            ActivePower = new LSPScadaPoint(MeasurementId, Name, NetworkPath);
            Average = new LSPScadaPoint(MeasurementId, Name, NetworkPath);
        }

    }
}
