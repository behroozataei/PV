using Irisa.Logger;
using System;

namespace LSP
{
    internal class CEAFBusbar
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        private float m_BusbarShedValue;

        private float m_BusbarPower = 0;


        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================
        public CEAFBusbar(ILogger logger)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                m_BusbarPower = 0;
                m_BusbarShedValue = 0;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        public bool CheckTransPosition()
        {
            bool result = false;
            try
            {
                // TODO:
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        public bool ClacTransActivePower()
        {
            bool result = false;
            try
            {
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        public bool SentLSPJobToDC()
        {
            bool result = false;
            try
            {
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        public bool AddTransPowerToBB()
        {
            bool result = false;
            try
            {
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        //
        public bool ResetBusbarData()
        {
            bool result = false;
            try
            {
                result = false;

                m_BusbarPower = 0;
                m_BusbarShedValue = 0;

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

        //
        //
        public float BusbarPower
        {
            get
            {
                try
                {
                    return m_BusbarPower;
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
                    m_BusbarPower = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

    }
}