using System;
using System.Collections.Generic;

using Irisa.Logger;

namespace OCP
{
    class OCPOverloadPreparation
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly IEnumerable<OCPCheckPoint> _checkPoints;
        UpdateScadaPointOnServer _updateScadaPointOnServer;
        OCPCycleValidator _cycleValidator;

        public OCPOverloadPreparation(ILogger logger, IRepository repository, IEnumerable<OCPCheckPoint> checkPoints,
            UpdateScadaPointOnServer aupdateScadaPointOnServer, OCPCycleValidator cycleValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _checkPoints = checkPoints ?? throw new ArgumentNullException(nameof(checkPoints));
            _updateScadaPointOnServer = aupdateScadaPointOnServer ?? throw new ArgumentNullException(nameof(aupdateScadaPointOnServer));
            _cycleValidator = cycleValidator ?? throw new ArgumentNullException(nameof(cycleValidator));
        }

        public bool PrepareOverloadData(int CycleNo, OCPOverloadCheck overloadCheck)
        {
            try
            {
                //'KAJI CycVal
                bool bOnetimeReset = true;

                bool LSPTrigger = false;

                //TODO: TEMPO CHECK: // 
                ResetOVERLCOND();

                foreach (var checkPoint in _checkPoints)
                {
                    // TODO : Check these point addresses!!
                    if ((checkPoint.Name == "CP59_NIS_L914") ||
                        (checkPoint.Name == "CP60_NIS_L915"))
                    {
                        if (checkPoint.OverloadFlag)
                        {
                            //'KAJI CycVal
                            if (bOnetimeReset)
                            {
                                //TODO: TEMPO CHECK: // 
                                overloadCheck.ResetIT(eResetType.ResetAll);
                                bOnetimeReset = false;
                                //_logger.WriteEntry("Trace Reset IT ", LogLevels.Info);
                            }
                            //'KAJI CycVal

                            // We have to trigger the LSP after preparing all the data
                            LSPTrigger = true;

                            _logger.WriteEntry("----- Prepare Overload Data for " + checkPoint.NetworkPath + " -----", LogLevels.Trace);
                            // Send to LSP (Write to SCADA)

                            //' 1394.04.10;   Ali.A.Kaji;  New lines, START
                            if (!_updateScadaPointOnServer.WriteIT(checkPoint, checkPoint.Overload.Value))
                            {
                                _logger.WriteEntry($"Unable to Write data to SCADA! - " + "Overload", LogLevels.Error);
                            }

                            if (!_updateScadaPointOnServer.WriteAAP(checkPoint, checkPoint.ActivePower.Value))
                            {
                                _logger.WriteEntry("Unable to Write Data for OCPCHECKPOINT into SCADA! - " + checkPoint.Name + " ALLOWEDACTIVEPOWER=" + checkPoint.Overload.Value, LogLevels.Error);
                            }

                            //' 1394.04.10;   Ali.A.Kaji;  New lines, END

                            // TOCHECK : Send Alarm
                            //if (!m_theCSCADADataInterface.SendAlarm("Network/Model Functions/OCP/MESSAGE/", overloadCheck.NetworkPath, overloadCheck.Overload))
                           
                            if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("OverloadAppear"), SinglePointStatus.Appear, checkPoint.NetworkPath + " ; " + checkPoint.Overload.Value ))
                                _logger.WriteEntry("Sending alarm failed for OverloadAppear", LogLevels.Error);

                            //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES
                            _cycleValidator.LogCycleVal(checkPoint);
                            //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES

                            _logger.WriteEntry("Average  = " + checkPoint.Average.Value.ToString(), LogLevels.Info);
                            _logger.WriteEntry("IT       = " + checkPoint.Overload.Value.ToString(), LogLevels.Info);
                            _logger.WriteEntry("AllowedActivePower  = " + checkPoint.ActivePower.Value.ToString(), LogLevels.Info);

                            // For the other points
                            checkPoint.OverloadFlag = false;
                        } // end of if : (arrShedPoint(i).OverloadFlag)
                    }
                    else
                    {
                        //Else of if : (arrShedPoint(i).NetworkPath ...)
                        if (CycleNo == 5 || CycleNo == 2)
                        {
                            if (checkPoint.OverloadFlag)
                            {
                                //'KAJI CycVal
                                if (bOnetimeReset)
                                {
                                    //TODO: TEMPO CHECK: // 
                                    overloadCheck.ResetIT(eResetType.ResetAll);
                                    bOnetimeReset = false;
                                    //_logger.WriteEntry("Trace Reset IT ", LogLevels.Info);
                                }
                                //'KAJI CycVal

                                // We have to trigger the LSP after preparing all the data
                                LSPTrigger = true;

                                _logger.WriteEntry($"----- Prepare Overload Data for " + checkPoint.NetworkPath + " -----", LogLevels.Info);

                                // Check if point is a NIS transformer
                                if (checkPoint.Category == "PRIMARY" || checkPoint.Category == "SECONDARY")
                                {
                                    // TODO: check: in C#, skip for MZ3
                                    if(checkPoint.Name != "CP40_MIS_T3AN-MZ3")
                                    if (!PrepareTransOverloadData(checkPoint))
                                    {
                                        _logger.WriteEntry($"Transformer " + checkPoint.Name + " in overload, but checking not successful!", LogLevels.Error);
                                    }
                                }

                                // Send to LSP (Write to SCADA)

                                //' 1394.04.10;   Ali.A.Kaji;  New lines, START
                                //if (!_updateScadaPointOnServer.WriteAverage(checkPoint, checkPoint.Average.Value))
                                //{
                                //    _logger.WriteEntry($"Unable to Write data to SCADA! - " + "Average", LogLevels.Error);
                                //}

                                if (!_updateScadaPointOnServer.WriteIT(checkPoint, checkPoint.Overload.Value))
                                {
                                    _logger.WriteEntry($"Unable to Write data to SCADA! - " + "Overload", LogLevels.Error);
                                }

                                if (!_updateScadaPointOnServer.WriteAAP(checkPoint, checkPoint.ActivePower.Value))
                                {
                                    _logger.WriteEntry($"Unable to Write data to SCADA! - " + "ALLOWEDACTIVEPOWER", LogLevels.Error);
                                }
                                //' 1394.04.10;   Ali.A.Kaji;  New lines, END

                                // Send Alarm
                               
                                if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("OverloadAppear"), SinglePointStatus.Appear, checkPoint.NetworkPath + " ; " + checkPoint.Overload.Value))
                                    _logger.WriteEntry("Sending alarm failed for OverloadAppear", LogLevels.Error);

                                //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES
                                _cycleValidator.LogCycleVal(checkPoint);
                                //' 1396.10.24 IMANIAN ; ADD LOGGS OF VALUES OF EACH CYCLES

                                _logger.WriteEntry($"Average  = " + checkPoint.Average.Value.ToString(), LogLevels.Info);
                                _logger.WriteEntry($"IT       = " + checkPoint.Overload.Value.ToString(), LogLevels.Info);
                                _logger.WriteEntry($"AllowedActivePower  = " + checkPoint.ActivePower.Value.ToString(), LogLevels.Info);

                                // Reset the values
                                // For NIS transformers reset both the primary and the secondary sides.
                                string strTagName2 = "", strTagName1 = "";
                                OCPCheckPoint jcheckpoint;
                                // TODO : check for "CP40_MIS_T3AN_MZ3"
                                if ((checkPoint.Category == "PRIMARY" || checkPoint.Category == "SECONDARY")
                                     && checkPoint.Name != "CP40_MIS_T3AN-MZ3")
                                {
                                    if (checkPoint.Category == "PRIMARY")
                                    {
                                        switch (_repository.GetCheckPoint(checkPoint.Name).Name)
                                        {
                                            case "CP31_NIS_T1AN":  // T1AN 
                                                strTagName2 = "CP21_MIS_T1AN";
                                                break;
                                            case "CP32_NIS_T2AN":  // T2AN 
                                                strTagName2 = "CP22_MIS_T2AN";
                                                break;
                                            case "CP33_NIS_T3AN":  // T3AN 
                                                strTagName2 = "CP23_MIS_T3AN_MV3";
                                                break;
                                            case "CP34_NIS_T4AN":  // T4AN 
                                                strTagName2 = "CP29_MIS_T4AN";
                                                break;
                                            case "CP41_NIS_T6AN":  // T6AN 
                                                strTagName2 = "CP43_MIS_T6AN";
                                                break;
                                            case "CP35_NIS_T5AN":  // T5AN 
                                                strTagName2 = "CP36_MIS_T5AN";
                                                break;
                                            case "CP42_NIS_T7AN":  // T7AN 
                                                strTagName2 = "CP44_MIS_T7AN";
                                                //' KAJI START of T8AN 
                                                break;
                                            case "CP56_NIS_T8AN":  // T8AN 
                                                strTagName2 = "CP57_MIS_T8AN";
                                                //' KAJI END of T8AN 
                                                break;
                                        }

                                        jcheckpoint = _repository.GetCheckPoint(strTagName2);
                                    }
                                    else
                                    {
                                        switch (_repository.GetCheckPoint(checkPoint.Name).Name)
                                        {
                                            case "CP21_MIS_T1AN":  // T1AN 
                                                strTagName1 = "CP31_NIS_T1AN";
                                                break;
                                            case "CP22_MIS_T2AN":  // T2AN 
                                                strTagName1 = "CP32_NIS_T2AN";
                                                break;
                                            case "CP23_MIS_T3AN_MV3":  // T3AN 
                                                strTagName1 = "CP33_NIS_T3AN";
                                                break;
                                            case "CP29_MIS_T4AN":  // T4AN 
                                                strTagName1 = "CP34_NIS_T4AN";
                                                break;
                                            case "CP43_MIS_T6AN":  // T6AN 
                                                strTagName1 = "CP41_NIS_T6AN";
                                                break;
                                            case "CP36_MIS_T5AN":  // T5AN 
                                                strTagName1 = "CP35_NIS_T5AN";
                                                break;
                                            case "CP44_MIS_T7AN":  // T7AN 
                                                strTagName1 = "CP42_NIS_T7AN";
                                                //' KAJI START of T8AN 
                                                break;
                                            case "CP57_MIS_T8AN":  // T8AN 
                                                strTagName1 = "CP56_NIS_T8AN";
                                                //' KAJI END of T8AN 
                                                break;
                                        }

                                        jcheckpoint = _repository.GetCheckPoint(strTagName1);
                                    } // enf of if :arrShedPoint(i).Category = "PRIMARY"

                                    // TODO: Check this part, I added it in C# version.
                                    if (checkPoint.primeSideBigTans is null)
                                    {
                                        _logger.WriteEntry("Error in primary side of Trans " + checkPoint.NetworkPath, LogLevels.Error);
                                        return false;
                                    }

                                    // For the other points
                                    checkPoint.primeSideBigTans.OverloadFlag = false;
                                    jcheckpoint.SecondSideBigTans.OverloadFlag = false;
                                }
                                checkPoint.OverloadFlag = false;
                            }
                        }
                        // End of if : CycleNo = 5 Or CycleNo = 2
                    }
                }

                // Trigger the LSP Function
                if (LSPTrigger)
                {
                    //_logger.WriteEntry("OCP in OVERLOAD, try to write into table ... ", LogLevels.Info);

                    // TODO : check
                    var overlcond = _repository.GetOCPScadaPoint("OVERLCOND");
                    //if (!_updateScadaPointOnServer.SendOverloadToLSP(overlcond, SinglePointStatus.Appear))
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("OVERLCOND"), SinglePointStatus.Appear,
                            "OCP is in OVERLOAD, trying to trigger the LSP ... "))
                    {
                            LSPTrigger = false;

                        // Send Alarm
                        if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Disappear,
                            "OCP is in OVERLOAD, but could not trigger the LSP."))
                            _logger.WriteEntry("Sending alarm failed for Functionality", LogLevels.Error);
                        if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Appear,
                            "OCP is in OVERLOAD, but could not trigger the LSP."))
                            _logger.WriteEntry("Sending alarm failed for Functionality", LogLevels.Error);

                        _logger.WriteEntry("OCP in OVERLOAD, but could not trigger the LSP.", LogLevels.Error);
                    }
                }
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
                return false;
            }
        }

        public void FillBigTransSides()
        {
            string strNameSide;
            try
            {
                foreach (var aCheckpoint in _checkPoints)
                {
                    // Reset the values
                    // For NIS transformers reset both the primary and the secondary sides.
                    if (aCheckpoint.Category == "PRIMARY" || aCheckpoint.Category == "SECONDARY")
                    {
                        strNameSide = "";

                        if (aCheckpoint.Category == "PRIMARY")
                        {
                            switch (aCheckpoint.Name)
                            {
                                case "CP31_NIS_T1AN":  // T1AN 
                                    strNameSide = "CP21_MIS_T1AN";
                                    break;
                                case "CP32_NIS_T2AN":  // T2AN 
                                    strNameSide = "CP22_MIS_T2AN";
                                    break;
                                case "CP33_NIS_T3AN":  // T3AN 
                                    strNameSide = "CP23_MIS_T3AN_MV3";
                                    break;
                                case "CP34_NIS_T4AN":  // T4AN 
                                    strNameSide = "CP29_MIS_T4AN";
                                    break;
                                case "CP41_NIS_T6AN":  // T6AN 
                                    strNameSide = "CP43_MIS_T6AN";
                                    break;
                                case "CP35_NIS_T5AN":  // T5AN 
                                    strNameSide = "CP36_MIS_T5AN";
                                    break;
                                case "CP42_NIS_T7AN":  // T7AN 
                                    strNameSide = "CP44_MIS_T7AN";
                                    break;
                                //'' KAJI START of T8AN
                                case "CP56_NIS_T8AN":  // T8AN 
                                    strNameSide = "CP57_MIS_T8AN";
                                    break;
                                 //'' KAJI END of T8AN
                            }

                            aCheckpoint.primeSideBigTans = aCheckpoint;
                            aCheckpoint.SecondSideBigTans = _repository.GetCheckPoint(strNameSide);
                        }
                        else
                        {
                            switch (aCheckpoint.Name)
                            {
                                case "CP21_MIS_T1AN":  // T1AN 
                                    strNameSide = "CP31_NIS_T1AN";
                                    break;
                                case "CP22_MIS_T2AN":  // T2AN 
                                    strNameSide = "CP32_NIS_T2AN";
                                    break;
                                case "CP23_MIS_T3AN_MV3":  // T3AN 
                                    strNameSide = "CP33_NIS_T3AN";
                                    break;
                                case "CP29_MIS_T4AN":  // T4AN 
                                    strNameSide = "CP34_NIS_T4AN";
                                    break;
                                case "CP43_MIS_T6AN":  // T6AN 
                                    strNameSide = "CP41_NIS_T6AN";
                                    break;
                                case "CP36_MIS_T5AN":  // T5AN 
                                    strNameSide = "CP35_NIS_T5AN";
                                    break;
                                case "CP44_MIS_T7AN":  // T7AN 
                                    strNameSide = "CP42_NIS_T7AN";
                                    break;
                                //'' KAJI START of T8AN
                                case "CP57_MIS_T8AN":  // T8AN 
                                    strNameSide = "CP56_NIS_T8AN";
                                    break;
                                //'' KAJI END of T8AN
                            }
                            aCheckpoint.primeSideBigTans = _repository.GetCheckPoint(strNameSide);
                            aCheckpoint.SecondSideBigTans = aCheckpoint;
                        }
                    }
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
                return;
            }
            return;
        }

        private bool PrepareTransOverloadData(OCPCheckPoint a_checkPoint)
        {
            float activePower;
            float activePower1;
            float activePower2;
            string voltageSide1;
            string voltageSide2;
            TransSideOverload transOverloadCondition;
            OCPCheckPointQuality activePowerQuality;
            OCPCheckPointQuality activePowerQuality1;
            OCPCheckPointQuality activePowerQuality2;

            try
            {
                // TODO : We have to check, side1 voltage from which busbar is coming, 
                //          TAN_PRIMVOLT_A or TAN_PRIMVOLT_C
                // It comes from RPC!

                voltageSide1 = String.Empty;
                voltageSide2 = String.Empty;

                // Check for this transformer if the other side is in overload.
                if (a_checkPoint.Category == "PRIMARY" || a_checkPoint.Category == "SECONDARY")
                {
                    switch (a_checkPoint.Name)
                    {
                        case "CP31_NIS_T1AN":  // T1AN 
                            voltageSide1 = "T1AN_PRIMEVOLT";
                            voltageSide2 = "T1AN_SECVOLT";
                            break;
                        case "CP32_NIS_T2AN":  // T2AN 
                            voltageSide1 = "T2AN_PRIMEVOLT";
                            voltageSide2 = "T2AN_SECVOLT";
                            break;
                        case "CP33_NIS_T3AN":  // T3AN 
                            voltageSide1 = "T3AN_PRIMEVOLT";
                            voltageSide2 = "T3AN_SECVOLT";
                            break;
                        case "CP34_NIS_T4AN":  // T4AN 
                            voltageSide1 = "T4AN_PRIMEVOLT";
                            voltageSide2 = "T4AN_SECVOLT";
                            break;
                        case "CP41_NIS_T6AN":  // T6AN 
                            voltageSide1 = "T6AN_PRIMEVOLT";
                            voltageSide2 = "T6AN_SECVOLT";
                            break;
                        case "CP35_NIS_T5AN":  // T5AN 
                            voltageSide1 = "T5AN_PRIMEVOLT";
                            voltageSide2 = "T5AN_SECVOLT";
                            break;
                        case "CP42_NIS_T7AN":  // T7AN 
                            voltageSide1 = "T7AN_PRIMEVOLT";
                            voltageSide2 = "T7AN_SECVOLT";
                            //' KAJI START of T8AN 
                            break;
                        case "CP56_NIS_T8AN":  // T8AN 
                            voltageSide1 = "T8AN_PRIMEVOLT";
                            voltageSide2 = "T8AN_SECVOLT";
                            //' KAJI END of T8AN 
                            break;

                        case "CP21_MIS_T1AN":  // T1AN 
                            voltageSide1 = "T1AN_PRIMEVOLT";
                            voltageSide2 = "T1AN_SECVOLT";
                            break;
                        case "CP22_MIS_T2AN":  // T2AN 
                            voltageSide1 = "T2AN_PRIMEVOLT";
                            voltageSide2 = "T2AN_SECVOLT";
                            break;
                        case "CP23_MIS_T3AN_MV3":  // T3AN 
                            voltageSide1 = "T3AN_PRIMEVOLT";
                            voltageSide2 = "T3AN_SECVOLT";
                            break;
                        case "CP29_MIS_T4AN":  // T4AN 
                            voltageSide1 = "T4AN_PRIMEVOLT";
                            voltageSide2 = "T4AN_SECVOLT";
                            break;
                        case "CP43_MIS_T6AN":  // T6AN 
                            voltageSide1 = "T6AN_PRIMEVOLT";
                            voltageSide2 = "T6AN_SECVOLT";
                            break;
                        case "CP36_MIS_T5AN":  // T5AN 
                            voltageSide1 = "T5AN_PRIMEVOLT";
                            voltageSide2 = "T5AN_SECVOLT";
                            break;
                        case "CP44_MIS_T7AN":  // T7AN 
                            voltageSide1 = "T7AN_PRIMEVOLT";
                            voltageSide2 = "T7AN_SECVOLT";
                            break;
                        //' KAJI START of T8AN 
                        case "CP57_MIS_T8AN":  // T8AN 
                            voltageSide1 = "T8AN_PRIMEVOLT";
                            voltageSide2 = "T8AN_SECVOLT";
                            break;
                        //' KAJI END of T8AN 
                    }
                }

                // TODO: Check this part, I added it in C# version.
                if( String.IsNullOrEmpty( voltageSide1 ) || 
                    String.IsNullOrEmpty( voltageSide2) )
                {
                    _logger.WriteEntry("Error in name of Big-Trans, the name is not in defined list, Name: " + a_checkPoint.Name, LogLevels.Error);
                    return false;
                }

                if (a_checkPoint.primeSideBigTans is null)
                {
                    _logger.WriteEntry("Error in primary side of Trans " + a_checkPoint.NetworkPath, LogLevels.Error);
                    return false;
                }

                // TODO:
                var actualVoltagePointPrimSideAllBigTransesA = _repository.GetOCPScadaPoint("TAN_PRIMVOLT_A");
                var actualVoltagePointPrimSideAllBigTransesC = _repository.GetOCPScadaPoint("TAN_PRIMVOLT_C");
              

                // Check if overload is on both side of the transformer or only on one side
                if (a_checkPoint.primeSideBigTans.OverloadFlag)
                {
                    // Overload on primary side
                    transOverloadCondition = TransSideOverload.Primary;
                    // TODO: Check with Mr. Imanian
                    //var actualVoltagePoint = _repository.GetOCPScadaPoint(voltageSide1);
                    var actualVoltagePoint = _repository.GetOCPScadaPoint("TAN_PRIMVOLT_A");

                    var voltageQuality1 = actualVoltagePoint.Quality;
                    activePower1 = Convert.ToSingle(Math.Sqrt(3) * a_checkPoint.primeSideBigTans.NominalValue * actualVoltagePoint.Value);

                    _logger.WriteEntry($"ActualVoltage1 = { actualVoltagePoint.Value}", LogLevels.Info);
                    _logger.WriteEntry($"Active Power1 = {activePower1}, NominalValue= {a_checkPoint.NominalValue}", LogLevels.Info);

                    if (voltageQuality1 == OCPCheckPointQuality.Valid)
                        activePowerQuality1 = OCPCheckPointQuality.Valid;
                    else
                        activePowerQuality1 = OCPCheckPointQuality.Invalid;

                    // Overload is also on secondary side
                    if (a_checkPoint.SecondSideBigTans.Overload.Value != 0)
                    {
                        transOverloadCondition = TransSideOverload.Both;
                        var actualVoltage2 = _repository.GetOCPScadaPoint(voltageSide2);
                        if (actualVoltage2 == null)
                        {
                            _logger.WriteEntry($"Error in GetOCPScadaPoint = { voltageSide2}", LogLevels.Error);
                            return false;
                        }
                        var voltageQuality2 = actualVoltage2.Quality;
                        activePower2 = Convert.ToSingle(Math.Sqrt(3) * a_checkPoint.SecondSideBigTans.NominalValue * actualVoltage2.Value);

                        if (voltageQuality2 == OCPCheckPointQuality.Valid)
                            activePowerQuality2 = OCPCheckPointQuality.Valid;
                        else
                            activePowerQuality2 = OCPCheckPointQuality.Invalid;
                    }
                    else
                    {
                        // Overload is only on primary side
                        activePower2 = 0;
                        activePowerQuality2 = OCPCheckPointQuality.Invalid;
                    }
                }
                else
                {
                    // Overload is only on secondary side
                    transOverloadCondition = TransSideOverload.Secondary;
                    var actualVoltage2Point = _repository.GetOCPScadaPoint(voltageSide2);
                    var voltageQuality2 = actualVoltage2Point.Quality;
                    activePower2 = Convert.ToSingle(Math.Sqrt(3) * a_checkPoint.NominalValue * actualVoltage2Point.Value);

                    _logger.WriteEntry($"ActualVoltage2 = { actualVoltage2Point.Value}", LogLevels.Info);
                    _logger.WriteEntry($"Active Power2= {activePower2}, NominalValue= {a_checkPoint.NominalValue}", LogLevels.Info);

                    if (voltageQuality2 == OCPCheckPointQuality.Valid)
                        activePowerQuality2 = OCPCheckPointQuality.Valid;
                    else
                        activePowerQuality2 = OCPCheckPointQuality.Invalid;

                    activePower1 = 0;
                    activePowerQuality1 = OCPCheckPointQuality.Invalid;
                }

                // Check the qualities and,
                // 1- select lowest allowed active power
                // 2- select highest overload

                // 1- select lowest allowed active power
                if (activePowerQuality1 == OCPCheckPointQuality.Valid &&
                    activePowerQuality2 == OCPCheckPointQuality.Valid)
                {
                    if (activePower1 <= activePower2)
                        activePower = activePower1;
                    else
                        activePower = activePower2;

                    activePowerQuality = OCPCheckPointQuality.Valid;
                }
                else
                {
                    if (activePowerQuality1 == OCPCheckPointQuality.Valid)
                    {
                        activePower = activePower1;
                        activePowerQuality = OCPCheckPointQuality.Valid;
                    }
                    else
                    {
                        if (activePowerQuality2 == OCPCheckPointQuality.Valid)
                        {
                            activePower = activePower2;
                            activePowerQuality = OCPCheckPointQuality.Valid;
                        }
                        else
                        {
                            activePower = 0;
                            activePowerQuality = OCPCheckPointQuality.Invalid;
                        }
                    }
                }

                a_checkPoint.ActivePower.Value = activePower;
                a_checkPoint.ActivePowerQuality = activePowerQuality;
                _logger.WriteEntry($"Primary ActivePower= {activePower1.ToString()}, Scondary ActivePower= {activePower2.ToString()}", LogLevels.Info);

                // 2- select highest overload
                _logger.WriteEntry($"Primary Overload= {a_checkPoint.Overload.Value.ToString()}, Secondary Overload= {a_checkPoint.Overload.Value.ToString()}", LogLevels.Info);

                switch (transOverloadCondition)
                {
                    case TransSideOverload.Both:
                        var overload1 = a_checkPoint.SecondSideBigTans.Overload.Value * (63 / 400);

                        // Select and send the maximum overload between Primary and Secondary to LSP 
                        if (overload1 > a_checkPoint.primeSideBigTans.Overload.Value)
                        {
                            // Not sure about this formula
                            a_checkPoint.primeSideBigTans.Overload.Value = overload1;
                        }
                        else
                        {
                            var overload2 = a_checkPoint.primeSideBigTans.Overload.Value * (400 / 63);
                            a_checkPoint.SecondSideBigTans.Overload.Value = overload2;
                        }
                        break;

                    case TransSideOverload.Primary:
                    case TransSideOverload.Secondary:
                        // Do nothing 
                        break;
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
                return false;
            }
            return true;
        }

        public bool ResetOVERLCOND()
        {
            // TODO: check
            //_logger.WriteEntry("Trace ResetOVERLCOND", LogLevels.Info);
            try
            {
                var overlcond = _repository.GetOCPScadaPoint("OVERLCOND");
                if (!_updateScadaPointOnServer.SendOverloadToLSP(overlcond, SinglePointStatus.Disappear))
                {
                    // Send Alarm
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Disappear, "Sending disappear for OVERLCOND is failed."))
                        _logger.WriteEntry("Sending appear alarm failed for Functionality", LogLevels.Error);
                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetOCPScadaPoint("Functionality"), SinglePointStatus.Appear, "Sending disappear for OVERLCOND is failed."))
                        _logger.WriteEntry("Sending appear alarm failed for Functionality", LogLevels.Error);

                    _logger.WriteEntry("Sending disappear for OVERLCOND is failed.", LogLevels.Error);
                }
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
                return false;
            }
        }
    }
}
