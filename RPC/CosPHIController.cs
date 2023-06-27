using System;
using System.Collections.Generic;
using System.Text;



using Irisa.Logger;
using Irisa.Message.CPS;

namespace RPC
{
    internal class CosPHIController
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;



        internal CosPHIController(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
        }

        private double UnbReactEnergy = 0;


        public bool CosPhiControl(int CycleNo)
        {
            bool result = false;
            try
            {

                double COS_TAV = 0;
                double COS_EAF = 0;
                double COS_PP = 0;
                double Er_UnbTAV = 0;
                double Er_UnbEAF = 0;
                double Er_UnbPP = 0;

                result = true;

                // Er_UnbTAV = 0
                // Er_UnbEAF = 0
                // Er_UnbPP = 0


                COS_TAV = _repository.GetRPCScadaPoint("COS_TAV").Value;
                COS_EAF = _repository.GetRPCScadaPoint("COS_EAF").Value;
                COS_PP = _repository.GetRPCScadaPoint("COS_PP").Value;

                if (COS_TAV >= 0.85d)
                {
                    Er_UnbTAV = 0;

                    if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), (float)0))
                    {
                        _logger.WriteEntry("Could not update value in SCADA: " + "MARK9", LogLevels.Error);
                    }

                    if (COS_EAF >= 0.85d)
                    {
                        Er_UnbEAF = 0;
                    }
                    else
                    {
                        _logger.WriteEntry("COS_EAF < 0.85", LogLevels.Info);
                        if (COS_EAF != 0)
                        {
                            CalcUnbReactEnergy(COS_EAF, _repository.GetRPCScadaPoint("Ea_EAF").Value);
                            Er_UnbEAF = UnbReactEnergy;
                        }
                        else
                        {
                            Er_UnbEAF = 0;
                        }
                    }

                    if (COS_PP >= 0.85d)
                    {
                        Er_UnbPP = 0;
                    }
                    else
                    {
                        _logger.WriteEntry("COS_PP < 0.85", LogLevels.Info);
                        if (COS_PP != 0)
                        {
                            CalcUnbReactEnergy(COS_PP, _repository.GetRPCScadaPoint("Ea_PP").Value);
                            Er_UnbPP = UnbReactEnergy;
                        }
                        else
                        {
                            Er_UnbPP = 0;
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry("COS_TAV < 0.85", LogLevels.Info);
                    if (COS_TAV != 0)
                    {
                        CalcUnbReactEnergy(COS_TAV, _repository.GetRPCScadaPoint("Ea_TAV").Value);
                        Er_UnbTAV = UnbReactEnergy;
                    }
                    else
                    {
                        Er_UnbTAV = 0;
                    }
                    if (COS_EAF >= 0.85d)
                    {
                        Er_UnbEAF = 0;
                    }
                    else
                    {
                        _logger.WriteEntry("COS_EAF < 0.85", LogLevels.Info);
                        if (COS_EAF != 0)
                        {
                            CalcUnbReactEnergy(COS_EAF, _repository.GetRPCScadaPoint("Ea_EAF").Value);
                            Er_UnbEAF = UnbReactEnergy;
                        }
                        else
                        {
                            Er_UnbEAF = 0;
                        }
                    }

                    if (COS_PP >= 0.85d)
                    {
                        Er_UnbPP = 0;
                        if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 0))
                        {
                            _logger.WriteEntry("Could not update value in SCADA: " + "MARK9", LogLevels.Error);
                        }
                    }
                    else
                    {
                        _logger.WriteEntry("COS_PP < 0.85", LogLevels.Info);
                        if (COS_PP != 0)
                        {
                            CalcUnbReactEnergy(COS_PP, _repository.GetRPCScadaPoint("Ea_PP").Value);
                            Er_UnbPP = UnbReactEnergy;
                        }
                        else
                        {
                            Er_UnbPP = 0;
                        }
                        // Check PP Busbar
                        if (CycleNo == 10 || CycleNo == 13)
                        {
                            if (_repository.GetRPCScadaPoint("QGEN_AUTO").Value <= _repository.GetRPCScadaPoint("NGEN_AUTO").Value * _repository.GetRPCScadaPoint("K2").Value + _repository.GetRPCScadaPoint("QMILL").Value)
                            {
                                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 1))
                                {
                                    _logger.WriteEntry("Could not update value in SCADA: " + "MARK9", LogLevels.Error);
                                }
                                if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "SUGGESTION: INCREASE SET OF GENERATORS"))
                                {
                                    _logger.WriteEntry("Sending alarm failed.", LogLevels.Info);
                                }
                                _logger.WriteEntry("SUGGESTION: INCREASE SET OF GENERATORS.", LogLevels.Info);
                            }
                            else
                            {
                                // DCO JOB? (POWFAC)

                            }
                        }
                    }

                }

                _logger.WriteEntry("Er_UnbTAV = " + Er_UnbTAV.ToString(), LogLevels.Info);
                _logger.WriteEntry("Er_UnbEAF = " + Er_UnbEAF.ToString(), LogLevels.Info);
                _logger.WriteEntry("Er_UnbPP  = " + Er_UnbPP.ToString(), LogLevels.Info);

                // Update the values in the SCADA

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_UnbTAV"), (float)Er_UnbTAV))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_UnbTAV", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_UnbEAF"), (float)Er_UnbEAF))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_UnbEAF", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("Er_UnbPP"), (float)Er_UnbPP))
                {
                    result = false;
                    _logger.WriteEntry("Could not update value in SCADA: Er_UnbPP", LogLevels.Error);
                }
            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }
            return result;
        }

        private void CalcUnbReactEnergy(double CosPhiActual, double ActiveEnergyActual)
        {
            try
            {

                double ReactEnergyActual = 0;
                double ReactEnergy85 = 0;

                ReactEnergyActual = Math.Sqrt(Math.Pow(ActiveEnergyActual / CosPhiActual, 2) - Math.Pow(ActiveEnergyActual, 2));
                ReactEnergy85 = Math.Sqrt(Math.Pow(ActiveEnergyActual / Math.Cos(0.85d), 2) - Math.Pow(ActiveEnergyActual, 2));

                UnbReactEnergy = ReactEnergyActual - ReactEnergy85;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error);
            }
        }
    }
}

