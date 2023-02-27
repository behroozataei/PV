using Microsoft.VisualBasic;
using System;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CCycleValidator
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
		//   This class is used for calculating CycleNo,
		//
		//***************************************************************************************

		private object[] m_Cycles = new object[16]; //Array for 15 minutes period

		private int m_CycleNo = 0;

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

		// An instance of CDBInterface for load and save data from/of T_CRPCCycles
		private CDBInterface m_theDBConnection = null;


		public bool GetRPCCycleNo()
		{
			bool result = false;
			try
			{

				object vTime = null;
				int FuncCycle = 0;
				int i = 0;

				// Suppose everything is Ok.
				result = true;

				// Load previous values into array
				if (!LoadRPCCycles())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.GetRPCCycleNo", "Could not load data from T_CRPCCycles!");
					return false;
				}

				vTime = DateTime.Now;

				m_CycleNo = ReflectionHelper.GetPrimitiveValue<System.DateTime>(vTime).Minute % 15 + 1; // Cycles begin from 1 to 15

				FuncCycle = ((m_CycleNo - 1) / 3) * 3;
				if (FuncCycle == 0)
				{
					FuncCycle = 15;
				}

				if (!m_theCSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("FuncCycle"), Conversion.Str(FuncCycle)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCycleValidator.GetRPCCycleNo()", "Could not update value in SCADA: FuncCycle");
				}

				if (m_CycleNo == 1)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCyleValidator.GetRPCCycleNo", " In the first cycle (Cycle 1), " + Conversion.Str(vTime));
				}

				//UPGRADE_WARNING: (1068) vTime of type Variant is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				m_Cycles[m_CycleNo] = ReflectionHelper.GetPrimitiveValue(vTime);

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCyleValidator.GetRPCCycleNo() ", "   Cycle Number is:                   " + Conversion.Str(m_CycleNo));

				if (m_CycleNo > 1)
				{
					result = IsContiniuousCycles(m_CycleNo);
					if (!result)
					{
						if (!m_theCSCADADataInterface.SendAlarm("RPCAlarm", "RPC Function is not running continuouslyx"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CCyleValidator.GetRPCCycleNo()", "Sending alarm failed.");
						}
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CCyleValidator.GetRPCCycleNo", "Function is not running continuously!");
						return result;
					}
				}

				// Save the last values into array
				if (!SaveRPCCycles())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.GetRPCCycleNo", "Could not save data into T_CRPCCycles!");
					return false;
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator..GetRPCCycleNo", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


			return result;
		}

		//
		public int CycleNo
		{
			get
			{

				return m_CycleNo;

			}
		}


		//
		private bool IsContiniuousCycles(int nCurCycle)
		{
			bool result = false;
			int CurrHour = 0;
			int PrevHour = 0;
			int PrevMin = 0;
			int CurrMin = 0;
			System.DateTime PrevDate = DateTime.FromOADate(0);
			System.DateTime CurrDate = DateTime.FromOADate(0);

			result = true;

			// Check continuosely in activation times
			int tempForEndVar = nCurCycle;
			for (int i = 2; i <= tempForEndVar; i++)
			{ // m_CycleNo
				PrevDate = DateTime.Parse(ReflectionHelper.GetPrimitiveValue<string>(m_Cycles[i - 1]));
				CurrDate = DateTime.Parse(ReflectionHelper.GetPrimitiveValue<string>(m_Cycles[i]));

				PrevHour = DateAndTime.Hour(ReflectionHelper.GetPrimitiveValue<System.DateTime>(m_Cycles[i - 1]));
				PrevMin = ReflectionHelper.GetPrimitiveValue<System.DateTime>(m_Cycles[i - 1]).Minute;

				CurrHour = DateAndTime.Hour(ReflectionHelper.GetPrimitiveValue<System.DateTime>(m_Cycles[i]));
				CurrMin = ReflectionHelper.GetPrimitiveValue<System.DateTime>(m_Cycles[i]).Minute;

				if ((PrevHour != CurrHour) || ((PrevMin + 1) != CurrMin) || (PrevDate != CurrDate))
				{
					result = false;
				}
			}
			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CCyleValidator.IsContiniuousCycles(), ", result.ToString());

			return result;
		}

		//
		private bool LoadRPCCycles()
		{
			bool result = false;
			object ADODB = null;
			try
			{
				string strSQL = "";
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset tempRecSet = null;

				result = false;

				if (m_theDBConnection == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.LoadRPCCycles()", "Could not access to connection");
					return result;
				}

				strSQL = "SELECT * FROM MODEL.T_CRPCCYCLES";
				if (!m_theDBConnection.runQuery(strSQL, ref tempRecSet))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.LoadRPCCycles()", "Could not read RPC Cycles");
					return result;
				}

				ReflectionHelper.Invoke(tempRecSet, "MoveFirst", new object[]{});
				for (int i = 1; i <= 15; i++)
				{
					//UPGRADE_WARNING: (1068) tempRecSet() of type Recordset is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
					m_Cycles[i] = ReflectionHelper.GetPrimitiveValue(tempRecSet("CYCLE_" + i.ToString()));
				}


				return true;
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator..LoadRPCCycles", excep.Message);
			}
			return result;
		}

		//
		private bool SaveRPCCycles()
		{
			bool result = false;
			object ADODB = null;
			try
			{

				string strSQL = "";
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset tempRecSet = null;

				if (m_theDBConnection == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.SaveRPCCycles()", "Could not access to connection!");
					return false;
				}

				strSQL = "UPDATE Model.T_CRPCCycles SET ";
				for (int i = 1; i <= 14; i++)
				{
					strSQL = strSQL + "CYCLE_" + i.ToString() + " = '" + ReflectionHelper.GetPrimitiveValue<string>(m_Cycles[i]) + "', ";
				}
				strSQL = strSQL + "CYCLE_" + "15" + " = '" + ReflectionHelper.GetPrimitiveValue<string>(m_Cycles[15]) + "' ";

				if (!m_theDBConnection.runQuery(strSQL, ref tempRecSet))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator.SaveRPCCycles()", "Could not save RPC Cycle Table");
					return false;
				}

				return true;
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CCyleValidator..SaveRPCCycles", excep.Message);
				result = false;
			}
			return result;
		}


		// In the start of each function, the connection must be set
		public void SetDBConnection(CDBInterface aDBConnection)
		{

			m_theDBConnection = aDBConnection;

		}

		public void SettheRPCParam(CRPCParameters aCRPCParameters)
		{
			m_theCRPCParameters = aCRPCParameters;
		}
	}
}