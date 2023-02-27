using Microsoft.VisualBasic;
using System;
using UpgradeHelpers.Helpers;

namespace RPC_Service_App
{
	internal class CSCADADataInterface
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
		//       This class is used for read/write from/to PowerCC SCADA Data.
		//       Any type of data is prepared by this class,
		//       after reading a value check the Quality code must be accomlpished here.
		//
		//***************************************************************************************

		private string ServerName = "";

		//UPGRADE_ISSUE: (2068) ScadaScriptComponent object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
		private UpgradeStubs.ScadaScriptComponent m_ScadaScrpit = null;

		private CDBInterface m_theCDBInterface = null;

		//
		public CSCADADataInterface()
		{

			m_ScadaScrpit = new UpgradeStubs.ScadaScriptComponent();
			ReflectionHelper.LetMember(m_ScadaScrpit, "UserName", "System");
			ReflectionHelper.LetMember(m_ScadaScrpit, "ConsoleName", "System");
			ReflectionHelper.LetMember(m_ScadaScrpit, "ContextName", "RT");
			ReflectionHelper.Invoke(m_ScadaScrpit, "SetActive", new object[]{true});

			m_theCDBInterface = new CDBInterface();
			if (!m_theCDBInterface.OpenConnection())
			{
				m_theCDBInterface = null;
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CSCADADataInterface.Class_Initialize", " Error in connect to DB.");
				return;
			}

		}

		//
		public bool ReadData(string strGUID, ref string strValue, ref bool aIsValid)
		{
			bool result = false;
			try
			{
				//UPGRADE_ISSUE: (2068) DomainObject object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DomainObject DomainObj = new UpgradeStubs.DomainObject();
				//UPGRADE_ISSUE: (2068) AnalogMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AnalogMeasurement AnalogObj = new UpgradeStubs.AnalogMeasurement();
				//UPGRADE_ISSUE: (2068) DigitalMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DigitalMeasurement DigitalObj = new UpgradeStubs.DigitalMeasurement();
				//UPGRADE_ISSUE: (2068) AccumulatorMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AccumulatorMeasurement AccumulObj = new UpgradeStubs.AccumulatorMeasurement();
				//UPGRADE_ISSUE: (2068) PositionMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.PositionMeasurement PositionObj = new UpgradeStubs.PositionMeasurement();

				result = true;

				ReflectionHelper.LetMember(DomainObj, "ObjectIdStr", strGUID);
				//Call theCTraceLogger.WriteLog(TraceInfo1, "CScadaDataInterface.ReadData", strGUID)

				//Check if this GUID exists

				strValue = "";

				switch(ReflectionHelper.GetMember<string>(DomainObj, "TypeName"))
				{
					case "AnalogMeasurement" : 
						ReflectionHelper.LetMember(AnalogObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						strValue = ReflectionHelper.GetMember<string>(AnalogObj, "Value"); 
						aIsValid = ReflectionHelper.GetMember<bool>(AnalogObj, "IsValidValue"); 
						break;
					case "DigitalMeasurement" : 
						ReflectionHelper.LetMember(DigitalObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						strValue = ReflectionHelper.GetMember<string>(DigitalObj, "Value"); 
						aIsValid = ReflectionHelper.GetMember<bool>(DigitalObj, "IsValidValue"); 
						break;
					case "AccumulatorMeasurement" : 
						ReflectionHelper.LetMember(AccumulObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						strValue = ReflectionHelper.GetMember<string>(AccumulObj, "Value"); 
						aIsValid = ReflectionHelper.GetMember<bool>(AccumulObj, "IsValidValue"); 
						break;
					case "PositionMeasurement" : 
						ReflectionHelper.LetMember(PositionObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						strValue = ReflectionHelper.GetMember<string>(PositionObj, "Value"); 
						aIsValid = ReflectionHelper.GetMember<bool>(PositionObj, "IsValidValue"); 
						break;
				}

				//Check the quality code
				if (!aIsValid)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CScadaDataInterface.ReadData", "Quality of '" + ReflectionHelper.GetMember<string>(DomainObj, "ObjectPath") + "' is not valid; Quality=" + getQualityString(DomainObj));
					if (!SendAlarm("RPCAlarm", "Qaulity is not valid for:" + ReflectionHelper.GetMember<string>(DomainObj, "ObjectPath")))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CScadaDataInterface.ReadData()", "Sending alarm failed.");
					}
				}

				//Destroy Domain Objects for the next use
				DomainObj = null;
				AnalogObj = null;
				DigitalObj = null;
				AccumulObj = null;
				DomainObj = null;
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..ReadData", excep.Message);
				result = false;
				//Resume Next
			}
			return result;
		}

		//
		public bool WriteData(string strGUID, string strValue)
		{
			bool result = false;
			object SSQ_GOOD = null;
			try
			{

				//UPGRADE_ISSUE: (2068) DomainObject object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DomainObject DomainObj = new UpgradeStubs.DomainObject();
				//UPGRADE_ISSUE: (2068) AnalogMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AnalogMeasurement AnalogObj = new UpgradeStubs.AnalogMeasurement();
				//UPGRADE_ISSUE: (2068) DigitalMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DigitalMeasurement DigitalObj = new UpgradeStubs.DigitalMeasurement();
				//UPGRADE_ISSUE: (2068) AccumulatorMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AccumulatorMeasurement AccumulObj = new UpgradeStubs.AccumulatorMeasurement();

				result = true;

				// If machine is StandBy do not write to SCADA
				if (!GeneralModule.IsMachineMaster())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo3, "CSCADADataInterface.WriteData()", "Exit Write Data: Machine is: StandBy");
					return result;
				}

				ReflectionHelper.LetMember(DomainObj, "ObjectIdStr", strGUID);

				//Check the quality code if neccessary

				switch(ReflectionHelper.GetMember<string>(DomainObj, "TypeName"))
				{
					case "AnalogMeasurement" : 
						ReflectionHelper.LetMember(AnalogObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						ReflectionHelper.Invoke(AnalogObj, "SendValue", new object[]{Conversion.Val(strValue), SSQ_GOOD}); 
						break;
					case "DigitalMeasurement" : 
						ReflectionHelper.LetMember(DigitalObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr")); 
						ReflectionHelper.Invoke(DigitalObj, "SendState", new object[]{Conversion.Val(strValue), SSQ_GOOD}); 
						break;
					case "AccumulatorMeasurement" : 
						ReflectionHelper.LetMember(AccumulObj, "ObjectIdStr", ReflectionHelper.GetMember(DomainObj, "ObjectIdStr"));  //Maybe we are not allowed to write to an AccumulatorMeasurement 
						ReflectionHelper.Invoke(AccumulObj, "SendValue", new object[]{Conversion.Val(strValue), SSQ_GOOD}); 
						break;
				}

				//Destroy Domain Objects for the next use
				DomainObj = null;
				AnalogObj = null;
				DigitalObj = null;
				AccumulObj = null;
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..WriteData", excep.Message);
				result = false;
			}

			return result;
		}

		//
		public double GetLimitValue(string aGUID, int aLimitLevel)
		{
			try
			{

				//UPGRADE_ISSUE: (2068) AnalogMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AnalogMeasurement AnalogObj = new UpgradeStubs.AnalogMeasurement();

				ReflectionHelper.LetMember(AnalogObj, "ObjectIdStr", aGUID);
				ReflectionHelper.Invoke(AnalogObj, "FetchLimits", new object[]{});

				return ReflectionHelper.Invoke<double>(AnalogObj, "GetLimitValue", new object[]{aLimitLevel});
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..GetLimitValue", excep.Message);
				//GetLimitValue = False
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}
			return 0;
		}

		//
		public bool SendAlarm(string strTagName, string strComment)
		{
			bool result = false;
			object ES_Appearing = null;
			object SPCSYSTEMCONFIGURATIONLib = null;
			object ALGCLIENTLib = null;
			object ADODB = null;
			try
			{
				//UPGRADE_ISSUE: (2068) DomainObject object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DomainObject DomObj = new UpgradeStubs.DomainObject();

				string strSQL = "";
				string strPath = "";
				string strGUID = "";
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset tempRecSet = null;
				//UPGRADE_ISSUE: (2068) ALGCLIENTLib.CxAlgClient object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ALGCLIENTLib.CxAlgClient AlgClient = null;
				//UPGRADE_ISSUE: (2068) SPCSYSTEMCONFIGURATIONLib.GTCAASRObjectID object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				SPCSYSTEMCONFIGURATIONLib.GTCAASRObjectID SPCSysASRObj = null;

				result = true;

				// If machine is StandBy do not send Alarm
				if (!GeneralModule.IsMachineMaster())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo3, "CSCADADataInterface.WriteData()", "Exit Send Alarm: Machine is: StandBy");
					return result;
				}

				AlgClient = new ALGCLIENTLib.CxAlgClient();

				if (m_theCDBInterface == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CSCADADataInterface.SendAlarm()", "Could not access to connection!");
					return false;
				}

				strSQL = "SELECT NETWORKPATH FROM MODEL.T_CSCADAPOINT WHERE NAME = '" + strTagName + "'";

				if (!m_theCDBInterface.runQuery(strSQL, ref tempRecSet))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CSCADADataInterface.SendAlarm()", "Could not select NetworkPath!");
					return false;
				}

				//UPGRADE_WARNING: (1068) tempRecSet() of type Recordset is being forced to string. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				strPath = ReflectionHelper.GetPrimitiveValue<string>(tempRecSet("NETWORKPATH"));

				// Find the GUID by NetworkPath
				ReflectionHelper.LetMember(DomObj, "ObjectPath", strPath);
				strGUID = ReflectionHelper.GetMember<string>(DomObj, "ObjectIdStr");

				ReflectionHelper.Invoke(AlgClient, "Initialize", new object[]{"RT", "RPC Service"});
				//UPGRADE_WARNING: (1068) SPCSysASRObj of type GTCAASRObjectID is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				ReflectionHelper.SetPrimitiveValue(SPCSysASRObj, ReflectionHelper.Invoke(AlgClient, "IDFromString", new object[]{strGUID}));

				ReflectionHelper.Invoke(AlgClient, "CreateSpontaneousMessage", new object[]{DateTimeHelper.Time, 0, 0, ES_Appearing, SPCSysASRObj, "", "", "", strComment, 0, ""});
				//Call DomObj.IssueSpontaneousAlarm(strComment)

				AlgClient = null;
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..SendAlarm", excep.Message);
				result = false;
				//Resume Next
			}

			return result;
		}

		//
		~CSCADADataInterface()
		{
			m_ScadaScrpit = null;
		}

		//
		public double GetHISValue(string aGUID, string aTimeDataType, System.DateTime dtFrom, System.DateTime dtTo)
		{
			double result = 0;
			object ADODB = null;
			try
			{

				//UPGRADE_ISSUE: (2068) CxCdaTimeSeries object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.CxCdaTimeSeries pTS = null;
				//UPGRADE_ISSUE: (2068) CxTimeSeriesAccess object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.CxTimeSeriesAccess ppTSA = null;
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset pRecSetDataTypes = null;
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset pRecSetObjects = null;
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset pRecSetData = null;
				int h1 = 0;


				// create component
				pTS = new UpgradeStubs.CxCdaTimeSeries();

				// add data sources
				h1 = ReflectionHelper.Invoke<int>(pTS, "AddDataSource", new object[]{"hisu", "hisp", "HIS", true});

				// retrieve available time series data types from the added data sources
				pRecSetDataTypes = ReflectionHelper.Invoke<Recordset>(pTS, "BrowseTimeSeriesDataTypes", new object[]{""});

				// Init time series for the specified time series data type
				ppTSA = ReflectionHelper.GetMember<UpgradeStubs.CxTimeSeriesAccess>(pTS, "InitTimeSeries");

				ReflectionHelper.Invoke(ppTSA, "AddTimeSeries", new object[]{aTimeDataType, "HIS", ""});

				// allow user to browse through the available objects
				pRecSetObjects = ReflectionHelper.GetMember<Recordset>(ppTSA, "BrowseTimeSeriesObjects");

				ReflectionHelper.Invoke(ppTSA, "AddObject", new object[]{aGUID});

				pRecSetData = ReflectionHelper.Invoke<Recordset>(ppTSA, "ReadTimeSeries", new object[]{dtFrom, dtTo});

				if (pRecSetData != null)
				{
					//pRecSetData.MoveLast
					result = ReflectionHelper.GetMember<double>(pRecSetData("VALUE"), "Value");
				}
				else
				{
					result = -1;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface.GetHISValue()", "Error in reading Time Series.");
				}

				ReflectionHelper.Invoke(pTS, "CloseHandle", new object[]{h1});
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..GetHISValue()", excep.Message);
				//GetHISValue = False
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


			return result;
		}

		//
		private string getQualityString(UpgradeStubs.DomainObject aDomainObj)
		{
			string result = "";
			try
			{
				//UPGRADE_ISSUE: (2068) AnalogMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AnalogMeasurement aDoAnalog = new UpgradeStubs.AnalogMeasurement();
				//UPGRADE_ISSUE: (2068) DigitalMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DigitalMeasurement aDoDigital = new UpgradeStubs.DigitalMeasurement();
				//UPGRADE_ISSUE: (2068) AccumulatorMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.AccumulatorMeasurement aDoAccumul = new UpgradeStubs.AccumulatorMeasurement();
				//UPGRADE_ISSUE: (2068) PositionMeasurement object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.PositionMeasurement aDoPosition = new UpgradeStubs.PositionMeasurement();


				result = "";
				if (ReflectionHelper.GetMember<string>(aDomainObj, "TypeName") == "AnalogMeasurement")
				{
					ReflectionHelper.LetMember(aDoAnalog, "ObjectPath", ReflectionHelper.GetMember(aDomainObj, "ObjectPath"));
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsBlocked"))
					{
						result = result + "Blocked | ";
					}
					else
					{
						result = result + "Not Blocked | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsCalculatedValue"))
					{
						result = result + "Calculated | ";
					}
					else
					{
						result = result + "Not Calculated | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsEnteredValue"))
					{
						result = result + "Entered | ";
					}
					else
					{
						result = result + "Not Entered | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsEstimatedValue"))
					{
						result = result + "Estimated | ";
					}
					else
					{
						result = result + "Not Estimated | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsUpdatedValue"))
					{
						result = result + "Updated | ";
					}
					else
					{
						result = result + "Not Updated | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsValidValue"))
					{
						result = result + "Valid | ";
					}
					else
					{
						result = result + "Not Valid | ";
					}
					if (ReflectionHelper.GetMember<bool>(aDoAnalog, "IsUsableValue"))
					{
						result = result + "Usable";
					}
					else
					{
						result = result + "Not Usable";
					}
				}
				else
				{
					if (ReflectionHelper.GetMember<string>(aDomainObj, "TypeName") == "DigitalMeasurement")
					{
						ReflectionHelper.LetMember(aDoDigital, "ObjectPath", ReflectionHelper.GetMember(aDomainObj, "ObjectPath"));
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsBlocked"))
						{
							result = result + "Blocked | ";
						}
						else
						{
							result = result + "Not Blocked | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsCalculatedValue"))
						{
							result = result + "Calculated | ";
						}
						else
						{
							result = result + "Not Calculated | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsEnteredValue"))
						{
							result = result + "Entered | ";
						}
						else
						{
							result = result + "Not Entered | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsEstimatedValue"))
						{
							result = result + "Estimated | ";
						}
						else
						{
							result = result + "Not Estimated | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsUpdatedValue"))
						{
							result = result + "Updated | ";
						}
						else
						{
							result = result + "Not Updated | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsValidValue"))
						{
							result = result + "Valid | ";
						}
						else
						{
							result = result + "Not Valid | ";
						}
						if (ReflectionHelper.GetMember<bool>(aDoDigital, "IsUsableValue"))
						{
							result = result + "Usable";
						}
						else
						{
							result = result + "Not Usable";
						}
					}
					else
					{
						if (ReflectionHelper.GetMember<string>(aDomainObj, "TypeName") == "AccumulatorMeasurement")
						{
							ReflectionHelper.LetMember(aDoAccumul, "ObjectPath", ReflectionHelper.GetMember(aDomainObj, "ObjectPath"));
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsBlocked"))
							{
								result = result + "Blocked | ";
							}
							else
							{
								result = result + "Not Blocked | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsCalculatedValue"))
							{
								result = result + "Calculated | ";
							}
							else
							{
								result = result + "Not Calculated | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsEnteredValue"))
							{
								result = result + "Entered | ";
							}
							else
							{
								result = result + "Not Entered | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsEstimatedValue"))
							{
								result = result + "Estimated | ";
							}
							else
							{
								result = result + "Not Estimated | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsUpdatedValue"))
							{
								result = result + "Updated | ";
							}
							else
							{
								result = result + "Not Updated | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsValidValue"))
							{
								result = result + "Valid | ";
							}
							else
							{
								result = result + "Not Valid | ";
							}
							if (ReflectionHelper.GetMember<bool>(aDoAccumul, "IsUsableValue"))
							{
								result = result + "Usable";
							}
							else
							{
								result = result + "Not Usable";
							}
						}
						else
						{
							if (ReflectionHelper.GetMember<string>(aDomainObj, "TypeName") == "PositionMeasurement")
							{
								ReflectionHelper.LetMember(aDoPosition, "ObjectPath", ReflectionHelper.GetMember(aDomainObj, "ObjectPath"));
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsBlocked"))
								{
									result = result + "Blocked | ";
								}
								else
								{
									result = result + "Not Blocked | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsCalculatedValue"))
								{
									result = result + "Calculated | ";
								}
								else
								{
									result = result + "Not Calculated | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsEnteredValue"))
								{
									result = result + "Entered | ";
								}
								else
								{
									result = result + "Not Entered | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsEstimatedValue"))
								{
									result = result + "Estimated | ";
								}
								else
								{
									result = result + "Not Estimated | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsUpdatedValue"))
								{
									result = result + "Updated | ";
								}
								else
								{
									result = result + "Not Updated | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsValidValue"))
								{
									result = result + "Valid | ";
								}
								else
								{
									result = result + "Not Valid | ";
								}
								if (ReflectionHelper.GetMember<bool>(aDoPosition, "IsUsableValue"))
								{
									result = result + "Usable";
								}
								else
								{
									result = result + "Not Usable";
								}
							}
						}
					}
				}

				//Destroy Domain Objects for the next use
				aDoAnalog = null;
				aDoDigital = null;
				aDoAccumul = null;
				aDoPosition = null;
			}
			catch (System.Exception excep)
			{
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface..getQualityString", excep.Message);
			}

			return result;
		}

		public bool SendAlarm_With_Value(string strNETWORKPATH, string strComment, double varValue = 0)
		{
			bool result = false;
			object ES_Appearing = null;
			object SPCSYSTEMCONFIGURATIONLib = null;
			object ALGCLIENTLib = null;
			object ADODB = null;
			try
			{
				//UPGRADE_ISSUE: (2068) DomainObject object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				UpgradeStubs.DomainObject DomObj = new UpgradeStubs.DomainObject();

				string strSQL = "";
				string strPath = "";
				string strGUID = "";
				//UPGRADE_ISSUE: (2068) ADODB.Recordset object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ADODB.Recordset tempRecSet = null;
				//UPGRADE_ISSUE: (2068) ALGCLIENTLib.CxAlgClient object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				ALGCLIENTLib.CxAlgClient AlgClient = null;
				//UPGRADE_ISSUE: (2068) SPCSYSTEMCONFIGURATIONLib.GTCAASRObjectID object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
				SPCSYSTEMCONFIGURATIONLib.GTCAASRObjectID SPCSysASRObj = null;

				result = true;

				// If machine is StandBy do not send Alarm
				if (!GeneralModule.IsMachineMaster())
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo3, "CSCADADataInterface.WriteData()", "Exit Send Alarm: Machine is: StandBy");
					return result;
				}

				AlgClient = new ALGCLIENTLib.CxAlgClient();

				if (m_theCDBInterface == null)
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CSCADADataInterface.SendAlarm()", "Could not access to connection!");
					return false;
				}

				strPath = strNETWORKPATH;

				// Find the GUID by NetworkPath
				ReflectionHelper.LetMember(DomObj, "ObjectPath", strPath);
				strGUID = ReflectionHelper.GetMember<string>(DomObj, "ObjectIdStr");

				ReflectionHelper.Invoke(AlgClient, "Initialize", new object[]{"RT", "RPC Service"});
				//UPGRADE_WARNING: (1068) SPCSysASRObj of type GTCAASRObjectID is being forced to Scalar. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				ReflectionHelper.SetPrimitiveValue(SPCSysASRObj, ReflectionHelper.Invoke(AlgClient, "IDFromString", new object[]{strGUID}));

				//UPGRADE_WARNING: (2065) Boolean method Information.IsMissing has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2065
				if (varValue == null)
				{
					ReflectionHelper.Invoke(AlgClient, "CreateSpontaneousMessage", new object[]{DateTimeHelper.Time, 0, 0, ES_Appearing, SPCSysASRObj, "", "", "", strComment, 0, ""});
				}
				else
				{
					ReflectionHelper.Invoke(AlgClient, "CreateSpontaneousMessage", new object[]{DateTimeHelper.Time, 0, 0, ES_Appearing, SPCSysASRObj, varValue, "", "", strComment, 0, ""});
				}


				//Call DomObj.IssueSpontaneousAlarm(strComment)

				AlgClient = null;
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CScadaDataInterface.SendAlarm", excep.Message);
				result = false;
				//Resume Next
			}

			return result;
		}
	}
}