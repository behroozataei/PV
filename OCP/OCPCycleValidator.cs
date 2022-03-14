using Irisa.Logger;
using System;
using System.Collections.Generic;

namespace OCP
{
    internal class OCPCycleValidator
    {
        private readonly ILogger _logger;
        UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;

        private readonly DateTime[] _cycles;  //Array for 15 seconds period
        private readonly IEnumerable<OCPCheckPoint> _checkPoints;
        private int _cycleNo = 0;
        private int _actualCycleNo = 0;

        internal OCPCycleValidator(IRepository repository, UpdateScadaPointOnServer aupdateScadaPointOnServer, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = aupdateScadaPointOnServer ?? throw new ArgumentNullException(nameof(aupdateScadaPointOnServer));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _checkPoints = _repository.GetCheckPoints();
            _cycles = new System.DateTime[6];
        }

        public bool GetOCPCycleNo(bool firstRun)
        {
            var result = true;

            int cycleDiff;
            int missedCycleNo;
            var vTime = DateTime.Now;
            var substitutionQuality = OCPCheckPointQuality.Invalid;
            var substitutionValue = 0.0f;

            SkipReadEval = false;
            var Rndsecend = (vTime.Millisecond > 800)&&((vTime.Second % 3) == 2) ?  1 : 0;
            _cycleNo = (((vTime.Second + Rndsecend)  % 15 ) / 3) + 1; // Cycles begin from 1 to 5
            //_cycleNo = ((vTime.Second % 15) / 3) + 1; // Cycles begin from 1 to 5
            _actualCycleNo = _cycleNo;
            _cycles[_cycleNo] = vTime;

            // After first running
            if (!firstRun)
            {
                if (!IsContinuousCycles(_cycleNo, 5))
                {
                    _logger.WriteEntry("Function is not running continously!", LogLevels.Warn);
                    result = false;
                }
                else
                {
                    MissedShotNo = 0;
                    LastTrueCycleNo = _cycleNo;
                    LastTrueTime = _cycles[_cycleNo];
                }

                if (DeviationIstoomuch(vTime,3,200))
                    _logger.WriteEntry($"Cycle time Deviation is too high,time = {vTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}", LogLevels.Warn);
            }
            // First running
            else
            {
                MissedShotNo = 0;
                LastTrueCycleNo = _cycleNo;
                LastTrueTime = _cycles[_cycleNo];

                _logger.WriteEntry("GetOCPCycleNo..First running : CycleNo = " +
                                    LastTrueCycleNo.ToString() + " ; Time = " +
                                    LastTrueTime.ToString()
                                    , LogLevels.Info);
            }

            cycleDiff = _cycleNo - LastTrueCycleNo;
            if (cycleDiff < 0)
                cycleDiff += 5;

            var timeDiff = (LastTrueTime - _cycles[_cycleNo]).TotalSeconds;

            if (!result)
            {
                // Losing 4 Cycles, or 3 Cycles in critical condition
                if (timeDiff >= 13)
                {
                    // Cancel the period and reset all the values and counters
                    foreach (var checkPoint in _checkPoints)
                    {
                        checkPoint.Quality1 = OCPCheckPointQuality.Invalid;
                        checkPoint.Quality2 = OCPCheckPointQuality.Invalid;
                        checkPoint.Quality3 = OCPCheckPointQuality.Invalid;
                        checkPoint.Quality4 = OCPCheckPointQuality.Invalid;
                        checkPoint.Quality5 = OCPCheckPointQuality.Invalid;
                        checkPoint.SubstitutionCounter = 0;
                        checkPoint.Average.Value = 0;
                        checkPoint.OverloadFlag = false;
                    }

                    LastTrueTime = _cycles[_cycleNo];
                    _logger.WriteEntry("OCP function is not running continuously!", LogLevels.Warn);

                    // TODO: Send Alarm
                    // "OCP_NOT_CONTINIOUS" should be defined in "OCPParams", now I use "Functionality".
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Disappear, "OCP function is not running continuously!"))
                        _logger.WriteEntry("Could not send Alarm for check the overload for OCP function is not running continuously!", LogLevels.Error);
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Appear, "OCP function is not running continuously!"))
                        _logger.WriteEntry("Could not send Alarm for check the overload for OCP function is not running continuously!", LogLevels.Error);
                }
                else
                {
                    // We are missing a few cycles
                    if (cycleDiff > MissedShotNo)
                    {
                        missedCycleNo = LastTrueCycleNo + MissedShotNo;

                        if (missedCycleNo > 5)
                            missedCycleNo -= 5;

                        _actualCycleNo = missedCycleNo;

                        if (missedCycleNo == 1)
                        {
                            foreach (var checkPoint in _checkPoints)
                            {
                                checkPoint.SubstitutionCounter = 0;
                                checkPoint.Average.Value = 0;
                                checkPoint.OverloadFlag = false;
                            }

                            _logger.WriteEntry("Do Initialize in the new period.", LogLevels.Info);
                        }

                        if (MissedShotNo == 1)
                        {
                            foreach (var checkPoint in _checkPoints)
                            {
                                switch (LastTrueCycleNo)
                                {
                                    case 1:
                                        substitutionValue = checkPoint.Value1;
                                        substitutionQuality = checkPoint.Quality1;
                                        break;
                                    case 2:
                                        substitutionValue = checkPoint.Value2;
                                        substitutionQuality = checkPoint.Quality2;
                                        break;
                                    case 3:
                                        substitutionValue = checkPoint.Value3;
                                        substitutionQuality = checkPoint.Quality3;
                                        break;
                                    case 4:
                                        substitutionValue = checkPoint.Value4;
                                        substitutionQuality = checkPoint.Quality4;
                                        break;
                                    case 5:
                                        substitutionValue = checkPoint.Value5;
                                        substitutionQuality = checkPoint.Quality5;
                                        break;
                                }

                                switch (missedCycleNo)
                                {
                                    case 1:
                                        checkPoint.Value1 = substitutionValue;
                                        if (substitutionQuality != OCPCheckPointQuality.Invalid)
                                        {
                                            checkPoint.Quality1 = OCPCheckPointQuality.Previous;
                                        }
                                        else
                                        {
                                            checkPoint.Quality1 = OCPCheckPointQuality.Invalid;
                                        }
                                        break;

                                    case 2:
                                        checkPoint.Value2 = substitutionValue;
                                        if (substitutionQuality != OCPCheckPointQuality.Invalid)
                                        {
                                            checkPoint.Quality2 = OCPCheckPointQuality.Previous;
                                        }
                                        else
                                        {
                                            checkPoint.Quality2 = OCPCheckPointQuality.Invalid;
                                        }
                                        break;

                                    case 3:
                                        checkPoint.Value3 = substitutionValue;
                                        if (substitutionQuality != OCPCheckPointQuality.Invalid)
                                        {
                                            checkPoint.Quality3 = OCPCheckPointQuality.Previous;
                                        }
                                        else
                                        {
                                            checkPoint.Quality3 = OCPCheckPointQuality.Invalid;
                                        }
                                        break;

                                    case 4:
                                        checkPoint.Value4 = substitutionValue;
                                        if (substitutionQuality != OCPCheckPointQuality.Invalid)
                                        {
                                            checkPoint.Quality4 = OCPCheckPointQuality.Previous;
                                        }
                                        else
                                        {
                                            checkPoint.Quality4 = OCPCheckPointQuality.Invalid;
                                        }
                                        break;

                                    case 5:
                                        checkPoint.Value5 = substitutionValue;
                                        if (substitutionQuality != OCPCheckPointQuality.Invalid)
                                        {
                                            checkPoint.Quality5 = OCPCheckPointQuality.Previous;
                                        }
                                        else
                                        {
                                            checkPoint.Quality5 = OCPCheckPointQuality.Invalid;
                                        }
                                        break;
                                }
                                checkPoint.SubstitutionCounter++;
                            }

                            result = true;
                            SkipReadEval = true;
                        }
                        else
                        {
                            foreach (var checkPoint in _checkPoints)
                            {
                                switch (missedCycleNo)
                                {
                                    case 1:
                                        checkPoint.Quality1 = OCPCheckPointQuality.Invalid;
                                        break;
                                    case 2:
                                        checkPoint.Quality2 = OCPCheckPointQuality.Invalid;
                                        break;
                                    case 3:
                                        checkPoint.Quality3 = OCPCheckPointQuality.Invalid;
                                        break;
                                    case 4:
                                        checkPoint.Quality4 = OCPCheckPointQuality.Invalid;
                                        break;
                                    case 5:
                                        checkPoint.Quality5 = OCPCheckPointQuality.Invalid;
                                        break;
                                }
                            }

                            _logger.WriteEntry("Set invalid !", LogLevels.Warn);
                        }
                    }
                    else
                    {
                        result = true;
                        MissedShotNo = 0;
                        LastTrueCycleNo = _cycleNo;
                        LastTrueTime = _cycles[_cycleNo];
                    }
                }
            }

            return result;
        }

        private bool IsContinuousCycles(int currCycle, int difference)
        {
            int prevCycle;
            bool result;

            if (currCycle == 1)
                prevCycle = 5;
            else
                prevCycle = currCycle - 1;

            var diffInSeconds = (int)(_cycles[prevCycle] - _cycles[currCycle]).TotalSeconds;
            if (diffInSeconds >= difference)
            {
                result = false;
                _logger.WriteEntry("IsContinuousCycles .. " +
                                    currCycle.ToString() + " ; " +
                                    prevCycle.ToString() + " ; " +
                                    difference.ToString() + " ; " +
                                    diffInSeconds.ToString() + " ; " +
                                    result.ToString() + " ; " +
                                    _cycles[prevCycle].ToString() + " ; " +
                                    _cycles[currCycle].ToString()
                                    , LogLevels.Info);
            }
            else
                result = true;

            return result;
        }

        private bool DeviationIstoomuch(DateTime vTime, int Cycletime, int MaxDeviation )
        {
            if ((((vTime.Second % Cycletime) == 0) && vTime.Millisecond < MaxDeviation) ||
                (((vTime.Second % Cycletime) == (Cycletime -1)) && (1000-vTime.Millisecond)< MaxDeviation))
                return false;
            else
                return true;
        }
        //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES
        public object LogCycleVal(OCPCheckPoint ocpCheckPoint)
        {
            var dict = new SortedDictionary<DateTime, Tuple<int, float>>();
            dict.Add(_cycles[1], new Tuple<int, float>(1, ocpCheckPoint.Value1));
            dict.Add(_cycles[2], new Tuple<int, float>(2, ocpCheckPoint.Value2));
            dict.Add(_cycles[3], new Tuple<int, float>(3, ocpCheckPoint.Value3));
            dict.Add(_cycles[4], new Tuple<int, float>(4, ocpCheckPoint.Value4));
            dict.Add(_cycles[5], new Tuple<int, float>(5, ocpCheckPoint.Value5));

            foreach (KeyValuePair<DateTime, Tuple<int, float>> kvp in dict)
            {
                _logger.WriteEntry($"Cycle({kvp.Value.Item1}) = " + kvp.Key.ToString("yyyy-MM-dd HH:mm:ss.fff") + " ; " + kvp.Value.Item2.ToString(), LogLevels.Info);
            }

            //_logger.WriteEntry("Cycle (1)  = " + _cycles[1].ToString() + " ; " + ocpCheckPoint.Value1.ToString(), LogLevels.Info);
            //_logger.WriteEntry("Cycle (2)  = " + _cycles[2].ToString() + " ; " + ocpCheckPoint.Value2.ToString(), LogLevels.Info);
            //_logger.WriteEntry("Cycle (3)  = " + _cycles[3].ToString() + " ; " + ocpCheckPoint.Value3.ToString(), LogLevels.Info);
            //_logger.WriteEntry("Cycle (4)  = " + _cycles[4].ToString() + " ; " + ocpCheckPoint.Value4.ToString(), LogLevels.Info);
            //_logger.WriteEntry("Cycle (5)  = " + _cycles[5].ToString() + " ; " + ocpCheckPoint.Value5.ToString(), LogLevels.Info);

            return null;
        }
        //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES

        internal bool SkipReadEval { get; private set; }
        internal int MissedShotNo { get; private set; }
        internal int LastTrueCycleNo { get; private set; }
        internal DateTime LastTrueTime { get; private set; }
        internal int CycleNo { get { return _actualCycleNo; } }
    }
}
