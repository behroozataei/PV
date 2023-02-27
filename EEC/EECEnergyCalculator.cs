//'*************************************************************************************
//' @author   Ali.A.Kaji, Hesam Akbari
//' @version  1.0
//'
//' Development Environment       : MS-Visual Basic 6.0
//' Name of the Application       : EEC_Service_App.vbp
//' Creation/Modification History :
//'
//' Ali.A.Kaji, Hesam Akbari       23-May-2007       Created
//'
//' Overview of Application       :
//'
//'
//'***************************************************************************************
using Irisa.Logger;
using Irisa.Message.CPS;
using System;

namespace EEC
{
    internal sealed class EECEnergyCalculator : IProcessing
    {
        private const float CPS_ZERO_VALUE = 0.00001f;
        private const int ZFAC = 4;  // Z-Factor is used in to convert 1-Hour Power to 15-Minute Energy

        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private bool isCompleted = true;

        private EECScadaPoint _PMAX15;      // Maximum Power in 15 Minute
        private EECScadaPoint _PMAXG;       // Available Max Power/Period
        private EECScadaPoint _EC;          // Contractual Energy
        private EECScadaPoint _PMAX1;       // Maximum Power for EAFs Group 1
        private EECScadaPoint _PMAX2;       // Maximum Power for EAFs Group 2
        private EECScadaPoint _K1;          // Safety Factor: PBNEW-PBMAXOLD
        private EECScadaPoint _PL;          // Limit Power
        private EECScadaPoint _PBMax;       // Maximum Basic Plant Power
        private EECScadaPoint _DeltaP;
        private EECScadaPoint _EC_User;          // Contractual Energy
        private EECScadaPoint _PMAX1_User;       // Maximum Power for EAFs Group 1
        private EECScadaPoint _PMAX2_User;       // Maximum Power for EAFs Group 2
        private EECScadaPoint _K1_User;          // Safety Factor: PBNEW-PBMAXOLD
        private EECScadaPoint _PL_User;          // Limit Power
        private EECScadaPoint _PBMax_User;       // Maximum Basic Plant Power
        private EECScadaPoint _DeltaP_User;
        private EECScadaPoint _EBSum;
        private EECScadaPoint _EFSum;
        private EECScadaPoint _EnergyResEnd;
        private EECScadaPoint _PB;
        private EECScadaPoint _PC;
        private EECScadaPoint _RESTIME;
        private EECScadaPoint _EBMAX;
        private EECScadaPoint _EAV_Sum;
        private EECScadaPoint _ER_Cycle;

        private EECScadaPoint _Ea_EAF_T1AN;
        private EECScadaPoint _Ea_EAF_T2AN;
        private EECScadaPoint _Ea_EAF_T3AN_MV3;
        private EECScadaPoint _Ea_EAF_T5AN;
        private EECScadaPoint _Ea_EAF_T7AN;
        private EECScadaPoint _Ea_EAF_T8AN;

        private EECScadaPoint _CSM;
        private EECScadaPoint _HSM;
        private EECScadaPoint _PEL;
        private EECScadaPoint _RED;
        private EECScadaPoint _GEN;
        private EECScadaPoint _EFS;
        private EECScadaPoint _MIS1_6_6;
        private EECScadaPoint _MIS2_WRS;
        private EECScadaPoint _LAD;
        private EECScadaPoint _NEW_OXY;
        private EECScadaPoint _MK1;
        private EECScadaPoint _EMIS2LF1;
        private EECScadaPoint _EMIS2LF2;
        private EECScadaPoint _EMIS2LF3;
        private EECScadaPoint _EMIS2LF4;
        private EECScadaPoint _EG1;
        private EECScadaPoint _EG2;
        private EECScadaPoint _EG3;
        private EECScadaPoint _EG4;
        private EECScadaPoint _MAB;
        private EECScadaPoint _MAB_EEC;
        private EECScadaPoint _OVERL1;
        private EECScadaPoint _OVERL2;
        private EECScadaPoint _ECONEAF_EREAF;
        private EECScadaPoint _EPURCH_EC;
        private EECScadaPoint _EREAF_ECONEAF;
        private EECScadaPoint _APPLY;
        private EECScadaPoint _E_CSM;
        private EECScadaPoint _E_HSM;
        private EECScadaPoint _E_GEN;
        private EECScadaPoint _E_EFS;
        private EECScadaPoint _E_MIS1_6_6;
        private EECScadaPoint _E_MIS2_WRS;
        private EECScadaPoint _E_LAD;
        private EECScadaPoint _E_NEW_OXY;
        private EECScadaPoint _E_MK1;
        private EECScadaPoint _E_GEN1;
        private EECScadaPoint _E_GEN2;
        private EECScadaPoint _E_GEN3;
        private EECScadaPoint _E_GEN4;
        private EECScadaPoint _E_EXP914;
        private EECScadaPoint _E_IMP914;
        private EECScadaPoint _E_EXP915;
        private EECScadaPoint _E_IMP915;
        private EECScadaPoint _E_TOTAL;
        private EECScadaPoint _ALARM_ENERGY;
        private EECScadaPoint _PSend1;
        private EECScadaPoint _PSend2;

        private bool _FirstRead_MAB;
        private bool _FirstRead_MAB_EEC;

        internal EECEnergyCalculator(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));

            //InitialValues();
        }

        public void InitialValues()
        {
            _DeltaP = _repository.GetScadaPoint("DELTAP");
            _EC = _repository.GetScadaPoint("ECONTRACT");
            _K1 = _repository.GetScadaPoint("K1");
            _PL = _repository.GetScadaPoint("PLIMIT");
            _PBMax = _repository.GetScadaPoint("PBMAX");
            _PMAX15 = _repository.GetScadaPoint("PMAX15");
            _PMAX1 = _repository.GetScadaPoint("PMAX1");
            _PMAX2 = _repository.GetScadaPoint("PMAX2");

            _DeltaP_User = _repository.GetScadaPoint("DELTAP_User");
            _EC_User = _repository.GetScadaPoint("ECONTRACT_User");
            _K1_User = _repository.GetScadaPoint("K1_User");
            _PL_User = _repository.GetScadaPoint("PLIMIT_User");
            _PBMax_User = _repository.GetScadaPoint("PBMAX_User");
            _PMAX1_User = _repository.GetScadaPoint("PMAX1_User");
            _PMAX2_User = _repository.GetScadaPoint("PMAX2_User");

            _EFSum = _repository.GetScadaPoint("EFSUM");
            _PMAXG = _repository.GetScadaPoint("PMAX");
            _EBSum = _repository.GetScadaPoint("EBSum");
            _EnergyResEnd = _repository.GetScadaPoint("EnergyResEnd");

            _PB = _repository.GetScadaPoint("PB");
            _PC = _repository.GetScadaPoint("PC");
            _RESTIME = _repository.GetScadaPoint("RESTIME");
            _EBMAX = _repository.GetScadaPoint("EBMAX");
            _EAV_Sum = _repository.GetScadaPoint("EAV");
            _ER_Cycle = _repository.GetScadaPoint("ER");
            _Ea_EAF_T1AN = _repository.GetScadaPoint("Ea_EAF_T1AN");
            _Ea_EAF_T2AN = _repository.GetScadaPoint("Ea_EAF_T2AN");
            _Ea_EAF_T3AN_MV3 = _repository.GetScadaPoint("Ea_EAF_T3AN_MV3");
            _Ea_EAF_T5AN = _repository.GetScadaPoint("Ea_EAF_T5AN");
            _Ea_EAF_T7AN = _repository.GetScadaPoint("Ea_EAF_T7AN");
            _Ea_EAF_T8AN = _repository.GetScadaPoint("Ea_EAF_T8AN");
            _CSM = _repository.GetScadaPoint("ECSM");
            _HSM = _repository.GetScadaPoint("EHSM");
            _PEL = _repository.GetScadaPoint("EPEL");
            _RED = _repository.GetScadaPoint("ERED");
            _GEN = _repository.GetScadaPoint("EGEN");
            _EFS = _repository.GetScadaPoint("EEFS");
            _MIS1_6_6 = _repository.GetScadaPoint("EMIS1_6_6");
            _MIS2_WRS = _repository.GetScadaPoint("EWRS"); //new EECScadaPoint(Guid.NewGuid(), "MIS2_WRS", "Not Usable", PointDirectionType.Input);// 
            _LAD = _repository.GetScadaPoint("ELAD");
            _NEW_OXY = _repository.GetScadaPoint("ENEW_OXY");
            _MK1 = _repository.GetScadaPoint("EMK1");
            _EMIS2LF1 = _repository.GetScadaPoint("EMIS2LF1");
            _EMIS2LF2 = _repository.GetScadaPoint("EMIS2LF2");
            _EMIS2LF3 = _repository.GetScadaPoint("EMIS2LF3");
            _EMIS2LF4 = _repository.GetScadaPoint("EMIS2LF4");
            _EG1 = _repository.GetScadaPoint("EGEN1");
            _EG2 = _repository.GetScadaPoint("EGEN2");
            _EG3 = _repository.GetScadaPoint("EGEN3");
            _EG4 = _repository.GetScadaPoint("EGEN4");
            _MAB = _repository.GetScadaPoint("MAB");
            _MAB_EEC = _repository.GetScadaPoint("MAB_EEC");
            _OVERL1 = _repository.GetScadaPoint("OVERLOAD1");
            _OVERL2 = _repository.GetScadaPoint("OVERLOAD2");
            _ECONEAF_EREAF = _repository.GetScadaPoint("ECONEAF_EREAF");
            _EPURCH_EC = _repository.GetScadaPoint("EPURCH_EC");
            _EREAF_ECONEAF = _repository.GetScadaPoint("EREAF_ECONEAF");
            _APPLY = _repository.GetScadaPoint("APPLY");
            
            _E_CSM = _repository.GetScadaPoint("E_CSM_Sum");
            _E_HSM = _repository.GetScadaPoint("E_HSM_Sum");
            _E_GEN = _repository.GetScadaPoint("E_GEN_Sum");
            _E_EFS = _repository.GetScadaPoint("E_EFS_Sum");
            _E_MIS1_6_6 = _repository.GetScadaPoint("E_MIS1_6_6_Sum");
            _E_MIS2_WRS = _repository.GetScadaPoint("E_MIS2_WRS_Sum");
            _E_LAD = _repository.GetScadaPoint("E_LAD_Sum");
            _E_NEW_OXY = _repository.GetScadaPoint("E_OXY_Sum");
            _E_MK1 = _repository.GetScadaPoint("E_MK1_Sum");
            _E_GEN1 = _repository.GetScadaPoint("E_GEN1_Sum");
            _E_GEN2 = _repository.GetScadaPoint("E_GEN2_Sum");
            _E_GEN3 = _repository.GetScadaPoint("E_GEN3_Sum");
            _E_GEN4 = _repository.GetScadaPoint("E_GEN4_Sum");
            _E_EXP914 = _repository.GetScadaPoint("E_EXP914");
            _E_IMP914 = _repository.GetScadaPoint("E_IMP914");
            _E_EXP915 = _repository.GetScadaPoint("E_EXP915");
            _E_IMP915 = _repository.GetScadaPoint("E_IMP915");
            _E_TOTAL = _repository.GetScadaPoint("E_TOTAL");
            _ALARM_ENERGY = _repository.GetScadaPoint("ALARM_ENERGY");
            _PSend1 = _repository.GetScadaPoint("PSend1");
            _PSend2 = _repository.GetScadaPoint("PSend2");

            _FirstRead_MAB = true;
            _FirstRead_MAB_EEC = true;

        }

        public void printInitialValues()
        {
            _logger.WriteEntry($"", LogLevels.Info);
            _logger.WriteEntry($"=================================================================", LogLevels.Info);
            _logger.WriteEntry($" EEC..EnergyCalculator.InitialValues is started . . . ", LogLevels.Info);

            _logger.WriteEntry($"------------------------------------------------------", LogLevels.Info);
            _logger.WriteEntry($"------- Constant Values ", LogLevels.Info);
            _logger.WriteEntry($"EC = \"{_EC.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"DeltaP = \"{_DeltaP.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"K1 = \"{_K1.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"PL = \"{_PL.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"PBMax = \"{_PBMax.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"PMAX1 = \"{_PMAX1.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"PMAX2 = \"{_PMAX2.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"OVERL1 = \"{_OVERL1.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"OVERL2 = \"{_OVERL2.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"PMAX15 = \"{_PMAX15.Value.ToString()}\" is read.", LogLevels.Info);

            _logger.WriteEntry($"------------------------------------------------------", LogLevels.Info);
            _logger.WriteEntry($"------- Calculated Values ", LogLevels.Info);
            _logger.WriteEntry($"_EBMAX = \"{_EBMAX.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EBSum = \"{_EBSum.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_ECONEAF_EREAF = \"{_ECONEAF_EREAF.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EPURCH_EC = \"{_EPURCH_EC.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EREAF_ECONEAF = \"{_EREAF_ECONEAF.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EFSum = \"{_EFSum.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EnergyResEnd = \"{_EnergyResEnd.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_RESTIME = \"{_RESTIME.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_EAV_Sum = \"{_EAV_Sum.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_PMAXG = \"{_PMAXG.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_PB = \"{_PB.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_PC = \"{_PC.Value.ToString()}\" is read.", LogLevels.Info);
            _logger.WriteEntry($"_ER = \"{_ER_Cycle.Value.ToString()}\" is read.", LogLevels.Info);

            _logger.WriteEntry($"------------------------------------------------------", LogLevels.Info);
            _logger.WriteEntry($"------- Measurement Values ", LogLevels.Info);

        }

        public bool UpdateCurrentValuesFromLastNewValues()
        {
            bool lastStatus = true;

            try
            {
                if (_DeltaP == null)
                    InitialValues();

                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_DeltaP, _DeltaP_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_K1, _K1_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_EC, _EC_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_PL, _PL_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_PBMax, _PBMax_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_PMAX1, _PMAX1_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteAnalog(_PMAX2, _PMAX2_User.Value);
                lastStatus &= _updateScadaPointOnServer.WriteEECTelegram(_APPLY, (float)0);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }

            return lastStatus;
        }

        public bool Calc15MinValue(bool aFullCycleTag)
        {
            bool result = false;
            float EAV15;
            float CONEAF;
            float EPURCH;

            try
            {
                _logger.WriteEntry("===========================================================", LogLevels.Info);
                _logger.WriteEntry("Calc15MinValue..Enter to method ", LogLevels.Info);

                //'----------------------------------------------
                //' PMAX15            ->      PMAX15
                //' PMAX15 is updated here, it's maybe showed the last value set by PMAXG,
                //'   but another value will be used in calculations.
                var scadaPoint = _repository.GetScadaPoint("PMAX15");
                if (!_updateScadaPointOnServer.WriteAnalog(scadaPoint, _PMAX15.Value))
                {
                    _logger.WriteEntry("Could not write value in SCADA: PMAX15", LogLevels.Error);
                    // TODO: Exit Function
                    return false;
                }

                //'------------------------------------------------
                //'Calculate 15 minutes Values
                //_updateScadaPointOnServer.WriteAnalog(_ECONEAF_EREAF, 0);
                //_updateScadaPointOnServer.WriteAnalog(_EPURCH_EC, 0);
                //_updateScadaPointOnServer.WriteAnalog(_EREAF_ECONEAF, 0);
                //_ECONEAF_EREAF.Value = 0;
                //_EPURCH_EC.Value = 0;
                //_EREAF_ECONEAF.Value = 0;

                EAV15 = 0;
                CONEAF = 0;
                EPURCH = 0;

                //'------------------------------------------------
                //' Read the Fixed Values, After calculations for previuos cycle
                _EC = _repository.GetScadaPoint("ECONTRACT");
                _PL = _repository.GetScadaPoint("PLIMIT");
                _PBMax = _repository.GetScadaPoint("PBMAX");
                _DeltaP = _repository.GetScadaPoint("DELTAP");
                _K1 = _repository.GetScadaPoint("K1");
                _PMAX1 = _repository.GetScadaPoint("PMAX1");
                _PMAX2 = _repository.GetScadaPoint("PMAX2");

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EC= {_EC.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PL= {_PL.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PBMax= {_PBMax.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"DELTAP= {_DeltaP.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"K1= {_K1.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX1= {_PMAX1.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX2= {_PMAX2.Value.ToString()}", LogLevels.Info);

                //'' KAJI T8AN Definition, Added this line
                var EF_Cycle = _Ea_EAF_T1AN.Value +
                                _Ea_EAF_T2AN.Value +
                                _Ea_EAF_T3AN_MV3.Value +
                                _Ea_EAF_T5AN.Value +
                                _Ea_EAF_T7AN.Value +
                                _Ea_EAF_T8AN.Value;

                var EMP = _CSM.Value +
                            _HSM.Value +
                            _PEL.Value +
                            _RED.Value +
                            _GEN.Value +
                            _EFS.Value +
                            _MIS1_6_6.Value +
                            _MIS2_WRS.Value +
                            _LAD.Value +
                            _NEW_OXY.Value +
                            _MK1.Value;


                // HMI
                _E_CSM.Value += _CSM.Value;
                _E_HSM.Value += _HSM.Value;
                _E_GEN.Value += _GEN.Value;
                _E_EFS.Value += _EFS.Value;
                _E_MIS1_6_6.Value += _MIS1_6_6.Value;
                _E_MIS2_WRS.Value += _MIS2_WRS.Value;
                _E_LAD.Value += _LAD.Value;
                _E_NEW_OXY.Value += _NEW_OXY.Value;
                _E_MK1.Value += _MK1.Value;
                _E_GEN1.Value += _EG1.Value;
                _E_GEN2.Value += _EG2.Value;
                _E_GEN3.Value += _EG3.Value;
                _E_GEN4.Value += _EG4.Value;


                //' This is commented in the last edition
                //' E_Exp_Gen = MIS2 + MIS1

                // '------------------------------------------------
                // 'Calculating Energy of Generators
                var EG = _EG1.Value + _EG2.Value + _EG3.Value + _EG4.Value;
                var EB_Cycle = EMP - EG;


                // Read the Sum EB, EF from table
                _EFSum.Value += EF_Cycle;
                _EBSum.Value += EB_Cycle;

                var EAV = _repository.GetScadaPoint("EAV").Value;
                var EAV_Sum = EAV - EB_Cycle - EF_Cycle;
                _EnergyResEnd.Value = EAV_Sum;

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EBSum= {_EBSum.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EFSum= {_EFSum.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EAV_Sum= {EAV_Sum.ToString()}", LogLevels.Info);



                // Calculat Total Consumed Energy 
                _E_TOTAL.Value += _E_IMP914.Value + _E_IMP915.Value - _E_EXP914.Value - _E_EXP915.Value;
                
                if (_E_TOTAL.Value > _EC.Value)
                {
                    _logger.WriteEntry($"Imported energy is over than Contractual value,  E_Import = {_E_TOTAL.Value}  EC = {_EC.Value}", LogLevels.Warn);
                    _updateScadaPointOnServer.SendAlarm(_ALARM_ENERGY, DigitalSingleStatus.Close, "");
                    _updateScadaPointOnServer.SendAlarm(_ALARM_ENERGY, DigitalSingleStatus.Open,
                   $"Allowed energy = {Math.Round(_EC.Value,2)} MWh " +
                   $" ; Total imported energy =  { Math.Round(_E_TOTAL.Value,2)}Mwh" +
                   $" ; Extra imported energy =  { Math.Round(_E_TOTAL.Value - _EC.Value,2)}Mwh"
                   );
                }



                // Check First 15 min in the World, FULLZYCLUS
                // If we have passed a complete period, we should calculate some values for this period,
                // a period is a 15 minutes cycles, start of 0 minute
                if (aFullCycleTag)
                {
                    // Caculate EAvailable 15-Min, EConsumedEAF, EPurchase
                    CONEAF = _EFSum.Value;
                    EPURCH = _EFSum.Value + _EBSum.Value;

                    //' Read the Fixed Values
                    _PL = _repository.GetScadaPoint("PLIMIT");
                    _PBMax = _repository.GetScadaPoint("PBMAX");
                    _DeltaP = _repository.GetScadaPoint("DELTAP");

                    EAV15 = _EC.Value - _PBMax.Value / ZFAC;

                    // Calculate and Store PMax15, based on previuos const params entered by operator in previous period
                    _PMAX15.Value = _PL.Value - (_PBMax.Value + _DeltaP.Value);
                    _updateScadaPointOnServer.WriteAnalog(_PMAX15, _PMAX15.Value);


                    _updateScadaPointOnServer.WriteAnalog(_E_HSM, _E_HSM.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_CSM, _E_CSM.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_NEW_OXY, _E_NEW_OXY.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_EFS, _E_EFS.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_MK1, _E_MK1.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_LAD, _E_LAD.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_GEN, _E_GEN.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_MIS1_6_6, _E_MIS1_6_6.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_MIS2_WRS, _E_MIS2_WRS.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_GEN1, _E_GEN1.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_GEN2, _E_GEN2.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_GEN3, _E_GEN3.Value);
                    _updateScadaPointOnServer.WriteAnalog(_E_GEN4, _E_GEN1.Value);

                    _updateScadaPointOnServer.WriteAnalog(_E_TOTAL, _E_TOTAL.Value);




                    // Calculate ECONEAF / EREAF
                    if (EAV15 > 0)
                    {
                        _ECONEAF_EREAF.Value = (CONEAF / EAV15) * 100;
                        //_updateScadaPointOnServer.WriteAnalog(_ECONEAF_EREAF, _ECONEAF_EREAF.Value);
                    }
                    else
                    {
                        _ECONEAF_EREAF.Value = 0;
                        //_updateScadaPointOnServer.WriteAnalog(_ECONEAF_EREAF, _ECONEAF_EREAF.Value);
                        _logger.WriteEntry("_Available_15Min <= 0", LogLevels.Warn);
                    }

                    // Calculate EPURCH / ECPrev, ECPrev comes from previuos period
                    if (_EC.Value > 0)
                    {
                        _EPURCH_EC.Value = (EPURCH / _EC.Value) * 100;
                        //_updateScadaPointOnServer.WriteAnalog(_EPURCH_EC, _EPURCH_EC.Value);
                    }
                    else
                    {
                        _EPURCH_EC.Value = 0;
                        //_updateScadaPointOnServer.WriteAnalog(_EPURCH_EC, _EPURCH_EC.Value);
                        _logger.WriteEntry("EC_OLD <= 0", LogLevels.Warn);
                    }

                    // Calculate EREAF - ECONEAF
                    _EREAF_ECONEAF.Value = EAV15 - CONEAF;
                    //_updateScadaPointOnServer.WriteAnalog(_EREAF_ECONEAF, _EREAF_ECONEAF.Value);
                }

                //' 0.4. updating all Const parameters, read from SCADA, write into Table
                //' Call for get updated Constants by Operator
                //'If Not m_CEECParams.checkUpdateConstParams() Then
                //'    Call theCTraceLogger.WriteLog(TraceError, "CEEC_Manager..runCyclicOperation", "Could not update EEC 1-Minute Values")
                //'    Calc15MinVal = False
                //'    Exit Function
                //'End If



                //Send to CPS : EAV,_EFSum,_EBSum,ER_Cycle,ECONEAF_EREAF,EPURCH_EC,EREAF_ECONEAF,_EnergyResEnd
                _updateScadaPointOnServer.Send15MinuteEnergy(_EAV_Sum,
                                                            _EFSum,
                                                            _EBSum,
                                                            _ER_Cycle,
                                                            _ECONEAF_EREAF,
                                                            _EPURCH_EC,
                                                            _EREAF_ECONEAF,
                                                            _EnergyResEnd);

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"ECONEAF/EREAF = {_ECONEAF_EREAF.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EPURCH/EC = {_EPURCH_EC.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EREAF-ECONEA F= {_EREAF_ECONEAF.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"This value is used in calculations, PMAX15 = {_PMAX15.Value.ToString()}", LogLevels.Info);


                System.Threading.Thread.Sleep(2000);

                // Must Be check 1401-09-13
                // TODO: In the original code, these lines are avilable
                // _updateScadaPointOnServer.WriteAnalog(_EAV_Sum, 0);
                //_updateScadaPointOnServer.WriteAnalog(_ER_Cycle, 0);
                _updateScadaPointOnServer.WriteAnalog(_EBSum, 0);
                _updateScadaPointOnServer.WriteAnalog(_EFSum, 0);

                // Must Be check 1401-09-13
                //_EAV_Sum.Value = 0;
                //_ER_Cycle.Value = 0;
                _EBSum.Value = 0;
                _EFSum.Value = 0;
                //------------------------------------------
                _E_CSM.Value = 0;
                _E_HSM.Value = 0;
                _E_GEN.Value = 0;
                _E_EFS.Value = 0;
                _E_MIS1_6_6.Value = 0;
                _E_MIS2_WRS.Value = 0;
                _E_LAD.Value = 0;
                _E_NEW_OXY.Value = 0;
                _E_MK1.Value = 0;
                _E_GEN1.Value = 0;
                _E_GEN2.Value = 0;
                _E_GEN3.Value = 0;
                _E_GEN4.Value = 0;
                //---------------------------------------------
                _E_TOTAL.Value = 0;

                _logger.WriteEntry("Calc15MinValue..Exit of method", LogLevels.Info);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
            return result;
        }

        public bool Calc1MinValue(int CycleNo)
        {
            var result = false;
            var PMAX = new float[15]; // Average Available Max Power

            try
            {
                _logger.WriteEntry("-----------------------------------------------------------", LogLevels.Info);
                _logger.WriteEntry(" Calc1MinValue..Enter to method ", LogLevels.Info);

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"Resitual Time = {((15 - CycleNo)).ToString()}", LogLevels.Info);

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EC= {_EC.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PL= {_PL.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PBMax= {_PBMax.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"DeltaP= {_DeltaP.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"K1= {_K1.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX1= {_PMAX1.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX2= {_PMAX2.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                //Calculations from the data above (PC, PBISTM, ...)
                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EBSum= {_EBSum.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EFSum= {_EFSum.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EAV_Sum= {_EAV_Sum.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                // Contractual Power
                _PC.Value = _EC.Value * ZFAC;

                //'------------------------------------------------
                // Instantanous Maximum Basic Power
                var PBISTM = _PBMax.Value + _DeltaP.Value;

                //'------------------------------------------------
                // PCL = Max(PC, PL)
                var PCL = 0.0f;
                if (_PC.Value < _PL.Value)
                    PCL = _PL.Value;
                else
                    PCL = _PC.Value;

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"PC= {_PC.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PL= {_PL.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PCL= {PCL.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                PMAX[0] = PCL - PBISTM;
                _PMAX15.Value = PMAX[0];
                _PMAXG.Value = PMAX[0];

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"PC = EC * ZFAC = {_PC.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PBISTM= PBMax + DELTAP = {PBISTM.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PBMax= {_PBMax.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX(0)= {PMAX[0].ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAX15= {_PMAX15.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAXG= {_PMAXG.Value.ToString()}", LogLevels.Info);


                //'------------------------------------------------
                //'Value Processing: Read substations, plant and generators energies and calculate EB
                //'------------------------------------------------
                var EF_Cycle = _Ea_EAF_T1AN.Value +
                                _Ea_EAF_T2AN.Value +
                                _Ea_EAF_T3AN_MV3.Value +
                                _Ea_EAF_T5AN.Value +
                                _Ea_EAF_T7AN.Value +
                                _Ea_EAF_T8AN.Value;

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"ET1AN {_Ea_EAF_T1AN.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ET2AN {_Ea_EAF_T2AN.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ET3AN {_Ea_EAF_T3AN_MV3.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ET5AN {_Ea_EAF_T5AN.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ET7AN {_Ea_EAF_T7AN.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ET8AN {_Ea_EAF_T8AN.Value.ToString()}", LogLevels.Info);

                _logger.WriteEntry($"EF_Cycle {EF_Cycle.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                _LAD.Value = _EMIS2LF1.Value +
                            _EMIS2LF2.Value +
                            _EMIS2LF3.Value +
                            _EMIS2LF4.Value;
                _updateScadaPointOnServer.WriteAnalog(_LAD, _LAD.Value);

                //'------------------------------------------------
                _logger.WriteEntry($"CSM= {_CSM.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"HSM= {_HSM.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PEL= {_PEL.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"RED= {_RED.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"GEN= {_GEN.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EFS= {_EFS.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"MIS1_6_6= {_MIS1_6_6.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"MIS2_WRS= {_MIS2_WRS.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"LAD= {_LAD.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"NEW_OXY= {_NEW_OXY.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"MK1= {_MK1.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                var EMP = _CSM.Value +
                            _HSM.Value;
                EMP = EMP +
                        _PEL.Value +
                        _RED.Value +
                        _GEN.Value +
                        _EFS.Value +
                        _MIS1_6_6.Value +
                        _MIS2_WRS.Value +
                        _LAD.Value +
                        _NEW_OXY.Value +
                        _MK1.Value;
                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EMP( E Total PP )= {EMP.ToString()}", LogLevels.Info);

                // HMI
                if (CycleNo > 0)
                {
                    _E_CSM.Value += _CSM.Value;
                    _E_HSM.Value += _HSM.Value;
                    _E_GEN.Value += _GEN.Value;
                    _E_EFS.Value += _EFS.Value;
                    _E_MIS1_6_6.Value += _MIS1_6_6.Value;
                    _E_MIS2_WRS.Value += _MIS2_WRS.Value;
                    _E_LAD.Value += _LAD.Value;
                    _E_NEW_OXY.Value += _NEW_OXY.Value;
                    _E_MK1.Value += _MK1.Value;
                    _E_GEN1.Value += _EG1.Value;
                    _E_GEN2.Value += _EG2.Value;
                    _E_GEN3.Value += _EG3.Value;
                    _E_GEN4.Value += _EG4.Value;
                }

                //'------------------------------------------------
                // Calculating Total of Consumed Energy from grid
                if (CycleNo > 0)
                    _E_TOTAL.Value += _E_IMP914.Value + _E_IMP915.Value - _E_EXP914.Value - _E_EXP915.Value;

                //_logger.WriteEntry($"_E_IMP914 = {_E_IMP914.Value} ; _E_IMP915 = {_E_IMP915.Value} ; _E_EXP914 = {_E_EXP914.Value} ; _E_EXP915 = {_E_EXP915.Value} ; ", LogLevels.Warn);
                //_logger.WriteEntry($"CycleNo = {CycleNo} ; _E_TOTAL = {_E_TOTAL.Value} ; _EC = {_EC.Value} ; ", LogLevels.Warn);

                if (((CycleNo == 5) && ((_E_TOTAL.Value) > (_EC.Value / 3.0))) ||
                    ((CycleNo == 10) && ((_E_TOTAL.Value) > (_EC.Value * 2.0) / 3.0)))
                {
                    _logger.WriteEntry($"Imported energy is going to over than Contractual value,  E = {_E_TOTAL.Value}", LogLevels.Warn);
                    _updateScadaPointOnServer.SendAlarm(_ALARM_ENERGY, DigitalSingleStatus.Close, "");

                    // In Minute 5, first Alarm for extra imported energy from Tavanir Lines
                    if (CycleNo == 5)
                        _updateScadaPointOnServer.SendAlarm(_ALARM_ENERGY, DigitalSingleStatus.Open,
                        $"Max Allowed Power = {Math.Round((_EC.Value - _E_TOTAL.Value)*60.0/10.0,2)} MW " +
                        " ; Extra used energy = " + Math.Round((_E_TOTAL.Value - (_EC.Value / 3.0)),2) + " Mwh" +
                        " ; Total imported energy = " + Math.Round(_E_TOTAL.Value,2) + " Mwh" +
                        " ; Allowed energy = "+ Math.Round(_EC.Value / 3.0,2) + "MWh"
                        );

                    // In Minute 10, second Alarm for extra imported energy from Tavanir Lines
                    if (CycleNo == 10)
                        _updateScadaPointOnServer.SendAlarm(_ALARM_ENERGY, DigitalSingleStatus.Open,
                        $"Max Allowed Power = { Math.Round((_EC.Value - _E_TOTAL.Value)*60.0/5.0,2)} MW " +
                        " ; Extra used energy = " + Math.Round((_E_TOTAL.Value - (_EC.Value * 2.0) / 3.0),2) + " Mwh" +
                        " ; Total imported energy = " + Math.Round(_E_TOTAL.Value,2) + " Mwh" +
                        " ; Allowed energy = "+ Math.Round(_EC.Value / 1.5,2) + "MWh" 
                        );
                }

                //'------------------------------------------------
                // Calculating Energy of Generators
                var EG = _EG1.Value +
                            _EG2.Value +
                            _EG3.Value +
                            _EG4.Value;
                var EB_Cycle = EMP - EG;

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EG1= {_EG1.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EG2= {_EG2.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EG3= {_EG3.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EG4= {_EG4.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EG= {EG.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EB_Cycle = EMP - EG= {EB_Cycle.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                //Calculate the new EB, EC (Total Consumed until now) and store it

                if (CycleNo > 0)
                {
                    _EBSum.Value += EB_Cycle;
                    _EFSum.Value += EF_Cycle;
                }

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EBSum= {_EBSum.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"EFSum {_EFSum.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                //Calculate Available Energy
                if (CycleNo == 0)
                {
                    _EAV_Sum.Value = _EC.Value;
                }
                else
                {
                    _EAV_Sum.Value = _EC.Value - _EBSum.Value - _EFSum.Value;
                }

                _logger.WriteEntry($"EAV_Sum = {_EAV_Sum.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                //Calculate Residual Time
                _RESTIME.Value = 15 - CycleNo;

                //'------------------------------------------------
                //Calculate Residual Energy
                _EBMAX.Value = _PBMax.Value / ZFAC; // m_CEECParams.getValuebyName("EBMAX

                PBISTM = _PBMax.Value + _DeltaP.Value;


                //'------------------------------------------------
                if (CycleNo == 0)
                {
                    _ER_Cycle.Value = _EC.Value - _EBMAX.Value;
                }
                else
                {
                    _ER_Cycle.Value = _EAV_Sum.Value - (15 - CycleNo) * (_EBMAX.Value / 15);
                }

                if (_ER_Cycle.Value < 0)
                    _logger.WriteEntry($"EResidual({CycleNo.ToString()}) < 0", LogLevels.Warn);

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"EBMAX= {_EBMAX.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"ER_Cycle= {_ER_Cycle.Value.ToString()}", LogLevels.Info);

                //'------------------------------------------------
                //Calculate Power
                if (CycleNo > 0)
                {
                    PMAX[CycleNo] = _PL.Value - PBISTM;
                    _PB.Value = EB_Cycle * 60; //EMP * 60   '

                    //'------------------------------------------------
                    // Max Active Power for EAF
                    _PMAXG.Value = PCL - _PB.Value - _K1.Value - _DeltaP.Value;

                    //'------------------------------------------------
                    // m_PMAXG = Min( PMAX(0), m_PMAXG)
                    if (PMAX[0] < _PMAXG.Value)
                    {
                        _PMAXG.Value = PMAX[0];
                    }

                    //'------------------------------------------------
                    // m_PMAX15 = Min( m_PMAXG, m_PMAX15 )
                    if (_PMAXG.Value < _PMAX15.Value)
                    {
                        _PMAX15.Value = _PMAXG.Value;
                    }
                }
                else
                {
                    _PB.Value = EB_Cycle * 60;
                }

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"PB { _PB.Value.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PMAXG {_PMAXG.Value.ToString()}", LogLevels.Info);

                //''-------------------------------------------------------------------------------------------------- -
                //''  NOT HERE, ONLY in the START of Calc15Min it should be stored !
                //'' Store calculated values in Table
                //''   PMAX15->PMAX15

              
                _updateScadaPointOnServer.WriteAnalog(_PMAX15, _PMAX15.Value);
                _updateScadaPointOnServer.SendOneMinuteEnergyCALCValues(_PMAXG, _EBSum, _EFSum, _EAV_Sum, _ER_Cycle, _EBMAX, _RESTIME, _PC, _PB);

                _updateScadaPointOnServer.WriteAnalog(_E_HSM, _E_HSM.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_CSM, _E_CSM.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_NEW_OXY, _E_NEW_OXY.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_EFS, _E_EFS.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_MK1, _E_MK1.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_LAD, _E_LAD.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_GEN, _E_GEN.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_MIS1_6_6, _E_MIS1_6_6.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_MIS2_WRS, _E_MIS2_WRS.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_GEN1, _E_GEN1.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_GEN2, _E_GEN2.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_GEN3, _E_GEN3.Value);
                _updateScadaPointOnServer.WriteAnalog(_E_GEN4, _E_GEN1.Value);

                _updateScadaPointOnServer.WriteAnalog(_E_TOTAL, _E_TOTAL.Value);


                //'------------------------------------------------
                // Send to CPS
                //' OVERL1 and OVERL2 are received from OCP/LSP ...				

                var PSend = 0.0f;
                var PSend1 = 0.0f;
                var PSend2 = 0.0f;
                var PDiv = 0.0f;

                _MAB_EEC = _repository.GetScadaPoint("MAB_EEC");
                _OVERL1 = _repository.GetScadaPoint("OVERLOAD1");
                _OVERL2 = _repository.GetScadaPoint("OVERLOAD2");

                if (_MAB_EEC.Value == (float)DigitalSingleStatusOnOff.On)
                {
                    if (_OVERL1.Value <= CPS_ZERO_VALUE && _OVERL2.Value <= CPS_ZERO_VALUE)
                    {

                        //Send Mimimum(PMAXG,PMAX1+PMAX2) for EAF
                        //' ----------------------- Modification By: Ataei - Date: 08/01/2023 ----------------------
                        PSend = (_PMAX1.Value + _PMAX2.Value > _PMAXG.Value) ? _PMAXG.Value : _PMAX1.Value + _PMAX2.Value;
                        //' ----------------------- Modification By: Akbari, Hematy, Ebrahimnejad - Date: 23/11/2008 ----------------------
                        PSend1 = PSend;
                        //' ---------------------------------------------------------------------------------------------------------------
                    }
                    else
                    {
                        PSend = _OVERL1.Value + _OVERL2.Value;
                        if (PSend > _PMAXG.Value)
                        {
                            PSend = _PMAXG.Value;
                        }

                        //' ----------------------- Modification By:  Hematy:28/4/2009
                        PSend1 = PSend;
                        //' ---------------------------------------------------------------------------------------------------------------
                    }
                }
                else
                {
                    // ' In case of MABOpen, only PSend1 and PSend2 are important for PCS
                    if (_MAB_EEC.Value == (float)DigitalSingleStatusOnOff.Off)
                    {
                        //'------------------------------------------------
                        // In case of MAB-EEC Open, only PSend1 and PSend2 are important for PCS
                        // To Be Checked: if (MABStat == GeneralModule.BREAKER_OPEN)
                        if (_OVERL1.Value <= CPS_ZERO_VALUE && _OVERL2.Value > CPS_ZERO_VALUE)
                        {
                            PSend1 = _PMAX1.Value;
                            PSend2 = _OVERL2.Value;
                            PSend = PSend1 + PSend2;
                            if (PSend > _PMAXG.Value)
                            {
                                if (PSend == 0)
                                {
                                    //Division by 0 !!!
                                    _logger.WriteEntry(" PSend = 0 ; Division by 0 ! ", LogLevels.Error);
                                }
                                else
                                {
                                    PDiv = PSend1 / PSend;
                                    PSend1 = _PMAXG.Value * PDiv;

                                    PDiv = PSend2 / PSend;
                                    PSend2 = _PMAXG.Value * PDiv;
                                }
                            }
                        }
                        //'PSend1 and PSend2 ready to send
                        else
                        {
                            if (_OVERL1.Value > CPS_ZERO_VALUE && _OVERL2.Value <= CPS_ZERO_VALUE)
                            {
                                PSend1 = _OVERL1.Value;
                                PSend2 = _PMAX2.Value;
                                PSend = PSend1 + PSend2;
                                if (PSend > _PMAXG.Value)
                                {
                                    if (PSend == 0)
                                    {
                                        //Division by 0 !!!
                                        _logger.WriteEntry(" PSend = 0 ; Division by 0 ! ", LogLevels.Error);
                                    }
                                    else
                                    {
                                        PDiv = PSend1 / PSend;
                                        PSend1 = _PMAXG.Value * PDiv;

                                        PDiv = PSend2 / PSend;
                                        PSend2 = _PMAXG.Value * PDiv;
                                    }
                                }
                                //PSend1 and PSend2 ready to send
                            }
                            else
                            {
                                if (_OVERL1.Value > CPS_ZERO_VALUE && _OVERL2.Value > CPS_ZERO_VALUE)
                                {
                                    PSend1 = _OVERL1.Value;
                                    PSend2 = _OVERL2.Value;
                                    PSend = PSend1 + PSend2;

                                    if (PSend > _PMAXG.Value)
                                    {
                                        if (PSend == 0)
                                        {
                                            //Division by 0 !!!
                                            _logger.WriteEntry(" PSend = 0 ; Division by 0 ! ", LogLevels.Error);
                                        }
                                        else
                                        {
                                            PDiv = PSend1 / PSend;
                                            PSend1 = _PMAXG.Value * PDiv;

                                            PDiv = PSend2 / PSend;
                                            PSend2 = _PMAXG.Value * PDiv;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_OVERL1.Value <= CPS_ZERO_VALUE && _OVERL2.Value <= CPS_ZERO_VALUE)
                                    {
                                        PSend1 = _PMAX1.Value;
                                        PSend2 = _PMAX2.Value;
                                        PSend = PSend1 + PSend2;
                                        if (PSend > _PMAXG.Value)
                                        {
                                            if (PSend == 0)
                                            {
                                                //Division by 0 !!!
                                            }
                                            else
                                            {
                                                PDiv = PSend1 / PSend;
                                                PSend1 = _PMAXG.Value * PDiv;

                                                PDiv = PSend2 / PSend;
                                                PSend2 = _PMAXG.Value * PDiv;
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        _logger.WriteEntry(" Value of MAB_EEC is not valid!, MAB_EEC = " + _MAB_EEC.Value.ToString(), LogLevels.Error);
                    }
                }
                _updateScadaPointOnServer.WriteAnalog(_PSend1, PSend1);
                _updateScadaPointOnServer.WriteAnalog(_PSend2, PSend2);


                //        ' -----------------------------------------------------------------------
                //        ' Psend, PSend1, PSend2, ...  should be sent to PCS
                //        ' EAV_Sum from Previous cycle !!!!!!!!!!!!!!!!!!!
                // TODO: We should create two SCADApoint for presenting "PSend1, PSend2" in EEC Mask, which values were sent to PCS, and using in SFSC
                if (!_repository.SendEECTelegramToDC(_RESTIME.Value, _ER_Cycle.Value, PSend, PSend1, PSend2, _EnergyResEnd.Value))
                {
                    _logger.WriteEntry("Send Job to PCS was failed", LogLevels.Error);
                    // TODO:           Exit Function
                    //return false;
                }

                //'------------------------------------------------
                // Send to CPS : PSend,PSend1,PSend2,EnergyResEnd  ???????????
                _EnergyResEnd.Value = 0;

                //        ' m_EnergyResEnd     ->      EnergyResEnd
                if (!_updateScadaPointOnServer.WriteResEnegy(_EnergyResEnd, _EnergyResEnd.Value))
                {
                    _logger.WriteEntry("Could not write value in SCADA: EnergyResEnd", LogLevels.Error);
                    // TODO:           Exit Function
                    //return false;
                }

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry($"PSend= {PSend.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PSend1= {PSend1.ToString()}", LogLevels.Info);
                _logger.WriteEntry($"PSend2= {PSend2.ToString()}", LogLevels.Info);

                //        ' -----------------------------------------------------------------------
                //        ' Set flag for indicating update of telegram
                var eecTelegram = _repository.GetScadaPoint("EECTELEGRAM");
                if (!_updateScadaPointOnServer.WriteEECTelegram(eecTelegram, (float)1))
                    _logger.WriteEntry("Updating EEC Telegram flag was failed in the SCADA", LogLevels.Error);
                else
                    result = true;

                _logger.WriteEntry("===============================", LogLevels.Info);
                _logger.WriteEntry(" Calc1MinValue..Exit of method ", LogLevels.Info);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
            return result;
        }
        public void Check_Apply_EECConst(MeasurementData measurement)
        {
            try
            {
                string strGUID = measurement.MeasurementId.ToString();
                string strValue = measurement.Value.ToString();

                //return;

                if (isCompleted == false)
                    return;

                isCompleted = false;

                var aTag = _repository.GetScadaPoint(Guid.Parse(strGUID));

                if (aTag is null)
                {
                    isCompleted = true;
                    return;
                }

                //--------------------------------------------------------------------------

                if (aTag.Name == "APPLY" && (aTag.Value == 1))
                {

                    _logger.WriteEntry("..............Applied EEC User Constants.................", LogLevels.Info);
                    _logger.WriteEntry($"EC_User= {_EC_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"EC= {_EC.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PL_User= {_PL_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PL= {_PL.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PBMax_User= {_PBMax_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PBMax= {_PBMax.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"DeltaP_User= {_DeltaP_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"DeltaP= {_DeltaP.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"K1_User= {_K1_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"K1= {_K1.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PMAX1_User= {_PMAX1_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PMAX1= {_PMAX1.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PMAX2_User= {_PMAX2_User.Value.ToString()}", LogLevels.Info);
                    _logger.WriteEntry($"PMAX2= {_PMAX2.Value.ToString()}", LogLevels.Info);

                    _updateScadaPointOnServer.WriteAnalog(_EC, _EC_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_PL, _PL_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_PBMax, _PBMax_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_DeltaP, _DeltaP_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_K1, _K1_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_PMAX1, _PMAX1_User.Value);
                    _updateScadaPointOnServer.WriteAnalog(_PMAX2, _PMAX2_User.Value);
                    _updateScadaPointOnServer.WriteEECTelegram(_APPLY, (float)0);

                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                _updateScadaPointOnServer.WriteEECTelegram(_APPLY, (float)0);
            }
            isCompleted = true;
        }

        public void Set_EEC_MAB_Status(EECScadaPoint scadaPoint)
        {
            EECScadaPoint _MAB;
            EECScadaPoint _MAB_EEC;
            _MAB = _repository.GetScadaPoint("MAB");
            _MAB_EEC = _repository.GetScadaPoint("MAB_EEC");
            if (_FirstRead_MAB_EEC && scadaPoint.Name == "MAB_EEC")
            {
                _FirstRead_MAB_EEC = false;
                return;
            }


            if (_FirstRead_MAB && scadaPoint.Name == "MAB" && _MAB.Value == (float)DigitalDoubleStatus.Open)
            {
                _FirstRead_MAB = false;
                return;
            }

            if (_FirstRead_MAB && scadaPoint.Name == "MAB" && _MAB.Value == (float)DigitalDoubleStatus.Close)
                _FirstRead_MAB = false;


            if ((scadaPoint.Name == "MAB" || scadaPoint.Name == "MAB_EEC") && _MAB.Value == (float)DigitalDoubleStatus.Close)
            {
                if (_MAB_EEC.Value != (float)DigitalSingleStatusOnOff.On)
                {
                    if (!_updateScadaPointOnServer.ApplyMarkerCommand(_MAB_EEC))
                    {
                        _logger.WriteEntry("Error in EEC in 'trying to remove Blocked Marker from _MAB_EEC!'", LogLevels.Error);
                        return;
                    }

                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("MAB_EEC"), (DigitalSingleStatus)DigitalSingleStatusOnOff.On, "MAB_EEC is Closed  . . . "))
                    {
                        _logger.WriteEntry("Fail to Close MAB_EEC", LogLevels.Error);
                        return;
                    }
                }

            }
            if (scadaPoint.Name == "MAB" && _MAB.Value == (float)DigitalDoubleStatus.Open)
            {
                if (_MAB_EEC.Value == (float)DigitalSingleStatusOnOff.On)
                {
                    if (!_updateScadaPointOnServer.ApplyMarkerCommand(_MAB_EEC))
                    {
                        _logger.WriteEntry("Error in EEC in 'trying to remove Blocked Marker from _MAB_EEC!'", LogLevels.Error);
                        return;
                    }

                    if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("MAB_EEC"), (DigitalSingleStatus)DigitalSingleStatusOnOff.Off, "MAB_EEC is Opened  . . . "))
                    {
                        _logger.WriteEntry("Fail to Open MAB_EEC", LogLevels.Error);
                        return;
                    }
                }
            }
        }

        public void AlarmAcked_Processing(EECScadaPoint scadaPoint)
        {
            if (!_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint(scadaPoint.Name), (DigitalSingleStatus)DigitalSingleStatusOnOff.Off, " "))
            {
                _logger.WriteEntry($"Fail to Disappear Alarm {scadaPoint.Name}", LogLevels.Error);
                return;
            }

        }
    }
}