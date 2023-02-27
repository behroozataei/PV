using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal partial class frmStartupRPC
		: System.Windows.Forms.Form
	{

		// Variables needed for timer service
		//UPGRADE_ISSUE: (2068) CxTimeService object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public UpgradeStubs.CxTimeService m_TimeService = null; // Time service base object
		//UPGRADE_ISSUE: (2068) CxTimer object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public UpgradeStubs.CxTimer m_Timer = null; // Triggers the timer events
		//UPGRADE_ISSUE: (2068) CxTimer object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public UpgradeStubs.CxTimer m_TimerOneShot = null; // Triggers the timer events

		// Variables need for ART
		//UPGRADE_ISSUE: (2068) CxArt object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public UpgradeStubs.CxArt ART = null; // Interface to Application Run-Time (ART)
		//UPGRADE_ISSUE: (2068) ARTLib.ProcessState object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private ARTLib.ProcessState eAppState = null; // Current ART state

		// Every PowerCC process must have a System Service Plug-In
		//UPGRADE_ISSUE: (2068) SYSSRVPLUGLib.IxSysSrvPlug2 object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private SYSSRVPLUGLib.IxSysSrvPlug2 SysSrvPlug = null;

		public CRPCManager theCRPCManager = null;

		private string ContextName = "";
		private int TempInteger = 0;
		public frmStartupRPC()
			: base()
		{
			if (m_vb6FormDefInstance == null)
			{
				if (m_InitializingDefInstance)
				{
					m_vb6FormDefInstance = this;
				}
				else
				{
					try
					{
						//For the start-up form, the first instance created is the default instance.
						if (System.Reflection.Assembly.GetExecutingAssembly().EntryPoint != null && System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType == this.GetType())
						{
							m_vb6FormDefInstance = this;
						}
					}
					catch
					{
					}
				}
			}
			//This call is required by the Windows Form Designer.
			InitializeComponent();
		}



		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load method and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
		private void Form_Load()
		{
			string strComputerName = new string('\0', 512);

			// Create a theCTraceLogger for any Logging, and Send dbConnection to TraceLogger
			GeneralModule.theCTraceLogger = new CTraceLogger();
			GeneralModule.theCTraceLogger.SetLogFile(CTraceLogger.TraceLogFile.TraceRPCService);

			strComputerName = StringsHelper.GetFixedLengthString(GeneralModule.GetMachineName(), 512);

			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", " ");
			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "", " ");
			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, " ", " ===================================================================================== ");
			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.Form_Load()", "Start of running on: " + strComputerName);

			// Initialize ART
			ART = new UpgradeStubs.CxArt();
			ReflectionHelper.Invoke(ART, "Init", new object[]{true});
			//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
			if (Information.Err().Number != 0)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC.Form_Load()", Information.Err().Description);
				return;
			}

			// Create System Service Plug-In
			SysSrvPlug = (IxSysSrvPlug2) new UpgradeStubs.CxSysSrvPlug();

			//
			TempInteger = 0;
			this.Hide();

		}

		// Release all of objects
		private void Form_Closed(Object eventSender, EventArgs eventArgs)
		{

			theCRPCManager = null;

			ReflectionHelper.Invoke(SysSrvPlug, "ReleaseAll", new object[]{});
			SysSrvPlug = null;

			ART = null;

			GeneralModule.theCTraceLogger = null;

		}


		//------------------------------------------------------------------------------
		// StartTimeServiceDemo - Initiates the timer
		//------------------------------------------------------------------------------
		public void StartTimeService()
		{

			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.StartTimeService()", "Start of Timer to work");

			int aSec = DateTimeHelper.Time.Second;
			if (aSec < 5)
			{
				ReflectionHelper.Invoke(m_TimerOneShot, "Start", new object[]{0, 0, 0, 5 - aSec, 0, false});
			}
			else
			{
				ReflectionHelper.Invoke(m_TimerOneShot, "Start", new object[]{0, 0, 1, 5 - aSec, 0, false});
			}

		}


		//------------------------------------------------------------------------------
		// StopTimeServiceDemo - Halts the timer
		//------------------------------------------------------------------------------
		public void StopTimeService()
		{

			GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.StopTimeService()", "End of Timer to work");
			ReflectionHelper.Invoke(m_Timer, "Stop", new object[]{});

		}


		//******************************************************************************
		// The following functions demonstrate the time service
		//******************************************************************************

		//------------------------------------------------------------------------------
		// Timer Service OnTimer - callback use by Timer Service when time interval
		//                         expires. Displays current time in local time and UTC.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (m_Timer_OnTimer) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void m_Timer_OnTimer()
		//{
			//
			// When this timer is treggered, runCyclic Operation will be called.
			//frmStartupRPC.DefInstance.theCRPCManager.RunCyclicOperation();
			//
		//}

		//UPGRADE_NOTE: (7001) The following declaration (m_TimerOneShot_OnTimer) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void m_TimerOneShot_OnTimer()
		//{
			//
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.m_TimerOneShot_OnTimer()", "Start of running");
			//
			// Trigger cyclic Timer
			//m_Timer = ReflectionHelper.Invoke<UpgradeStubs.CxTimer>(m_TimeService, "CreateTimer", new object[]{});
			//ReflectionHelper.Invoke(m_Timer, "Start", new object[]{0, 0, 0, 60, 0, true});
			//
			// The first trigerring of the program
			//frmStartupRPC.DefInstance.theCRPCManager.RunCyclicOperation();
			//
			//
			// Stop the one shot timer
			//ReflectionHelper.Invoke(m_TimerOneShot, "Stop", new object[]{});
			//
		//}


		//******************************************************************************
		// The following blocks of code are the required functions needed to
		// support the Application Run-Time (ART) callbacks.
		//******************************************************************************

		//------------------------------------------------------------------------------
		//   OnContextInit - ART calls this method when the application is required
		//                   to perform initialization. Typically in this method the
		//                   application initializes System Services it's going to
		//                   use for the specified context. The application must
		//                   then wait for the OnContextStart method to continue its
		//                   work.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnContextInit) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnContextInit(string strContext, ARTLib.ProcessState eState)
		//{
			//Call EventMsg.LogString(LET_INFORMATION, "MySource", "MyClass", "Method", 0, 2000, "OnContextInit")
			//Call EventMsg.LogString(LTT_INFO1, "MySource", "MyClass", "MyMethod", 0, 0, "OnContextInit")
			//
			// ART Context Initialization
			////UPGRADE_WARNING: (1068) eState of type ProcessState is being forced to ProcessState. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			////UPGRADE_WARNING: (1068) eAppState of type ProcessState is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			//ReflectionHelper.SetPrimitiveValue(eAppState, eState);
			////UPGRADE_WARNING: (1068) eAppState of type ProcessState is being forced to ProcessState. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			////UPGRADE_WARNING: (1068) eRPCState of type ProcessState is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			//ReflectionHelper.SetPrimitiveValue(GeneralModule.eRPCState, eAppState);
			//ContextName = strContext;
			//
			// Initialize Timer Service
			// Can only be done in OnContextInit
			//m_TimeService = new UpgradeStubs.CxTimeService();
			//ReflectionHelper.Invoke(ART, "InitializeServiceEx", new object[]{ContextName, m_TimeService});
			//
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnContextInit()", "ART: Initializing context");
			//
		//}


		//------------------------------------------------------------------------------
		//   OnContextStart - ART calls this method when its time for the
		//                    application to start working in the specified context.
		//                    ART also tells the application what mode it should start
		//                    working in: "Master" or "Standby". The IState parameter
		//                    can have one of the following values:
		//                    0 - the application starts the context in "Standby" mode.
		//                    1 - The application starts the context in "Master" mode.
		//
		//                    At this time the application MUST call UpdateState.
		//                    This notifies ART of application's present State.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnContextStart) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnContextStart(string strContext)
		//{
			//Call EventMsg.LogString(LET_INFORMATION, "MySource", "MyClass", "Method", 0, 2000, "OnContextStart")
			//Call EventMsg.LogString(LTT_INFO1, "MySource", "MyClass", "MyMethod", 0, 0, "OnContextStart")
			//
			// Create timer object
			//m_TimerOneShot = ReflectionHelper.Invoke<UpgradeStubs.CxTimer>(m_TimeService, "CreateTimer", new object[]{});
			//Set m_Timer = m_TimeService.CreateTimer()
			//
			// Notify ART the application Context is Active!
			//'eAppState = PCS_ACTIVE
			//ReflectionHelper.Invoke(ART, "UpdateState", new object[]{strContext, eAppState});
			//
			// Initialize RPCManager
			//theCRPCManager = new CRPCManager();
			//
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnContextStart()", "ART: Context is started");
			//
			//Timer1.Enabled = false;
			//RPCSupport.PInvoke.SafeNative.kernel32.Sleep(60000);
			//Timer1.Enabled = true;
			//
		//}


		//------------------------------------------------------------------------------
		// OnContextPause - When this method is called the application is supposed
		//                  to pause working for the specified context.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnContextPause) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnContextPause(string strContext)
		//{
			//
			// Process has been requested to PAUSE
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnContextPause()", "ART: Context has been paused");
			//
		//}// Open a connection in CDBInterface


		//------------------------------------------------------------------------------
		//   OnContextResume - When this method is called the application is to
		//                     resume working for the specified context.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnContextResume) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnContextResume(string strContext)
		//{
			//
			//Process has been requested to RESUME
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnContextResume()", "ART: Context has been resumed");
			//
		//}


		//------------------------------------------------------------------------------
		//   OnContextStop - When this method is called the application is to stop
		//                   working for the specified context.
		//
		//                   At this time the application MUST call UpdateState.
		//                   This notifies ART of application's present State.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnContextStop) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnContextStop(string strContext)
		//{
			//object PCS_CTX_STOPPED = null;
			//
			// Context has been STOPPED
			////UPGRADE_WARNING: (1068) PCS_CTX_STOPPED of type Variant is being forced to ProcessState. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			////UPGRADE_WARNING: (1068) eAppState of type ProcessState is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			//ReflectionHelper.SetPrimitiveValue(eAppState, PCS_CTX_STOPPED);
			////UPGRADE_WARNING: (1068) eAppState of type ProcessState is being forced to ProcessState. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			////UPGRADE_WARNING: (1068) eRPCState of type ProcessState is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			//ReflectionHelper.SetPrimitiveValue(GeneralModule.eRPCState, eAppState);
			//
			// Update state
			//ReflectionHelper.Invoke(ART, "UpdateState", new object[]{strContext, eAppState});
			//
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnContextStop", "ART: Context has been stopped on: " + GeneralModule.GetMachineName());
			//
		//}


		//------------------------------------------------------------------------------
		//   OnModeChange - ART calls this method in case of fail-over (recovery)
		//                  notification. The application must change its mode to a
		//                  new mode of operation ("Master" or "Standby").
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnModeChange) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnModeChange(string strContext, ARTLib.ProcessState eState)
		//{
			//
			// Context Mode has been changed
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnModeChange()", "ART: Mode has changed. Now I am " + GeneralModule.GetProcessState(eState));
			//
			////UPGRADE_WARNING: (1068) eState of type ProcessState is being forced to ProcessState. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			////UPGRADE_WARNING: (1068) eRPCState of type ProcessState is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
			//ReflectionHelper.SetPrimitiveValue(GeneralModule.eRPCState, eState);
			//
			////
			//ReflectionHelper.Invoke(ART, "Update", new object[]{GeneralModule.GetMachineName(), "RT", "MSC Model Functions", "MSC Functions HotStandby Group", "RPC Service", GeneralModule.eRPCState});
			//
		//}


		//------------------------------------------------------------------------------
		//   OnResObjectChange - ART calls this method when it gets a notification
		//                       about a resource object change. The application must
		//                       have previously subscribed to the resource object to
		//                       be notified of a resource object change
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnResObjectChange) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnResObjectChange(string strMachineName, string strPackage, string strProcess, string strSoftwareGroup, string strContext, int lState)
		//{
			//
			// Resource Object change has been detected
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnResObjectChange()", "ART: Resource object has changed: " + strMachineName + ", " + strPackage + ", " + strSoftwareGroup + ", " + strProcess);
			//
		//}


		//------------------------------------------------------------------------------
		//   OnTerminate - ART calls this method when processing of the application
		//                 has been terminated. The application should release all
		//                 resources that it may have allocated and exit.
		//------------------------------------------------------------------------------
		//UPGRADE_NOTE: (7001) The following declaration (ART_OnTerminate) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void ART_OnTerminate()
		//{
			//
			// Set the System Status to Terminated
			//if (theCRPCManager == null)
			//{
				//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC.ART_OnTerminate()", "theCRPCManager Is Nothing");
				//this.Close();
				//return;
			//}
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC.ART_OnTerminate()", "theCRPCManager Is ");
			//
			//if (theCRPCManager.m_theCRPCParameters == null)
			//{
				//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC.ART_OnTerminate()", "theCRPCManager.m_theCRPCParameters Is Nothing ");
				//this.Close();
				//return;
			//}
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC.ART_OnTerminate()", "theCRPCManager.m_theCRPCParameters Is ");
			//
			//
			// Open a connection in CDBInterface
			//if (!theCRPCManager.m_theCDBInterface.OpenConnection())
			//{
				//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "frmStartupRPC", " Error in connect to DB in frmStartupRPC.ART_OnTerminate()");
				//return;
			//}
			//
			// Close connection to DB in CDBInterface
			//theCRPCManager.m_theCDBInterface.CloseConnection();
			//
			//
			// Process has been requested to terminate
			//GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "frmStartupRPC.ART_OnTerminate()", "ART: Terminating application");
			//
			//this.Close();
			//
		//}


		private void Timer1_Tick(Object eventSender, EventArgs eventArgs)
		{
			try
			{
				theCRPCManager.Tap_Change();
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "Timer1", excep.Message);
			}
		}
		[STAThread]
		static void Main()
		{
			Application.Run(CreateInstance());
		}
	}
}