using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;
using System.Timers;
using COM;


namespace RPC
{
    internal class RPCManager:IProcessing
    {
        
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;
        private readonly Timer _timer;

		private const int RPC_TIMER_TICKS = 60000;
		public NetworkConfValidator _theCNetworkConfValidator;
        public CycleValidator _theCCycleValidator;
		private RPCCalculation _theCRPCCalculation;
		private LimitChecker _theCLimitChecker;
		private VoltageController _theCVoltageController;
        private QController _theCQController;
        private CosPHIController _theCCosPhiController;





		public RPCManager(ILogger logger, IRepository repository, ICpsCommandService scadaCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(logger, scadaCommand);
            _repository = repository;
            _timer = new Timer();
            _timer.Interval = RPC_TIMER_TICKS;
            _timer.Elapsed += RunCyclicOperation;

            _theCNetworkConfValidator = new NetworkConfValidator(_repository, _logger, _updateScadaPointOnServer);
            _theCCycleValidator = new CycleValidator(_repository, _logger, _updateScadaPointOnServer);
            _theCRPCCalculation = new RPCCalculation(_repository, _logger, _updateScadaPointOnServer);
            _theCLimitChecker = new LimitChecker(_repository, _logger, _updateScadaPointOnServer);
            _theCVoltageController = new VoltageController(repository, _logger, _updateScadaPointOnServer);
            _theCQController = new QController(repository, _logger, _updateScadaPointOnServer);
            _theCCosPhiController = new CosPHIController(repository, _logger, _updateScadaPointOnServer);


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
            _timer.Start();
        }

        
		public void RunCyclicOperation(object sender, ElapsedEventArgs e)
		{
			try
			{

				int aCycleNo = 0;

				_logger.WriteEntry("-----------------------------------------------------------------------  ", LogLevels.Info);
				_logger.WriteEntry("Enter to method on: " + Environment.MachineName.ToString(),LogLevels.Info);
                //_logger.WriteEntry(" Machine State is: " + GeneralModule.GetProcessState(GeneralModule.eRPCState));

                // Stop the process if it is not Enable
                if (_repository.GetRPCScadaPoint("RPCSTATUS").Value != 2)
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
					_logger.WriteEntry("Network Configuration is Not Admitted!",LogLevels.Info);
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
                        _logger.WriteEntry("Reading GMT Difference Parameters is not successful", LogLevels.Error);
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

                    // CosPhi Limit Checking is done automatically in PowerCC
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

	

	}
}
