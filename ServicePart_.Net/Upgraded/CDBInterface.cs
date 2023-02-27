using Microsoft.VisualBasic;
using System;
using System.Collections;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CDBInterface
	{

		//'*************************************************************************************
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


		private const string CONNECTION_STRING_LOCAL = "FetchSize=200;CacheType=Memory;" + "OSAuthent=0;PLSQLRSet=1;Data Source=RTSLND;" + "User ID=MODEL;Password=MODEL;";

		private string m_connectionString = "";

		private string RemoteDBStatus = "";

		private string LocalDBStatus = "";

		// a Connection to Local/Remote Database
		//UPGRADE_ISSUE: (2068) ADODB.Connection object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private ADODB.Connection objConn = null;

		//UPGRADE_ISSUE: (2068) ADODB.Error object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private ADODB.Error objErr = null;

		public bool runQuery(string aStrSql, ref ADODB.Recordset aRecSet)
		{
			object ADODB = null;
			object adCmdText = null;
			bool aResultFunc = false;
			try
			{
				//UPGRADE_ISSUE: (2068) ADODB.Error object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				//UPGRADE_ISSUE: (2068) ADODB.Command object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Command objCmd = new ADODB.Command();
				string strMsg = "";


				if (objConn == null)
				{
					strMsg = " Failed Connection does not exist : " + aStrSql.Substring(0, Math.Min(20, aStrSql.Length));
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CDBInterface.runQuery()", "Connection does not exist, " + strMsg);
					return false;
				}

				aResultFunc = true;
				//UPGRADE_WARNING: (1068) objConn of type Connection is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				ReflectionHelper.LetMember(objCmd, "ActiveConnection", ReflectionHelper.GetPrimitiveValue(objConn));
				ReflectionHelper.LetMember(objCmd, "CommandText", aStrSql);
				//UPGRADE_WARNING: (1068) adCmdText of type Variant is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				ReflectionHelper.LetMember(objCmd, "CommandType", ReflectionHelper.GetPrimitiveValue(adCmdText));
				ReflectionHelper.LetMember(objCmd, "Properties", true, "IRowsetChange");
				ReflectionHelper.LetMember(objCmd, "Properties", 7, "Updatability");

				//Creates an updatable rowset
				aRecSet = ReflectionHelper.GetMember<Recordset>(objCmd, "Execute");

				return aResultFunc;
			}
			catch
			{

				aResultFunc = false;
				if (objConn != null)
				{
					foreach (Error objErr in ReflectionHelper.GetMember<IEnumerable>(objConn, "Errors"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CDBInterface..runQuery()", ReflectionHelper.GetMember<string>(objErr, "Description"));
					}
					ReflectionHelper.Invoke(ReflectionHelper.GetMember(objConn, "Errors"), "Clear", new object[]{});
				}
			}
			return false;
		}

		public void checkConnection()
		{
			m_connectionString = CONNECTION_STRING_LOCAL;
		}

		public CDBInterface()
		{

		}

		~CDBInterface()
		{
			objConn = null;
		}

		public bool OpenConnection()
		{
			object ADODB = null;
			bool bErrorFlag = false;
			try
			{

				bErrorFlag = true;
				objConn = new ADODB.Connection();

				// Check the connection to determine if Remote DB is available or not
				checkConnection();

				ReflectionHelper.LetMember(objConn, "Provider", "OraOLEDB.Oracle");
				ReflectionHelper.LetMember(objConn, "ConnectionString", m_connectionString);
				ReflectionHelper.LetMember(objConn, "ConnectionTimeout", 100);
				ReflectionHelper.Invoke(objConn, "Open", new object[]{});

				return bErrorFlag;
			}
			catch (System.Exception excep)
			{
				bErrorFlag = false;
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CDBInterface..open()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}
			return false;
		}

		public bool CloseConnection()
		{
			bool bErrorFlag = false;
			try
			{

				bErrorFlag = true;
				objConn = null;

				return bErrorFlag;
			}
			catch (System.Exception excep)
			{
				bErrorFlag = false;
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CDBInterface..CloseConnection()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}
			return false;
		}
	}
}