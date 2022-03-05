using Irisa.Logger;
using Irisa.Message;
using System;
using System.Threading.Tasks;
using System.Timers;


namespace EEC
{
    internal sealed class EECManager
    {
        private const int TIMER_TICKS = 60000;
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly EECCycleValidator _cycleValidator;
        private readonly EECEnergyCalculator _energyCalculator;
        private readonly Timer _timer_1_Minute;
        private UpdateScadaPointOnServer _updateScadaPointOnServer;
        private EECSFSCManager _SFSCManager = null;

        internal EECManager(ILogger logger, IRepository repository, ICpsCommandService commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cycleValidator = new EECCycleValidator(_logger);

            _timer_1_Minute = new Timer();
            _timer_1_Minute.Interval = TIMER_TICKS;
            _timer_1_Minute.Elapsed += RunCyclicOperation;

            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, commandService);
            _energyCalculator = new EECEnergyCalculator(_repository, _logger, _updateScadaPointOnServer);

            _SFSCManager = new EECSFSCManager(_logger, repository, commandService);
            
        }

        private void RunCyclicOperation(object sender, ElapsedEventArgs e)
        {
            CheckCPSStatus();

            try
            {

                //-------------------------------------------------------------------------
                // Update setting for writing logs

                _logger.WriteEntry("▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼", LogLevels.Info);

                //'-------------------------------------------------------------------------
                //TODO:  Check Machine State, if this is StandBy, Exit CyclicActivation
                //_logger.WriteEntry("Enter to method on: " + "& GetMachineName()", LogLevels.Info);
                //_logger.WriteEntry(" Machine State is: " + "& GetProcessState(eEECState)", LogLevels.Info);

                //'-------------------------------------------------------------------------
                //' 0.2. Read FStatus of SCADA, Check it, and update it in the table or update SCADA of table
                //'-------------------------------------------------------------------------
                //' 1. Check function status, If not enabled --> Return
                var functionStatus = (DigitalSingleStatusOnOff)_repository.GetScadaPoint("FSTATUS").Value;
                if (functionStatus == DigitalSingleStatusOnOff.Off)
                {
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("SCADAError"), (DigitalSingleStatus)DigitalSingleStatusOnOff.Off, "Warning: EEC Function is Off"))
                        _logger.WriteEntry("Error: Can not send Alarm for 'EEC Function is Disabled'.", LogLevels.Error);
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("SCADAError"), (DigitalSingleStatus)DigitalSingleStatusOnOff.On, "Warning: EEC Function is Off"))
                        _logger.WriteEntry("Error: Can not send Alarm for 'EEC Function is Disabled'.", LogLevels.Error);

                    _logger.WriteEntry("EEC Function is Disabled", LogLevels.Warn);
                    // m_CCycleValidator.resetCyclesArray();
                    return;
                }

                if (!GlobalData.CPSStatus)
                {

                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("SCADAError"), (DigitalSingleStatus)DigitalSingleStatusOnOff.On, "CPS communication could not established"))
                        _logger.WriteEntry("Error: Can not send Alarm for 'CPS is gone!'.", LogLevels.Error);

                    _logger.WriteEntry("CPS is gone!", LogLevels.Warn);
                    return;
                }
                if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("SCADAError"), (DigitalSingleStatus)DigitalSingleStatusOnOff.Off, ""))
                    _logger.WriteEntry("Error: Can not send Alarm for 'EEC Function is Disabled'.", LogLevels.Error);

                //If Not m_theCEECParameters.EECStatus Then
                //'---------------------------------------
                //' Send Alarm To Operator
                //If Not m_theCSCADADataInterface.SendAlarm("FSTATUS", "OFF", "Warning: EEC Function is Off") Then

                //'---------------------------------------
                //' Log event
                //Call theCTraceLogger.WriteLog(TraceInfo1, "CEEC_Manager..runCyclicOperation", "Function is Disabled")
                //Call m_theCCycleValidator.resetCyclesArray
                //Exit Sub

                //' 0.3. updating all 1-Minute values, read from SCADA, write into Table
                //If Not m_theCEECParameters.updateEEC1MinuteValues() Then
                //Call theCTraceLogger.WriteLog(TraceError, "CEEC_Manager..runCyclicOperation", "Could not update EEC 1-Minute Values")
                //Call m_theCCycleValidator.resetCyclesArray
                //Exit Sub

                //'-------------------------------------------------------------------------
                //' 2. Get right CycleNo to start of 1-minute processing:
                //If Not m_theCCycleValidator.getEECCycleNo(m_FullCycleTag) Then
                //Call theCTraceLogger.WriteLog(TraceWarning, "CEEC_Manager..runCyclicOperation", "CycleNo is not correct")
                //Call m_theCCycleValidator.resetCyclesArray
                //Exit Sub
                //Else
                //aCycleNo = m_theCCycleValidator.CycleNo
                //End If
                var cycleNo = _cycleValidator.GetEECCycleNo();

                //'-------------------------------------------------------------------------
                //' 3. Call EnergyCalculator
                if (cycleNo == 0)
                {
                    //' 0.4. updating all Const parameters, read from SCADA, write into Table
                    //'   In the m_theCEnergyCalculator.Calc15MinVal(m_FullCycleTag),
                    //'       m_theCEECParameters.checkUpdateConstParams() will be called.
                    //If TempRecordSet(0).Value = 0 Then
                    //Call theCTraceLogger.WriteLog(TraceInfo1, "CEEC_Manager..runCyclicOperation", "Content of Table= " & TempRecordSet(0).Value)
                    //m_FullCycleTag = True
                    //End If

                    var fullCycleTag = true;

                    if (!_energyCalculator.Calc15MinValue(fullCycleTag))
                    {
                        _logger.WriteEntry("Calc15Min could not be completed", LogLevels.Info);
                        _logger.WriteEntry("Error in running Calc15MinValue", LogLevels.Error);

                        // TODO:
                        return;
                    }
                    //' 0.5. Updating all 15-Minute calculated values in SCADA
                }

                _energyCalculator.Calc1MinValue(cycleNo);

                //' 0.6. Updating all 1-Minute calculated values in SCADA

                //'-------------------------------------------------------------------------
                //' Writing exit message
                _logger.WriteEntry("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬   End of Running Cycle   ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬", LogLevels.Info);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
        }

        public void CheckCPSStatus()
        {
            int counter = 0;
            while (!GlobalData.CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
                _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);
                //if (counter++ > 30)
                //{
                //    _logger.WriteEntry("Error: after waiting for 120 seconds, CPSStatus is not connected!", LogLevels.Error);
                //    Environment.Exit(0);
                //  // throw new InvalidOperationException(" Error: after waiting for 120 seconds, CPSStatus is not connected! ");
                //}
            }
        }

        public void StartCyclicOperation()
        {
            _SFSCManager.Start();
            _energyCalculator.InitialValues();
            _energyCalculator.printInitialValues();

            if (!_energyCalculator.UpdateCurrentValuesFromLastNewValues())
            {
                _logger.WriteEntry("Error in UpdateCurrentValuesFromLastNewValues!", LogLevels.Error);
            };

            _energyCalculator.printInitialValues();

            // TODO: Write Const Values into SCADA
            //_updateScadaPointOnServer.SendOneMinuteEnergyCONSTValues();

            var currentTime = DateTime.UtcNow;
            var delay = (60 - currentTime.Second) * 1000 + (30000 - currentTime.Millisecond);
            var delayTask = Task.Delay(delay);
            delayTask.ContinueWith((t) =>
            {
                _timer_1_Minute.Start();
                RunCyclicOperation(null, null);
            });
        }

        public void ReinitializeCurrentFromNewAfterCPSStartToWork()
        {
            // Wait to be loaded all User set-points
            var _EC = _repository.GetScadaPoint("ECONTRACT_User");
            var _PL = _repository.GetScadaPoint("PLIMIT_User");
            while (true)
            {
                if ((_EC.Value > 0) && (_PL.Value > 0))
                {
                    if (!_energyCalculator.UpdateCurrentValuesFromLastNewValues())
                        _logger.WriteEntry("Error in UpdateCurrentValuesFromLastNewValues!", LogLevels.Error);

                    break;
                }

                System.Threading.Thread.Sleep(1000);
            }
        }

        public IProcessing RuntimeDataProcessing
        {
            get { return _energyCalculator; }
        }
    }
}
