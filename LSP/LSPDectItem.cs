using Irisa.Logger;
using System;

namespace LSP
{
    internal class CDectItem
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        // Item number in a priority list
        private byte m_ItemNo = 0;

        // Network path for this item in SCADA
        private string m_NetworkPath = "";

        public string _Name;

        // Status of this Circuit Breaker/Disconnetor Switch;
        private string m_Status = "";

        // Quality for status of this Circuit Breaker/Disconnetor Switch;
        private string m_Quality = "";

        public Guid _GUID;

        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================

        //
        public CDectItem(ILogger logger)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                m_ItemNo = 0;
                m_NetworkPath = "";
                m_Status = "";
                m_Quality = "";
            }
            catch (System.Exception excep)
            {

                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }

        }

        public byte ItemNo
        {
            get
            {
                try
                {

                    return m_ItemNo;
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

                    m_ItemNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

        public string NetworkPath
        {
            get
            {
                try
                {


                    return m_NetworkPath;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
                return "";
            }
            set
            {
                try
                {

                    m_NetworkPath = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

        public byte Status
        {
            get
            {
                try
                {


                    return Convert.ToByte(Double.Parse(m_Status));
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

                    m_Status = value.ToString();
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }

    }
}