using Irisa.Common.Utils;
using Irisa.Logger;
using System;

namespace LSP
{
    internal class LSPBreakerToShed
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        private readonly ILogger _logger;

        // Breaker number in priority list
        public byte BreakerNo { get; set; }

        // Network path of status for this breaker
        public string NetworkPath_Item { get; set; }

        // Status value of this breaker
        //private Breaker_Status m_Status = Breaker_Status.BIntransient;

        // Quality for the status
        public bool StatusQuality { get; set; }

        // Network path of current for this breaker
        public string NetworkPath_Cur { get; set; }

        // Current value on this breaker
        //private float m_Current = 0;

        // Current qualiy value for this breaker
        //private byte m_CurrentQuality = 0;

        // Last time of shedding this breaker
        public System.DateTime LastShedTime = DateTime.UtcNow;
        //Modification By Mr.Hematy For GIS Key
        public string HasPartner { get; set; }
        public string AddressPartner { get; set; }
        public string FurnaceIndex { get; set; }
        public Guid guid_item { get; set; }
        public Guid guid_curr { get; set; }
        public Guid addressPartner_guid { get; set; }
        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================

        public LSPBreakerToShed(ILogger logger)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                BreakerNo = 0;
                NetworkPath_Item = "";
                //m_Status = Breaker_Status.BDisturbed;
                StatusQuality = false;
                NetworkPath_Cur = "";
                //m_Current = 0;
                //m_CurrentQuality = 0;
                LastShedTime = DateTime.UtcNow.AddSeconds(-1000);
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
            }

        }

        //==============================================================================
        //PROPERTIES
        //==============================================================================
        //MODIFICATION:
        //'MODIFICATION:

        //MODIFICATION:
        //'MODIFICATION:
    }
}