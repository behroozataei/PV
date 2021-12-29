using Irisa.Logger;
using System;

namespace LSP
{
    internal class CDectComb
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        const int MAXCOMBINPRIORITYLIST = 16;

        // The combination number
        private byte m_CombNo = 0;

        // The priority list number
        private byte m_PriorityListNo = 0;

        // The number of values/Breakers in this combination
        private byte m_nItems = 0;

        //
        private byte[] m_arrItemValues = new byte[MAXCOMBINPRIORITYLIST + 1];

        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================

        //
        public CDectComb(ILogger logger)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                m_CombNo = 0;
                m_PriorityListNo = 0;

                for (int I = 0; I <= MAXCOMBINPRIORITYLIST; I++)
                {
                    m_arrItemValues[I] = 0;
                }
            }
            catch (System.Exception excep)
            {

                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

        //
        ~CDectComb()
        {
            //Call theCTraceLogger.WriteLog(TraceInfo4, "CDectComb..Class_Terminate()", " Terminating is started... ")
        }

        //
        //
        public byte CombNo
        {
            get
            {
                try
                {

                    return m_CombNo;
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

                    m_CombNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }


        //
        //
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

        //
        public byte GetArrItemValues(int Index)
        {
            byte result = 0;
            try
            {

                if (Index < 1 || Index > m_nItems)
                {
                    // Error message
                    _logger.WriteEntry("Index is  out of range", LogLevels.Error);
                }
                else
                {
                    result = m_arrItemValues[Index];
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return result;
        }

        public void SetArrItemValues(int Index, byte value)
        {
            try
            {
                if (Index < 1 || Index > m_nItems)
                {
                    // Error message
                    _logger.WriteEntry("Index is  out of range", LogLevels.Error);
                }
                else
                {
                    m_arrItemValues[Index] = value;
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        //
        public byte PriorityListNo
        {
            get
            {
                try
                {

                    return m_PriorityListNo;
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

                    m_PriorityListNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }
    }
}