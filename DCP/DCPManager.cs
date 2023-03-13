using COMMON;
using Irisa.Logger;
using Irisa.Message;
using System;
using System.Timers;

namespace DCP
{
    internal enum eApp_Disapp
    {
        Disappear = 0,
        Appear = 1
    }

    public sealed class DCPManager : IProcessing
    {
        private const int EAFs = 8;
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        // In second
        private int TIMER_CYCLE_LoadPowerForEAFS = 4000;
        private Timer _timer_LoadPowerForEAFS;
        private ICpsCommandService _cpsCommandService;

        // TODO : StartupForm.frm -> reset_DCP_Function_Status

        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================
        // An object for handling EAFs Consumptions
        private EAFConsumptionManager _eafConsumptonManager;

        // An object for handling PCS Telegrams
        private PCSManager _pcsManager = null;

        public DCPManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);
            _cpsCommandService = cpsCommandService;


            //--------------------------------------------------------
            // Step 3: For LoadPowerForEAFS:
            //DCManager_start();

            //--------------------------------------------------------
            _logger.WriteEntry($"=================================================================", LogLevels.Info);
            _logger.WriteEntry($" DCP is started . . . ", LogLevels.Info);
        }

        public void SCADAEventRaised(DCPScadaPoint scadaPoint)
        {
            if (_pcsManager != null)
                _pcsManager.SCADAEventReceived(scadaPoint);
        }

        public void DCManager_start()
        {
            //--------------------------------------------------------
            // Step 1: For DC-EAFS Consumption:
            _eafConsumptonManager = new EAFConsumptionManager(_logger, _repository, _cpsCommandService);

            //--------------------------------------------------------
            // Step 2: For DC-PCS:
            _pcsManager = new PCSManager(_logger, _repository, _updateScadaPointOnServer);


            try
            {
                _timer_LoadPowerForEAFS = new Timer();
                _timer_LoadPowerForEAFS.Interval = TIMER_CYCLE_LoadPowerForEAFS;
                _timer_LoadPowerForEAFS.Elapsed += RunCyclicOperation4Second;

                _timer_LoadPowerForEAFS.Start();

                // Create a theCTraceLogger for any Logging, and Send dbConnection to TraceLogger
                _logger.WriteEntry($"Start of running LoadPowerForEAFS ... ", LogLevels.Info);
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }
        public void CheckCPSStatus()
        {

            while (!GlobalData.CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
                _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);

            }
        }

        public void RunCyclicOperation4Second(object sender, ElapsedEventArgs e)
        {
            try
            {
                CheckCPSStatus();
                Retrive_Power();
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        public bool Retrive_Power()
        {
            try
            {
                var sql = "SELECT TelDateTime, Sumation, PowerGrp1, PowerGrp2, Furnace1, Furnace2, " +
                    "Furnace3, Furnace4, Furnace5, Furnace6, Furnace7, Furnace8 " +
                    $"from APP_SFSC_EAFsPower Where ID = (Select Max(ID) From APP_SFSC_EAFsPower)";
                //DataTable dataTable = _repository.GetFromHistoricalDB(sql);
                SFSC_EAFSPOWER_Str sfsc_eafsp = _repository.GetFromHistoricalCache();
                //var rows = dataTable1.Rows.OfType<SFSC_EAFSPOWER_Str>().ToArray();
                if ((sfsc_eafsp is null))
                {
                    _logger.WriteEntry($"Error: Select from Historical APP_SFSC_EAFsPower is null or no Row ", LogLevels.Error);
                    return false;
                }
                else
                {
                    //DataRow dr = dataTable1.Rows[0];
                    sql = "INSERT INTO [PU10_PCS].[dbo].[T_EAFsPower](TelDateTime, Sumation, " +
                        "PowerGrp1, PowerGrp2, Furnace1, Furnace2, Furnace3, Furnace4, Furnace5, " +
                        "Furnace6, Furnace7, Furnace8) Values('" +
                        sfsc_eafsp.TELDATETIME.ToString() + "', " +
                        sfsc_eafsp.SUMATION.ToString() + ", " +
                        sfsc_eafsp.POWERGRP1.ToString() + ", " +
                        sfsc_eafsp.POWERGRP2.ToString() + ", " +
                        sfsc_eafsp.FURNACE1.ToString() + ", " +
                        sfsc_eafsp.FURNACE2.ToString() + ", " +
                        sfsc_eafsp.FURNACE3.ToString() + ", " +
                        sfsc_eafsp.FURNACE4.ToString() + ", " +
                        sfsc_eafsp.FURNACE5.ToString() + ", " +
                        sfsc_eafsp.FURNACE6.ToString() + ", " +
                        sfsc_eafsp.FURNACE7.ToString() + ", " +
                        sfsc_eafsp.FURNACE8.ToString() + " " +
                        " );";
                    if (!_repository.ModifyOnLinkDB(sql))
                    {
                        System.Threading.Thread.Sleep(100);
                        if (!_repository.ModifyOnLinkDB(sql))
                            _logger.WriteEntry("1_LinkServer: INSERT INTO [PU10_PCS].[dbo].[T_EAFsPower]; "+sql, LogLevels.Error);
                    }
                }

                //1401_08_23 Preparing Data For HMI
                try
                {
                    string sqlhis =
                        "INSERT INTO APP_DCP_EAFSPOWER (DATETIME, SUMMATION, " +
                        "POWER_GRP1, POWER_GRP2, FURNACE1, FURNACE2, FURNACE3, FURNACE4, FURNACE5, " +
                        "FURNACE6, FURNACE7, FURNACE8) Values('" +
                        sfsc_eafsp.TELDATETIME.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                        Math.Round(sfsc_eafsp.SUMATION, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.POWERGRP1,2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.POWERGRP2,2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE1, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE2, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE3, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE4, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE5, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE6, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE7, 2).ToString() + "', '" +
                        Math.Round(sfsc_eafsp.FURNACE8, 2).ToString() + "' " +
                        " )";
                    if (!_repository.ModifyOnHistoricalDB(sqlhis))
                    {
                        _logger.WriteEntry($"Error: Insert into APP_DCP_EAFsPower", LogLevels.Error);

                    }
                }
                catch (Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);

                }
                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return false;
        }
    }
}
