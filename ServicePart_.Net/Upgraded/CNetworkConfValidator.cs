using Microsoft.VisualBasic;
using System;

namespace RPC_Service_App
{
	internal class CNetworkConfValidator
	{


		private CRPCParameters m_theCRPCParameters = null;

		private CSCADADataInterface _m_theCSCADADataInterface = null;
		private CSCADADataInterface m_theCSCADADataInterface
		{
			get
			{
				if (_m_theCSCADADataInterface == null)
				{
					_m_theCSCADADataInterface = new CSCADADataInterface();
				}
				return _m_theCSCADADataInterface;
			}
			set
			{
				_m_theCSCADADataInterface = value;
			}
		}


		// This method checks the running condition of RPC.
		public bool isAdmittedNetConf()
		{
			bool result = false;
			try
			{

				if ((m_theCRPCParameters.MAB == 1 && ((m_theCRPCParameters.MAC_B == 1 && m_theCRPCParameters.MBD_B == 2) || (m_theCRPCParameters.MAC_B == 2 && m_theCRPCParameters.MBD_B == 1))) || (m_theCRPCParameters.MAC_B == 1 && m_theCRPCParameters.MBD_B == 1))
				{
					result = true;
				}
				else
				{
					result = false;
					if (!m_theCSCADADataInterface.SendAlarm("RPCAlarm", "Network Configuration is Not Admitted"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CNetworkConfValidator.isAdmittedNetConf()", "Sending alarm failed.");
					}
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CNetworkConfValidator..isAdmittedNetConf()", excep.Message);
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