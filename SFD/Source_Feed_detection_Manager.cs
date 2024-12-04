using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;

namespace SRC_FEED_DETECTION
{
    public sealed class Source_Feed_detection_Manager : IProcessing
    {
        private const float VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK = 5.0f;
        


        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        private DigitalStatus _oldSFD;
        private bool _initialize_Voltage_Source;
        private bool _SFDInitialized;
        private bool CPSStatus;
        

        internal Source_Feed_detection_Manager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);
        }

        public void Build()
        {
            
        }

        public void InitializeSFD()
        {
            try
            {

                _SFDInitialized = true;
             //    Update_VoltageSources();
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
        }

        public void CheckCPSStatus()
        {
            _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);
            while (!CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
              

            }
        }

        // TODO : StartupForm.frm -> reset_SFD_Function_Status
        
       

        

       

        

        

        public void Update_VoltageSources()
        {
            if (_SFDInitialized == false) return;

            try
            {
                // For FL01
                var CB_FL01 = _repository.GetScadaPoint("FL01_CB");
                var FL01VT = _repository.GetScadaPoint("FL01_VT").Value;
                if ((FL01VT > VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_FL01.Value != (float)DigitalStatus.Close) || CB_FL01.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_FL01 is connected, {CB_FL01.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_FL01, (int)DigitalStatus.Close, "Line_FL01 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_FL01.NetworkPath}", LogLevels.Error);
                }
                else if ((FL01VT <= VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_FL01.Value != (float)DigitalStatus.Open) || CB_FL01.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_FL01 is disconnected, {CB_FL01.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_FL01, (int)DigitalStatus.Open, "Line_FL01 is disconnected "))
                        _logger.WriteEntry($"It is not possible to Open, {CB_FL01.NetworkPath}", LogLevels.Error);
                }
                
                // For FL02
                var CB_FL02 = _repository.GetScadaPoint("FL02_CB");
                var FL02VT = _repository.GetScadaPoint("FL02_VT").Value;
                if ((FL02VT > VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_FL02.Value != (float)DigitalStatus.Close) || CB_FL02.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_FL02 is connected, {CB_FL02.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_FL02, (int)DigitalStatus.Close, "Line_FL02 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_FL02.NetworkPath}", LogLevels.Error);
                }
                else if ((FL02VT <= VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_FL02.Value != (float)DigitalStatus.Open) || CB_FL02.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_FL02 is disconnected, {CB_FL02.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_FL02, (int)DigitalStatus.Open, "Line_FL02 is disconnected "))
                        _logger.WriteEntry($"It is not possible to Open, {CB_FL02.NetworkPath}", LogLevels.Error);
                }

                

                if (_initialize_Voltage_Source == false)
                    _logger.WriteEntry(" ----- Initialize_VoltageSources -> Process VoltageSources is finished ---- ", LogLevels.Info);
                _initialize_Voltage_Source = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                _logger.WriteEntry("Updating Voltage Sources was failed.", LogLevels.Error);
            }
        }

        

        public bool GetCPSStatus()
        {
            return CPSStatus;
        }

        public void SetCPSStatus(bool state)
        {
            CPSStatus = state;
        }

       
    }
}