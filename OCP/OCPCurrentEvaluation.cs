using Irisa.Common;
using Irisa.Logger;
using System;

namespace OCP
{
    internal class OCPCurrentEvaluation
    {
        private const int MAXPREVIOUS = 1; // Maximum times that a value can be substituted with the last valid value in a period of 15sec.
        private const int MAXCALCULATED = 5; // Maximum times that a value can be substituted with the calculated value in a period of 15sec.

        private readonly ILogger _logger;
        private readonly IRepository _repository;
        UpdateScadaPointOnServer _updateScadaPointOnServer;

        public OCPCurrentEvaluation(IRepository repository, UpdateScadaPointOnServer aupdateScadaPointOnServer, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = aupdateScadaPointOnServer ?? throw new ArgumentNullException(nameof(aupdateScadaPointOnServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitializeCheckPoint()
        {
            foreach (var checkPoint in _repository.GetCheckPoints())
            {
                checkPoint.SubstitutionCounter = 0;
                checkPoint.Average.Value = 0;
                checkPoint.OverloadFlag = false;
            }
        }

        public void ReadValue(int CycleNo)
        {
            //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT
            System.DateTime cycletime;
            //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT

            //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT
            cycletime = DateTime.UtcNow;
            //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT
            foreach (var checkPoint in _repository.GetCheckPoints())
            {
                if (checkPoint.CheckOverload != 'Y')
                    continue;

                switch (CycleNo)
                {
                    case 1:
                        checkPoint.Value1 = checkPoint.Value;
                        checkPoint.Quality1 = checkPoint.Quality;
                        checkPoint.TCycle1 = cycletime;  //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT 
                        break;
                    case 2:
                        checkPoint.Value2 = checkPoint.Value;
                        checkPoint.Quality2 = checkPoint.Quality;
                        checkPoint.TCycle2 = cycletime;  //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT 
                        break;
                    case 3:
                        checkPoint.Value3 = checkPoint.Value;
                        checkPoint.Quality3 = checkPoint.Quality;
                        checkPoint.TCycle3 = cycletime;  //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT 
                        break;
                    case 4:
                        checkPoint.Value4 = checkPoint.Value;
                        checkPoint.Quality4 = checkPoint.Quality;
                        checkPoint.TCycle4 = cycletime;  //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT 
                        break;
                    case 5:
                        checkPoint.Value5 = checkPoint.Value;
                        checkPoint.Quality5 = checkPoint.Quality;
                        checkPoint.TCycle5 = cycletime;  //"IMANIAN 96-11-07  ADD CYCLTIME FOR EACH POINT 
                        break;
                }
                
                 _updateScadaPointOnServer.WriteSample(checkPoint, checkPoint.Value);
                // 2021.04.24 A.K and B.A, added these lines:
                if (OCPQualityConvertor.GetCheckPointQuality((QualityCodes)checkPoint.QualityCodes) == OCPCheckPointQuality.Invalid)
                {
                    _logger.WriteEntry("Quality warning: " + "QualityCode = " + (QualityCodes)checkPoint.QualityCodes + " ; Value = " + checkPoint.Value.ToString() + " ; Name = " + checkPoint.Name + " ; Network Path = " + checkPoint.NetworkPath.ToString(), LogLevels.Warn);
                }
            }
        }

        public void EvaluateCurrent(int CycleNo)
        {
            //_logger.WriteEntry($"Evaluate Checkpoints in Cycle {CycleNo.ToString()}", LogLevels.Trace);

            foreach (var checkPoint in _repository.GetCheckPoints())
            {
                if (checkPoint.CheckOverload != 'Y')
                    continue;

                //_logger.WriteEntry($"Evaluate Checkpoint {checkPoint.Name} in cycle  {CycleNo.ToString()}", LogLevels.Trace);
                if (!CheckCurrentQuality(checkPoint, CycleNo))
                {
                    // TODO: 
                    // Send 2 alarms:
                    // 1- A general one that indicates a point had a problem in evaluation.
                    // 2- An alarm to that point(for having the NetworkPath).
                    //string message = "Could not send Alarm for check the overload for OCP function is not running continuously!";
                    //// TODO: which point?
                    //if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint(" ????? "), AlarmStatus.Appear, message))
                    //    _logger.WriteEntry(message, LogLevels.Error);

                    _logger.WriteEntry("Could not evaluate Shed Point, CheckCurrentQuality, " + checkPoint.NetworkPath + " in Cycle" + CycleNo.ToString(), LogLevels.Warn);
                }
                float sampleValue;

                switch (CycleNo)
                {
                    case 1:
                        sampleValue = checkPoint.Value1;
                        break;
                    case 2:
                        sampleValue = checkPoint.Value2;
                        break;
                    case 3:
                        sampleValue = checkPoint.Value3;
                        break;
                    case 4:
                        sampleValue = checkPoint.Value4;
                        break;
                    case 5:
                        sampleValue = checkPoint.Value5;
                        break;
                }

                // Send to SCADA  ==> RootPath + "/SAMPLE", SampleValue.ToString()
                //if( checkPoint.Value1 > 0 || checkPoint.Value2 > 0 || checkPoint.Value3 > 0 ||
                //    checkPoint.Value4 > 0 || checkPoint.Value5 > 0 )
                //    _logger.WriteEntry($"Evaluate {checkPoint.Name}; Cycle {CycleNo.ToString()} ; " +
                //    $"{checkPoint.Value1, 0:n1} ; " + $"{checkPoint.Value2, 0:n1}; " +
                //    $"{checkPoint.Value3, 0:n1}; " + $"{checkPoint.Value4, 0:n1}; " +
                //    $"{checkPoint.Value5, 0:n1} " , LogLevels.Trace);
            }
        }

        private bool CheckCurrentQuality(OCPCheckPoint checkPoint, int cycleNo)
        {
            try
            {
                var nowValueAandQulity = GetCheckPointValQuality(checkPoint, cycleNo);
                var nowValue = nowValueAandQulity.Item1;
                var nowQuality = nowValueAandQulity.Item2;

#if DEBUG
                if (nowQuality == OCPCheckPointQuality.Calculated || nowQuality == OCPCheckPointQuality.Previous)
                    throw new InvalidOperationException();
#endif

                if (nowQuality == OCPCheckPointQuality.Valid)
                    return true;

                // if nowQuality is invalid, check secondary side       
                if (checkPoint.Category == "SECONDARY")
                {
                    // Use the calculatd value
                    if (checkPoint.SubstitutionCounter < MAXCALCULATED)
                    {
                        float subsValue = 0;
                        if (!CalcSubstitution(checkPoint, ref subsValue))
                        {
                            _logger.WriteEntry("Could not calculate the substitution value!", LogLevels.Error);
                            return false;
                        }

                        SetCheckPointValQuality(checkPoint, cycleNo, subsValue, OCPCheckPointQuality.Calculated);
                    }
                    else
                    {
                        _logger.WriteEntry($"The value is substituted for {MAXCALCULATED.ToString()}" +
                            $" time(s) for the check point {checkPoint.NetworkPath}", LogLevels.Info);
                    }
                }
                else
                {
                    // Use the previous value
                    if (checkPoint.SubstitutionCounter < MAXPREVIOUS)
                    {
                        var prevCycleNo = cycleNo - 1;

                        if (cycleNo == 1)
                            prevCycleNo = 5;

                        var prevValueAandQulity = GetCheckPointValQuality(checkPoint, prevCycleNo);
                        var prevValue = nowValueAandQulity.Item1;
                        var prevQuality = nowValueAandQulity.Item2;

                        if (prevQuality == OCPCheckPointQuality.Valid)
                            SetCheckPointValQuality(checkPoint, prevCycleNo, prevValue, OCPCheckPointQuality.Previous);
                    }
                    else
                    {
                        _logger.WriteEntry($"The value is substituted for {MAXPREVIOUS.ToString()}" +
                            $" time(s) for the check point {checkPoint.NetworkPath}.", LogLevels.Info);
                    }
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry("COCPCurrentEvaluation..CheckCurrentQuality() " + excep.Message, LogLevels.Error);
                return false;
            }

            return true;
        }

        private bool CalcSubstitution(OCPCheckPoint checkPoint, ref float aValue)
        {
            var P = 0.0f;
            var Q = 0.0f;
            var V = 0.0f;
            var PQuality = OCPCheckPointQuality.Invalid;
            var QQuality = OCPCheckPointQuality.Invalid;
            var VQuality = OCPCheckPointQuality.Invalid;

            if (checkPoint.Name == "CP21_MIS_T1AN")  // T1AN secondary side
            {
                var T1AN_P = _repository.GetOCPScadaPoint("T1AN_P");
                var T1AN_Q = _repository.GetOCPScadaPoint("T1AN_Q");
                var T1AN_SECVOLT = _repository.GetOCPScadaPoint("T1AN_SECVOLT");

                P = T1AN_P.Value;
                Q = T1AN_Q.Value;
                V = T1AN_SECVOLT.Value;

                PQuality = T1AN_P.Quality;
                QQuality = T1AN_Q.Quality;
                VQuality = T1AN_SECVOLT.Quality;
            }
            else if (checkPoint.Name == "CP22_MIS_T2AN")     // T2AN secondary side
            {
                var T2AN_P = _repository.GetOCPScadaPoint("T2AN_P");
                var T2AN_Q = _repository.GetOCPScadaPoint("T2AN_Q");
                var T2AN_SECVOLT = _repository.GetOCPScadaPoint("T2AN_SECVOLT");
                P = T2AN_P.Value;
                Q = T2AN_Q.Value;
                V = T2AN_SECVOLT.Value;
                PQuality = T2AN_P.Quality;
                QQuality = T2AN_Q.Quality;
                VQuality = T2AN_SECVOLT.Quality;
            }
            else if (checkPoint.Name == "CP23_MIS_T3AN_MV3")   // T3AN secondary side
            {
                var MV3 = _repository.GetOCPScadaPoint("MV3");
                if (MV3 == null)
                    _logger.WriteEntry("Error: MV3 is null!", LogLevels.Error);

                if ((MV3 != null) && ((DigitalDoubleStatus)MV3.Value == DigitalDoubleStatus.Close))
                {
                    var T3AN_MV3_P = _repository.GetOCPScadaPoint("T3AN_MV3_P");
                    var T3AN_MV3_Q = _repository.GetOCPScadaPoint("T3AN_MV3_Q");
                    P = T3AN_MV3_P.Value;
                    Q = T3AN_MV3_Q.Value;
                    PQuality = T3AN_MV3_P.Quality;
                    QQuality = T3AN_MV3_Q.Quality;
                }
                else
                {
                    var MZ3 = _repository.GetOCPScadaPoint("MZ3");
                    if (MZ3 == null)
                        _logger.WriteEntry("Error: MZ3 is null!", LogLevels.Error);

                    var T3AN_SECVOLT = _repository.GetOCPScadaPoint("T3AN_SECVOLT");

                    if ((MZ3 != null) && ((DigitalDoubleStatus)MZ3.Value == DigitalDoubleStatus.Close))
                    {
                        var T3AN_MZ3_P = _repository.GetOCPScadaPoint("T3AN_MZ3_P");
                        var T3AN_MZ3_Q = _repository.GetOCPScadaPoint("T3AN_MZ3_Q");
                        P = T3AN_MZ3_P.Value;
                        Q = T3AN_MZ3_Q.Value;
                        PQuality = T3AN_MZ3_P.Quality;
                        QQuality = T3AN_MZ3_Q.Quality;

                    }
                    else
                    {
                        if ((DigitalDoubleStatus)MV3.Value == DigitalDoubleStatus.Intransit ||
                            (DigitalDoubleStatus)MV3.Value == DigitalDoubleStatus.Disturb ||
                            (DigitalDoubleStatus)MZ3.Value == DigitalDoubleStatus.Intransit ||
                            (DigitalDoubleStatus)MZ3.Value == DigitalDoubleStatus.Disturb)
                            _logger.WriteEntry("MV3 and MZ3 in bad condition!", LogLevels.Info);
                    }
                }
            }
            else if (checkPoint.Name == "CP29_MIS_T4AN")   // T4AN secondary side
            {
                var T4AN_P = _repository.GetOCPScadaPoint("T4AN_P");
                var T4AN_Q = _repository.GetOCPScadaPoint("T4AN_Q");
                var T4AN_SECVOLT = _repository.GetOCPScadaPoint("T4AN_SECVOLT");
                P = T4AN_P.Value;
                Q = T4AN_Q.Value;
                V = T4AN_SECVOLT.Value;
                PQuality = T4AN_P.Quality;
                QQuality = T4AN_Q.Quality;
                VQuality = T4AN_SECVOLT.Quality;
            }
            else if (checkPoint.Name == "CP43_MIS_T6AN")   // T6AN secondary side
            {
                var T6AN_P = _repository.GetOCPScadaPoint("T6AN_P");
                var T6AN_Q = _repository.GetOCPScadaPoint("T6AN_Q");
                var T6AN_SECVOLT = _repository.GetOCPScadaPoint("T6AN_SECVOLT");
                P = T6AN_P.Value;
                Q = T6AN_Q.Value;
                V = T6AN_SECVOLT.Value;
                PQuality = T6AN_P.Quality;
                QQuality = T6AN_Q.Quality;
                VQuality = T6AN_SECVOLT.Quality;
            }
            else if (checkPoint.Name == "CP36_MIS_T5AN")   // T5AN secondary side
            {
                var T5AN_P = _repository.GetOCPScadaPoint("T5AN_P");
                var T5AN_Q = _repository.GetOCPScadaPoint("T5AN_Q");
                var T5AN_SECVOLT = _repository.GetOCPScadaPoint("T5AN_SECVOLT");
                P = T5AN_P.Value;
                Q = T5AN_Q.Value;
                V = T5AN_SECVOLT.Value;
                PQuality = T5AN_P.Quality;
                QQuality = T5AN_Q.Quality;
                VQuality = T5AN_SECVOLT.Quality;
            }
            else if (checkPoint.Name == "CP44_MIS_T7AN")    // T7AN secondary side 
            {
                var T7AN_P = _repository.GetOCPScadaPoint("T7AN_P");
                var T7AN_Q = _repository.GetOCPScadaPoint("T7AN_Q");
                var T7AN_SECVOLT = _repository.GetOCPScadaPoint("T7AN_SECVOLT");
                P = T7AN_P.Value;
                Q = T7AN_Q.Value;
                V = T7AN_SECVOLT.Value;
                PQuality = T7AN_P.Quality;
                QQuality = T7AN_Q.Quality;
                VQuality = T7AN_SECVOLT.Quality;
            }

            //' KAJI START fo T8AN 
            else if (checkPoint.Name == "CP57_MIS_T8AN")    // T8AN secondary side 
            {
                var T8AN_P = _repository.GetOCPScadaPoint("T8AN_P");
                var T8AN_Q = _repository.GetOCPScadaPoint("T8AN_Q");
                var T8AN_SECVOLT = _repository.GetOCPScadaPoint("T8AN_SECVOLT");
                P = T8AN_P.Value;
                Q = T8AN_Q.Value;
                V = T8AN_SECVOLT.Value;
                PQuality = T8AN_P.Quality;
                QQuality = T8AN_Q.Quality;
                VQuality = T8AN_SECVOLT.Quality;
            }
            //' KAJI END fo T8AN 

            if ((PQuality != OCPCheckPointQuality.Valid) ||
                (QQuality != OCPCheckPointQuality.Valid) ||
                (VQuality != OCPCheckPointQuality.Valid))
            {
                // One of the measures are is Invalid
                _logger.WriteEntry("Error in Quality of P/V/Q of " + checkPoint.NetworkPath, LogLevels.Error);
                aValue = 0;
                return false;
            }

            if (V < 0.001)
            {
                // One of the measures are is Invalid
                _logger.WriteEntry("Error in Value of Voltage of " + checkPoint.NetworkPath, LogLevels.Error);
                aValue = 0;
                return false;
            }

            aValue = Convert.ToSingle((Math.Sqrt(P * P + Q * Q) / (Math.Sqrt(3) * V)) * 1000);
            return true;
        }

        private Tuple<float, OCPCheckPointQuality> GetCheckPointValQuality(OCPCheckPoint checkPoint, int cycleNo)
        {
            switch (cycleNo)
            {
                case 1:
                    return new Tuple<float, OCPCheckPointQuality>(checkPoint.Value1, checkPoint.Quality1);
                case 2:
                    return new Tuple<float, OCPCheckPointQuality>(checkPoint.Value2, checkPoint.Quality2);
                case 3:
                    return new Tuple<float, OCPCheckPointQuality>(checkPoint.Value3, checkPoint.Quality3);
                case 4:
                    return new Tuple<float, OCPCheckPointQuality>(checkPoint.Value4, checkPoint.Quality4);
                case 5:
                    return new Tuple<float, OCPCheckPointQuality>(checkPoint.Value5, checkPoint.Quality5);
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetCheckPointValQuality(OCPCheckPoint checkPoint, int cycleNo, float value, OCPCheckPointQuality quality)
        {
            switch (cycleNo)
            {
                case 1:
                    checkPoint.Value1 = value;
                    checkPoint.Quality1 = quality;
                    break;
                case 2:
                    checkPoint.Value2 = value;
                    checkPoint.Quality2 = quality;
                    break;
                case 3:
                    checkPoint.Value3 = value;
                    checkPoint.Quality4 = quality;
                    break;
                case 4:
                    checkPoint.Value4 = value;
                    checkPoint.Quality4 = quality;
                    break;
                case 5:
                    checkPoint.Value5 = value;
                    checkPoint.Quality5 = quality;
                    break;
            }
            checkPoint.SubstitutionCounter++;
        }
        internal void QualityErrorAlarm(OCPCheckPoint checkpoint, QualityCodes Quality, SinglePointStatus Status)
        {
            if (checkpoint.QualityErrorId != Guid.Empty)
            {
                if (!_updateScadaPointOnServer.SendAlarm(checkpoint.QualityErrorId, Status, "Quality Code = " + Quality + " ; Value = " + checkpoint.Value.ToString() + " ; Network Path = " + checkpoint.NetworkPath))
                    _logger.WriteEntry("Sending alarm failed for QualityError", LogLevels.Error);
            }
        }
    }
}
