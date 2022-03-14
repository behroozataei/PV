using Irisa.Logger;
using System;
using System.Collections.Generic;

namespace OCP
{
    internal class OCPOverloadCheck
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<OCPCheckPoint> _checkPoints;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;

        internal OCPOverloadCheck(IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _checkPoints = _repository.GetCheckPoints();
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
        }

        // If IT in SCADA is not set to 0 from the previous overload, it will be set to 0 here.
        internal void ResetIT(eResetType iResetType)
        {
            foreach (var checkPoint in _checkPoints)
            {
                if (checkPoint.ResetIT || (iResetType == eResetType.ResetAll))
                {
                    // Write data to SCADA, checkPoint.Tag ====> 0
                    _updateScadaPointOnServer.WriteIT(checkPoint, 0);

                    checkPoint.ResetIT = false;

                    checkPoint.OverloadAlarmFiveCycle = false;
                    checkPoint.OverloadAlarmFourCycle = false;
                    checkPoint.OverloadWarningFiveCycle = false;
                    checkPoint.OverloadWarningFourCycle = false;
                }
            }
        }

        // Loops all the data points to be checked
        internal bool CheckOverload(int CycleNo)
        {
            bool isOverloadedStatus = false;
            bool overloadOnPoint = false;
            bool overloadWarningFourValueAllCPs = false;
            bool overloadAlarmFourValueAllCPs = false;
            bool overloadWarningFiveValueAllCPs = false;
            bool overloadAlarmFiveValueAllCPs = false;

            var Four_Value_OverloadWarning = _repository.GetOCPScadaPoint("FOURVALUEWARNING");
            var Four_Value_Overloaded = _repository.GetOCPScadaPoint("FOURVALUEOVERLOADED");
            var Five_Value_OverloadWarning = _repository.GetOCPScadaPoint("FIVEVALUEWARNING");
            var Five_Value_Overloaded = _repository.GetOCPScadaPoint("FIVEVALUEOVERLOADED");
            var alarmPointOverloadWarning = _repository.GetOCPScadaPoint("OverloadWarning");
            var alarmPointOverloadAppear = _repository.GetOCPScadaPoint("OverloadAppear");

            foreach (var checkPoint in _checkPoints)
            {
                if (checkPoint.CheckOverload != 'Y') continue;

                if (!CheckOverloadonPoint(checkPoint, CycleNo, ref overloadOnPoint))
                {
                    // TODO: check
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Disappear, "Could not check the overload for " + checkPoint.NetworkPath.ToString() + " in Cycle" + CycleNo))
                        _logger.WriteEntry($"Could not send Alarm for check the overload for {checkPoint.NetworkPath}", LogLevels.Error);
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Appear, "Could not check the overload for " + checkPoint.NetworkPath.ToString() + " in Cycle" + CycleNo))
                        _logger.WriteEntry($"Could not send Alarm for check the overload for {checkPoint.NetworkPath}", LogLevels.Error);

                    _logger.WriteEntry($"Could not check the overload for {checkPoint.NetworkPath}", LogLevels.Error);

                }
                isOverloadedStatus = isOverloadedStatus || overloadOnPoint;
                overloadWarningFourValueAllCPs = overloadWarningFourValueAllCPs || checkPoint.OverloadWarningFourCycle;
                overloadAlarmFourValueAllCPs = overloadAlarmFourValueAllCPs || checkPoint.OverloadAlarmFourCycle;
                overloadAlarmFiveValueAllCPs = overloadAlarmFiveValueAllCPs || checkPoint.OverloadAlarmFiveCycle;
                overloadWarningFiveValueAllCPs = overloadWarningFiveValueAllCPs || checkPoint.OverloadWarningFiveCycle;
            }

            if (!overloadWarningFourValueAllCPs && (Four_Value_OverloadWarning.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(Four_Value_OverloadWarning, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending warning disappear failed for four value OverloadWarning", LogLevels.Error);

            if (!overloadAlarmFourValueAllCPs && (Four_Value_Overloaded.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(Four_Value_Overloaded, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending alarm disappear failed for four value OverloadAlarm", LogLevels.Error);

            if (!overloadWarningFiveValueAllCPs && (Five_Value_OverloadWarning.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_OverloadWarning, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending warning disappear failed for five value OverloadWarning", LogLevels.Error);

            if (!overloadWarningFiveValueAllCPs && (alarmPointOverloadWarning.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);

            if (!overloadAlarmFiveValueAllCPs && (Five_Value_Overloaded.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_Overloaded, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending alarm disappear failed for five value OverloadAlarm", LogLevels.Error);

            if (!overloadAlarmFiveValueAllCPs && !overloadAlarmFourValueAllCPs && !overloadWarningFiveValueAllCPs && (alarmPointOverloadAppear.Value == (float)SinglePointStatus.Appear))
                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadAppear, SinglePointStatus.Disappear, ""))
                    _logger.WriteEntry("Sending alarm failed for OverloadAppear", LogLevels.Error);



            return isOverloadedStatus;
        }

        // Checks a check point according to the cycle number,
        // if there is an overload or a four value limit violation possibility.
        private bool CheckOverloadonPoint(OCPCheckPoint checkPoint, int cycleNo, ref bool isOverload)
        {
            isOverload = false;

            var Four_Value_OverloadWarning = _repository.GetOCPScadaPoint("FOURVALUEWARNING");
            var Four_Value_Overloaded = _repository.GetOCPScadaPoint("FOURVALUEOVERLOADED");
            var Five_Value_OverloadWarning = _repository.GetOCPScadaPoint("FIVEVALUEWARNING");
            var Five_Value_Overloaded = _repository.GetOCPScadaPoint("FIVEVALUEOVERLOADED");

            try
            {
                //_logger.WriteEntry("CheckOverloadonPoint -> " + checkPoint.NetworkPath, LogLevels.Info);

                // Not valid data, quit for this data point.
                if ((checkPoint.NominalValue <= 0) || (checkPoint.LIMITPERCENT <= 0))
                {
                    _logger.WriteEntry($"NominalValue or LimitPercent is/are not valid for {checkPoint.NetworkPath}", LogLevels.Warn);
                    return false;
                }

                //"IMANIAN ; 1396-11-09 ; CHECK 400KV LINES IN EACH CYCLES
                // Calculate Limit Value
                var limitOncheckPoint = checkPoint.NominalValue * checkPoint.LIMITPERCENT;
                var alarmPointOverloadWarning = _repository.GetOCPScadaPoint("OverloadWarning");

                // TODO : Check these point addresses!!
                if ((checkPoint.Name == "CP59_NIS_L914") || (checkPoint.Name == "CP60_NIS_L915"))
                //if ((== "Network/Substations/NIS1/400kV/C02.A/CT") ||  == "Network/Substations/NIS2/400kV/C03.A/CT"))
                {
                    checkPoint.OverloadWarningFiveCycle = false;
                    switch (cycleNo)
                    {
                        case 1:
                            if (checkPoint.Value1 >= limitOncheckPoint && checkPoint.Quality1 == OCPCheckPointQuality.Valid)
                            {
                                checkPoint.OverloadFlag = true;
                                checkPoint.ResetIT = true;
                                checkPoint.OverloadWarningFiveCycle = true;
                                checkPoint.Overload.Value = checkPoint.Value1 - checkPoint.NominalValue;
                                _logger.WriteEntry("* Overload Appeared By One-Value-Trigger( 3 Seconds on LINES 400KV) * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                _logger.WriteEntry("* Overload Occured in Cycle (1)", LogLevels.Info);
                                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                            }
                            break;
                        case 2:
                            if (checkPoint.Value2 >= limitOncheckPoint && checkPoint.Quality2 == OCPCheckPointQuality.Valid)
                            {
                                checkPoint.OverloadFlag = true;
                                checkPoint.ResetIT = true;
                                checkPoint.OverloadWarningFiveCycle = true;
                                checkPoint.Overload.Value = checkPoint.Value2 - checkPoint.NominalValue;
                                _logger.WriteEntry("* Overload Appeared By One-Value-Trigger( 3 Seconds on LINES 400KV) * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                _logger.WriteEntry("* Overload Occured in Cycle (2)", LogLevels.Info);
                                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                            }
                            break;
                        case 3:
                            if (checkPoint.Value3 >= limitOncheckPoint && checkPoint.Quality3 == OCPCheckPointQuality.Valid)
                            {
                                checkPoint.OverloadFlag = true;
                                checkPoint.ResetIT = true;
                                checkPoint.OverloadWarningFiveCycle = true;
                                checkPoint.Overload.Value = checkPoint.Value3 - checkPoint.NominalValue;
                                _logger.WriteEntry("* Overload Appeared By One-Value-Trigger( 3 Seconds on LINES 400KV) * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                _logger.WriteEntry("* Overload Occured in Cycle (3)", LogLevels.Info);
                                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                            }
                            break;
                        case 4:
                            if (checkPoint.Value4 >= limitOncheckPoint && checkPoint.Quality4 == OCPCheckPointQuality.Valid)
                            {
                                checkPoint.OverloadFlag = true;
                                checkPoint.ResetIT = true;
                                checkPoint.OverloadWarningFiveCycle = true;
                                checkPoint.Overload.Value = checkPoint.Value4 - checkPoint.NominalValue;
                                _logger.WriteEntry("* Overload Appeared By One-Value-Trigger( 3 Seconds on LINES 400KV) * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                _logger.WriteEntry("* Overload Occured in Cycle (4)", LogLevels.Info);
                                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                            }
                            break;
                        case 5:
                            if (checkPoint.Value5 >= limitOncheckPoint && checkPoint.Quality5 == OCPCheckPointQuality.Valid)
                            {
                                checkPoint.OverloadFlag = true;
                                checkPoint.ResetIT = true;
                                checkPoint.OverloadWarningFiveCycle = true;
                                checkPoint.Overload.Value = checkPoint.Value5 - checkPoint.NominalValue;
                                _logger.WriteEntry("* Overload Appeared By One-Value-Trigger( 3 Seconds on LINES 400KV) * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                _logger.WriteEntry("* Overload Occured in Cycle (5)", LogLevels.Info);
                                if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                            }
                            break;
                    }
                }
                else
                {
                    switch (cycleNo)
                    {
                        case 5:
                            CalcAverage(checkPoint, cycleNo);
                            if (checkPoint.AverageQuality == OCPCheckPointQuality.Valid)
                            {
                                if (checkPoint.Average.Value > limitOncheckPoint)
                                {
                                    // Set the OverloadFlag
                                    checkPoint.OverloadFlag = true;

                                    //----------------- Modification 1388/05/12, By: Akbari, Hemmaty -------------------
                                    // ResetIT is set for the next cycle to write 0 to IT on SCADA

                                    checkPoint.ResetIT = true;

                                    //----------------------------------------------------------------------------------

                                    // Calculate Overload
                                    checkPoint.Overload.Value = checkPoint.Average.Value - checkPoint.NominalValue;

                                    //  1399.11.27; Commented these lines
                                    //// Send Alarm
                                    //if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Disappear, checkPoint.NetworkPath))
                                    //    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                                    //if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath))
                                    //    _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);

                                    _logger.WriteEntry("* Overload Appeared By Five-Value-Trigger( 15 Seconds ) * -> " + checkPoint.NetworkPath, LogLevels.Trace);
                                }

                                if ((checkPoint.Value4 > limitOncheckPoint) && (checkPoint.Value5 > limitOncheckPoint))
                                {
                                    // Set the FourValueFlag
                                    checkPoint.FourValueFlag = true;

                                    //  1399.11.27; Commented these lines
                                    // Send Warning
                                    //  if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Disappear, checkPoint.NetworkPath))
                                    //     _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                                    //  if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath))
                                    //      _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);

                                    // 2020.09.09   Commented as agreed with Mr.Imanian
                                    // _logger.WriteEntry("* Four Value Flag set * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                }
                                else
                                {
                                    // Reset the FourValueFlag
                                    checkPoint.FourValueFlag = false;
                                }
                            }
                            else
                            {
                                if ((checkPoint.Value4 > limitOncheckPoint) && (checkPoint.Value5 > limitOncheckPoint))
                                {
                                    if ((checkPoint.Quality4 != OCPCheckPointQuality.Invalid) && (checkPoint.Quality5 != OCPCheckPointQuality.Invalid))
                                    {
                                        checkPoint.FourValueFlag = true;
                                        //  1399.11.27; Commented these lines
                                        // Send Warning
                                        // if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Disappear, checkPoint.NetworkPath))
                                        //     _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);
                                        // if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath))
                                        //     _logger.WriteEntry("Sending alarm failed for OverloadWarning", LogLevels.Error);

                                        // 2020.09.09   Commented as agreed with Mr.Imanian
                                        //_logger.WriteEntry("* Four Value Flag set * -> " + checkPoint.NetworkPath, LogLevels.Error);
                                    }
                                    else
                                    {
                                        checkPoint.FourValueFlag = false;
                                    }
                                }
                                else
                                {
                                    checkPoint.FourValueFlag = false;
                                }
                            }

                            //  1399.11.27; Start of Alarm and Warning processing 
                            // Checking disappear condition for OverloadWarningFourCycle
                            if ((checkPoint.Value5 <= limitOncheckPoint) && (checkPoint.OverloadWarningFourCycle))
                                checkPoint.OverloadWarningFourCycle = false;

                            if ((checkPoint.Value5 <= limitOncheckPoint) && checkPoint.OverloadAlarmFourCycle)
                                checkPoint.OverloadAlarmFourCycle = false;

                            if ((checkPoint.OverloadFlag) && (!checkPoint.OverloadAlarmFiveCycle))
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_Overloaded, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value5 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending alarm Appear failed for five vaue Overloaded Alarm", LogLevels.Error);
                                checkPoint.OverloadAlarmFiveCycle = true;
                            }

                            // Checking condition for OverloadWarningFiveCycle
                            checkPoint.OverloadWarningFiveCycle = false;

                            // Checking disappear condition for OverloadAlarmFiveCycle
                            if (!checkPoint.OverloadFlag)
                                if (checkPoint.OverloadAlarmFiveCycle)
                                    checkPoint.OverloadAlarmFiveCycle = false;
                            //  1399.11.27; End
                            break;

                        case 1:
                            if (checkPoint.FourValueFlag)
                            {
                                if ((checkPoint.Value1 <= limitOncheckPoint || checkPoint.Quality1 == OCPCheckPointQuality.Invalid) || (checkPoint.Quality1 == OCPCheckPointQuality.Previous &&
                                    (checkPoint.Quality4 == OCPCheckPointQuality.Previous ||
                                    checkPoint.Quality5 == OCPCheckPointQuality.Previous)))
                                {
                                    // It is not allowed to have two Previous qualities in a four value set.
                                    checkPoint.FourValueFlag = false;
                                }
                            }
                            //  1399.11.27; Start of Alarm and Warning processing 
                            // Checking disappear condition for OverloadWarningFourCycle
                            if ((checkPoint.Value1 <= limitOncheckPoint) && (checkPoint.OverloadWarningFourCycle))
                                checkPoint.OverloadWarningFourCycle = false;

                            // Checking disappear condition for OverloadAlarmFourCycle
                            if ((checkPoint.Value1 <= limitOncheckPoint) && checkPoint.OverloadAlarmFourCycle)
                                checkPoint.OverloadAlarmFourCycle = false;

                            // Checking appear condition for OverloadWarningFiveCycle
                            if (checkPoint.Value1 > limitOncheckPoint)
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_OverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value1 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending warning Appear failed for five value Overloaded Warning for " + checkPoint.NetworkPath, LogLevels.Error);
                                checkPoint.OverloadWarningFiveCycle = true;
                            }
                            else
                                checkPoint.OverloadWarningFiveCycle = false;
                            //  1399.11.27; End
                            break;
                        case 2:
                            if (checkPoint.FourValueFlag)
                            {
                                if (checkPoint.Value2 > limitOncheckPoint && checkPoint.Quality2 != OCPCheckPointQuality.Invalid)
                                {
                                    if (!(checkPoint.Quality2 == OCPCheckPointQuality.Previous && (checkPoint.Quality1 == OCPCheckPointQuality.Previous || checkPoint.Quality4 == OCPCheckPointQuality.Previous || checkPoint.Quality5 == OCPCheckPointQuality.Previous)))
                                    {
                                        // Overload occured

                                        CalcAverage(checkPoint, cycleNo);
                                        // Set the OverloadFlag
                                        checkPoint.OverloadFlag = true;

                                        //----------------- Modification 1388/05/12, By: Akbari, Hemmaty -------------------
                                        // ResetIT is set for the next cycle to write 0 to IT on SCADA

                                        checkPoint.ResetIT = true;

                                        //----------------------------------------------------------------------------------

                                        // Calculate Overload
                                        checkPoint.Overload.Value = checkPoint.Average.Value - checkPoint.NominalValue;

                                        //  1399.11.27; Commented these lines
                                        // Send Alarm
                                        //if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Disappear, checkPoint.NetworkPath))
                                        //    _logger.WriteEntry("Sending alarm failed for OverloadAppear", LogLevels.Error);
                                        //if (!_updateScadaPointOnServer.SendAlarm(alarmPointOverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath))
                                        //    _logger.WriteEntry("Sending alarm failed for OverloadAppear", LogLevels.Error);

                                        _logger.WriteEntry("* Overload Appeared By Four-Value-Trigger( 12 Seconds )  * -> " + checkPoint.NetworkPath, LogLevels.Info);
                                    }
                                }
                                // We are finished in Cycle2 so reset the FourValueFlag anyway
                                checkPoint.FourValueFlag = false;
                            }
                            //  1399.11.27; Start of Alarm and Warning processing 
                            // Checking disappear condition for OverloadWarningFourCycle
                            if (checkPoint.OverloadWarningFourCycle)
                                checkPoint.OverloadWarningFourCycle = false;

                            if ((checkPoint.Value2 > limitOncheckPoint) && (checkPoint.OverloadFlag) && !checkPoint.OverloadAlarmFourCycle)
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Four_Value_Overloaded, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value2 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending alarm Appear failed for four value Overloaded", LogLevels.Error);
                                checkPoint.OverloadAlarmFourCycle = true;
                            }

                            if ((checkPoint.Value2 <= limitOncheckPoint) && checkPoint.OverloadAlarmFourCycle)
                                checkPoint.OverloadAlarmFourCycle = false;

                            // Checking condition for OverloadWarningFiveCycle
                            if (((checkPoint.Value1 + checkPoint.Value2) / 2) > limitOncheckPoint)
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_OverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value2 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending warning Appear failed for five value Overloaded Warning for " + checkPoint.NetworkPath, LogLevels.Error);
                                checkPoint.OverloadWarningFiveCycle = true;
                            }
                            else
                                checkPoint.OverloadWarningFiveCycle = false;
                            //  1399.11.27; End
                            break;
                        case 3:
                            //  1399.11.27; Start of Alarm and Warning processing 
                            if ((checkPoint.Value3 <= limitOncheckPoint) && checkPoint.OverloadAlarmFourCycle)
                                checkPoint.OverloadAlarmFourCycle = false;

                            // Checking condition for OverloadWarningFiveCycle
                            if (((checkPoint.Value1 + checkPoint.Value2 + checkPoint.Value3) / 3) > limitOncheckPoint)
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_OverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value3 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending warning Appear failed for five value Overloaded warning for " + checkPoint.NetworkPath, LogLevels.Error);
                                checkPoint.OverloadWarningFiveCycle = true;
                            }
                            else
                                checkPoint.OverloadWarningFiveCycle = false;
                            //  1399.11.27; End
                            break;
                        case 4:
                            //  1399.11.27; Start of Alarm and Warning processing 
                            // Checking appear condition for OverloadWarningFourCycle
                            if ((checkPoint.Value4 > limitOncheckPoint)&&(Five_Value_OverloadWarning.Value == (float)SinglePointStatus.Disappear)) 
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Four_Value_OverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath + " ; DeltaOverLoadValue = " + (checkPoint.Value4 - limitOncheckPoint)))
                                    _logger.WriteEntry("Sending alarm Appear failed for four value Overload Alarm for " + checkPoint.NetworkPath, LogLevels.Error);
                                checkPoint.OverloadWarningFourCycle = true;
                            }

                            if ((checkPoint.Value4 <= limitOncheckPoint) && checkPoint.OverloadAlarmFourCycle)
                                checkPoint.OverloadAlarmFourCycle = false;
                            // Checking condition for OverloadWarningFiveCycle
                            if (((checkPoint.Value1 + checkPoint.Value2 + checkPoint.Value3 + checkPoint.Value4) / 4) > limitOncheckPoint)
                            {
                                if (!_updateScadaPointOnServer.SendAlarm(Five_Value_OverloadWarning, SinglePointStatus.Appear, checkPoint.NetworkPath))
                                    _logger.WriteEntry("Sending warning Appear  failed for five value Overloaded Warning for " + checkPoint.NetworkPath, LogLevels.Error);
                                checkPoint.OverloadWarningFiveCycle = true;
                            }
                            else
                                checkPoint.OverloadWarningFiveCycle = false;
                            //  1399.11.27; End
                            break;
                    }
                }

                if (checkPoint.OverloadFlag)
                    isOverload = true;

                //"IMANIAN ; 1396-11-09 ; CHECK 400KV LINES IN EACH CYCLES
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                return false;
            }
        }

        // Calculates the average and its quality
        private void CalcAverage(OCPCheckPoint checkPoint, int cycleNo)
        {
            var average = 0.0f;
            var quality = OCPCheckPointQuality.Invalid;

            if (cycleNo == 5)
            {
                average = (checkPoint.Value1 + checkPoint.Value2 + checkPoint.Value3 + checkPoint.Value4 + checkPoint.Value5) / 5;

                if ((checkPoint.Quality1 == OCPCheckPointQuality.Invalid) || (checkPoint.Quality2 == OCPCheckPointQuality.Invalid) ||
                    (checkPoint.Quality3 == OCPCheckPointQuality.Invalid) || (checkPoint.Quality4 == OCPCheckPointQuality.Invalid) ||
                    (checkPoint.Quality5 == OCPCheckPointQuality.Invalid))
                    quality = OCPCheckPointQuality.Invalid;
                else
                    quality = OCPCheckPointQuality.Valid;

                checkPoint.Average.Value = average;
                checkPoint.AverageQuality = quality;
            }
            else if (cycleNo == 2)
            {
                average = (checkPoint.Value1 + checkPoint.Value2 + checkPoint.Value4 + checkPoint.Value5) / 4;

                if ((checkPoint.Quality1 == OCPCheckPointQuality.Invalid) || (checkPoint.Quality2 == OCPCheckPointQuality.Invalid) ||
                    (checkPoint.Quality4 == OCPCheckPointQuality.Invalid) || (checkPoint.Quality5 == OCPCheckPointQuality.Invalid))
                    quality = OCPCheckPointQuality.Invalid;
                else
                    quality = OCPCheckPointQuality.Valid;

                checkPoint.Average.Value = average;
                checkPoint.AverageQuality = quality;
            }

        }
        public bool OCP_Function_Status()
        {
            try
            {
                var FStatus = _repository.GetOCPScadaPoint("OCPStatus");
                if (FStatus.Value == 1.0)
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                return false;
            }


        }
    }
}
