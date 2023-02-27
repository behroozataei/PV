using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;
using System.Timers;
using COM;

namespace OCP
{
    internal class OCPManager : IProcessing
    {
        private const int OCP_TIMER_TICKS = 3000;
        private readonly ILogger _logger;
        private readonly OCPCycleValidator _cycleValidator;
        private readonly OCPCurrentEvaluation _currentEvaluation;
        private readonly OCPOverloadCheck _overloadCheck;
        private readonly OCPOverloadPreparation _overloadPreparation;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;
        private readonly Timer _timer;
        private bool _firstRun;
        private bool isCompleted = true;
        private bool isActived = false;

        internal OCPManager(ILogger logger, IRepository repository, ICpsCommandService scadaCommand)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(logger, scadaCommand);
            _cycleValidator = new OCPCycleValidator(repository, _updateScadaPointOnServer, logger);
            _currentEvaluation = new OCPCurrentEvaluation(repository, _updateScadaPointOnServer, logger);
            _overloadCheck = new OCPOverloadCheck(repository, _updateScadaPointOnServer, logger);
            _overloadPreparation = new OCPOverloadPreparation(logger, repository, repository.GetCheckPoints(), _updateScadaPointOnServer, _cycleValidator);
            _repository = repository;
            _firstRun = true;

            _timer = new Timer();
            _timer.Interval = OCP_TIMER_TICKS;

            // A.K  1401.11.24 Commented this line
         
           // _timer.Elapsed += OnTimerElapsed;
        }

        
        public void Startwork()
        {
            // _timer.Start();
            CheckOCPTimer();
        }

        public void StopWork()
        {
            _timer.Stop();
        }

       // A.K  1401.11.24 Added this method
        public void CheckOCPTimer()
        {
            var callme = true;
            while (true)
            {
                DateTime vTime = DateTime.Now;
                if (callme && ((vTime.Second % 3) == 0) && (vTime.Millisecond < 500))
                {
                    //_logger.WriteEntry(" --------------- OnOCPTimer :     " + vTime.ToString(), LogLevels.Info);
                    OnOCPTimer();
                    callme = false;
                }

                if (!callme && ((vTime.Millisecond >= 500)))
                    callme = true;

                System.Threading.Thread.Sleep(100);
            }
        }

        // private void OnTimerElapsed(object sender, EventArgs e )
        void OnOCPTimer()
        {
            var cycleNo = 0;
            //_logger.WriteEntry($"Delay of Runtime Cycle: {e.Delay} , Timer faults = {e.Fallouts}", LogLevels.Info);

            try
            {
                if (isCompleted == false)
                    return;

                if (GlobalData.CPSStatus == false)
                {
                    isCompleted = true;
                    _logger.WriteEntry("CPS Connection Error!", LogLevels.Error);
                    return;
                }
                    

                isCompleted = false;
                //_logger.WriteEntry("_________________________________________________________________________ ", LogLevels.Info);
                //_logger.WriteEntry("OCP Cylce processing is started . . . ", LogLevels.Info);

                //' 1394.05.05 Start
                System.DateTime vTimeStart = DateTime.FromOADate(0);
                vTimeStart = DateTime.Now;
                //Call theCTraceLogger.WriteLog(TraceInfo1, "COCPManager.RunCyclicOperation", "")
                //Call theCTraceLogger.WriteLog(TraceInfo1, "COCPManager.RunCyclicOperation", "Start: " & Time)
                //' 1394.05.05 End

                if (!_overloadCheck.OCP_Function_Status())
                {
                    isCompleted = true;
                    if (isActived)
                    {
                        _logger.WriteEntry("OCP Funtion is OFF!", LogLevels.Error);
                        isActived=false;
                    }


                    return;
                }
                else
                    if (!isActived)
                    {
                        _logger.WriteEntry("OCP Funtion is ON!", LogLevels.Info);
                        isActived = true;
                    }



                if (!_cycleValidator.GetOCPCycleNo(_firstRun))
                {
                    _logger.WriteEntry("CycleNo is not correct!", LogLevels.Info);
                    _firstRun = false;

                    isCompleted = true;
                    return;
                }
                else
                {
                    cycleNo = _cycleValidator.CycleNo;
                    // If CycleNo is not correct the qualities are set to Invalid in CCycleValidator.

                    _firstRun = false;
                }

                if (!_cycleValidator.SkipReadEval)
                {
                    if (cycleNo == 1)
                        _currentEvaluation.InitializeCheckPoint();

                    _currentEvaluation.ReadValue(cycleNo);
                    _currentEvaluation.EvaluateCurrent(cycleNo);
                }

                // TODO : In the original code, this line is commented!!!
                //_overloadCheck.ResetIT(eResetType.ResetAll);

                bool isAnyOverload = _overloadCheck.CheckOverload(cycleNo);

                if (!_overloadPreparation.PrepareOverloadData(cycleNo, _overloadCheck))
                    _logger.WriteEntry("Try to run PrepareOverloadData for processing OVERLOAD_CONDITION was failed.", LogLevels.Error);

                if (isAnyOverload)
                    _logger.WriteEntry("_________________________ Overload is detected and processed ______________________________ ", LogLevels.Info);

                //' 1394.05.05 Start
                // Check duration of running OCP.
                int iDiffSec;
                System.DateTime vtime = DateTime.Now;
                iDiffSec = (vtime.Minute * 60 + vtime.Second) - (vTimeStart.Minute * 60 + vTimeStart.Second);
                if (iDiffSec > 2)
                {
                    _logger.WriteEntry("Duration(Sec): " + iDiffSec, LogLevels.Warn);
                }

                //' 1394.05.05 End
                isCompleted = true;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
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

        //------------------------------------------------------------------------------
        // StartTimeServiceDemo - Initiates the timer
        //------------------------------------------------------------------------------
        public void StartTimeService()
        {
            // 
            _overloadPreparation.FillBigTransSides();

            // 
            int aSec = DateTime.Now.Second;
            int mSec = DateTime.Now.Millisecond;

            // Start from the next 3 seconds
            // Sampling is done in the seconds 0, 3, 6, 9, ...
            _logger.WriteEntry("StartTimeService(), Start of Timer", LogLevels.Info);

            _firstRun = true;
            System.Threading.Thread.Sleep((3000 - ((aSec % 3) * 1000 + mSec)));
            Startwork();
        }

        public void QualityError(OCPCheckPoint checkpoint, QualityCodes Quality, SinglePointStatus Status)
        {
            _currentEvaluation.QualityErrorAlarm(checkpoint, Quality, Status);
        }

        public void AlarmAcked_Processing(OCPScadaPoint ocpScadaPoint)
        {
            if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint(ocpScadaPoint.Name), SinglePointStatus.Disappear, string.Empty))
            {
                _logger.WriteEntry($"Fail to Disappear Alarm {ocpScadaPoint.Name}", LogLevels.Error);
                return;
            }
        }
    }
}
