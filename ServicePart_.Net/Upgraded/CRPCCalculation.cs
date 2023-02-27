using Microsoft.VisualBasic;
using System;

namespace RPC_Service_App
{
	internal class CRPCCalculation
	{

		//*************************************************************************************
		// @author   Hesam Akbari
		// @version  1.0
		//
		// Development Environment       : MS-Visual Basic 6.0
		// Name of the Application       : RPC_Service.vbp
		// Creation/Modification History :
		//
		// Hesam Akbari       23-July-2007       Created
		//
		// Overview of Application       : In this class, all the functions and/or procedures
		//                                 that are needed for RPC calculations are available.
		//
		//
		//***************************************************************************************

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

				// The counters used in CVoltageController and CQController should be reset for the new 15Min period.
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

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo2, "CRPCCalculation.Preset1Min()", "--- Reset the Values in Cycle1 ---");

				result = true;

				// Set the progressive energies to 0
				Ea_TAV = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_TAV"), Conversion.Str(Ea_TAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Ea_TAV");
				}

				Er_TAV = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_TAV"), Conversion.Str(Er_TAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Er_TAV");
				}

				Ea_EAF = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_EAF"), Conversion.Str(Ea_EAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Ea_EAF");
				}

				Er_EAF = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_EAF"), Conversion.Str(Er_EAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Er_EAF");
				}

				Ea_PP = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_PP"), Conversion.Str(Ea_PP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Ea_PP");
				}
				Er_PP = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_PP"), Conversion.Str(Er_PP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Er_PP");
				}

				Ea_MF = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_MF"), Conversion.Str(Ea_MF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Ea_MF");
				}

				Er_MF = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_MF"), Conversion.Str(Er_MF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Er_MF");
				}

				Er_SVC = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_SVC"), Conversion.Str(Er_SVC)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: Er_SVC");
				}

				// Set the counters to 0.
				C1_1 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_1"), Conversion.Str(C1_1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C1_1");
				}

				C1_2 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_2"), Conversion.Str(C1_2)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C1_2");
				}

				C1_3 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_3"), Conversion.Str(C1_3)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C1_3");
				}

				C1_4 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_4"), Conversion.Str(C1_4)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C1_4");
				}

				C2_1 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_1"), Conversion.Str(C2_1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C2_1");
				}

				C2_2 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_2"), Conversion.Str(C2_2)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C2_2");
				}

				C2_3 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_3"), Conversion.Str(C2_3)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C2_3");
				}

				C2_4 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_4"), Conversion.Str(C2_4)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C2_4");
				}

				C5 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C5"), Conversion.Str(C5)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C5");
				}

				C6 = 0;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C6"), Conversion.Str(C6)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: C6");
				}

				// Reset the Suggestions
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK1"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK1");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK2"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK2");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK3"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK3");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK4"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK4");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK5"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK5");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK6"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK6");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK7"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK7");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK8"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK8");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK9");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_1"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK9_1");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_2"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK9_2");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK10"), Conversion.Str(1)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.Preset1Min()", "Could not update value in SCADA: MARK10");
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation..Preset1Min()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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
				Ea_TAV = Ea_TAV + m_theCRPCParameters.Ea_TAV_1 + m_theCRPCParameters.Ea_TAV_2;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_TAV"), Conversion.Str(Ea_TAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Ea_TAV");
					return result;
				}

				Er_TAV = Er_TAV + m_theCRPCParameters.Er_TAV_1 + m_theCRPCParameters.Er_TAV_2;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_TAV"), Conversion.Str(Er_TAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Er_TAV");
					return result;
				}


				// SVC
				Er_SVC = Er_SVC + m_theCRPCParameters.Er_SVC1 + m_theCRPCParameters.Er_SVC2;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_SVC"), Conversion.Str(Er_SVC)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Er_SVC");
					return result;
				}


				// EAF
				// Compensated
				// Ea_EAF = Ea_EAF + m_theCRPCParameters.Ea_EAF_T1AN + m_theCRPCParameters.Ea_EAF_T2AN + m_theCRPCParameters.Ea_EAF_T5AN
				// Er_EAF = Er_EAF + m_theCRPCParameters.Er_EAF_T1AN + m_theCRPCParameters.Er_EAF_T2AN + m_theCRPCParameters.Er_EAF_T5AN - Er_SVC

				// Without T5 to compare with the old system
				Ea_EAF = Ea_EAF + m_theCRPCParameters.Ea_EAF_T1AN + m_theCRPCParameters.Ea_EAF_T2AN + m_theCRPCParameters.Ea_EAF_T5AN + m_theCRPCParameters.Ea_EAF_T7AN;
				Er_EAF = Er_EAF + m_theCRPCParameters.Er_EAF_T1AN + m_theCRPCParameters.Er_EAF_T2AN + m_theCRPCParameters.Er_EAF_T5AN + m_theCRPCParameters.Er_EAF_T7AN - m_theCRPCParameters.Er_SVC1 - m_theCRPCParameters.Er_SVC2;

				switch(m_theCRPCParameters.MV3_CB)
				{
					case 0 : 
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.ProgressEnergyCalc()", "MV3_CB state is invalid!"); 
						break;
					case 2 : 
						Ea_EAF += m_theCRPCParameters.Ea_EAF_T3AN_MV3; 
						Er_EAF += m_theCRPCParameters.Er_EAF_T3AN_MV3; 
						break;
				}
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_EAF"), Conversion.Str(Ea_EAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Ea_EAF");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_EAF"), Conversion.Str(Er_EAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Er_EAF");
					return result;
				}

				// Uncompensated
				Ea_MF += m_theCRPCParameters.Ea_MF_1MinSum;
				Er_MF += m_theCRPCParameters.Er_MF_1MinSum;
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_MF"), Conversion.Str(Ea_MF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Ea_MF");
					return result;
				}
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_MF"), Conversion.Str(Er_MF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Er_MF");
					return result;
				}


				// PP
				Ea_PP = Ea_PP + m_theCRPCParameters.Ea_PP_T4AN + m_theCRPCParameters.Ea_PP_T6AN + m_theCRPCParameters.Ea_LF;
				Er_PP = Er_PP + m_theCRPCParameters.Er_PP_T4AN + m_theCRPCParameters.Er_PP_T6AN + m_theCRPCParameters.Er_LF;
				switch(m_theCRPCParameters.MZ3_CB)
				{
					case 0 : 
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.ProgressEnergyCalc()", "MZ3_CB state is invalid!"); 
						break;
					case 2 : 
						Ea_PP += m_theCRPCParameters.Ea_PP_T3AN_MZ3; 
						Er_PP += m_theCRPCParameters.Er_PP_T3AN_MZ3; 
						break;
				}
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Ea_PP"), Conversion.Str(Ea_PP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Ea_PP");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("Er_PP"), Conversion.Str(Er_PP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.ProgressEnergyCalc()", "Could not update value in SCADA: Er_PP");
					return result;
				}

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCCalculation.ProgressEnergyCalc()", "----- Progressive Energy Calculation -----");
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_TAV_1 = " + m_theCRPCParameters.Ea_TAV_1.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_TAV_2 = " + m_theCRPCParameters.Ea_TAV_2.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_TAV = " + Ea_TAV.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_TAV_1 = " + m_theCRPCParameters.Er_TAV_1.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_TAV_2 = " + m_theCRPCParameters.Er_TAV_2.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_TAV = " + Er_TAV.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF_T1AN = " + m_theCRPCParameters.Ea_EAF_T1AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF_T2AN = " + m_theCRPCParameters.Ea_EAF_T2AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF_T3AN_MV3 = " + m_theCRPCParameters.Ea_EAF_T3AN_MV3.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF_T5AN = " + m_theCRPCParameters.Ea_EAF_T5AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF_T7AN = " + m_theCRPCParameters.Ea_EAF_T7AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_EAF = " + Ea_EAF.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF_T1AN = " + m_theCRPCParameters.Er_EAF_T1AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF_T2AN = " + m_theCRPCParameters.Er_EAF_T2AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF_T3AN_MV3 = " + m_theCRPCParameters.Er_EAF_T3AN_MV3.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF_T5AN = " + m_theCRPCParameters.Er_EAF_T5AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF_T7AN = " + m_theCRPCParameters.Er_EAF_T7AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_SVC1 = " + m_theCRPCParameters.Er_SVC1.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_SVC2 = " + m_theCRPCParameters.Er_SVC2.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_SVC = " + Er_SVC.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_EAF = " + Er_EAF.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_PP_T3AN_MZ3 = " + m_theCRPCParameters.Ea_PP_T3AN_MZ3.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_PP_T4 = " + m_theCRPCParameters.Ea_PP_T4AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_PP_T6 = " + m_theCRPCParameters.Ea_PP_T6AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_PP = " + Ea_PP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_PP_T3AN_MZ3 = " + m_theCRPCParameters.Er_PP_T3AN_MZ3.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_PP_T4 = " + m_theCRPCParameters.Er_PP_T4AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_PP_T6 = " + m_theCRPCParameters.Er_PP_T6AN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_PP = " + Er_PP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Ea_MF = " + Ea_MF.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.ProgressEnergyCalc()", "Er_MF = " + Er_MF.ToString());
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation..ProgressEnergyCalc()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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
				Divisor = Math.Sqrt(m_theCRPCParameters.Ea_TAV * m_theCRPCParameters.Ea_TAV + m_theCRPCParameters.Er_TAV * m_theCRPCParameters.Er_TAV);
				if (Divisor == 0)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.CosPhiCalc()", "Cos_TAV --> Division by zero!");
				}
				else
				{
					COS_TAV = m_theCRPCParameters.Ea_TAV / Divisor;
				}

				if (m_theCRPCParameters.Er_TAV > 0)
				{
					SignDef(ref COS_TAV, "+");
				}
				else
				{
					SignDef(ref COS_TAV, "-");
				}

				// CosPhi EAF (Compensated)
				Divisor = Math.Sqrt(m_theCRPCParameters.Ea_EAF * m_theCRPCParameters.Ea_EAF + m_theCRPCParameters.Er_EAF * m_theCRPCParameters.Er_EAF);
				if (Divisor == 0)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.CosPhiCalc()", "COS_EAF --> Division by zero!");
				}
				else
				{
					COS_EAF = m_theCRPCParameters.Ea_EAF / Divisor;
				}

				if (m_theCRPCParameters.Er_EAF > 0)
				{
					SignDef(ref COS_EAF, "+");
				}
				else
				{
					SignDef(ref COS_EAF, "-");
				}

				// CosPhi EAF (Uncompensated)
				Divisor = Math.Sqrt(m_theCRPCParameters.Ea_MF * m_theCRPCParameters.Ea_MF + m_theCRPCParameters.Er_MF * m_theCRPCParameters.Er_MF);
				if (Divisor == 0)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.CosPhiCalc()", "COS_EAF_Uncompens --> Division by zero!");
				}
				else
				{
					COS_EAF_Uncompens = m_theCRPCParameters.Ea_MF / Divisor;
				}

				// CosPhi PP
				Divisor = Math.Sqrt(m_theCRPCParameters.Ea_PP * m_theCRPCParameters.Ea_PP + m_theCRPCParameters.Er_PP * m_theCRPCParameters.Er_PP);
				if (Divisor == 0)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCCalculation.CosPhiCalc()", "COS_PP --> Division by zero!");
				}
				else
				{
					COS_PP = m_theCRPCParameters.Ea_PP / Divisor;
				}

				if (m_theCRPCParameters.Er_PP > 0)
				{
					SignDef(ref COS_PP, "+");
				}
				else
				{
					SignDef(ref COS_PP, "-");
				}


				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("COS_TAV"), Conversion.Str(COS_TAV)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.CosPhiCalc()", "Could not update value in SCADA: COS_TAV");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("COS_EAF"), Conversion.Str(COS_EAF)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.CosPhiCalc()", "Could not update value in SCADA: COS_EAF");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("COS_EAF_Uncompens"), Conversion.Str(COS_EAF_Uncompens)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.CosPhiCalc()", "Could not update value in SCADA: COS_EAF_Uncompens");
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("COS_PP"), Conversion.Str(COS_PP)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.CosPhiCalc()", "Could not update value in SCADA: COS_PP");
				}


				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.CosPhiCalc()", "COS_TAV = " + COS_TAV.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.CosPhiCalc()", "COS_EAF = " + COS_EAF.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.CosPhiCalc()", "COS_EAF_Uncompens = " + COS_EAF_Uncompens.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.CosPhiCalc()", "COS_PP = " + COS_PP.ToString());
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation..CosPhiCalc()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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

				int C01A_CB = 0;
				int C01A_DS1 = 0;
				int C01A_DS2 = 0;
				int C01A_DS3 = 0;
				int C01B_CB = 0;
				int C01B_DS1 = 0;
				int C01B_DS2 = 0;
				int C01C_CB = 0;
				int C01C_DS1 = 0;
				int C01C_DS2 = 0;
				int C01C_DS3 = 0;

				int C02A_CB = 0;
				int C02A_DS1 = 0;
				int C02A_DS2 = 0;
				int C02B_CB = 0;
				int C02B_DS1 = 0;
				int C02B_DS2 = 0;
				int C02C_CB = 0;
				int C02C_DS1 = 0;
				int C02C_DS2 = 0;
				int C02C_DS3 = 0;

				int C03A_CB = 0;
				int C03A_DS1 = 0;
				int C03A_DS2 = 0;
				int C03B_CB = 0;
				int C03B_DS1 = 0;
				int C03B_DS2 = 0;
				int C03C_CB = 0;
				int C03C_DS1 = 0;
				int C03C_DS2 = 0;
				int C03C_DS3 = 0;

				int C04A_CB = 0;
				int C04A_DS1 = 0;
				int C04A_DS2 = 0;
				int C04B_CB = 0;
				int C04B_DS1 = 0;
				int C04B_DS2 = 0;
				int C04C_CB = 0;
				int C04C_DS1 = 0;
				int C04C_DS2 = 0;
				int C04C_DS3 = 0;
				int C04A_DS3 = 0;

				int C05A_CB = 0;
				int C05A_DS1 = 0;
				int C05A_DS2 = 0;
				int C05B_CB = 0;
				int C05B_DS1 = 0;
				int C05B_DS2 = 0;
				int C05C_CB = 0;
				int C05C_DS1 = 0;
				int C05C_DS2 = 0;
				int C05C_DS3 = 0;

				result = true;

				C01A_CB = m_theCRPCParameters.C01A_CB;
				C01A_DS1 = m_theCRPCParameters.C01A_DS1;
				C01A_DS2 = m_theCRPCParameters.C01A_DS2;
				C01A_DS3 = m_theCRPCParameters.C01A_DS3;
				C01B_CB = m_theCRPCParameters.C01B_CB;
				C01B_DS1 = m_theCRPCParameters.C01B_DS1;
				C01B_DS2 = m_theCRPCParameters.C01B_DS2;
				C01C_CB = m_theCRPCParameters.C01C_CB;
				C01C_DS1 = m_theCRPCParameters.C01C_DS1;
				C01C_DS2 = m_theCRPCParameters.C01C_DS2;
				C01C_DS3 = m_theCRPCParameters.C01C_DS3;

				C02A_CB = m_theCRPCParameters.C02A_CB;
				C02A_DS1 = m_theCRPCParameters.C02A_DS1;
				C02B_CB = m_theCRPCParameters.C02B_CB;
				C02B_DS1 = m_theCRPCParameters.C02B_DS1;
				C02B_DS2 = m_theCRPCParameters.C02B_DS2;
				C02C_CB = m_theCRPCParameters.C02C_CB;
				C02C_DS1 = m_theCRPCParameters.C02C_DS1;
				C02C_DS2 = m_theCRPCParameters.C02C_DS2;
				C02C_DS3 = m_theCRPCParameters.C02C_DS3;

				C03A_CB = m_theCRPCParameters.C03A_CB;
				C03A_DS1 = m_theCRPCParameters.C03A_DS1;
				C03B_CB = m_theCRPCParameters.C03B_CB;
				C03B_DS1 = m_theCRPCParameters.C03B_DS1;
				C03B_DS2 = m_theCRPCParameters.C03B_DS2;
				C03C_CB = m_theCRPCParameters.C03C_CB;
				C03C_DS1 = m_theCRPCParameters.C03C_DS1;
				C03C_DS2 = m_theCRPCParameters.C03C_DS2;
				C03C_DS3 = m_theCRPCParameters.C03C_DS3;

				C04A_CB = m_theCRPCParameters.C04A_CB;
				C04A_DS1 = m_theCRPCParameters.C04A_DS1;
				C04A_DS2 = m_theCRPCParameters.C04A_DS2;
				C04A_DS3 = m_theCRPCParameters.C04A_DS3;
				C04B_CB = m_theCRPCParameters.C04B_CB;
				C04B_DS1 = m_theCRPCParameters.C04B_DS1;
				C04B_DS2 = m_theCRPCParameters.C04B_DS2;
				C04C_CB = m_theCRPCParameters.C04C_CB;
				C04C_DS1 = m_theCRPCParameters.C04C_DS1;
				C04C_DS2 = m_theCRPCParameters.C04C_DS2;
				C04C_DS3 = m_theCRPCParameters.C04C_DS3;

				C05A_CB = m_theCRPCParameters.C05A_CB;
				C05A_DS1 = m_theCRPCParameters.C05A_DS1;
				C05A_DS2 = m_theCRPCParameters.C05A_DS2;
				C05B_CB = m_theCRPCParameters.C05B_CB;
				C05B_DS1 = m_theCRPCParameters.C05B_DS1;
				C05B_DS2 = m_theCRPCParameters.C05B_DS2;
				C05C_CB = m_theCRPCParameters.C05C_CB;
				C05C_DS1 = m_theCRPCParameters.C05C_DS1;
				C05C_DS2 = m_theCRPCParameters.C05C_DS2;
				C05C_DS3 = m_theCRPCParameters.C05C_DS3;

				// Bus 400 voltages
				V400_1 = m_theCRPCParameters.V400_1;
				V400_2 = m_theCRPCParameters.V400_2;

				// Set the default value
				T1AN_PRIMEVOLT = 0;
				T2AN_PRIMEVOLT = 0;
				T3AN_PRIMEVOLT = 0;
				T4AN_PRIMEVOLT = 0;
				T5AN_PRIMEVOLT = 0;
				T6AN_PRIMEVOLT = 0;
				T7AN_PRIMEVOLT = 0;

				switch(C01A_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C01A_CB == 2) && (C01A_DS1 == 2) && (C01A_DS2 == 2))
						{
							T1AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T1AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C01B_CB == 2) && (C01B_DS1 == 2) && (C01B_DS2 == 2) && (C01C_CB == 2) && (C01C_DS1 == 2) && (C01C_DS2 == 2))
							{
								T1AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T1AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}

				switch(C01C_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C01A_CB == 2) && (C01A_DS1 == 2) && (C01A_DS2 == 2) && (C01B_CB == 2) && (C01B_DS1 == 2) && (C01B_DS2 == 2))
						{
							T2AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T2AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C01C_CB == 2) && (C01C_DS1 == 2) && (C01C_DS2 == 2))
							{
								T2AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T2AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}

				switch(C02C_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C02A_CB == 2) && (C02A_DS1 == 2) && (C02A_DS2 == 2) && (C02B_CB == 2) && (C02B_DS1 == 2) && (C02B_DS2 == 2))
						{
							T3AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T3AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C02C_CB == 2) && (C02C_DS1 == 2) && (C02C_DS2 == 2))
							{
								T3AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T3AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}

				switch(C03C_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C03A_CB == 2) && (C03A_DS1 == 2) && (C03A_DS2 == 2) && (C03B_CB == 2) && (C03B_DS1 == 2) && (C03B_DS2 == 2))
						{
							T4AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T4AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C03C_CB == 2) && (C03C_DS1 == 2) && (C03C_DS2 == 2))
							{
								T4AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T4AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}

				switch(C04C_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C04A_CB == 2) && (C04A_DS1 == 2) && (C04A_DS2 == 2) && (C04B_CB == 2) && (C04B_DS1 == 2) && (C04B_DS2 == 2))
						{
							T5AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T5AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C04C_CB == 2) && (C04C_DS1 == 2) && (C04C_DS2 == 2))
							{
								T5AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T5AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}
				//MODIFICATION FOR EXTENDED NIS FOR T6AN & T7AN

				switch(C05C_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 

						 
						if ((C05B_CB == 2) && (C05B_DS1 == 2) && (C05B_DS2 == 2) && (C05A_CB == 2) && (C05A_DS1 == 2) && (C05A_DS2 == 2))
						{
							T6AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T6AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C05C_CB == 2) && (C05C_DS1 == 2) && (C05C_DS2 == 2))
							{
								T6AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T6AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}



				switch(C04A_DS3)
				{
					case 0 : case 1 : case 3 : 
						break;
					case 2 : 
						if ((C04A_CB == 2) && (C04A_DS1 == 2) && (C04A_DS2 == 2))
						{
							T7AN_PRIMEVOLT = V400_1;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T7AN_PRIMEVOLT (In Logic) : " + V400_1.ToString());
						}
						else
						{
							if ((C04B_CB == 2) && (C04B_DS1 == 2) && (C04B_DS2 == 2) && (C04C_CB == 2) && (C04C_DS1 == 2) && (C04C_DS2 == 2))
							{
								T7AN_PRIMEVOLT = V400_2;
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.TransPrimeVoltageCalc()", "T7AN_PRIMEVOLT (In Logic) : " + V400_2.ToString());
							}
						} 
						break;
				}

				//END OF MODIFICATION

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T1AN_PRIMEVOLT"), Conversion.Str(T1AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T1AN_PRIMEVOLT");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T2AN_PRIMEVOLT"), Conversion.Str(T2AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T2AN_PRIMEVOLT");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T3AN_PRIMEVOLT"), Conversion.Str(T3AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T3AN_PRIMEVOLT");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T4AN_PRIMEVOLT"), Conversion.Str(T4AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T4AN_PRIMEVOLT");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T5AN_PRIMEVOLT"), Conversion.Str(T5AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T5AN_PRIMEVOLT");
					return result;
				}

				//MODIFICATION FOR EXTENDED NIS FOR T6AN & T7AN
				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T6AN_PRIMEVOLT"), Conversion.Str(T6AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T6AN_PRIMEVOLT");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("T7AN_PRIMEVOLT"), Conversion.Str(T7AN_PRIMEVOLT)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: T7AN_PRIMEVOLT");
					return result;
				}

				//END OF MODIFICATION
				m_theCRPCParameters.UpdateRecordSet();
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation..TransPrimeVoltageCalc()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

			return result;
		}

		private void SignDef(ref double dValue, string Sign)
		{
			try
			{

				switch(Sign)
				{
					case "+" : 
						if (dValue < 0)
						{
							dValue *= (-1);
						} 
						break;
					case "-" : 
						if (dValue > 0)
						{
							dValue *= (-1);
						} 
						break;
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation..SignDef()", excep.Message);
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