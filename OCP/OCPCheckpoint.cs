//*************************************************************************************
// @author   Hesam Akbari
// @version  1.0
//
// Development Environment       : MS-Visual Basic 6.0
// Name of the Application       : OCP_Service_App.vbp
// Creation/Modification History :
//
// Hesam Akbari       04-Feb-2008       Created
//
// Overview of Application       :
//
//
//***************************************************************************************
using System;

namespace OCP
{
    public sealed class OCPCheckPoint
    {
        public Guid MeasurementId { get; set; }
        public string Name { get; set; }
        public string NetworkPath { get; set; }
        public int DecisionTable { get; set; }
        public string ShedType { get; set; }
        public string Category { get; set; }
        public char CheckOverload { get; set; }
        public int SubstitutionCounter { get; set; }
        public float Value { get; set; }
        public OCPCheckPointQuality Quality { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public float Value3 { get; set; }
        public float Value4 { get; set; }
        public float Value5 { get; set; }
        public float LIMITPERCENT { get; set; }
        public float NominalValue { get; set; }
        public OCPScadaPoint Overload { get; set; }
        public OCPScadaPoint ActivePower { get; set; }
        public OCPScadaPoint Average { get; set; }
        public OCPCheckPointQuality Quality1 { get; set; }
        public OCPCheckPointQuality Quality2 { get; set; }
        public OCPCheckPointQuality Quality3 { get; set; }
        public OCPCheckPointQuality Quality4 { get; set; }
        public OCPCheckPointQuality Quality5 { get; set; }
        public OCPCheckPointQuality ActivePowerQuality { get; set; }
        public OCPCheckPointQuality AverageQuality { get; set; }
        public bool OverloadFlag { get; set; }
        public bool ResetIT { get; set; }
        public bool FourValueFlag { get; set; }
        public Guid IT_GUID { get; set; }
        public Guid ALLOWEDACTIVEPOWER_GUID { get; set; }
        public Guid SAMPLE_GUID { get; set; }
        public Guid AVERAGE_GUID { get; set; }

        public OCPCheckPoint primeSideBigTans;

        public OCPCheckPoint SecondSideBigTans;
        public bool OverloadAlarmFourCycle { get; set; }
        public bool OverloadWarningFourCycle { get; set; }
        public bool OverloadAlarmFiveCycle { get; set; }
        public bool OverloadWarningFiveCycle { get; set; }

        public int QualityCodes { get; set; }
        public int QualityCodes_Old { get; set; }
        public Guid QualityErrorId { get; set; }

        public OCPCheckPoint()
        {
            Overload = new OCPScadaPoint(MeasurementId, Name, NetworkPath);
            ActivePower = new OCPScadaPoint(MeasurementId, Name, NetworkPath);
            Average = new OCPScadaPoint(MeasurementId, Name, NetworkPath);
        }

        //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT
        public System.DateTime TCycle1 { get; set; }
        public System.DateTime TCycle2 { get; set; }
        public System.DateTime TCycle3 { get; set; }
        public System.DateTime TCycle4 { get; set; }
        public System.DateTime TCycle5 { get; set; }
        //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT
    }
}
