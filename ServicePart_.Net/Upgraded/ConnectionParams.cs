using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal static class GeneralModule
	{


		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("kernel32.dll", EntryPoint = "GetComputerNameA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetComputerName([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpBuffer, ref int nSize);

		// Genral object for Logging which is valid anywhere in the program
		public static CTraceLogger theCTraceLogger = null;

		// Variable needed for ART
		//UPGRADE_ISSUE: (2068) ARTLib.ProcessState object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public static ARTLib.ProcessState eRPCState = null;

		public static System.DateTime GMTApplyDate = DateTime.FromOADate(0);

		public static int GMTHourDiff = 0;

		public static int GMTMinuteDiff = 0;

		public static int GMTSecondDiff = 0;

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("Kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static void Sleep(int dwMiliseconds);


		internal static string GetProcessState(ARTLib.ProcessState eState)
		{
			object PCS_ACTIVE = null;
			object PCS_CTX_STOPPED = null;
			object PCS_DEAD = null;
			object PCS_DEGRADED = null;
			object PCS_FAILED_TO_START = null;
			object PCS_GOING_ACTIVE = null;
			object PCS_GOING_STANDBY = null;
			object PCS_JOIN_CTX = null;
			object PCS_NODE_DEAD = null;
			object PCS_PAUSE = null;
			object PCS_QUIT_CTX = null;
			object PCS_READY_TERMINATE = null;
			object PCS_REPORT_CTX_STOPPED = null;
			object PCS_RESUME = null;
			object PCS_SSR_LOCAL_DOWN = null;
			object PCS_SSR_LOCAL_MASTER = null;
			object PCS_SSR_LOCAL_STARTING = null;
			object PCS_SSR_LOCAL_STOP = null;
			object PCS_SSR_LOCAL_UP = null;
			object PCS_STANDBY = null;
			object PCS_START = null;
			object PCS_START_COMP = null;
			object PCS_STARTING = null;
			object PCS_STOP = null;
			object PCS_STOPPED = null;
			object PCS_SUBSCRIBE = null;

			if (eState == PCS_FAILED_TO_START)
			{
				return "PCS_FAILED_TO_START";
			}
			else if (eState == PCS_DEAD)
			{ 
				return "PCS_DEAD";
			}
			else if (eState == PCS_STARTING)
			{ 
				return "PCS_STARTING";
			}
			else if (eState == PCS_FAILED_TO_START)
			{ 
				return "PCS_FAILED_TO_START";
			}
			else if (eState == PCS_STOPPED)
			{ 
				return "PCS_STOPPED";
			}
			else if (eState == PCS_START_COMP)
			{ 
				return "PCS_START_COMP";
			}
			else if (eState == PCS_JOIN_CTX)
			{ 
				return "PCS_JOIN_CTX";
			}
			else if (eState == PCS_QUIT_CTX)
			{ 
				return "PCS_QUIT_CTX";
			}
			else if (eState == PCS_CTX_STOPPED)
			{ 
				return "PCS_CTX_STOPPED";
			}
			else if (eState == PCS_ACTIVE)
			{ 
				return "PCS_ACTIVE";
			}
			else if (eState == PCS_STANDBY)
			{ 
				return "PCS_STANDBY";
			}
			else if (eState == PCS_GOING_STANDBY)
			{ 
				return "PCS_GOING_STANDBY";
			}
			else if (eState == PCS_GOING_ACTIVE)
			{ 
				return "PCS_GOING_ACTIVE";
			}
			else if (eState == PCS_START)
			{ 
				return "PCS_START";
			}
			else if (eState == PCS_STOP)
			{ 
				return "PCS_STOP";
			}
			else if (eState == PCS_PAUSE)
			{ 
				return "PCS_PAUSE";
			}
			else if (eState == PCS_RESUME)
			{ 
				return "PCS_RESUME";
			}
			else if (eState == PCS_DEGRADED)
			{ 
				return "PCS_DEGRADED";
			}
			else if (eState == PCS_READY_TERMINATE)
			{ 
				return "PCS_READY_TERMINATE";
			}
			else if (eState == PCS_REPORT_CTX_STOPPED)
			{ 
				return "PCS_REPORT_CTX_STOPPED";
			}
			else if (eState == PCS_SSR_LOCAL_STARTING)
			{ 
				return "PCS_SSR_LOCAL_STARTING";
			}
			else if (eState == PCS_SSR_LOCAL_UP)
			{ 
				return "PCS_SSR_LOCAL_UP";
			}
			else if (eState == PCS_SSR_LOCAL_DOWN)
			{ 
				return "PCS_SSR_LOCAL_DOWN";
			}
			else if (eState == PCS_SSR_LOCAL_MASTER)
			{ 
				return "PCS_SSR_LOCAL_MASTER";
			}
			else if (eState == PCS_SSR_LOCAL_STOP)
			{ 
				return "PCS_SSR_LOCAL_STOP";
			}
			else if (eState == PCS_SUBSCRIBE)
			{ 
				return "PCS_SUBSCRIBE";
			}
			else if (eState == PCS_NODE_DEAD)
			{ 
				return "PCS_NODE_DEAD";
			}
			else
			{
				return " State is not Valid";
			}

		}

		internal static string GetMachineName()
		{
			string result = "";
			try
			{

				string strComputerName = "";
				string strBuffer = new string('\0', 512);
				int nLen = 0;

				nLen = Strings.Len(strBuffer);
				if (RPCSupport.PInvoke.SafeNative.kernel32.GetComputerName(ref strBuffer, ref nLen) != 0)
				{
					strBuffer = StringsHelper.GetFixedLengthString(strBuffer, 512);
					// Returns nonzero if successful, and modifies the length argument
					result = strBuffer.Substring(0, Math.Min(nLen, strBuffer.Length));
				}
				else
				{
					strBuffer = StringsHelper.GetFixedLengthString(strBuffer, 512);
				}
			}
			catch
			{
				result = " ";
			}

			return result;
		}

		internal static bool IsMachineMaster()
		{
			bool result = false;
			object PCS_ACTIVE = null;

			if (eRPCState == PCS_ACTIVE)
			{
				result = true;
			}
			else
			{
				result = false;
				theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "GeneralModule.IsMachineMaster", "The Process State is: " + GetProcessState(eRPCState));
			}

			return result;
		}

		internal static void Delay(int WaitSecond)
		{

			int StartSecond = DateTime.Now.Second;

			do 
			{
			}
			while(DateTime.Now.Second - StartSecond < WaitSecond);
		}
	}
}