using Microsoft.VisualBasic;
using System;

namespace RPC_Service_App
{
	internal class CCosPHIController
	{


		private CRPCParameters m_theCRPCParameters = null;
		private CSCADADataInterface _m_theSCADADataInterface = null;
		private CSCADADataInterface m_theSCADADataInterface
		{
			get
			{
				if (_m_theSCADADataInterface == null)
				{
					_m_theSCADADataInterface = new CSCADADataInterface();
				}
				return _m_theSCADADataInterface;
			}
			set
			{
				_m_theSCADADataInterface = value;
			}
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


				COS_TAV = m_theCRPCParameters.COS_TAV;
				COS_EAF = m_theCRPCParameters.COS_EAF;
				COS_PP = m_theCRPCParameters.COS_PP;

				if (COS_TAV >= 0.85d)
				{
					Er_UnbTAV = 0;

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "1"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: " + "MARK9");
					}

					if (COS_EAF >= 0.85d)
					{
						Er_UnbEAF = 0;
					}
					else
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "COS_EAF < 0.85");
						if (COS_EAF != 0)
						{
							CalcUnbReactEnergy(COS_EAF, m_theCRPCParameters.Ea_EAF);
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
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "COS_PP < 0.85");
						if (COS_PP != 0)
						{
							CalcUnbReactEnergy(COS_PP, m_theCRPCParameters.Ea_PP);
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
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "COS_TAV < 0.85");
					if (COS_TAV != 0)
					{
						CalcUnbReactEnergy(COS_TAV, m_theCRPCParameters.Ea_TAV);
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
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "COS_EAF < 0.85");
						if (COS_EAF != 0)
						{
							CalcUnbReactEnergy(COS_EAF, m_theCRPCParameters.Ea_EAF);
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
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: " + "MARK9");
						}
					}
					else
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "COS_PP < 0.85");
						if (COS_PP != 0)
						{
							CalcUnbReactEnergy(COS_PP, m_theCRPCParameters.Ea_PP);
							Er_UnbPP = UnbReactEnergy;
						}
						else
						{
							Er_UnbPP = 0;
						}
						// Check PP Busbar
						if (CycleNo == 10 || CycleNo == 13)
						{
							if (m_theCRPCParameters.QGEN_AUTO <= m_theCRPCParameters.NGEN_AUTO * m_theCRPCParameters.K2 + m_theCRPCParameters.QMILL)
							{
								if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "2"))
								{
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: " + "MARK9");
								}
								if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCREASE SET OF GENERATORS"))
								{
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "Sending alarm failed.");
								}
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCosPhiController.CosPhiControl()", "SUGGESTION: INCREASE SET OF GENERATORS.");
							}
							else
							{
								// DCO JOB? (POWFAC)

							}
						}
					}

				}

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CCosPhiController.CosPhiControl()", "Er_UnbTAV = " + Er_UnbTAV.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CCosPhiController.CosPhiControl()", "Er_UnbEAF = " + Er_UnbEAF.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CCosPhiController.CosPhiControl()", "Er_UnbPP  = " + Er_UnbPP.ToString());

				// Update the values in the SCADA

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_UnbTAV"), Conversion.Str(Er_UnbTAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: Er_UnbTAV");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_UnbEAF"), Conversion.Str(Er_UnbEAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: Er_UnbEAF");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_UnbPP"), Conversion.Str(Er_UnbPP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPhiController.CosPhiControl()", "Could not update value in SCADA: Er_UnbPP");
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPHIController..CosPHIControl()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCosPHIController..CalcUnbReactEnergy()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		public void SettheRPCParam(CRPCParameters aCRPCParameters)
		{
			m_theCRPCParameters = aCRPCParameters;
		}
	}
}