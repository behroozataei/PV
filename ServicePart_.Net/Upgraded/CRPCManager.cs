using Microsoft.VisualBasic;
using System;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CRPCManager
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

		private const int RPC_TIMER_TICKS = 60000;

		public CRPCParameters m_theCRPCParameters = null;

		public CNetworkConfValidator m_theCNetworkConfValidator = null;

		public CCycleValidator m_theCCycleValidator = null;

		public CDBInterface m_theCDBInterface = null;

		public CSCADADataInterface m_theCSCADADataInterface = null;

		private CRPCCalculation m_theCRPCCalculation = null;

		private CLimitChecker m_theCLimitChecker = null;

		private CVoltageController m_theCVoltageController = null;

		private CQController m_theCQController = null;

		private CCosPHIController m_theCCosPhiController = null;

		private int m_timerID = 0;

		private CSCADADataInterface _m_Scada_Alarm = null;
		private CSCADADataInterface m_Scada_Alarm
		{
			get
			{
				if (_m_Scada_Alarm == null)
				{
					_m_Scada_Alarm = new CSCADADataInterface();
				}
				return _m_Scada_Alarm;
			}
			set
			{
				_m_Scada_Alarm = value;
			}
		}


		private double TempInteger_T1_TAP = 0;

		private double TempInteger_T2_TAP = 0;

		private double TempInteger_T3_TAP = 0;

		private double TempInteger_T5_TAP = 0;

		private double TempInteger_T6_TAP = 0;

		private double TempInteger_T7_TAP = 0;



		public void RunCyclicOperation()
		{
			try
			{

				int aCycleNo = 0;

				// Open a connection in CDBInterface
				if (!m_theCDBInterface.OpenConnection())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager.RunCyclicOperation", " Error in connect to DB in CRPCManager.RunCyclicOperation()");
					return;
				}


				// Update setting for writing logs
				GeneralModule.theCTraceLogger.Read_LogSetting("RPC");

				// Log activation of RPC
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, " ", "   -----------------------------------------------------------------------  ");
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.RunCyclicOperation", "Enter to method on: " + GeneralModule.GetMachineName());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.RunCyclicOperation", " Machine State is: " + GeneralModule.GetProcessState(GeneralModule.eRPCState));


				// Reading Static values for parameters from Tables
				if (!m_theCRPCParameters.ReadRPCParameters())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager.RunCyclicOperation", "Reading RPC Parameters is not successful");
					m_theCDBInterface.CloseConnection();
					return;
				}

				// Stop the process if it is not Enable
				if (m_theCRPCParameters.RPCStatus != 2)
				{
					if (!m_theCSCADADataInterface.SendAlarm("RPCAlarm", "RPC Function is Disabled"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation()", "Sending alarm failed.");
					}
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.RunCyclicOperation", "Function is Disabled!");
					m_theCDBInterface.CloseConnection();
					return;
				}

				// Network configuration should be checked here
				if (!m_theCNetworkConfValidator.isAdmittedNetConf())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.RunCyclicOperation", "Network Configuration is Not Admitted!");
					return;
				}

				// Get right CycleNo to start of 1-minute processing:
				if (!m_theCCycleValidator.GetRPCCycleNo())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "Error in Cyclic Operation!");
					m_theCDBInterface.CloseConnection();
					return;
				}
				else
				{
					aCycleNo = m_theCCycleValidator.CycleNo;
				}

				if (aCycleNo == 1)
				{
					if (!m_theCRPCParameters.ReadGMTDiff())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager.RunCyclicOperation", "Reading GMT Difference Parameters is not successful");
					}
				}

				// ----------------------------------------------------

				// For Every 1 Minute
				if (!m_theCRPCCalculation.ProgressEnergyCalc())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "ProgressEnergyCalc() does not work!");
					return;
				}

				if (!m_theCRPCCalculation.TransPrimeVoltageCalc())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "TransPrimeVoltageCalc() could not be completed!");
					return;
				}

				// For Every 3 Minutes
				if ((aCycleNo - 1) % 3 == 0)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo3, "CRPCManager.RunCyclicOperation", "       ----- 3 Minute Cycle -----");

					// CosPhi Limit Checking is done automatically in PowerCC
					if (!m_theCRPCCalculation.CosPhiCalc())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "CosPhiCalc() does not work!");
						return;
					}

					// After calculating the 15Min CosPhi, Reset the energies and counters.
					if (aCycleNo == 1)
					{
						if (!m_theCRPCCalculation.Preset1Min())
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "Preset1Min() could not be completed!");
							return;
						}
					}

					// Limit Checking
					if (!m_theCLimitChecker.VoltageLimitChecking())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "VoltageLimitChecking() does not work!");
					}

					if (!m_theCLimitChecker.QLimitChecking())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "QLimitChecking() does not work!");
					}

					// Voltage Control
					if (!m_theCVoltageController.VoltageControl())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "VoltageControl() does not work!");
					}

					// CosPhi Control
					if (!m_theCCosPhiController.CosPhiControl(aCycleNo))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "CosPhiControl() does not work!");
					}

					// Q of Generators Control
					if (!m_theCQController.QControl())
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CRPCManager.RunCyclicOperation", "QControl() does not work!");
					}

				}

				// ----------------------------------------------------

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.RunCyclicOperation", "Exit of method.");

				// Close connection to DB in CDBInterface
				m_theCDBInterface.CloseConnection();
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager..RunCyclicOperation()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		// Important Note:
		//   Creating of all objects are accomplished here,
		//   and they will be destroyed only at the end of CRPCManager.
		public CRPCManager()
		{
			try
			{

				// Create new object of CDBInterface
				m_theCDBInterface = new CDBInterface();
				if (!m_theCDBInterface.OpenConnection())
				{
					m_theCDBInterface = null;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CRPCManager.Class_Initialize", " Error in connect to DB in CRPCManager.Class_Initialize()");
					return;
				}

				// Send dbConnection to TraceLogger
				GeneralModule.theCTraceLogger.SetDBConnection(m_theCDBInterface);

				// Create a timer that sends a notification every RPC_TIMER_TICKS milliseconds.
				frmStartupRPC.DefInstance.StartTimeService();

				// Create new object of the classes
				m_theCSCADADataInterface = new CSCADADataInterface();

				m_theCRPCParameters = new CRPCParameters();
				m_theCRPCParameters.SetDBConnection(m_theCDBInterface);
				m_theCRPCParameters.SetSCADAConnection(m_theCSCADADataInterface);

				m_theCNetworkConfValidator = new CNetworkConfValidator();
				m_theCNetworkConfValidator.SettheRPCParam(m_theCRPCParameters);

				m_theCCycleValidator = new CCycleValidator();
				m_theCCycleValidator.SettheRPCParam(m_theCRPCParameters);
				m_theCCycleValidator.SetDBConnection(m_theCDBInterface);

				m_theCRPCCalculation = new CRPCCalculation();
				m_theCRPCCalculation.SettheRPCParam(m_theCRPCParameters);

				m_theCLimitChecker = new CLimitChecker();
				m_theCLimitChecker.SettheRPCParam(m_theCRPCParameters);

				m_theCVoltageController = new CVoltageController();
				m_theCVoltageController.SettheRPCParam(m_theCRPCParameters);

				m_theCQController = new CQController();
				m_theCQController.SettheRPCParam(m_theCRPCParameters);

				m_theCCosPhiController = new CCosPHIController();
				m_theCCosPhiController.SettheRPCParam(m_theCRPCParameters);


				// Close the connection
				m_theCDBInterface.CloseConnection();
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager..Class_Initialize()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		~CRPCManager()
		{
			try
			{

				// For Test
				m_theCDBInterface.CloseConnection();


				// Stopping cyclic timer
				frmStartupRPC.DefInstance.StopTimeService();

				// Destroy objects
				m_theCDBInterface = null;

				m_theCCycleValidator = null;

				m_theCRPCParameters = null;

				//Set m_theCEnergyCalculator = Nothing
			}
			catch (System.Exception excep)
			{
				if (GeneralModule.theCTraceLogger != null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CRPCManager..Class_Terminate()", excep.Message);
				}
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		public void Tap_Change()
		{
			try
			{

				//UPGRADE_ISSUE: (2068) AnalogMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AnalogMeasurement AnalogPoint = null;
				AnalogPoint = new UpgradeStubs.AnalogMeasurement();
				//UPGRADE_ISSUE: (2068) DigitalMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DigitalMeasurement DigitalPoint = null;
				DigitalPoint = new UpgradeStubs.DigitalMeasurement();
				string TempString = "";

				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T1AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T1AN TAP ANALOG", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "TempInteger_T1_TAP", "VALUE=" + TempInteger_T1_TAP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", "");
				if (Math.Abs(ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T1_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T1AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");

					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T1AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T1AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}
					}
					else
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T1AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T1AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}
					}
					TempInteger_T1_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}


				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T2AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T2AN TAP ANALOG", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "TempInteger_T2_TAP", "VALUE=" + TempInteger_T2_TAP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", "");
				if (Math.Abs(ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T2_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T2AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");

					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{

						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T2AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T2AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}

					}
					else
					{

						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T2AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T2AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}
					}

					TempInteger_T2_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}


				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T3AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T3AN TAP ANALOG", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "TempInteger_T3_TAP", "VALUE=" + TempInteger_T3_TAP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", "");
				if (Math.Abs(ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T3_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T3AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");
					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T3AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T3AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}
					}
					else
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T3AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T3AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))).ToString());
						}
					}
					TempInteger_T3_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}


				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T5AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T5AN TAP ANALOG", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "TempInteger_T5_TAP", "VALUE=" + TempInteger_T5_TAP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", "");
				if (Math.Abs(ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T5_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T5AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");

					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{

						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T5AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T5AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					else
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T5AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T5AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					TempInteger_T5_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}

				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T6AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T6_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T6AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");

					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{

						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T6AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T6AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					else
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T6AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T6AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					TempInteger_T6_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}

				ReflectionHelper.LetMember(AnalogPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T7AN_TAP_NEW"));
				ReflectionHelper.Invoke(AnalogPoint, "FetchValue", new object[]{});
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T7AN TAP ANALOG", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "TempInteger_T7_TAP", "VALUE=" + TempInteger_T7_TAP.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", "");
				if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - TempInteger_T7_TAP) > 0.4d)
				{
					ReflectionHelper.LetMember(DigitalPoint, "ObjectIdStr", m_theCRPCParameters.FindGUID("T7AN_TAP_NEW_ALARM"));
					TempString = ReflectionHelper.GetMember<string>(DigitalPoint, "ObjectPath");

					if ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") - ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")))) > 0.5d)
					{

						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T7AN TAP CHANGED!", ((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T7AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					else
					{
						if (m_Scada_Alarm.SendAlarm_With_Value(TempString, "T7AN TAP CHANGED!", (ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "T7AN TAP CHANGED!", "VALUE=" + ReflectionHelper.GetMember<string>(AnalogPoint, "Value"));
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "FIX VALUE", "VALUE=" + (((ReflectionHelper.GetMember<double>(AnalogPoint, "Value") > 0) ? Math.Floor(ReflectionHelper.GetMember<double>(AnalogPoint, "Value")) : Math.Ceiling(ReflectionHelper.GetMember<double>(AnalogPoint, "Value"))) + 1).ToString());
						}
					}
					TempInteger_T7_TAP = ReflectionHelper.GetMember<double>(AnalogPoint, "Value");
				}

				AnalogPoint = null;
				DigitalPoint = null;
			}
			catch
			{
				//MsgBox "TAP_CHANGE()" & Err.Description
			}
		}
	}
}