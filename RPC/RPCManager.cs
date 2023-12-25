using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;
using System.Timers;
using COMMON;
using Irisa.Message.CPS;
using System.Threading.Tasks;

namespace RPC
{
    internal class RPCManager:IProcessing
    {
        
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;
        private readonly Timer _timer;
        private readonly RedisUtils _RTDBManager;

        private const int RPC_TIMER_TICKS = 60000;
		public NetworkConfValidator _theCNetworkConfValidator;
        public CycleValidator _theCCycleValidator;
		private RPCCalculation _theCRPCCalculation;
		private LimitChecker _theCLimitChecker;
		private VoltageController _theCVoltageController;
        private QController _theCQController;
        private CosPHIController _theCCosPhiController;
        public EnergyCalc _energyCalc;
       





        public RPCManager(ILogger logger, IRepository repository, ICpsCommandService scadaCommand, RedisUtils RTDBManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(logger, scadaCommand);
            _RTDBManager = RTDBManager ?? throw new ArgumentNullException(nameof(RTDBManager));
            _repository = repository;
            _timer = new Timer();
            _timer.Interval = RPC_TIMER_TICKS;
            _timer.Elapsed += RunCyclicOperation;

            _theCNetworkConfValidator = new NetworkConfValidator(_repository, _logger, _updateScadaPointOnServer);
            _theCCycleValidator = new CycleValidator(_repository, _logger, _updateScadaPointOnServer, _RTDBManager);
            _theCRPCCalculation = new RPCCalculation(_repository, _logger, _updateScadaPointOnServer);
            _theCLimitChecker = new LimitChecker(_repository, _logger, _updateScadaPointOnServer);
            _theCVoltageController = new VoltageController(repository, _logger, _updateScadaPointOnServer);
            _theCQController = new QController(repository, _logger, _updateScadaPointOnServer);
            _theCCosPhiController = new CosPHIController(repository, _logger, _updateScadaPointOnServer);
            _energyCalc = new EnergyCalc(_logger, _repository, _updateScadaPointOnServer);



        }

		private double TempInteger_T1_TAP = 0;
		private double TempInteger_T2_TAP = 0;
		private double TempInteger_T3_TAP = 0;
		private double TempInteger_T5_TAP = 0;
		private double TempInteger_T6_TAP = 0;
		private double TempInteger_T7_TAP = 0;


		public void SCADAEventRaised(RPCScadaPoint scadaPoint)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            var currentTime = DateTime.UtcNow;
            var delay = (60 - currentTime.Second) * 1000 + (30000 - currentTime.Millisecond);
            var delayTask = Task.Delay(delay);
            delayTask.ContinueWith((t) =>
            {
                _timer.Start();
                _energyCalc.Start();
            });
        }
        public void CheckCPSStatus()
        {

            while (!GlobalData.CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
                _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);

            }
        }



        public void RunCyclicOperation(object sender, ElapsedEventArgs e)
		{
			try
			{

				int aCycleNo = 0;

				_logger.WriteEntry("-----------------------------------------------------------------------  ", LogLevels.Info);
				_logger.WriteEntry("Enter to method on: " + Environment.MachineName.ToString(),LogLevels.Info);
                //_logger.WriteEntry(" Machine State is: " + GeneralModule.GetProcessState(GeneralModule.eRPCState));

                //_Er_SVCA.Energy1Min();
                //_Er_SVCB.Energy1Min();
                //_Er_T4AN.Energy1Min();
                //_Ea_T4AN.Energy1Min();
                //_Er_T6AN.Energy1Min();
                //_Ea_T6AN.Energy1Min();

                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("SVCA_Q"));
                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("SVCB_Q"));
                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("T4AN_Q"));
                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("T4AN_P"));
                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("T6AN_Q"));
                //_energyCalc.Energy1Min(_repository.GetAccScadaPoint("T6AN_P"));



                //_theCRPCCalculation.SVC_ReactiveEnergy();

                // Stop the process if it is not Enable
                if (_repository.GetRPCScadaPoint("RPCFSTATUS").Value == 0.0)
                {
                    if (_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "RPC Function is Disabled"))
                    {
                        _logger.WriteEntry("Sending \"RPC Function is Disabled\" alarm failed.", LogLevels.Warn);
                    }
                    _logger.WriteEntry("Function is Disabled!", LogLevels.Info);
                    return;
                }

                // Network configuration should be checked here
                if (!_theCNetworkConfValidator.isAdmittedNetConf())
                {
                    _logger.WriteEntry("Network Configuration is Not Admitted!", LogLevels.Info);
                    return;
                }


                // Get right CycleNo to start of 1-minute processing:
                if (!_theCCycleValidator.GetRPCCycleNo())
				{
					_logger.WriteEntry("Error in Cyclic Operation!",LogLevels.Warn);
                    return;
				}
				else
				{
					aCycleNo = _theCCycleValidator.CycleNo;
				}

                if (aCycleNo == 1)
                {
                    //if (!m_theCRPCParameters.ReadGMTDiff())
                    {
                        ;
                        //_logger.WriteEntry("Reading GMT Difference Parameters is not successful", LogLevels.Error);
                    }
                }

                // ----------------------------------------------------

                // For Every 1 Minute
                if (!_theCRPCCalculation.ProgressEnergyCalc())
                {
                    _logger.WriteEntry("ProgressEnergyCalc() does not work!", LogLevels.Warn);
                    return;
                }

               
                if (!_theCRPCCalculation.TransPrimeVoltageCalc())
                {
                    _logger.WriteEntry("TransPrimeVoltageCalc() could not be completed!", LogLevels.Warn);
                    return;
                }

                // For Every 3 Minutes
                if ((aCycleNo - 1) % 3 == 0)
                {
                    _logger.WriteEntry("       ----- 3 Minute Cycle -----", LogLevels.Info);

                    // CosPhi Limit Checking is done automatically in Scada
                    if (!_theCRPCCalculation.CosPhiCalc())
                    {
                        _logger.WriteEntry("CosPhiCalc() does not work!", LogLevels.Warn);
                        return;
                    }

                    // After calculating the 15Min CosPhi, Reset the energies and counters.
                    if (aCycleNo == 1)
                    {
                        if (!_theCRPCCalculation.Preset1Min())
                        {
                            _logger.WriteEntry("Preset1Min() could not be completed!", LogLevels.Warn);
                            return;
                        }
                    }
                    
                    // Limit Checking
                    if (!_theCLimitChecker.VoltageLimitChecking())
                    {
                        _logger.WriteEntry("VoltageLimitChecking() does not work!", LogLevels.Warn);
                    }




                    if (!_theCLimitChecker.QLimitChecking())
                    {
                        _logger.WriteEntry("QLimitChecking() does not work!", LogLevels.Warn);
                    }



                    // Voltage Control
                    if (!_theCVoltageController.VoltageControl())
                    {
                        _logger.WriteEntry("VoltageControl() does not work!", LogLevels.Warn);
                    }

                    // CosPhi Control
                    if (!_theCCosPhiController.CosPhiControl(aCycleNo))
                    {
                        _logger.WriteEntry("CosPhiControl() does not work!", LogLevels.Warn);
                    }

                    // Q of Generators Control
                    if (!_theCQController.QControl())
                    {
                        _logger.WriteEntry("QControl() does not work!", LogLevels.Warn);
                    }

                }

                // ----------------------------------------------------

                _logger.WriteEntry("Exit of method.",LogLevels.Info);

				
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message,LogLevels.Error);
				
			}

		}

        

        internal void init()
        {
            try
            {
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP19"), 340.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP18"), 346.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP17"), 353.3f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP16"), 360.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP15"), 366.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP14"), 373.3f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP13"), 380.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP12"), 386.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP11"), 393.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP10"), 400.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP9"), 405.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP8"), 413.3f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP7"), 420.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP6"), 426.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP5"), 433.3f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP4"), 440.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP3"), 446.7f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP2"), 453.3f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VTAP1"), 460.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("K"), 10.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("K1"), 10.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("K2"), 15.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("M"), 10.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VR_EAF"), 4.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VR_PP"), 4.0f);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VR_TAV"), 3.0f);
                _updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Disappear, string.Empty);
                _updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Disappear, string.Empty);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK1"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK2"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK3"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK4"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK5"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK6"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK7"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK8"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 0);
                _updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), 0);




            }
            catch(System.Exception excep)
            {
                _logger.WriteEntry($"Could not init value in SCADA: {excep}", LogLevels.Error);

            }
        }

        public void Integrator(RPCScadaPoint scadaPoint)
        {
            var accScadaPoint = _repository.GetAccScadaPoint(scadaPoint.Name);
            _energyCalc.TotalEnergy(accScadaPoint);

        }
        public void AlarmAcked_Processing(RPCScadaPoint scadaPoint)
        {
            if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint(scadaPoint.Name), SinglePointStatus.Disappear, string.Empty))
            {
                _logger.WriteEntry($"Fail to Disappear Alarm {scadaPoint.Name}", LogLevels.Error);
                return;
            }
        }
    }
}
