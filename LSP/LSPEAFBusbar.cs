using System;
using Irisa.Logger;

namespace LSP
{
    internal class CBusbarPowerCalc
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;


        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================

        //
        public CBusbarPowerCalc(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        //
        ~CBusbarPowerCalc()
        {

        }

        //
        public bool CheckTransPosition()
        {
            bool result = false;
            try
            {
                result = false;


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
    }
}