using Microsoft.VisualBasic;
using System;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CRPCParameters
	{

		//*************************************************************************************
		// @author   Hesam Akbari
		// @version  1.0
		//
		// Development Environment       : MS-Visual Basic 6.0
		// Name of the Application       : RPC_Service_App.vbp
		// Creation/Modification History :
		//
		// Hesam Akbari       23-May-2007       Created
		//
		// Overview of Application       :
		//
		//
		//***************************************************************************************


		public enum enumSYSStatus
		{
			SYSStat_Unknown = 0,
			SYSStat_Started = 1,
			SYSStat_Terminated = 2
		}

		public enum enumRPCFStatus
		{
			RPCStat_Unknown = 0,
			RPCStat_Enabled = 2,
			RPCStat_Disabled = 1
		}

		// The maximum number of RPC Params which are listed in the T_CRPCPARAMS.
		private const int nPOINTS = 350;

		// Triple TAG, GUID, Value is stored in this array
		private string[, ] m_arrGUIDs = ArraysHelper.InitializeArray<string[, ]>(new int[]{nPOINTS + 1, 3}, new int[]{0, 0});


		private CDBInterface m_theDBConnection = null;
		private CSCADADataInterface m_theCSCADADataInterface = null;

		private string ServerName = "";

		private object Residual_Time = null;

		//Tag is used in VB code for using of one parameter.
		private string Tag = "";

		private string Description = "";

		//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private ADODB.Recordset m_RecSet = null;

		private bool m_IsSCADAFirstRun = false;

		// This function returns Value of parameters with name of "TAG" straight from SCADA.
		private string GetValueByTag(string strTagName)
		{
			string result = "";
			try
			{
				string strValue = "";
				bool aIsValid = false;

				result = "";

				if (!m_theCSCADADataInterface.ReadData(FindGUID(strTagName), ref strValue, ref aIsValid))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters.GetValueByTag()", "Unable to Read data from SCADA! - " + strTagName);
					return result;
				}


				return strValue;
			}
			catch (System.Exception excep)
			{
				//Call FileLog(" CRPCParameters..GetValueByTag(): " & Err.Description)
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GetValueByTag()", excep.Message + " - " + strTagName);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}
			return result;
		}

		//
		public int RPCStatus
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 1;
					strValue = GetValueByTag("RPCFSTATUS");
					if (strValue == "")
					{
						return result;
					}
					if (Conversion.Val(strValue) == ((int) enumRPCFStatus.RPCStat_Enabled))
					{
						result = 2;
					}
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..RPCStatus()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}

				return result;
			}
		}


		//
		public double V400_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("V400_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..V400_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double V400_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("V400_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..V400_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VEAF_A
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VEAF_A");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VEAF_A()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VEAF_B
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VEAF_B");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VEAF_B()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VPP_E
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VPP_E");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VPP_E()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VPP_F
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VPP_F");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VPP_F()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double V400_1_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("V400_1_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..V400_1_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double V400_2_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("V400_2_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..V400_2_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VEAF_A_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VEAF_A_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VEAF_A_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VEAF_B_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VEAF_B_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VEAF_B_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VPP_E_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VPP_E_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VPP_E_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VPP_F_Avg
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VPP_F_Avg");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VPP_F_Avg()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double HYST_0
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("HYST_0");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..HYST_0()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double HYST_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("HYST_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..HYST_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double HYST_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("HYST_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..HYST_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double HYST_3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("HYST_3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..HYST_3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_TAV_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_TAV_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_TAV_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_TAV_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_TAV_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_TAV_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_TAV_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_TAV_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_TAV_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_TAV_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_TAV_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_TAV_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double COS_TAV
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("COS_TAV");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..COS_TAV()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_EAF_T1AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF_T1AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF_T1AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_EAF_T2AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF_T2AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF_T2AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_EAF_T5AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF_T5AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF_T5AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		//
		public double Ea_EAF_T7AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF_T7AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF_T7AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}




		//
		public double Ea_EAF_T3AN_MV3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF_T3AN_MV3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF_T3AN_MV3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_EAF_T1AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF_T1AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF_T1AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_EAF_T2AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF_T2AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF_T2AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_EAF_T5AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF_T5AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF_T5AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		//
		public double Er_EAF_T7AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF_T7AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF_T7AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}




		//
		public double Er_EAF_T3AN_MV3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF_T3AN_MV3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF_T3AN_MV3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_SVC1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_SVC1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_SVC1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_SVC2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_SVC2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_SVC2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		// Status will be:
		//   0: Invalid status
		//   1: Open
		//   2: Close
		public int MV3_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MV3_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MV3_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		// Status will be:
		//   0: Invalid status
		//   1: Open
		//   2: Close
		public int MZ3_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MZ3_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MZ3_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double COS_EAF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("COS_EAF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..COS_EAF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double COS_EAF_Uncompens
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("COS_EAF_Uncompens");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..COS_EAF_Uncompens()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double COS_PP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("COS_PP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..COS_PP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_TAV
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_TAV");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_TAV()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		//
		public double Er_TAV
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_TAV");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_TAV()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_EAF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_EAF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_EAF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_EAF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_EAF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_EAF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_PP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_PP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_PP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_PP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_PP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_PP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_PP_T3AN_MZ3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_PP_T3AN_MZ3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_PP_T3AN_MZ3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_PP_T4AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_PP_T4AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_PP_T4AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}




		//
		public double Ea_PP_T6AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_PP_T6AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_PP_T6AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}






		//
		public double Er_PP_T3AN_MZ3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_PP_T3AN_MZ3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_PP_T3AN_MZ3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_PP_T4AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_PP_T4AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_PP_T4AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}





		//
		public double Er_PP_T6AN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_PP_T6AN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_PP_T6AN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}




		//
		public double Er_SVC
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_SVC");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_SVC()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_BANK
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_BANK");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_BANK()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_LF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_LF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_LF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_LF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_LF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_LF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_MF_1MinSum
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_MF_1MinSum");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_MF_1MinSum()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_MF_1MinSum
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_MF_1MinSum");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_MF_1MinSum()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Ea_MF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Ea_MF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Ea_MF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double Er_MF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("Er_MF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Er_MF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double K
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("K");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..K()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double M
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		// Status will be:
		//   0: Invalid status
		//   1: Open
		//   2: Close
		public int MT1A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT1A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT1A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		public int MT1A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT1A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT1A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		public int MT1A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT1A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT1A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		public int MT1A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT1A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT1A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		// Status will be:
		//   0: Invalid status
		//   1: Open
		//   2: Close
		public int MT2A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT2A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT2A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT2A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT2A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT2A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT2A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT2A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT2A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT2A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT2A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Conversion.Val(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT2A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double MT4A_VT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT4A_VT");
					if (strValue == "")
					{
						return result;
					}

					return Conversion.Val(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT4A_VT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double MT5A_VT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT5A_VT");
					if (strValue == "")
					{
						return result;
					}

					return Conversion.Val(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT5A_VT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP4
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP4");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP4()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP5
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP5");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP5()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP6
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP6");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP6()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP7
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP7");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP7()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP8
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP8");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP8()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP9
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP9");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP9()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP10
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP10");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP10()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP11
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP11");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP11()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP12
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP12");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP12()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP13
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP13");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP13()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP14
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP14");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP14()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP15
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP15");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP15()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP16
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP16");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP16()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP17
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP17");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP17()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP18
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP18");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP18()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VTAP19
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VTAP19");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VTAP19()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VR_EAF
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VR_EAF");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VR_EAF()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VR_PP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VR_PP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VR_PP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double VR_TAV
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("VR_TAV");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..VR_TAV()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GEN1_AUTO
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GEN1_AUTO");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GEN1_AUTO()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GEN2_AUTO
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GEN2_AUTO");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GEN2_AUTO()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GEN3_AUTO
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GEN3_AUTO");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GEN3_AUTO()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double NGEN_AUTO
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("NGEN_AUTO");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..NGEN_AUTO()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QGEN1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QGEN1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QGEN1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QGEN2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QGEN2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QGEN2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QGEN3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QGEN3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QGEN3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QGEN4
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QGEN4");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QGEN4()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QGEN_AUTO
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QGEN_AUTO");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QGEN_AUTO()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QMILL
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QMILL");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QMILL()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double K1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("K1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..K1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double K2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("K2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..K2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QFIN
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QFIN");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QFIN()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QROUGH
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QROUGH");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QROUGH()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double QTANDEM
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("QTANDEM");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..QTANDEM()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C1_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C1_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C1_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C1_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C1_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C1_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C1_3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C1_3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C1_3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C1_4
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C1_4");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C1_4()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C2_1
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C2_1");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C2_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C2_2
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C2_2");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C2_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C2_3
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C2_3");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C2_3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C2_4
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C2_4");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C2_4()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C5
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C5");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C5()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double C6
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C6");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C6()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T1AN_TAP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T1AN_TAP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T1AN_TAP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T2AN_TAP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T2AN_TAP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T2AN_TAP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T3AN_TAP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T3AN_TAP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T3AN_TAP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MV3_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MV3_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MV3_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MV3_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MV3_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MV3_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MV3_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MV3_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MV3_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MZ3_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MZ3_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MZ3_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MZ3_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MZ3_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MZ3_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MZ3_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MZ3_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MZ3_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT3_DS
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT3_DS");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT3_DS()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T4AN_TAP
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T4AN_TAP");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T4AN_TAP()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT4A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT4A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT4A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT4A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT4A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT4A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT4A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT4A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT4A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT4A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT4A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT4A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MP4_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MP4_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MP4_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MP4_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MP4_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MP4_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MP4_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MP4_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MP4_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M1P_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M1P_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M1P_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M1P_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M1P_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M1P_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M1P_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M1P_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M1P_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T5AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T5AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T5AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		//
		public double T6AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T6AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T6AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		//
		public double T7AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T7AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T7AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		public double T3AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T3AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T3AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		public double T2AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T2AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T2AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		//
		public double T1AN_TAP_NEW
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T1AN_TAP_NEW");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T1AN_TAP_NEW()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		public int GMF1_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GMF1_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GMF1_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GMF1_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GMF1_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GMF1_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GMF1_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GMF1_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GMF1_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int GMF1_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("GMF1_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..GMF1_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M51_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M51_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M51_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M51_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M51_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M51_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int M51_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("M51_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..M51_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT5A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT5A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT5A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT5A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT5A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT5A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT5A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT5A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT5A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MT5A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MT5A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MT5A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T1AN_PRIMEVOLT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T1AN_PRIMEVOLT");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T1AN_PRIMEVOLT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T2AN_PRIMEVOLT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T2AN_PRIMEVOLT");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T2AN_PRIMEVOLT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T3AN_PRIMEVOLT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T3AN_PRIMEVOLT");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T3AN_PRIMEVOLT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T4AN_PRIMEVOLT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T4AN_PRIMEVOLT");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T4AN_PRIMEVOLT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double T5AN_PRIMEVOLT
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("T5AN_PRIMEVOLT");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..T5AN_PRIMEVOLT()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01B_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01B_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01B_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01B_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01B_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01B_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01B_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01B_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01B_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01C_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01C_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01C_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01C_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01C_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01C_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01C_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01C_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01C_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C01C_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C01C_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C01C_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02B_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02B_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02B_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02B_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02B_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02B_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02B_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02B_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02B_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02C_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02C_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02C_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02C_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02C_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02C_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02C_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02C_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02C_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C02C_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C02C_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C02C_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03B_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03B_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03B_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03B_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03B_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03B_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03B_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03B_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03B_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03C_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03C_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03C_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03C_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03C_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03C_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03C_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03C_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03C_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C03C_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C03C_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C03C_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		//
		public int C04A_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04A_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04A_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}

		//
		public int C04A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04B_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04B_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04B_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04B_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04B_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04B_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04B_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04B_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04B_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04C_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04C_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04C_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04C_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04C_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04C_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04C_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04C_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04C_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C04C_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C04C_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C04C_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MAC_B
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MAC_B");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MAC_B()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MBD_B
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MBD_B");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MBD_B()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public double FuncCycle
		{
			get
			{
				double result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("FuncCycle");
					if (strValue == "")
					{
						return result;
					}

					return Double.Parse(strValue);
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..FuncCycle()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int RPCAlarm
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("RPCAlarm");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..RPCAlarm()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK4
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK4");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK4()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK5
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK5");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK5()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK6
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK6");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK6()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK7
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK7");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK7()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK8
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK8");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK8()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK9
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK9");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK9()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK9_1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK9_1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK9_1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK9_2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK9_2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK9_2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MARK10
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MARK10");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MARK10()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int MAB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("MAB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..MAB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}









		public int C05A_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05A_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05A_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05A_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05A_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05A_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05A_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05A_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05A_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}



		//
		public int C05B_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05B_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05B_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05B_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05B_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05B_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05B_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05B_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05B_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05C_CB
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05C_CB");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05C_CB()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05C_DS1
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05C_DS1");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05C_DS1()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05C_DS2
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05C_DS2");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05C_DS2()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		//
		public int C05C_DS3
		{
			get
			{
				int result = 0;
				try
				{
					string strValue = "";

					result = 0;
					strValue = GetValueByTag("C05C_DS3");
					if (strValue == "")
					{
						return result;
					}

					return Convert.ToInt32(Double.Parse(strValue));
				}
				catch (System.Exception excep)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..C05C_DS3()", excep.Message);
					//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
				}
				return result;
			}
		}


		public CRPCParameters()
		{
			try
			{

				//
				m_IsSCADAFirstRun = false;

				// Log Creating CRPCParameters
				if (GeneralModule.theCTraceLogger != null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCParameters.Class_Initialize()", " Creating Instance ");
				}

				for (int i = 0; i <= (nPOINTS - 1); i++)
				{
					m_arrGUIDs[i, 1] = " ";
					m_arrGUIDs[i, 2] = " ";
				}
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCParameters..Class_Initialize()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		~CRPCParameters()
		{
			try
			{

				// Log Creating CRPCParameters
				if (GeneralModule.theCTraceLogger != null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCParameters.Class_Terminate()", " Destroying Instance ");
				}

				// m_RecSet.Close
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCParameters..Class_Terminate()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		public bool ReadRPCParameters()
		{
			bool bRetFlag = false;
			try
			{
				string strSQL = "";
				int i = 0;

				bRetFlag = true;

				// Prepare str SQL for read from Table
				strSQL = "SELECT * FROM Model.V_GET_RPCPARAMS";
				if (m_theDBConnection == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters.ReadRPCParameters()", "Could not access to connection");
					return false;
				}

				if (!m_theDBConnection.runQuery(strSQL, ref m_RecSet))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters.ReadRPCParameters()", "Could not read Parameters Table Model.V_GET_RPCPARAMS");
					return false;
				}

				ReflectionHelper.Invoke(m_RecSet, "MoveFirst", new object[]{});
				//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to string. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				strSQL = ReflectionHelper.GetPrimitiveValue<string>(m_RecSet("NetworkPath"));
				//Call theCTraceLogger.WriteLog(TraceInfo1, m_RecSet("NetworkPath"), m_RecSet("GUID"))

				while(!ReflectionHelper.GetMember<bool>(m_RecSet, "EOF"))
				{
					//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to string. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					m_arrGUIDs[i, 1] = ReflectionHelper.GetPrimitiveValue<string>(m_RecSet("TAG"));
					//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to string. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					m_arrGUIDs[i, 2] = ReflectionHelper.GetPrimitiveValue<string>(m_RecSet("GUID"));
					//MsgBox i & "  " & m_arrGUIDs(i, 1)
					//Call theCTraceLogger.WriteLog(TraceInfo1, "CDCParameters.readDCParameters()", m_arrGUIDs(i, 1) & " - " & m_arrGUIDs(i, 2))
					i++;

					ReflectionHelper.Invoke(m_RecSet, "MoveNext", new object[]{});
				};


				// Updating the Recordset
				ReflectionHelper.Invoke(m_RecSet, "Resync", new object[]{});



				return bRetFlag;
			}
			catch (System.Exception excep)
			{

				bRetFlag = false;
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..ReadRPCParameters()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}
			return false;
		}

		public bool ReadGMTDiff()
		{
			bool result = false;
			try
			{
				string strSQL = "";

				result = true;

				// Prepare str SQL for reading GMT Diffference
				strSQL = "SELECT * FROM Model.T_CGMTDIFF";
				if (m_theDBConnection == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "COCPParameters.ReadGMTDiff()", "Could not access to connection");
					return false;
				}

				if (!m_theDBConnection.runQuery(strSQL, ref m_RecSet))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "COCPParameters.ReadGMTDiff()", "Could not read Parameters Table Model.T_CGMTDIFF");
					return false;
				}

				GeneralModule.GMTHourDiff = -3;
				GeneralModule.GMTMinuteDiff = -30;
				GeneralModule.GMTSecondDiff = 0;

				//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to DateTime. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				GeneralModule.GMTApplyDate = ReflectionHelper.GetPrimitiveValue<System.DateTime>(m_RecSet("APPLYDATE"));

				if (((int) DateAndTime.DateDiff("s", GeneralModule.GMTApplyDate, DateTime.Now.AddSeconds(-DateTime.Now.Second), FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1)) >= 0)
				{
					//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to int. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					GeneralModule.GMTHourDiff = ReflectionHelper.GetPrimitiveValue<int>(m_RecSet("HOURDIFF"));
					//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to int. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					GeneralModule.GMTMinuteDiff = ReflectionHelper.GetPrimitiveValue<int>(m_RecSet("MINUTEDIFF"));
					//UPGRADE_WARNING: (1068) m_RecSet() of type Recordset is being forced to int. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					GeneralModule.GMTSecondDiff = ReflectionHelper.GetPrimitiveValue<int>(m_RecSet("SECONDDIFF"));
				}


				// Updating the Recordset
				ReflectionHelper.Invoke(m_RecSet, "Resync", new object[]{});
			}
			catch (System.Exception excep)
			{

				result = false;
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..ReadGMTDiff()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


			return result;
		}

		// In the start of each function, the SCADAConnection must be set
		public void SetSCADAConnection(CSCADADataInterface aCSCADADataInterface)
		{
			m_theCSCADADataInterface = aCSCADADataInterface;
		}

		// In the start of each function, the connection must be set
		public void SetDBConnection(CDBInterface aDBConnection)
		{
			m_theDBConnection = aDBConnection;
		}

		// This method is called after any update in tables
		public bool UpdateRecordSet()
		{
			ReflectionHelper.Invoke(m_RecSet, "Resync", new object[]{});
			return false;
		}

		// This function returns "TAG" field of parameters with name of "GUID"
		public string FindTAG(string strGUIDName)
		{
			string result = "";
			try
			{

				result = " ";
				for (int i = 0; i <= (nPOINTS - 1); i++)
				{
					if (m_arrGUIDs[i, 2] == strGUIDName)
					{
						return m_arrGUIDs[i, 1];
					}
				}

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters.FindTAG()", " Error in finding " + strGUIDName);

				return "";
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..Findtag() for ", strGUIDName + " " + excep.Message);
			}
			return result;
		}

		// This function returns "GUID" field of parameters with name of "TAG"
		public string FindGUID(string strTagName)
		{
			string result = "";
			try
			{

				result = " ";
				for (int i = 0; i <= (nPOINTS - 1); i++)
				{
					if (m_arrGUIDs[i, 1] == strTagName)
					{
						//Call theCTraceLogger.WriteLog(Traceinfo1, "CRPCParameters.FindGUID()", " " & strTagName)
						return m_arrGUIDs[i, 2];
					}
				}

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters.FindGUID()", " Error in finding " + strTagName);

				return "";
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCParameters..FindGUID() for ", strTagName + " " + excep.Message);
			}
			return result;
		}
	}
}