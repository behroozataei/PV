using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CTraceLogger
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

		const bool MODE_DEBUG = true;

		const int OUTPUTLOGFILE = 1;
		const int OUTPUTLOGTABLE = 2;
		const int OUTPUTLOGEVENTVIEWER = 4;
		const int OUTPUTLOGSPCLOGGER = 8;


		// Suitable RTS Address
		// Also for the AIO from 2007-10-17

		// Old Address:
		// E:\IRISA\RPC\Log\

		const string LOGFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\RPC_Log\\";

		const string LOGTRACEFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\RPC_Log\\";

		const string LOGFILEPATHDESKAPP = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\RPC_Log\\";


		public enum TraceType
		{
			TraceError = 1, // All types of errors, system or function errors
			TraceWarning = 2, // All types of warnings, system or function warnings
			TraceValue = 4, //   Write ant value
			TraceInfo1 = 8, //   Enter/Exit from a method,
			TraceInfo2 = 16, //
			TraceInfo3 = 32, //
			TraceInfo4 = 64 //
		}


		//Enum TraceLogFile
		//TraceEECService = 1
		//TraceEECDeskApp
		//End Enum

		public enum TraceLogFile
		{
			TraceRPCService = 3,
			TraceRPCDeskApp
		}

		private string m_TodayDate = "";
		private TraceLogFile m_OutLogFile = (TraceLogFile) 0;

		private int m_lOutputTraceType = 0;
		private int m_lOutputPath = 0;

		private string m_strOutputFilePath = "";
		private int m_nFileOutput = 0;

		private string m_strLogFilePath = "";
		private string m_strLogFilePathDeskApp = "";

		private CDBInterface m_theDBInterface = null;

		// For sending events to Windows Event Viewer
		//UPGRADE_ISSUE: (2068) DIAGNOSTICSLib.DiagMsgLog object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		public DIAGNOSTICSLib.DiagMsgLog WinEventMsg = null;


		public CTraceLogger()
		{
			object DIAGNOSTICSLib = null;

			// For Logging the RPC Service events
			//m_strOutputFilePath = LOGFILEPATH
			//m_nFileOutput = FreeFile

			//Open m_strOutputFilePath For Append As #m_nFileOutput

			// For Logging the Desktop Application events
			//m_strOutputFilePathDeskApp = LOGFILEPATHDESKAPP
			//m_nFileOutputDeskApp = FreeFile

			//Open m_strOutputFilePathDeskApp For Append As #m_nFileOutputDeskApp

			// Create the Windows Event Msg Object
			WinEventMsg = new DIAGNOSTICSLib.DiagMsgLog();

			// Creating the log file path
			m_TodayDate = StringsHelper.Format(DateTime.Now, "yyyy_mm_dd");
			string strTodayLog = "_" + m_TodayDate + ".txt";
			CreateLogPath(strTodayLog);

			// Set for writing to File and Table
			m_lOutputPath = 0;
			m_lOutputPath = m_lOutputPath | OUTPUTLOGFILE;
			//'m_lOutputPath = m_lOutputPath Or OUTPUTLOGTABLE

			// Set for writing all event types
			m_lOutputTraceType = 0;
			m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceError);
			m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceWarning);
			m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceInfo1);
			m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceValue);

		}

		//
		private void CreateLogPath(string strTodayLog)
		{

			m_strLogFilePath = LOGFILEPATH + "RPC_Log" + strTodayLog;
			m_strLogFilePathDeskApp = LOGFILEPATHDESKAPP + "RPC_Desktop_Log" + strTodayLog;

		}

		//
		// This function reads the setting for generating logs from T_CLogSetting
		public bool Read_LogSetting(string strFuncName)
		{
			bool result = false;
			object ADODB = null;
			try
			{
				string strSQL = "";
				//UPGRADE_ISSUE: (2068) Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.Recordset tempRecSet = null;

				result = false;

				//
				strSQL = "Select * From Model.T_CLOGSETTING Where FUNCTIONNAME = '" + strFuncName + "'";

				if (m_theDBInterface == null)
				{
					WriteLog(TraceType.TraceError, "CTRaceLogger.Read_LogSetting()", "Could not access to connection");
					return result;
				}

				ADODB.Recordset tempRefParam = tempRecSet;
				if (!m_theDBInterface.runQuery(strSQL, ref tempRefParam))
				{
					tempRecSet = tempRefParam;
					WriteLog(TraceType.TraceError, "CTRaceLogger.Read_LogSetting()", "Could not read LogSetting Values");
					return result;
				}
				else
				{
					tempRecSet = tempRefParam;
				}

				ReflectionHelper.Invoke(tempRecSet, "MoveFirst", new object[]{});

				// Set for writing all event types
				m_lOutputTraceType = 0;
				if (ReflectionHelper.GetMember<string>(tempRecSet("ERROR"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceError);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("WARNING"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceWarning);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("VALUE"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceValue);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("INFO1"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceInfo1);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("INFO2"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceInfo2);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("INFO3"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceInfo3);
				}
				if (ReflectionHelper.GetMember<string>(tempRecSet("INFO4"), "Value") == "Y")
				{
					m_lOutputTraceType = m_lOutputTraceType | ((int) TraceType.TraceInfo4);
				}

				return true;
			}
			catch (System.Exception excep)
			{

				WriteLog(TraceType.TraceError, "CTRaceLogger..Read_LogSetting()", excep.Message);
			}
			return result;
		}
		//
		public void WriteLog(TraceType aTraceType, string aEventLocation, string aEventText)
		{
			object ADODB = null;
			string strOutText = "";
			string strDate = "";
			try
			{

				string strTodayDate = "";
				string strTodayLog = "";
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset aRecSet = null;
				string strSQL = "";
				string strTraceType = "";

				// Prepare the output log file
				m_nFileOutput = FileSystem.FreeFile();

				strTodayDate = StringsHelper.Format(DateTime.Now, "yyyy_mm_dd");
				if (m_TodayDate != strTodayDate)
				{
					m_TodayDate = strTodayDate;
					strTodayLog = "_" + m_TodayDate + ".txt";
					CreateLogPath(strTodayLog);
				}

				switch(m_OutLogFile)
				{
					case TraceLogFile.TraceRPCService : 
						m_strOutputFilePath = m_strLogFilePath; 
						break;
					case TraceLogFile.TraceRPCDeskApp : 
						m_strOutputFilePath = m_strLogFilePathDeskApp; 
						break;
				}

				FileSystem.FileOpen(m_nFileOutput, m_strOutputFilePath, OpenMode.Append, OpenAccess.Default, OpenShare.Default, -1);

				// Prepare strTraceType to write to file
				switch(aTraceType)
				{
					case TraceType.TraceError : 
						strTraceType = "Error: "; 
						break;
					case TraceType.TraceInfo1 : case TraceType.TraceInfo2 : case TraceType.TraceInfo3 : case TraceType.TraceInfo4 : 
						strTraceType = "Info: "; 
						break;
					case TraceType.TraceValue : 
						strTraceType = "Value: "; 
						break;
					case TraceType.TraceWarning : 
						strTraceType = "Warning: "; 
						break;
					default:
						strTraceType = "Unknown: "; 
						break;
				}


				// Prepare Date to write
				strDate = DateTimeHelper.ToString(DateTime.Today) + " " + DateTimeHelper.ToString(DateTimeHelper.Time);

				// Prepare for writing to File
				if (Strings.Len(aEventLocation) != 0)
				{
					strOutText = strDate + " - " + strTraceType + " - " + aEventLocation + " - " + aEventText;
				}

				// Prepare for writing to Table
				if (Strings.Len(aEventText) == 0)
				{
					aEventText = " ";
				}
				// This SQL may needs to be changed for the Desktop Application!!
				strSQL = "Insert Into Model.T_CTRACELOGS (LOGDATE, EVENTLOCATION, EVENTTYPE, TEXT) VALUES('" + strDate + "', '" + aEventLocation + "', " + ((int) aTraceType).ToString() + ", '" + aEventText + "')";

				// Default place for Errors:
				//'Call FileLog(strOutText)

				if ((((int) aTraceType) & m_lOutputTraceType) != 0)
				{
					if ((m_lOutputPath & OUTPUTLOGFILE) != 0)
					{
						FileSystem.PrintLine(m_nFileOutput, strOutText);

					}

					if ((m_lOutputPath & OUTPUTLOGTABLE) != 0)
					{
						if (m_theDBInterface == null)
						{
							strOutText = strDate + " - " + ((int) TraceType.TraceError).ToString() + " - " + "CTraceLogger.WriteLog()" + " - " + "theDBInterface Is Nothing";
							FileSystem.PrintLine(m_nFileOutput, strOutText);
							return;
						}
						else
						{
							if (!m_theDBInterface.runQuery(strSQL, ref aRecSet))
							{
								strOutText = strDate + " - " + ((int) TraceType.TraceError).ToString() + " - " + "CTraceLogger.WriteLog()" + " - " + "Could not write to Table :" + strSQL.Substring(0, Math.Min(10, strSQL.Length));
								FileSystem.PrintLine(m_nFileOutput, strOutText);
								return;
							}
						}
					}

				}

				// Close the open log file
				FileSystem.FileClose(m_nFileOutput);
			}
			catch (System.Exception excep)
			{

				strOutText = strDate + " - " + ((int) TraceType.TraceError).ToString() + " - " + "CTraceLogger.WriteLog()" + " - " + excep.Message;
				//MsgBox Err.Description
				//Print #m_nFileOutput, strOutText
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

		}

		// In the start of each function, the connection must be set
		public void SetDBConnection(CDBInterface aDBConnection)
		{
			m_theDBInterface = aDBConnection;
		}

		//UPGRADE_NOTE: (7001) The following declaration (FileLog) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void FileLog(string strText)
		//{
			//
			//if (!MODE_DEBUG)
			//{
				//return;
			//}
			//
			//string strTime = DateTimeHelper.ToString(DateTime.Now);
			//
			//Debug.WriteLine(strTime + "  " + strText);
			//
			//int nFile = FileSystem.FreeFile();
			//FileSystem.FileOpen(nFile, LOGTRACEFILEPATH, OpenMode.Append, OpenAccess.Default, OpenShare.Default, -1);
			//
			//FileSystem.PrintLine(nFile, strTime + "  " + strText);
			//
			//FileSystem.FileClose(nFile);
		//}

		public void SetLogFile(TraceLogFile TraceLogFile)
		{
			m_OutLogFile = TraceLogFile;
		}

		public string OutputLogPath
		{
			get
			{
				return LOGFILEPATH;
			}
		}


		public string OutputLogPathDeskApp
		{
			get
			{
				return LOGFILEPATHDESKAPP;
			}
		}

	}
}