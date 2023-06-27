using Irisa.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LSP
{
    internal class CDecisionTable
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        // The maximum number of items in one decision table
        const int MAXITEMSINDECT = 32;

        // The maximum number of combinations in one decision table
        const int MAXCOMBINDECT = 32;

        // The decision table number
        private byte m_DectNo = 0;

        // The number of items in this decision table
        private byte m_nItems = 0;

        // The number of combinations for this decision table
        private byte m_nCombinations = 0;

        // List of items for this decision table
        private CDectItem[] m_arrItems = new CDectItem[MAXITEMSINDECT + 1];

        // List of combinations for this decision table
        private CDectComb[] m_arrCombinations = new CDectComb[MAXCOMBINDECT + 1];

        private readonly IRepository _repository;

        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================
        public CDecisionTable(IRepository repository, ILogger logger)
        {
            try
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                m_nItems = 0;
                m_nCombinations = 0;

                for (int I = 0; I <= MAXITEMSINDECT; I++)
                {
                    m_arrItems[I] = null;
                }

                for (int I = 0; I <= MAXCOMBINDECT; I++)
                {
                    m_arrCombinations[I] = null;
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        ~CDecisionTable()
        {
            try
            {
                //Call theCTraceLogger.WriteLog(TraceInfo4, "CDecisionTable..Class_Terminate()", " Terminating is started... ")

                for (int I = 0; I <= MAXITEMSINDECT; I++)
                {
                    m_arrItems[I] = null;
                }

                for (int I = 0; I <= MAXCOMBINDECT; I++)
                {
                    m_arrCombinations[I] = null;
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

        }

        //
        // This function loads all items for this decision table
        public bool LoadItems()
        {
            bool result;

            try
            {
               // List<SqlParameter> listNull = new List<SqlParameter>();

                var dtbMeasurements = _repository.FetchItems(m_DectNo);
                var rows = dtbMeasurements.Where(n => n.DECTNO.ToString() == m_DectNo.ToString())
                                                                 .OrderBy(n => n.DECTITEMNO).ToArray();
                //DECTNO = " + decisionTableNo.ToString()
                int I = 1;
                // -------------------------------------------------------------------------
                // Read all detail info about decision tables from three tables
                _logger.WriteEntry($"------------------------  Load Decision Table {m_DectNo} Items  ----------------------------- ", LogLevels.Info);
                foreach (var dr in rows)
                {
                    if (m_nItems > 0)
                    {
                        {
                            m_arrItems[I] = new CDectItem(_logger);

                            m_arrItems[I].NetworkPath = dr.NETWORKPATH.ToString();
                            m_arrItems[I].ItemNo = Convert.ToByte(dr.DECTITEMNO.ToString());
                            m_arrItems[I]._GUID = Guid.Parse(dr.ID.ToString());
                            m_arrItems[I]._GUID = _repository.GetGuid(m_arrItems[I].NetworkPath);
                            m_arrItems[I]._Name = dr.NAME.ToString();

                            _logger.WriteEntry("DectItemNo = " + m_arrItems[I].ItemNo.ToString(), LogLevels.Info);
                            _logger.WriteEntry("Item NetworkPath = " + m_arrItems[I].NetworkPath.ToString() + "; ItemName = " + dr.NAME.ToString(), LogLevels.Info);
                        }
                    }
                    I++;
                }
                result = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry("CLSPManager..LoadItems()" + excep.Message, LogLevels.Error, excep);
                result = false;
            }

            return result;
        }

        //
        // This function loads all combinations for this decision table
        public bool LoadCombinations()
        {
            bool result;
            byte I;

            try
            {
                var dtbMeasurements = _repository.FetchCombinations(m_DectNo);
                var rows = dtbMeasurements.Rows.OfType<DataRow>().Where(n => n["DECTNO"].ToString() == m_DectNo.ToString())
                                                                 .OrderBy(n => n["COMBINATIONNO"])
                                                                 .ThenBy(n => n["DECTITEMNO"]).ToArray();

                if (m_nCombinations > 0)
                {
                    for (I = 1; I <= m_nCombinations; I++)
                    {
                        m_arrCombinations[I] = new CDectComb(_logger);

                        m_arrCombinations[I].nItems = m_nItems;
                        m_arrCombinations[I].CombNo = I;
                    }
                    foreach (var dr in rows)
                    {
                        I = Convert.ToByte(dr["COMBINATIONNO"].ToString());
                        byte K = Convert.ToByte(dr["DECTITEMNO"].ToString());
                        m_arrCombinations[I].SetArrItemValues(K, Convert.ToByte(dr["Value"].ToString()));

                        _logger.WriteEntry("Combination = " + I.ToString() + " ; Item = " + K.ToString() + " , Value = " + CombinationSwitchStatusbyName(m_arrCombinations[I].GetArrItemValues(K)), LogLevels.Info);
                    }

                    // -------------------------------------------------------------------------
                    // Read all detail info about decision tables from three tables
                    //_logger.WriteEntry(" ----------------------------------------------------- ", LogLevels.Info);
                }
                result = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                result = false;
            }

            return result;
        }

        //
        // This function loads all priority lists number for all combinations
        public bool LoadPriorityListsNoForCombinations()
        {
            bool result;
            int I;

            try
            {
                DataTable dtbMeasurements = _repository.FetchPriorityListsNoForCombinations(m_DectNo);
                var rows = dtbMeasurements.Rows.OfType<DataRow>().Where(n => n["DECTNO"].ToString() == m_DectNo.ToString())
                                                                .OrderBy(n => n["COMBINATIONNO"]).ToArray();


                if (m_nCombinations > 0)
                {
                    foreach (var dr in rows)
                    {
                        I = Convert.ToByte(dr["COMBINATIONNO"].ToString());
                        m_arrCombinations[I].PriorityListNo = Convert.ToByte(dr["PRIORITYLISTNO"].ToString());

                        _logger.WriteEntry("Combination = " + I.ToString() + " ; Priority List Number = " + m_arrCombinations[I].PriorityListNo, LogLevels.Info);
                    }

                    _logger.WriteEntry(" ----------------------------------------------------- ", LogLevels.Info);
                }
                result = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                result = false;
            }

            return result;
        }

        // The Status of all CBes/DSes regarding to thier quality codes is read for the decision table
        public bool ReadDectItemValues()
        {
            try
            {
                string strValue = "";

                byte tempForEndVar = m_nItems;
                for (byte I = 1; I <= tempForEndVar; I = (byte)(I + 1))
                {
                    var guid = m_arrItems[I]._GUID;

                    var scadapoint = _repository.GetLSPScadaPoint(guid);
                    if (!(scadapoint == null))
                        strValue = scadapoint.Value.ToString();
                    else
                    {
                        _logger.WriteEntry("Error in finding " + m_arrItems[I].NetworkPath, LogLevels.Error);
                        strValue = " ";
                    }

                    m_arrItems[I].Status = Convert.ToByte(Double.Parse(strValue));
                    _logger.WriteEntry("" + m_arrItems[I].NetworkPath + " = " + SwitchStatusbyName((int)m_arrItems[I].Status), LogLevels.Info);


                    // Only for MDF
                    if (m_arrItems[I].NetworkPath.IndexOf("MDF") >= 0)
                    {
                        //If (m_DectNo = 27 And I = 4) Or _
                        //'    (m_DectNo = 28 And I = 2) Or _
                        //'    (m_DectNo = 29 And I = 2) Then
                        m_arrItems[I].Status = (byte)Breaker_Status.bClose;
                    }
                }

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return false;
        }

        static public string SwitchStatusbyName(int aValue)
        {
            if (aValue == (int)DigitalDoubleStatus.Close)
                return "Close";
            if (aValue == (int)DigitalDoubleStatus.Open)
                return "Open";
            if (aValue == (int)DigitalDoubleStatus.Intransit)
                return "Intransit";
            if (aValue == (int)DigitalDoubleStatus.Disturb)
                return "Disturb";
            return " ";
        }

        static public string CombinationSwitchStatusbyName(int aValue)
        {
            if (aValue == (int)DigitalDoubleStatus.Close)
                return "Close";
            if (aValue == (int)DigitalDoubleStatus.Open)
                return "Open";
            if (aValue == (int)DigitalDoubleStatus.Intransit)
                return "Don't Care";
            return " ";
        }

        // With comparision between combinations and current status of breakers we find a matched combination
        public bool FindCombination(ref byte nFoundCombNo)
        {
            bool result = false;
            try
            {
                bool bFindComb = false;
                byte CombItemVal = 0;

                result = false;
                nFoundCombNo = 0;

                _logger.WriteEntry("Number of combinations = " + m_nCombinations.ToString(), LogLevels.Info);

                // Check all combinations
                byte tempForEndVar = m_nCombinations;
                for (byte idxComb = 1; idxComb <= tempForEndVar; idxComb = (byte)(idxComb + 1))
                {
                    //
                    bFindComb = true;
                    _logger.WriteEntry(" ", LogLevels.Info);
                    _logger.WriteEntry("idxComb = " + idxComb.ToString() + " ; nItems= " + m_arrCombinations[idxComb].nItems.ToString(), LogLevels.Info);
                    byte tempForEndVar2 = m_arrCombinations[idxComb].nItems;
                    for (int idxCB = 1; idxCB <= tempForEndVar2; idxCB++)
                    {
                        CombItemVal = m_arrCombinations[idxComb].GetArrItemValues(idxCB);
                        var item = _repository.GetLSPScadaPoint(m_arrItems[idxCB]._GUID);

                        _logger.WriteEntry("Item = "
                            + idxCB.ToString() + " ; Value of Item in Comb= "
                            + CombinationSwitchStatusbyName(CombItemVal) + " ; CB_Status= "
                            + SwitchStatusbyName((int)item.Value), LogLevels.Info);

                        if (CombItemVal != ((byte)eCombItem_Status.SDo_Not_Care))
                        {
                            // Check equality of values
                            if ((CombItemVal == ((byte)eCombItem_Status.SOff) && (DigitalDoubleStatus)item.Value == DigitalDoubleStatus.Close) ||
                                (CombItemVal == ((byte)eCombItem_Status.SOn) && (DigitalDoubleStatus)item.Value == DigitalDoubleStatus.Open))
                            {
                                bFindComb = false;
                            }

                            // Check quality of read value
                            if ((DigitalDoubleStatus)item.Value == DigitalDoubleStatus.Disturb)
                            {
                                bFindComb = false;
                            }
                        }
                    }

                    // Combination is found
                    if (bFindComb)
                    {
                        nFoundCombNo = idxComb;
                        _logger.WriteEntry("idxComb = " + idxComb.ToString() + " Matched ", LogLevels.Info);
                        break;
                    }
                    else
                        _logger.WriteEntry("idxComb = " + idxComb.ToString() + " Not Matched ", LogLevels.Info);


                }

                _logger.WriteEntry("DectNo= " + m_DectNo.ToString() + " ; Found combNo=" + nFoundCombNo.ToString(), LogLevels.Info);

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        // For one CombinationNo finds PriolNo
        public bool FindPriorityListNo(byte aCombNo, ref byte aPriolNo)
        {
            bool result = false;
            try
            {
                result = false;
                aPriolNo = 0;

                if (aCombNo < 1 || aCombNo > m_nCombinations)
                {
                    _logger.WriteEntry("CombNo is not valid", LogLevels.Info);
                    return result;
                }

                aPriolNo = m_arrCombinations[aCombNo].PriorityListNo;

                _logger.WriteEntry("CombNo= " + aCombNo.ToString() + " ; Found PriolNo =" + aPriolNo.ToString(), LogLevels.Info);

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        // This function converts the value of Shed value on Primary side to Shed value on secondary side
        public bool ConvertItToSV(CLSPJob aJob)
        {
            bool result = false;
            try
            {
                result = false;

                if ((aJob.PrimaryVoltage == 0) || (aJob.SecondaryVoltage == 0))
                {
                    _logger.WriteEntry("Primary Side or Secondary Side Voltage is not correct, for Checkpoint = " + aJob.CheckPointNo.ToString(), LogLevels.Error);
                    return result;
                }
                else
                {
                    aJob.ShedValue = aJob.SumIt * (aJob.PrimaryVoltage / aJob.SecondaryVoltage);
                    _logger.WriteEntry("SumIt = " + aJob.SumIt.ToString() + " A", LogLevels.Info);
                    _logger.WriteEntry("PrimaryVoltage = " + (aJob.PrimaryVoltage/10.0f).ToString() + " kV", LogLevels.Info);
                    _logger.WriteEntry("SecondaryVoltage = " + (aJob.SecondaryVoltage/10.0f).ToString() + " kV", LogLevels.Info);
                }

                _logger.WriteEntry("Shed value of Job = " + aJob.ShedValue.ToString() + " A", LogLevels.Info);

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }


        //==============================================================================
        //PROPERTIES
        //==============================================================================
        public byte nItems
        {
            get
            {
                try
                {


                    return m_nItems;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
                return 0;
            }
            set
            {
                try
                {

                    m_nItems = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

        public byte nCombinations
        {
            get
            {
                try
                {


                    return m_nCombinations;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
                return 0;
            }
            set
            {
                try
                {

                    m_nCombinations = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

        public byte DectNo
        {
            get
            {
                try
                {


                    return m_DectNo;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
                return 0;
            }
            set
            {
                try
                {

                    m_DectNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

        // This function loads all decision tables
        public bool LoadDecisionTable()
        {
            bool result;

            try
            {
                // Loading all CB/DS es for this Decition Table
                if (!LoadItems())
                {
                    _logger.WriteEntry("Could not load all items for DectNo : " + DectNo.ToString(), LogLevels.Info);
                }

                // Loading all combinations for this Decision Table
                if (!LoadCombinations())
                {
                    _logger.WriteEntry("Could not load all combinations for DectNo : " + DectNo.ToString(), LogLevels.Info);
                }

                // Loading all PriorityListNumbers for this Decision Table
                // TODO : check
                if (!LoadPriorityListsNoForCombinations())
                {
                    _logger.WriteEntry("Could not load all priority list numbers for DectNo : " + DectNo.ToString(), LogLevels.Info);
                }

                result = true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry("CLSPManager..LoadDecisionTables()" + excep.Message, LogLevels.Info);
                result = false;
            }

            return result;
        }
    }
}