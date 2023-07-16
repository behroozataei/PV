using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Irisa.Logger;
using Irisa.Message.CPS;


namespace RPC
{
    internal class RPCCalculation
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;



        internal RPCCalculation(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
        }

        private double Ea_TAV = 0; // Progressive 400kV Active Energy
        private double Er_TAV = 0; // Progressive 400kV Reactive Energy
        private double Ea_EAF = 0; // Progressive EAF Active Energy
        private double Er_EAF = 0; // Progressive EAF Reactive Energy
        private double Ea_MF = 0; // Progressive MF Active Energy
        private double Er_MF = 0; // Progressive MF Reactive Energy
        private double Ea_PP = 0; // Progressive PP Active Energy
        private double Er_PP = 0; // Progressive PP Reactive Energy
        private double Er_SVC = 0; // Progressive SVC Reactive Energy


        // Running in every minute calculations
        public bool Preset1Min()
        {
            bool result = false;
            try
            {

                // The counters used in VoltageController and QController should be reset for the new 15Min period.
                double C1_1 = 0;
                double C1_2 = 0;
                double C1_3 = 0;
                double C1_4 = 0;
                double C2_1 = 0;
                double C2_2 = 0;
                double C2_3 = 0;
                double C2_4 = 0;
                double C5 = 0;
                double C6 = 0;

                _logger.WriteEntry("--- Reset the Values in Cycle1 ---", LogLevels.Info);

                result = true;

                // Set the progressive energies to 0
                Ea_TAV = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_TAV"), (float)Ea_TAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_TAV", LogLevels.Error);
                }

                Er_TAV = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_TAV"), (float)Er_TAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_TAV", LogLevels.Error);
                }

                Ea_EAF = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_EAF"), (float)Ea_EAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_EAF", LogLevels.Error);
                }

                Er_EAF = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_EAF"), (float)Er_EAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_EAF", LogLevels.Error);
                }

                Ea_PP = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_PP"), (float)Ea_PP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_PP", LogLevels.Error);
                }
                Er_PP = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_PP"), (float)Er_PP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_PP", LogLevels.Error);
                }

                Ea_MF = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_MF"), (float)Ea_MF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_MF", LogLevels.Error);
                }

                Er_MF = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_MF"), (float)Er_MF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_MF", LogLevels.Error);
                }

                Er_SVC = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_SVC"), (float)Er_SVC))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_SVC", LogLevels.Error);
                }

                // Set the counters to 0.
                C1_1 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_1"), (float)C1_1))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C1_1", LogLevels.Error);
                }

                C1_2 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_2"), (float)C1_2))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C1_2", LogLevels.Error);
                }

                C1_3 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_3"), (float)C1_3))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C1_3", LogLevels.Error);
                }

                C1_4 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_4"), (float)C1_4))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C1_4", LogLevels.Error);
                }

                C2_1 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_1"), (float)C2_1))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C2_1", LogLevels.Error);
                }

                C2_2 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_2"), (float)C2_2))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C2_2", LogLevels.Error);
                }

                C2_3 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_3"), (float)C2_3))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C2_3", LogLevels.Error);
                }

                C2_4 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_4"), (float)C2_4))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C2_4", LogLevels.Error);
                }

                C5 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C5"), (float)C5))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C5", LogLevels.Error);
                }

                C6 = 0;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C6"), (float)C6))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: C6", LogLevels.Error);
                }

                // Reset the Suggestions
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK1"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK1", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK2"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK2", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK3"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK3", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK4"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK4", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK5"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK5", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK6"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK6", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK7"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK7", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK8"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK8", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK9", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_1"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK9_1", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_2"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK9_2", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), (float)0))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: MARK10", LogLevels.Error);
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }


            return result;
        }

 


        // Reads the active and reactive energies every 1 minute and calculates
        // the progressive active and reactive energy.
        public bool ProgressEnergyCalc()
        {
            bool result = false;
            try
            {

                result = true;

                // TAVANIR (400kV)
                Ea_TAV = Ea_TAV + _repository.GetRPCScadaPoint("Ea_TAV_1").Value + _repository.GetRPCScadaPoint("Ea_TAV_2").Value;
                               // - _repository.GetRPCScadaPoint("Ea_TAV_1_E").Value - _repository.GetRPCScadaPoint("Ea_TAV_2_E").Value;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_TAV"), (float)Ea_TAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_TAV", LogLevels.Error);
                    return result;
                }

                Er_TAV = Er_TAV + _repository.GetRPCScadaPoint("Er_TAV_1").Value + _repository.GetRPCScadaPoint("Er_TAV_2").Value;
                                //- _repository.GetRPCScadaPoint("Er_TAV_1_E").Value - _repository.GetRPCScadaPoint("Er_TAV_2_E").Value;
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_TAV"), (float)Er_TAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_TAV", LogLevels.Error);
                    return result;
                }


                // SVC
                Er_SVC = Er_SVC + _repository.GetRPCScadaPoint("Er_SVC1").Value + _repository.GetRPCScadaPoint("Er_SVC2").Value +
                                  _repository.GetRPCScadaPoint("Er_SVCA").Value + _repository.GetRPCScadaPoint("Er_SVCB").Value;


                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_SVC"), (float)Er_SVC))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_SVC", LogLevels.Error);
                    return result;
                }


                // EAF
                // Compensated
                // Ea_EAF = Ea_EAF + _repository.GetRPCScadaPoint("Ea_EAF_T1AN + _repository.GetRPCScadaPoint("Ea_EAF_T2AN + _repository.GetRPCScadaPoint("Ea_EAF_T5AN
                // Er_EAF = Er_EAF + _repository.GetRPCScadaPoint("Er_EAF_T1AN + _repository.GetRPCScadaPoint("Er_EAF_T2AN + _repository.GetRPCScadaPoint("Er_EAF_T5AN - Er_SVC

                Ea_EAF = Ea_EAF + _repository.GetRPCScadaPoint("Ea_EAF_T1AN").Value + _repository.GetRPCScadaPoint("Ea_EAF_T2AN").Value + _repository.GetRPCScadaPoint("Ea_EAF_T5AN").Value
                                + _repository.GetRPCScadaPoint("Ea_EAF_T7AN").Value + _repository.GetRPCScadaPoint("Ea_EAF_T8AN").Value + _repository.GetRPCScadaPoint("Ea_LF").Value;

                Er_EAF = Er_EAF + _repository.GetRPCScadaPoint("Er_EAF_T1AN").Value + _repository.GetRPCScadaPoint("Er_EAF_T2AN").Value + _repository.GetRPCScadaPoint("Er_EAF_T5AN").Value
                                + _repository.GetRPCScadaPoint("Er_EAF_T7AN").Value + _repository.GetRPCScadaPoint("Er_EAF_T8AN").Value + _repository.GetRPCScadaPoint("Er_LF").Value +
                -_repository.GetRPCScadaPoint("Er_SVC1").Value - _repository.GetRPCScadaPoint("Er_SVC2").Value;


                switch ((int)_repository.GetRPCScadaPoint("MV3_CB").Value)
                {
                    case 0:
                        _logger.WriteEntry("MV3_CB state is invalid!", LogLevels.Warn);
                        break;
                    case 2:
                        Ea_EAF += _repository.GetRPCScadaPoint("Ea_EAF_T3AN_MV3").Value;
                        Er_EAF += _repository.GetRPCScadaPoint("Er_EAF_T3AN_MV3").Value;
                        break;
                }
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_EAF"), (float)Ea_EAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_EAF", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_EAF"), (float)Er_EAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_EAF", LogLevels.Error);
                    return result;
                }

                // Uncompensated
                Ea_MF += _repository.GetRPCScadaPoint("Ea_MF_1MinSum").Value;
                Er_MF += _repository.GetRPCScadaPoint("Er_MF_1MinSum").Value;

               

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_MF"), (float)Ea_MF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_MF", LogLevels.Error);
                    return result;
                }
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_MF"), (float)Er_MF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_MF", LogLevels.Error);
                    return result;
                }



                // PP
                Ea_PP = Ea_PP + _repository.GetRPCScadaPoint("Ea_PP_T4AN").Value + _repository.GetRPCScadaPoint("Ea_PP_T6AN").Value;// + _repository.GetRPCScadaPoint("Ea_LF").Value;
                Er_PP = Er_PP + _repository.GetRPCScadaPoint("Er_PP_T4AN").Value + _repository.GetRPCScadaPoint("Er_PP_T6AN").Value;// + _repository.GetRPCScadaPoint("Er_LF").Value;
                switch ((int)_repository.GetRPCScadaPoint("MZ3_CB").Value)
                {
                    case 0:
                        _logger.WriteEntry("MZ3_CB state is invalid!", LogLevels.Warn);
                        break;
                    case 2:
                        Ea_PP += _repository.GetRPCScadaPoint("Ea_PP_T3AN_MZ3").Value;
                        Er_PP += _repository.GetRPCScadaPoint("Er_PP_T3AN_MZ3").Value;
                        break;
                }
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Ea_PP"), (float)Ea_PP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Ea_PP", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_PP"), (float)Er_PP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_PP", LogLevels.Error);
                    return result;
                }



                _logger.WriteEntry("----- Progressive Energy Calculation -----", LogLevels.Info);
                _logger.WriteEntry("Ea_TAV_1 = " + _repository.GetRPCScadaPoint("Ea_TAV_1").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_TAV_2 = " + _repository.GetRPCScadaPoint("Ea_TAV_2").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_TAV = " + Ea_TAV.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_TAV_1 = " + _repository.GetRPCScadaPoint("Er_TAV_1").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_TAV_2 = " + _repository.GetRPCScadaPoint("Er_TAV_2").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_TAV = " + Er_TAV.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_LF = " + _repository.GetRPCScadaPoint("Er_LF").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T1AN = " + _repository.GetRPCScadaPoint("Ea_EAF_T1AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T2AN = " + _repository.GetRPCScadaPoint("Ea_EAF_T2AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T3AN_MV3 = " + _repository.GetRPCScadaPoint("Ea_EAF_T3AN_MV3").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T5AN = " + _repository.GetRPCScadaPoint("Ea_EAF_T5AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T7AN = " + _repository.GetRPCScadaPoint("Ea_EAF_T7AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF_T8AN = " + _repository.GetRPCScadaPoint("Ea_EAF_T8AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_LF = " + _repository.GetRPCScadaPoint("Ea_LF").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_EAF = " + Ea_EAF.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T1AN = " + _repository.GetRPCScadaPoint("Er_EAF_T1AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T2AN = " + _repository.GetRPCScadaPoint("Er_EAF_T2AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T3AN_MV3 = " + _repository.GetRPCScadaPoint("Er_EAF_T3AN_MV3").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T5AN = " + _repository.GetRPCScadaPoint("Er_EAF_T5AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T7AN = " + _repository.GetRPCScadaPoint("Er_EAF_T7AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_EAF_T8AN = " + _repository.GetRPCScadaPoint("Er_EAF_T8AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_SVC1 = " + _repository.GetRPCScadaPoint("Er_SVC1").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_SVC2 = " + _repository.GetRPCScadaPoint("Er_SVC2").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_SVCA = " + _repository.GetRPCScadaPoint("Er_SVCA").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_SVCB = " + _repository.GetRPCScadaPoint("Er_SVCB").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_SVC = " + Er_SVC.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_EAF = " + Er_EAF.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Ea_PP_T3AN_MZ3 = " + _repository.GetRPCScadaPoint("Ea_PP_T3AN_MZ3").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_PP_T4 = " + _repository.GetRPCScadaPoint("Ea_PP_T4AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_PP_T6 = " + _repository.GetRPCScadaPoint("Ea_PP_T6AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Ea_PP = " + Ea_PP.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_PP_T3AN_MZ3 = " + _repository.GetRPCScadaPoint("Er_PP_T3AN_MZ3").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_PP_T4 = " + _repository.GetRPCScadaPoint("Er_PP_T4AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_PP_T6 = " + _repository.GetRPCScadaPoint("Er_PP_T6AN").Value, LogLevels.Trace);
                _logger.WriteEntry("Er_PP = " + Er_PP.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Ea_MF = " + Ea_MF.ToString(), LogLevels.Trace);
                _logger.WriteEntry("Er_MF = " + Er_MF.ToString(), LogLevels.Trace);

            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }  
            return result;
        }

        // Calculating the CosPhi of the lines via Energies
        public bool CosPhiCalc()
        {
            bool result = false;
            try
            {

                double COS_TAV = 0;
                double COS_EAF = 0;
                double COS_EAF_Uncompens = 0;
                double COS_PP = 0;
                double Divisor = 0;

                result = true;

                // CosPhi TAV
                Divisor = Math.Sqrt(_repository.GetRPCScadaPoint("Ea_TAV").Value * _repository.GetRPCScadaPoint("Ea_TAV").Value +
                                    _repository.GetRPCScadaPoint("Er_TAV").Value * _repository.GetRPCScadaPoint("Er_TAV").Value);

                if (Divisor == 0)
                {
                    _logger.WriteEntry("Cos_TAV --> Division by zero!", LogLevels.Warn);
                }
                else
                {
                    COS_TAV = _repository.GetRPCScadaPoint("Ea_TAV").Value / Divisor;
                }

                if (_repository.GetRPCScadaPoint("Er_TAV").Value >= 0)
                {
                    SignDef(ref COS_TAV, "+");
                }
                else
                {
                    SignDef(ref COS_TAV, "-");
                }

                // CosPhi EAF (Compensated)
                Divisor = Math.Sqrt(_repository.GetRPCScadaPoint("Ea_EAF").Value * _repository.GetRPCScadaPoint("Ea_EAF").Value +
                                    _repository.GetRPCScadaPoint("Er_EAF").Value * _repository.GetRPCScadaPoint("Er_EAF").Value);
                if (Divisor == 0)
                {
                    _logger.WriteEntry("COS_EAF --> Division by zero!", LogLevels.Warn);
                }
                else
                {
                    COS_EAF = _repository.GetRPCScadaPoint("Ea_EAF").Value / Divisor;
                }

                if (_repository.GetRPCScadaPoint("Er_EAF").Value >= 0)
                {
                    SignDef(ref COS_EAF, "+");
                }
                else
                {
                    SignDef(ref COS_EAF, "-");
                }

                // CosPhi EAF (Uncompensated)
                Divisor = Math.Sqrt(_repository.GetRPCScadaPoint("Ea_MF").Value * _repository.GetRPCScadaPoint("Ea_MF").Value +
                                    _repository.GetRPCScadaPoint("Er_MF").Value * _repository.GetRPCScadaPoint("Er_MF").Value);

                if (Divisor == 0)
                {
                    _logger.WriteEntry("COS_EAF_Uncompens --> Division by zero!", LogLevels.Warn);
                }
                else
                {
                    COS_EAF_Uncompens = _repository.GetRPCScadaPoint("Ea_MF").Value / Divisor;
                }

                // CosPhi PP
                Divisor = Math.Sqrt(_repository.GetRPCScadaPoint("Ea_PP").Value * _repository.GetRPCScadaPoint("Ea_PP").Value +
                                    _repository.GetRPCScadaPoint("Er_PP").Value * _repository.GetRPCScadaPoint("Er_PP").Value);

                if (Divisor == 0)
                {
                    _logger.WriteEntry("COS_PP --> Division by zero!", LogLevels.Warn);
                }
                else
                {
                    COS_PP = _repository.GetRPCScadaPoint("Ea_PP").Value / Divisor;
                }

                if (_repository.GetRPCScadaPoint("Er_PP").Value >= 0)
                {
                    SignDef(ref COS_PP, "+");
                }
                else
                {
                    SignDef(ref COS_PP, "-");
                }


                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("COS_TAV"), (float)COS_TAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: COS_TAV", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("COS_EAF"), (float)COS_EAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: COS_EAF", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("COS_EAF_Uncompens"), (float)COS_EAF_Uncompens))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: COS_EAF_Uncompens", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("COS_PP"), (float)COS_PP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: COS_PP", LogLevels.Error);
                }


                _logger.WriteEntry("COS_TAV = " + COS_TAV.ToString(), LogLevels.Trace);
                _logger.WriteEntry("COS_EAF = " + COS_EAF.ToString(), LogLevels.Trace);
                _logger.WriteEntry("COS_EAF_Uncompens = " + COS_EAF_Uncompens.ToString(), LogLevels.Trace);
                _logger.WriteEntry("COS_PP = " + COS_PP.ToString(), LogLevels.Trace);
            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }

            return result;
        }
        // Update the Transformers Primary Voltages
        public bool TransPrimeVoltageCalc()
        {
            bool result = false;
            try
            {

                double V400_1 = 0;
                double V400_2 = 0;

                double T1AN_PRIMEVOLT = 0;
                double T2AN_PRIMEVOLT = 0;
                double T3AN_PRIMEVOLT = 0;
                double T4AN_PRIMEVOLT = 0;
                double T5AN_PRIMEVOLT = 0;
                double T6AN_PRIMEVOLT = 0;
                double T7AN_PRIMEVOLT = 0;
                double T8AN_PRIMEVOLT = 0;

                DigitalDoubleStatus C01A_CB = 0;
                DigitalDoubleStatus C01A_DS1 = 0;
                DigitalDoubleStatus C01A_DS2 = 0;
                DigitalDoubleStatus C01A_DS3 = 0;
                DigitalDoubleStatus C01B_CB = 0;
                DigitalDoubleStatus C01B_DS1 = 0;
                DigitalDoubleStatus C01B_DS2 = 0;
                DigitalDoubleStatus C01C_CB = 0;
                DigitalDoubleStatus C01C_DS1 = 0;
                DigitalDoubleStatus C01C_DS2 = 0;
                DigitalDoubleStatus C01C_DS3 = 0;

                DigitalDoubleStatus C02A_CB = 0;
                DigitalDoubleStatus C02A_DS1 = 0;
                DigitalDoubleStatus C02A_DS2 = 0;
                DigitalDoubleStatus C02B_CB = 0;
                DigitalDoubleStatus C02B_DS1 = 0;
                DigitalDoubleStatus C02B_DS2 = 0;
                DigitalDoubleStatus C02C_CB = 0;
                DigitalDoubleStatus C02C_DS1 = 0;
                DigitalDoubleStatus C02C_DS2 = 0;
                DigitalDoubleStatus C02C_DS3 = 0;

                DigitalDoubleStatus C03A_CB = 0;
                DigitalDoubleStatus C03A_DS1 = 0;
                DigitalDoubleStatus C03A_DS2 = 0;
                DigitalDoubleStatus C03B_CB = 0;
                DigitalDoubleStatus C03B_DS1 = 0;
                DigitalDoubleStatus C03B_DS2 = 0;
                DigitalDoubleStatus C03C_CB = 0;
                DigitalDoubleStatus C03C_DS1 = 0;
                DigitalDoubleStatus C03C_DS2 = 0;
                DigitalDoubleStatus C03C_DS3 = 0;

                DigitalDoubleStatus C04A_CB = 0;
                DigitalDoubleStatus C04A_DS1 = 0;
                DigitalDoubleStatus C04A_DS2 = 0;
                DigitalDoubleStatus C04B_CB = 0;
                DigitalDoubleStatus C04B_DS1 = 0;
                DigitalDoubleStatus C04B_DS2 = 0;
                DigitalDoubleStatus C04C_CB = 0;
                DigitalDoubleStatus C04C_DS1 = 0;
                DigitalDoubleStatus C04C_DS2 = 0;
                DigitalDoubleStatus C04C_DS3 = 0;
                DigitalDoubleStatus C04A_DS3 = 0;

                DigitalDoubleStatus C05A_CB = 0;
                DigitalDoubleStatus C05A_DS1 = 0;
                DigitalDoubleStatus C05A_DS2 = 0;
                DigitalDoubleStatus C05A_DS3 = 0;
                DigitalDoubleStatus C05B_CB = 0;
                DigitalDoubleStatus C05B_DS1 = 0;
                DigitalDoubleStatus C05B_DS2 = 0;
                DigitalDoubleStatus C05C_CB = 0;
                DigitalDoubleStatus C05C_DS1 = 0;
                DigitalDoubleStatus C05C_DS2 = 0;
                DigitalDoubleStatus C05C_DS3 = 0;

                result = true;

                C01A_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01A_CB").Value;
                C01A_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01A_DS1").Value;
                C01A_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01A_DS2").Value;
                C01A_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01A_DS3").Value;
                C01B_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01B_CB").Value;
                C01B_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01B_DS1").Value;
                C01B_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01B_DS2").Value;
                C01C_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01C_CB").Value;
                C01C_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01C_DS1").Value;
                C01C_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01C_DS2").Value;
                C01C_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C01C_DS3").Value;

                C02A_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02A_CB").Value;
                C02A_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02A_DS1").Value;
                C02B_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02B_CB").Value;
                C02B_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02B_DS1").Value;
                C02B_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02B_DS2").Value;
                C02C_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02C_CB").Value;
                C02C_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02C_DS1").Value;
                C02C_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02C_DS2").Value;
                C02C_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C02C_DS3").Value;

                C03A_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03A_CB").Value;
                C03A_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03A_DS1").Value;
                C03B_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03B_CB").Value;
                C03B_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03B_DS1").Value;
                C03B_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03B_DS2").Value;
                C03C_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03C_CB").Value;
                C03C_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03C_DS1").Value;
                C03C_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03C_DS2").Value;
                C03C_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C03C_DS3").Value;

                C04A_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04A_CB").Value;
                C04A_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04A_DS1").Value;
                C04A_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04A_DS2").Value;
                C04A_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04A_DS3").Value;
                C04B_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04B_CB").Value;
                C04B_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04B_DS1").Value;
                C04B_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04B_DS2").Value;
                C04C_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04C_CB").Value;
                C04C_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04C_DS1").Value;
                C04C_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04C_DS2").Value;
                C04C_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C04C_DS3").Value;

                C05A_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05A_CB").Value;
                C05A_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05A_DS1").Value;
                C05A_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05A_DS2").Value;
                C05A_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05A_DS3").Value;
                C05B_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05B_CB").Value;
                C05B_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05B_DS1").Value;
                C05B_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05B_DS2").Value;
                C05C_CB = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05C_CB").Value;
                C05C_DS1 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05C_DS1").Value;
                C05C_DS2 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05C_DS2").Value;
                C05C_DS3 = (DigitalDoubleStatus)_repository.GetRPCScadaPoint("C05C_DS3").Value;

                // Bus 400 voltages
                V400_1 = _repository.GetRPCScadaPoint("V400_1").Value;
                V400_2 = _repository.GetRPCScadaPoint("V400_2").Value;

                // Set the default value
                T1AN_PRIMEVOLT = 0;
                T2AN_PRIMEVOLT = 0;
                T3AN_PRIMEVOLT = 0;
                T4AN_PRIMEVOLT = 0;
                T5AN_PRIMEVOLT = 0;
                T6AN_PRIMEVOLT = 0;
                T7AN_PRIMEVOLT = 0;
                T8AN_PRIMEVOLT = 0;

                switch ((int)C01A_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C01A_CB == DigitalDoubleStatus.Close) && (C01A_DS1 == DigitalDoubleStatus.Close) && (C01A_DS2 == DigitalDoubleStatus.Close))
                        {
                            T1AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T1AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C01B_CB == DigitalDoubleStatus.Close) && (C01B_DS1 == DigitalDoubleStatus.Close) && (C01B_DS2 == DigitalDoubleStatus.Close) && (C01C_CB == DigitalDoubleStatus.Close) && (C01C_DS1 == DigitalDoubleStatus.Close) && (C01C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T1AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T1AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }

                switch ((int)C01C_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C01A_CB == DigitalDoubleStatus.Close) && (C01A_DS1 == DigitalDoubleStatus.Close) && (C01A_DS2 == DigitalDoubleStatus.Close) &&
                            (C01B_CB == DigitalDoubleStatus.Close) && (C01B_DS1 == DigitalDoubleStatus.Close) && (C01B_DS2 == DigitalDoubleStatus.Close))
                        {
                            T2AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T2AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C01C_CB == DigitalDoubleStatus.Close) && (C01C_DS1 == DigitalDoubleStatus.Close) && (C01C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T2AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T2AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }

                switch ((int)C02C_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C02A_CB == DigitalDoubleStatus.Close) && (C02A_DS1 == DigitalDoubleStatus.Close) && (C02A_DS2 == DigitalDoubleStatus.Close) &&
                            (C02B_CB == DigitalDoubleStatus.Close) && (C02B_DS1 == DigitalDoubleStatus.Close) && (C02B_DS2 == DigitalDoubleStatus.Close))
                        {
                            T3AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T3AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C02C_CB == DigitalDoubleStatus.Close) && (C02C_DS1 == DigitalDoubleStatus.Close) && (C02C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T3AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T3AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }

                switch ((int)C03C_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C03A_CB == DigitalDoubleStatus.Close) && (C03A_DS1 == DigitalDoubleStatus.Close) && (C03A_DS2 == DigitalDoubleStatus.Close) &&
                            (C03B_CB == DigitalDoubleStatus.Close) && (C03B_DS1 == DigitalDoubleStatus.Close) && (C03B_DS2 == DigitalDoubleStatus.Close))
                        {
                            T4AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T4AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C03C_CB == DigitalDoubleStatus.Close) && (C03C_DS1 == DigitalDoubleStatus.Close) && (C03C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T4AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T4AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }

                switch ((int)C04C_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C04A_CB == DigitalDoubleStatus.Close) && (C04A_DS1 == DigitalDoubleStatus.Close) && (C04A_DS2 == DigitalDoubleStatus.Close) &&
                            (C04B_CB == DigitalDoubleStatus.Close) && (C04B_DS1 == DigitalDoubleStatus.Close) && (C04B_DS2 == DigitalDoubleStatus.Close))
                        {
                            T5AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T5AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C04C_CB == DigitalDoubleStatus.Close) && (C04C_DS1 == DigitalDoubleStatus.Close) && (C04C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T5AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T5AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }
                //MODIFICATION FOR EXTENDED NIS FOR T6AN & T7AN

                switch ((int)C05C_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:


                        if ((C05B_CB == DigitalDoubleStatus.Close) && (C05B_DS1 == DigitalDoubleStatus.Close) && (C05B_DS2 == DigitalDoubleStatus.Close) &&
                            (C05A_CB == DigitalDoubleStatus.Close) && (C05A_DS1 == DigitalDoubleStatus.Close) && (C05A_DS2 == DigitalDoubleStatus.Close))
                        {
                            T6AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T6AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C05C_CB == DigitalDoubleStatus.Close) && (C05C_DS1 == DigitalDoubleStatus.Close) && (C05C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T6AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T6AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }



                switch ((int)C04A_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:
                        if ((C04A_CB == DigitalDoubleStatus.Close) && (C04A_DS1 == DigitalDoubleStatus.Close) && (C04A_DS2 == DigitalDoubleStatus.Close))
                        {
                            T7AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T7AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C04B_CB == DigitalDoubleStatus.Close) && (C04B_DS1 == DigitalDoubleStatus.Close) && (C04B_DS2 == DigitalDoubleStatus.Close) &&
                                (C04C_CB == DigitalDoubleStatus.Close) && (C04C_DS1 == DigitalDoubleStatus.Close) && (C04C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T7AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T7AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }

                //END OF MODIFICATION

                switch ((int)C05A_DS3)
                {
                    case 0:
                    case 1:
                    case 3:
                        break;
                    case 2:

                        if ((C05A_CB == DigitalDoubleStatus.Close) && (C05A_DS1 == DigitalDoubleStatus.Close) && (C05A_DS2 == DigitalDoubleStatus.Close))
                        {
                            T7AN_PRIMEVOLT = V400_1;
                            _logger.WriteEntry("T8AN_PRIMEVOLT (In Logic) : " + V400_1.ToString(), LogLevels.Trace);
                        }
                        else
                        {
                            if ((C05B_CB == DigitalDoubleStatus.Close) && (C05B_DS1 == DigitalDoubleStatus.Close) && (C05B_DS2 == DigitalDoubleStatus.Close) &&
                                (C05C_CB == DigitalDoubleStatus.Close) && (C05C_DS1 == DigitalDoubleStatus.Close) && (C05C_DS2 == DigitalDoubleStatus.Close))
                            {
                                T7AN_PRIMEVOLT = V400_2;
                                _logger.WriteEntry("T8AN_PRIMEVOLT (In Logic) : " + V400_2.ToString(), LogLevels.Trace);
                            }
                        }
                        break;
                }


                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T1AN_PRIMEVOLT"), (float)T1AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T1AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T2AN_PRIMEVOLT"), (float)T2AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T2AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T3AN_PRIMEVOLT"), (float)T3AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T3AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T4AN_PRIMEVOLT"), (float)T4AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T4AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T5AN_PRIMEVOLT"), (float)T5AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T5AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                //MODIFICATION FOR EXTENDED NIS FOR T6AN & T7AN
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T6AN_PRIMEVOLT"), (float)T6AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T6AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T7AN_PRIMEVOLT"), (float)T7AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T7AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("T8AN_PRIMEVOLT"), (float)T8AN_PRIMEVOLT))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: T8AN_PRIMEVOLT", LogLevels.Error);
                    return result;
                }


            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }

            return result;
        }

        private void SignDef(ref double dValue, string Sign)
        {
            try
            {

                switch (Sign)
                {
                    case "+":
                        if (dValue < 0)
                        {
                            dValue *= (-1);
                        }
                        break;
                    case "-":
                        if (dValue > 0)
                        {
                            dValue *= (-1);
                        }
                        break;
                }
            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
            }
        }


    }

}
