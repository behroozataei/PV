using Microsoft.VisualBasic;
using System;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CLimitChecker
	{


		private CRPCParameters m_theCRPCParameters = null;

		private CSCADADataInterface m_theCSCADADataInterface = null;

		private bool[] QLimitViolApp = new bool[3];


		public CLimitChecker()
		{

			m_theCSCADADataInterface = new CSCADADataInterface();

			for (int i = 1; i <= 2; i++)
			{
				QLimitViolApp[i] = false;
			}

		}

		// Busbars Voltage Limit Checking --> EAF, PP, 400kV
		// This method reads the 3Min averages values and writes to SCADA.
		// The PowerCC will itself check the limit checking.
		public bool VoltageLimitChecking()
		{
			bool result = false;
			try
			{


				double[] VoltageValue = new double[7];

				System.DateTime dtFrom = DateTime.FromOADate(0);
				System.DateTime dtTo = DateTime.FromOADate(0);
				System.DateTime aDate = DateTime.FromOADate(0);
				object aTime = null;
				object strD = null;
				object strM = null;
				string strY = "";
				object strH = null;
				object strMi = null;
				string strS = "";

				int i = 0;

				result = true;

				// Calculate time range:

				// The difference is for Greenwich Mean Time (GMT) differential
				dtTo = DateTime.Now.AddHours(GeneralModule.GMTHourDiff);
				dtTo = dtTo.AddMinutes(GeneralModule.GMTMinuteDiff);
				dtTo = dtTo.AddSeconds(GeneralModule.GMTSecondDiff);
				// Adjust the time to the begining of the period
				dtTo = dtTo.AddSeconds(-dtTo.Second);

				//dtFrom = DateAdd("n", -3, dtTo)

				// Retreiving a value for the previous 3 minutes from HIS:
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CLimitChecker.VoltageLimitChecking", "Average Sampling Time=" + DateTimeHelper.ToString(dtTo));

				VoltageValue[1] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("V400_1"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				VoltageValue[2] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("V400_2"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				VoltageValue[3] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VEAF_A"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				VoltageValue[4] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VEAF_B"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				VoltageValue[5] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VPP_E"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				VoltageValue[6] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VPP_F"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);

				// The Alarming in the Alarm List is done by the SCADA.

				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("V400_1_Avg"), Conversion.Str(VoltageValue[1])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: V400_1_Avg");
				}
				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("V400_2_Avg"), Conversion.Str(VoltageValue[2])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: V400_2_Avg");
				}
				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("VEAF_A_Avg"), Conversion.Str(VoltageValue[3])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: VEAF_A_Avg");
				}
				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("VEAF_B_Avg"), Conversion.Str(VoltageValue[4])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: VEAF_B_Avg");
				}
				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("VPP_E_Avg"), Conversion.Str(VoltageValue[5])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: VPP_E_Avg");
				}
				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("VPP_F_Avg"), Conversion.Str(VoltageValue[6])))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCCalculation.TransPrimeVoltageCalc()", "Could not update value in SCADA: VPP_F_Avg");
				}


				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCCalculation.VoltageLimitChecking()", "----- 3 Min Average Voltages -----");
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "V400_1_Avg = " + VoltageValue[1].ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "V400_2_Avg = " + VoltageValue[2].ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "VEAF_A_Avg = " + VoltageValue[3].ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "VEAF_B_Avg = " + VoltageValue[4].ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "VPP_E_Avg  =  " + VoltageValue[5].ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CRPCCalculation.VoltageLimitChecking()", "VPP_F_Avg  =  " + VoltageValue[6].ToString());
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLimitChecker..VoltageLimitChecking()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

			return result;
		}

		// Q-SVC Checking
		public bool QLimitChecking()
		{
			bool result = false;
			try
			{

				double K = 0;
				double M = 0;
				string Er_EAF = "";
				double Er_SVC = 0;
				string Er_BANK = "";
				double Er_LF = 0;


				result = true;

				Er_EAF = m_theCRPCParameters.Er_EAF.ToString();
				Er_SVC = m_theCRPCParameters.Er_SVC;
				K = m_theCRPCParameters.K;

				if (Er_SVC > Double.Parse(Er_EAF) + K)
				{
					QLimitViolApp[1] = true;
					// Sending Alarm
					if (!m_theCSCADADataInterface.SendAlarm("RPCAlarm", "Er_SVC=" + Er_SVC.ToString() + ">" + "Er_EAF + K=" + (Double.Parse(Er_EAF) + K).ToString()))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CLimitChecker.QLimitChecking()", "Sending alarm failed.");
					}
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CLimitChecker.QLimitChecking()", "Limit Violation Appeared --> Er_SVC = " + Er_SVC.ToString() + " > Er_EAF + K = " + (Double.Parse(Er_EAF) + K).ToString());
				}
				else
				{
					if (QLimitViolApp[1])
					{
						QLimitViolApp[1] = false;
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CLimitChecker.QLimitChecking()", "Limit Violation Disappeared --> Er_SVC = " + Er_SVC.ToString() + " <= Er_EAF + K = " + (Double.Parse(Er_EAF) + K).ToString());
					}
				}

				// In the documents ther's a section that checks Capacitor Bank with LFS,
				// but in the perl codes there's not such a section.
				Er_BANK = m_theCRPCParameters.Er_BANK.ToString();
				Er_LF = m_theCRPCParameters.Er_LF;
				M = m_theCRPCParameters.M;

				if (StringsHelper.ToDoubleSafe(Er_BANK) > Er_LF + M)
				{
					QLimitViolApp[2] = true;
					// Sending Alarm
					if (!m_theCSCADADataInterface.SendAlarm("RPCAlarm", "Er_BANK=" + Er_BANK + ">" + "Er_LF + M=" + (Er_LF + M).ToString()))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CLimitChecker.QLimitChecking()", "Sending alarm failed.");
					}
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CLimitChecker.QLimitChecking()", "Limit Violation Appeared --> Er_BANK = " + Er_BANK + " > Er_LF + M = " + (Er_LF + M).ToString());
				}
				else
				{
					if (QLimitViolApp[2])
					{
						QLimitViolApp[2] = false;
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CLimitChecker.QLimitChecking()", "Limit Violation Disappeared --> Er_BANK = " + Er_BANK + " <= Er_LF + M = " + (Er_LF + M).ToString());
					}
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CLimitChecker..QLimitChecking()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


			return result;
		}

		public void SettheRPCParam(CRPCParameters aCRPCParameters)
		{
			m_theCRPCParameters = aCRPCParameters;
		}
	}
}