using Irisa.Logger;
using System;

namespace LSP
{
    internal class CLSPJob
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        //
        private int m_CheckPointNo = 0;

        //
        private float m_SumIt = 0;

        // Shed value is calculated from SumIt, It is on primary side, SV is for secondary side
        private float m_ShedValue = 0;

        //
        private eShedType m_ShedType = eShedType.None;

        //
        private float m_AllowedActivePower = 0;

        //
        private float m_PrimaryVoltage = 0;

        //
        private float m_SecondaryVoltage = 0;

        // The priority list number related to this job, is stored here after finding in CDecisionTable
        private byte m_PriolNo = 0;

        // The decision atble number related to this Job
        private byte m_DectNo = 0;


        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================

        public CLSPJob(ILogger logger)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                m_AllowedActivePower = 0;
                m_CheckPointNo = 0;
                m_PrimaryVoltage = 0;
                m_SecondaryVoltage = 0;
                m_SumIt = 0;
                m_PriolNo = 0;
                m_DectNo = 0;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

        }

        public void ResetJobValues()
        {
            try
            {
                m_PrimaryVoltage = 0;
                m_SecondaryVoltage = 0;
                m_PriolNo = 0;
                m_DectNo = 0;
                m_ShedType = eShedType.None;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        //==============================================================================
        //PROPERTIES
        //==============================================================================

        //
        //
        public int CheckPointNo
        {
            get
            {
                try
                {
                    return m_CheckPointNo;
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
                    m_CheckPointNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

        //
        //
        public float SumIt
        {
            get
            {
                try
                {
                    return m_SumIt;
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
                    m_SumIt = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

        //
        //
        public float ShedValue
        {
            get
            {
                try
                {
                    return m_ShedValue;
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
                    m_ShedValue = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

        //
        //
        public eShedType ShedType
        {
            get
            {
                try
                {
                    return m_ShedType;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
                return eShedType.None;
            }
            set
            {
                try
                {
                    m_ShedType = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }

        //
        //
        public float AllowedActivePower
        {
            get
            {
                try
                {
                    return m_AllowedActivePower;
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

                    m_AllowedActivePower = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }


        //
        //
        public float PrimaryVoltage
        {
            get
            {
                try
                {
                    return m_PrimaryVoltage;
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
                    m_PrimaryVoltage = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }


        //
        //
        public float SecondaryVoltage
        {
            get
            {
                try
                {
                    return m_SecondaryVoltage;
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
                    m_SecondaryVoltage = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }

            }
        }


        //
        //
        public byte PriolNo
        {
            get
            {
                try
                {
                    return m_PriolNo;
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

                    m_PriolNo = value;
                }
                catch (System.Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                }
            }
        }


        //
        //
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

    }
}