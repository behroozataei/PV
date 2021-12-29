namespace LSP
{
    static class Constants
    {
        // The priority list number assigned for EAFes
        //'  2016.02.16  A.K Modification 35 -> 144
        public const int PRIORITYLISTNO_EAF = 144;
        //const int IdxPriolsEAF = 44;

        //IMANIAN  1395.12  ADDING L914, L915
        public const int PRIORITYLISTNO_NISLINES = 160;
        public const int IdxPriolsLINES = 60;

        // The priority list number assigned for PPes
        //'  2016.02.16  A.K Modification 38 -> 145
        public const int PRIORITYLISTNO_PP = 145;

        public const int BigTransOnPP_VoltageNum = 630;
        public const int BigTransOnPP_VoltageDenom = 66;

        // Maximum number of decision tables
        //'KAJI T8AN Definition
        public const int MAXDECISIONTABLES = 100;

        // Maximum number of prioriyt lists for all decision tables
        //'KAJI T8AN Definition
        public const int MAXPRIORITYLISTS = 100;

        // Maximum number of jobs
        //'KAJI T8AN Definition
        //IMANIAN  1395.12  ADDING L914, L915 'CHANGE 100 --> 102
        public const int MAXJOBS = 102;

        // The maximum number of check points in LSP, T_CLSPSHEDPINT
        public const int MAXCHECKPOINTS = MAXJOBS;

        public const string SHEDPOINTADDRESS = "Network/Model Functions/LSP/SHEDPOINT/";

        // A list of all Decision Table Numbers related to big Transformers
        public const int T1AN_DectNo = 25;
        public const int T2AN_DectNo = 26;
        public const int T3AN_DectNo = 27;
        public const int T4AN_DectNo = 28;
        public const int T5AN_DectNo = 29;
        public const int T6AN_DectNo = 33;
        public const int T7AN_DectNo = 34;

        //'KAJI T8AN Definition
        public const int T8AN_DectNo = 43;

        //IMANIAN  1395.12  ADDING L914, L915
        public const int L914_DectNo = 48;
        public const int L915_DectNo = 49;

        public const int ReducePowerPrefer = 470;
        public const string EAFSPowerLimit = "Network/Model Functions/EEC/CALCULATED/PMAX";

        // The maximum number of points of DCParams which their value changes are important for DC.
        public const int MaxPointsInChange = 50;

        //' KAJI T8AN Definiton, Changing 7 to 8
        public const int MaxNoOfTransformers = 8;
    }
}
