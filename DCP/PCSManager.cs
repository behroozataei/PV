using System;
using System.Timers;
using System.Data;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

using COM;
using Irisa.Logger;

namespace DCP
{
	public sealed class PCSManager
	{
		//==============================================================================
		//MEMBER VARIABLES
		//==============================================================================
		//
		private PCSInterface _CPCSInterface = null;

		private EECTelegram _EECTelegram = null;

		private RPCTelegram _RPCTelegram = null;

		private EAFGrpTelegram _EAFGroupTelegram = null;

		private bool m_EECTelergramIsUpdated = false;

		private const int nEAFs = 8;

		private const int nSVCPoints = 20;

		// Network Configuration for running EEC and RPC is ok or no
		public bool m_NetworkConfig = false;

		// The Breaker status for EAF's, DS1: DS for Bus 1 or group 1, DS2: DS for Bus 2 or group 2
		private int[, ] m_EAFBreakers = new int[nEAFs + 1, 3];

		// A list of all points should be monitored if hey changed, do some actions ...
		private string[] m_arrChangeableDPoints = null;

		// This timer is for m_Timer_PCSRequest
		private int TIMER_CYCLE_EAFGroupReq = 10000;
		private Timer _timer_EAFGroupReq_10_Seconds;

		private int TIMER_CYCLE_EECTelegramReq = 60000;
		private Timer _timer_EECTelegramReq_1_Minute;

		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

		private bool isWorking;
		private bool isWoring_timer_EAFGroupReq;
		IDatabase _cache;


		//==============================================================================
		//MEMBER FUNCTIONS
		//==============================================================================
		public PCSManager(ILogger logger, IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer)
		{
			try
			{
				_logger = logger ?? throw new ArgumentNullException(nameof(logger));
				_repository = repository ?? throw new ArgumentNullException(nameof(repository));
				_updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));

				//-----------------------------------------------------------------------------------
				// An array for Tag of all Digital Points in SCADA may be changed
				// Important Note: These names should match with TAG field of T_CDCPARAMS

				// "MAC_DS", "MBD_DS", _
				//    ' "MAB", ,
				// "SVC_Q1_CB", "SVC_Q2_CB"
				m_arrChangeableDPoints = new string[]{"SVC_A1", "SVC_A2", "SVC_B1", "SVC_B2", "SVC_AB1", "SVC_AB2", "SVC1", "PCSACT", "RPCACT", "EECACT", "OVERLCOND", "EECTELEGRAM", "LSPTELEGRAM", "RPCTELEGRAM", "EAFGrpsChanged", ""};

				//
				m_EECTelergramIsUpdated = false;

				_CPCSInterface = new PCSInterface(_logger, _repository, _updateScadaPointOnServer);

				DCManager_start();

			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}

		}

		public void DCManager_start()
		{
			try
			{
				// TODO: compltion is required.
				//---------------------------------------------------------------------------------
				// For DC-PCS
				_timer_EAFGroupReq_10_Seconds = new Timer();
				_timer_EAFGroupReq_10_Seconds.Interval = TIMER_CYCLE_EAFGroupReq;
				_timer_EAFGroupReq_10_Seconds.Elapsed += runCyclicOperationPCSRequest;

				_timer_EECTelegramReq_1_Minute = new Timer();
				_timer_EECTelegramReq_1_Minute.Interval = TIMER_CYCLE_EECTelegramReq;
				_timer_EECTelegramReq_1_Minute.Elapsed += runCyclicOperationPCS1Min;

				//--------------------------------------------------------------------------
				// This procesure runs some required activities in start of DC, about PCS Part
				DC_StartupProcedure();

				//----------------------------------------------------------------------------------
				// TODO: commented for test 
				_timer_EAFGroupReq_10_Seconds.Start();
				_timer_EECTelegramReq_1_Minute.Start();

				//--------------------------------------------------------------------------
				// Create a theCTraceLogger for any Logging, and Send dbConnection to TraceLogger
				_logger.WriteEntry("Start of running ", LogLevels.Info);
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		// Each minute onetime this callback sub is called.
		public void runCyclicOperationPCS1Min(object sender, ElapsedEventArgs e)
		{
			try
			{
				//--------------------------------------------------------------------------
				// Log activation of 1-Min Telegram Processing
				_logger.WriteEntry("-----------------------------------------------------------------------", LogLevels.Info);
				//log_in_myLogger("Enter to method on: " + "GeneralModule.GetMachineName()", LogLevels.Info);
				//log_in_myLogger(" Machine State is: " + "GeneralModule.GetProcessState(GeneralModule.eDCState)", LogLevels.Info);
				_logger.WriteEntry("Start of 1-Minute Telegram proccesing", LogLevels.Info);

				//--------------------------------------------------------------------------
				// 1. Check status of function, If function is not enabled, Return
				// TODO:
				//if (!m_theCDCParameters.FunctionStatus)
				//{
				//	log_in_myLogger("Function is Disabled", LogLevels.Info);
				//	return;
				//}
				//else
				//{
				//	// Logging enter to process
				//	log_in_myLogger("Start of 1-Minute proccesing", LogLevels.Info);
				//}

				//--------------------------------------------------------------------------
				// 2. Open a connection in CDBInterface

				//--------------------------------------------------------------------------
				// 3. Running real process here
				CycleProcessing1Min();

				//--------------------------------------------------------------------------
				// 4. Writing exit message
				_logger.WriteEntry("End of 1-Minute proccesing", LogLevels.Info);

				//--------------------------------------------------------------------------
				// 5. Close connection to DB in CDBInterface
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}
		}

		public void runCyclicOperationPCSRequest(object sender, ElapsedEventArgs e)
		{
			try
			{
                // Debug only
                if (isWoring_timer_EAFGroupReq)
                {
                    _logger.WriteEntry("Warning: runCyclicOperationPCSRequest is busy!", LogLevels.Warn);
                    return;
                }
                else
                    isWoring_timer_EAFGroupReq = true;

                //--------------------------------------------------------------------------
                // Log activation of 1-Min Telegram Processing
                //log_in_myLogger($"-----------------------------------------------------------------------", LogLevels.Info);
                //log_in_myLogger("Start of Request for EAFGroup Telegram proccesing", LogLevels.Info);

                //--------------------------------------------------------------------------
                // 1. Check status of function, If function is not enabled, Return
                // TODO : 
                //if (!m_theCDCParameters.FunctionStatus)
                //{
                //	log_in_myLogger($"CDC_Manager..runCyclicOperationPCSRequest" + "Function is Disabled", LogLevels.Info);
                //	return;
                //}
                //else
                //{
                //	// Logging enter to process
                //	//Call theCTraceLogger.WriteLog(TraceInfo1, "CDC_Manager..runCyclicOperationPCSRequest()", "Request for EAFGroup proccesing")
                //}

                //--------------------------------------------------------------------------
                // 3. Running real process here
                EAFGroupRequest();

				//--------------------------------------------------------------------------
				// 4. Writing exit message
				//log_in_myLogger("End of Request for EAFGroup proccesing", LogLevels.Info);

				//--------------------------------------------------------------------------
				// 5. Close connection to DB in CDBInterface
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}

			isWoring_timer_EAFGroupReq = false;
		}

		// Important NotE: EAF1Group is determined in the VMAB application, and here is read from SCADAPoint "GRPEAF1"
		// After installing other GISes, they should be changed same as EAF1
		// This Job/Event will be trigerred by PowerCC, when group of one EAF is changed.
		// This method should be subscribed in the PowerCC.
		public void ProcessEAFGroupJob()
		{
			try
			{
				// Only for GIS's MF1
				//    _EAFGroupTelegram.EAF1Group = m_CDCParam.EAFGroups(1)
				//    strTemp = m_CDCParam.FindGUID("GRPEAF1")
				//    If Not m_CDCParam.m_theCSCADADataInterface.ReadData(strTemp, strValue, aIsValid) Then
				//        Call theCTraceLogger.WriteLog(TraceError, "CDCPCSManager..ProcessEAFGroupJob()", "Could not read the value for: " & strTemp & "from SCADA")
				//        m_CDCParam.EAFGroups(1) = 0
				//    Else
				//        m_CDCParam.EAFGroups(1) = Val(strValue)
				//    End If

				var EAF1Group = _repository.GetScadaPoint("GRPEAF1");
				var EAF2Group = _repository.GetScadaPoint("GRPEAF2");
				var EAF3Group = _repository.GetScadaPoint("GRPEAF3");
				var EAF4Group = _repository.GetScadaPoint("GRPEAF4");
				var EAF5Group = _repository.GetScadaPoint("GRPEAF5");
				var EAF6Group = _repository.GetScadaPoint("GRPEAF6");
				var EAF7Group = _repository.GetScadaPoint("GRPEAF7");
				var EAF8Group = _repository.GetScadaPoint("GRPEAF8");

				_EAFGroupTelegram = new EAFGrpTelegram();
				_EAFGroupTelegram.EAF1Group = (int)EAF1Group.Value;
				_EAFGroupTelegram.EAF2Group = (int)EAF2Group.Value;
				_EAFGroupTelegram.EAF3Group = (int)EAF3Group.Value;
				_EAFGroupTelegram.EAF4Group = (int)EAF4Group.Value;
				_EAFGroupTelegram.EAF5Group = (int)EAF5Group.Value;
				_EAFGroupTelegram.EAF6Group = (int)EAF6Group.Value;
				_EAFGroupTelegram.EAF7Group = (int)EAF7Group.Value;
				_EAFGroupTelegram.EAF8Group = (int)EAF8Group.Value;

				_EAFGroupTelegram.TelDate = DateTime.Now;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}

		}


		// Process RPC Telegram
		// PCRPC: PROC(SVC FIXED, OVERFL FIXED, POW1 FIXED, POW2 FIXED);
		public void RPCTelegramReceived(string strData)
		{
			try
			{
				string strSendData = "";

				// -------------------------------------------------------------------------
				// 1. Receiving telegram is logged
				_logger.WriteEntry("CDCPCSManager..RPCTelegramReceived()" , LogLevels.Info);

				// -------------------------------------------------------------------------
				// 2. The Status of PCS should be checked
				// TODO:
				//if (m_CDCParam.PCS_IsActive != GeneralModule.STATUS_ON)
				//{
				//	_logger.WriteEntry("RPC is not active", LogLevels.Info);
				//	return;
				//}

				// -------------------------------------------------------------------------
				// 3. The Status of function should be checked
				// TODO:
				//if (!m_CDCParam.FunctionStatus)
				//{
				//	_logger.WriteEntry("DC function is not active", LogLevels.Info);
				//	return;
				//}

				// -------------------------------------------------------------------------
				// 4. In start of each telegram, update the record set and related values should be done

				// -------------------------------------------------------------------------
				// 5. Check received Data in Telegram
				if (!_RPCTelegram.PrepareToUse(strData))
				{
					// Error in received Telegram
					_logger.WriteEntry("Received Telegram is not correct.", LogLevels.Error);
					return;
				}

				// -------------------------------------------------------------------------
				// 6. Prepare data to send
				_RPCTelegram.PrepareToSend(ref strSendData);

				// -------------------------------------------------------------------------
				// 7. Look wether sending is allowed,
				// TODO:
				//if ((m_CDCParam.RPC_IsActive & ((m_NetworkConfig) ? -1 : 0)) != 0)
				//{
				//	// -------------------------------------------------------------------------
				//	// 8. Sending RPC Telegram to PCS
				//	_CPCSInterface.SendRPCTelegram(_RPCTelegram);

				//	// /* NOW SEND TELEGRAMM */
				//	// CALL PCDU04(175,9,PCSRPC,N);
				//	// CALL PCS5KO(N,MYPUFF,1);
				//	_logger.WriteEntry("RPC Telegram was sent to PCS", LogLevels.Info);
				//}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}

		}

		public void GetEAFGroupTelegram()
		{

		}

		//
		// Check EEC,RPC Configuration based on parameters here
		//   HANDLING EEC AND RPC CONDITION/CONFIGURATION
		private void CheckNetworkConfig()
		{
			try
			{
				m_NetworkConfig = false;
				var MAC_DS = _repository.GetDigitalStatusByScadaName("MAC_DS");
				var MBD_DS = _repository.GetDigitalStatusByScadaName("MBD_DS");
				var MABStatus = _repository.GetDigitalStatusByScadaName("MAB");

				if (MAC_DS == DigitalStatus.Close && MBD_DS == DigitalStatus.Close)
				{
					// MCE_DS is BREAKER_CLOSE And MDF_DS is BREAKER_CLOSE forever
					//If Not (m_CDCParam.MCE_DS = BREAKER_CLOSE And m_CDCParam.MDF_DS = BREAKER_CLOSE) Then
					//    m_NetworkConfig = True
					//End If
				}

				if ((MAC_DS == DigitalStatus.Open && MBD_DS == DigitalStatus.Close) || 
					(MAC_DS == DigitalStatus.Close && MBD_DS == DigitalStatus.Open))
				{
					// MCE_DS is BREAKER_CLOSE And MDF_DS is BREAKER_CLOSE forever
					//If Not (m_CDCParam.MCE_DS = BREAKER_CLOSE And m_CDCParam.MDF_DS = BREAKER_CLOSE) Then
					// MAB CLOSED
					if (MABStatus == DigitalStatus.Close)
					{
						m_NetworkConfig = true;
					}
					//End If
				}

				// Temporarily bypass the checking
				m_NetworkConfig = true;

				//_logger.WriteEntry("Configuration is:" + m_NetworkConfig.ToString(), LogLevels.Info);
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		public int GetEAFBreakers(int Index, int Col)
		{
			return m_EAFBreakers[Index, Col];
		}

		public void SetEAFBreakers(int Index, int Col, int value)
		{
			// Check for subscript out of range error.
			if (Index < m_EAFBreakers.GetLowerBound(0) || Index > m_EAFBreakers.GetUpperBound(0))
			{
				throw new System.Exception("9");
			}
			if (Col < m_EAFBreakers.GetLowerBound(1) || Col > m_EAFBreakers.GetUpperBound(1))
			{
				throw new System.Exception("9");
			}

			m_EAFBreakers[Index, Col] = value;
		}

		//    IF EECCON == 1 THEN     /* EEC TELEGRAM ??? */
		//        IF EECACT == 1 THEN     /* EEC FUNCTION ??? */
		//            /* PROZESSFREIGABE UND AKTIV ? */
		//            IF MCSPAC == 1 AND ALTOSE == 1 THEN
		//                /*-------------------------------------------------------------*/
		//                RESTIME := PCSZWA(3);
		//                IF RESTIME > 0
		//                THEN
		//                    CALL PCDU04(175,14,PCSZWA,N);
		//                    CALL PCS5KO(N,MYPUFF ,1);
		//                FIN;
		//                /*-------------------------------------------------------------*/
		//            FIN;
		//        FIN;
		//    FIN;

		//
		// Some duties are accomplished here:
		//   1. Sending EEC 1-Minute Telgram
		//   2. Checking some ...
		public void CycleProcessing1Min()
		{
			object PCS_ACTIVE = null;
			int PCS_GOING_ACTIVE = 0;
			try
			{
				string strData = "";
				string strTelDate = "";
				string strTime = "";
				string strErrorPath = "";
				EEC_TELEGRAM_Str _eec_telegram = new EEC_TELEGRAM_Str();

				// Debug only
				//if (isWorking)
				//{
				//	_logger.WriteEntry("Warning: CycleProcessing1Min is busy! ", LogLevels.Warn);
				//	return;
				//}
				//else
				//	isWorking = true;

				//--------------------------------------------------------------------------
				// 1. Is PCS system enable?
				// TODO: check
				///if (m_CDCParam.PCS_IsActive != GeneralModule.STATUS_ON)
				//{
				//	_logger.WriteEntry("PCS System is not acive", LogLevels.Info);
				//	return;
				//}

				//--------------------------------------------------------------------------
				// 2. Checking prerequisites to send EEC Telegram to PCS
				// TODO: check
				//if (m_CDCParam.EEC_IsActive != GeneralModule.STATUS_ON)
				//{
				//	_logger.WriteEntry("EEC Function is not acive", LogLevels.Info);
				//	return;
				//}

				//--------------------------------------------------------------------------
				// 3. Read Function Parameters, Reading Static values for parameters from Tables

				//--------------------------------------------------------------------------
				// 4. Updating Network Configuration variable
				CheckNetworkConfig();
				if (!m_NetworkConfig)
				{
					_logger.WriteEntry("Network configuration is not Ok", LogLevels.Info);
					isWorking = false;
					return;
				}

				//--------------------------------------------------------------------------
				// 5. Read Last Telegram from DB
				//var dtbEECTelegram = _repository.GetEECTELEGRAM();
				//foreach( DataRow dr in dtbEECTelegram.Rows)
				//{
				//	var aTime = DateTime.Parse(dr["SENTTIME"].ToString()); 
				//	if (aTime.Year != 1900 )
				//	{
				//		_logger.WriteEntry("There is no new EEC Telegram in the table to Send for PCS.", LogLevels.Warn);
				//		isWorking = false;
				//		return;
				//	}
				//	else
				//	{
				//		_logger.WriteEntry("TELDATETIME= " + dr["TELDATETIME"].ToString(), LogLevels.Info);
				//	}

					//--------------------------------------------------------------------------
					

					// 4. Preparing telegram members to Send
					m_EECTelergramIsUpdated = true;

					//_EECTelegram.m_TelegramID = aRecSet("T_CEECTELEGRAM_ID")
					_EECTelegram = new EECTelegram();

					var keys = _repository.GetRedisUtiles().GetKeys(pattern: RedisKeyPattern.EEC_TELEGRAM);
					if (keys.Length == 0)
					{

						_logger.WriteEntry("There is no new EEC Telegram in the Cashe to Send for PCS.", LogLevels.Warn);
						isWorking = false;
						return;
					}

					var dataTable_cache = _repository.GetRedisUtiles().StringGet<EEC_TELEGRAM_Str>(keys);
					
					_eec_telegram = dataTable_cache.FirstOrDefault();

					if (_eec_telegram.SENTTIME.Year != 1900)
					{
						_logger.WriteEntry("There is no new EEC Telegram in the Cashe to Send for PCS.", LogLevels.Warn);
						isWorking = false;
						return;
					}
					else
					{
						_logger.WriteEntry("TELDATETIME= " + _eec_telegram.TELDATETIME.ToString(), LogLevels.Info);
					}

					_EECTelegram.m_Date = _eec_telegram.TELDATETIME;
					_EECTelegram.m_ResidualTime = _eec_telegram.RESIDUALTIME.ToString();
					_EECTelegram.m_ResidualEnergy = _eec_telegram.RESIDUALENERGY.ToString();
					_EECTelegram.m_OverLoad1 = _eec_telegram.MAXOVERLOAD1.ToString();
					_EECTelegram.m_OverLoad2 = _eec_telegram.MAXOVERLOAD2.ToString();
					_EECTelegram.m_ResidualEnergyEnd = _eec_telegram.RESIDUALENERGYEND.ToString();


					//_EECTelegram.m_Date = System.Convert.ToDateTime( dr["TELDATETIME"].ToString());
					//_EECTelegram.m_ResidualTime = dr["ResidualTime"].ToString();
					//_EECTelegram.m_ResidualEnergy = dr["ResidualEnergy"].ToString();
					//_EECTelegram.m_OverLoad1 = dr["MAXOVERLOAD1"].ToString();
					//_EECTelegram.m_OverLoad2 = dr["MAXOVERLOAD2"].ToString();
					//_EECTelegram.m_ResidualEnergyEnd = dr["ResidualEnergyEnd"].ToString();

					//strTelDate = Convert.ToDateTime(dr["TELDATETIME"]).ToString("yyyy-MM-dd HH:mm:ss");
					
				//}

				//if (dtbEECTelegram.Rows.Count > 1)
				//{
				//	// There is more than one telegram to send!
				//	_logger.WriteEntry("There is more than one EEC Telegram to Send for PCS", LogLevels.Warn);
				//}

				//--------------------------------------------------------------------------
				// 5. Check the sent time also ResTime
				//   ResTime > 0 and ResTime <= 15

				//--------------------------------------------------------------------------
				// 6. Prepare EEC Telegram
				_EECTelegram.PrepareToSend(ref strData);

				if (_CPCSInterface is null)
				{
					_logger.WriteEntry("Wrong intial value for _CPCSInterface ", LogLevels.Error);
				}

				if (_EECTelegram is null)
				{
					_logger.WriteEntry("Wrong intial value for _EECTelegram", LogLevels.Error);
				}

				//--------------------------------------------------------------------------
				// 7. Sending to PCS
				//If m_EECTelergramIsUpdated Then
				// TODO: PCS_ACTIVE PCS_GOING_ACTIVE
				///if ((((GeneralModule.eDCState == PCS_ACTIVE) ? -1 : 0) | PCS_GOING_ACTIVE) != 0)
				{
					if (!_CPCSInterface.SendEECTelegram(_EECTelegram))
					{
						_logger.WriteEntry("Could not send EECTelegram To PCS", LogLevels.Error);
						var eecErrorPath = _repository.GetScadaPoint("EECTelegramError");
						if (!_updateScadaPointOnServer.SendAlarm(eecErrorPath, SinglePointStatus.Disappear, "Could not send EECTelegram To PCS"))
							_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
						if (!_updateScadaPointOnServer.SendAlarm(eecErrorPath, SinglePointStatus.Appear, "Could not send EECTelegram To PCS"))
							_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
					}
					else
					{
						m_EECTelergramIsUpdated = false;
						_logger.WriteEntry("EECTelegram was sent To PCS", LogLevels.Info);
					}
				}
				//else
				//{
				//	_logger.WriteEntry("EECTelegram was not sent To PCS", LogLevels.Info);
				//}

				//--------------------------------------------------------------------------
				// 8. Updating sent record in the Table
				//strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 

				//if (!_repository.UpdateEECTELEGRAM(strTime, strTelDate))
				//{
				//	_logger.WriteEntry("Could not update T_EECTelegram Table", LogLevels.Error);
				//}
				_cache = _repository.GetRedisUtiles().DataBase;
				_eec_telegram.SENTTIME = DateTime.Now;
				_cache.StringSet(RedisKeyPattern.EEC_TELEGRAM, JsonConvert.SerializeObject(_eec_telegram));		

			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			isWorking = false;
		}

		// When DC_Service starts to run, this sub does some initial actions
		public void DC_StartupProcedure()
		{
			try
			{
				//
				_logger.WriteEntry("CDCPCSManager..DC_StartupProcedure()" , LogLevels.Info);

				//    1. ALTOSE := 1;        /* PROZESSFREIGABE IST ERTEILT  */ ????

				//--------------------------------------------------------------------------
				// 1. CALL PCMEWE, Read all values from SCADA
				//'Call m_CDCParam.readDCParameters(True)

				//--------------------------------------------------------------------------
				// 3. Dummy Call, Only run some checking
				// TODO: check is needed or no!
				///SCADAEventReceived(" ", " ");

				//--------------------------------------------------------------------------
				// 2. Update Active values
				//''If m_CDCParam.EEC_IsActive Then
				//'    m_EECActive = True
				//'Else
				//'    m_EECActive = False
				//'End If

				//'If m_CDCParam.RPC_IsActive Then
				//'    m_RPCActive = True
				//'Else
				//'    m_RPCActive = False
				//'End If

				//'If m_CDCParam.PCS_IsActive Then
				//'    m_PCSActive = True
				//'Else
				//'    m_PCSActive = False
				//'End If


				//--------------------------------------------------------------------------
				// 4. Send EAFGroup Telegram with EAF STATUS, IF MCSPAC  == 1
				//If m_PCSActive Then
				//    Call m_CTCPServer.SendPCSEAFGroupTelegram
				//End If

				//---------------------------------------------------------------------------
				var lspTelegram = _repository.GetScadaPoint("LSPTELEGRAM");
				if( !_updateScadaPointOnServer.WriteSCADAPoint(lspTelegram, (float)SinglePointStatus.Disappear))
				{
					_logger.WriteEntry("Could not reset LSPTelegram point in SCADA", LogLevels.Error);
				}

				var eecTelegram = _repository.GetScadaPoint("EECTELEGRAM");
				if (!_updateScadaPointOnServer.WriteSCADAPoint(eecTelegram, (float)SinglePointStatus.Disappear))
				{
					_logger.WriteEntry("Could not reset EECTelegram point in SCADA", LogLevels.Error);
				}

				var rpcTelegram = _repository.GetScadaPoint("RPCTELEGRAM");
				if (!_updateScadaPointOnServer.WriteSCADAPoint(rpcTelegram, (float)SinglePointStatus.Disappear))
				{
					_logger.WriteEntry("Could not reset RPCTelegram point in SCADA", LogLevels.Error);
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		//
		// When one important event in the SCADA is raised, This sub will be called.
		// List of events cause this call are:
		//   1. Change in Breaker Status
		//   2. Change in EAF Group Numbers
		//   3. Change in Status of EEC and RPC Services
		//   4. Change in SVC values
		//   Important Note: MLBE does not consider ALTOSE, Only in the OCP and SVC   !!!!!!!!
		//   PCMLBE: PROC (B1 FIXED,B2 FIXED, B3 FIXED, ELEM FIXED, INFO FIXED, WERT FIXED)
		public void SCADAEventReceived(DCPScadaPoint scadaPoint)
		{
			try
			{
				string strData = "";
				bool bSVCFound = false;
				bool bSVCQFound = false;
				byte aEAFGroupNo = 0;
				float aPMax = 0;

				//--------------------------------------------------------------------------
				// 1. Update new value in the Table

				//--------------------------------------------------------------------------
				// 3. If "OVERLOAD CONDITION" was changed: HANDLING OCP INFORMATION
				//If eDCState = PCS_ACTIVE Or PCS_GOING_ACTIVE Then
				//    If strTag = "OVERLCOND" Then
				//        If strValue = 1 Then
				//            If m_CDCParam.PCS_IsActive = STATUS_ON And m_CDCParam.FunctionStatus Then
				//                aEAFGroupNo = m_CDCParam.EAFGroupNo
				//                aPMax = m_CDCParam.PMax
				//                Call theCTraceLogger.WriteLog(TraceValue, "CDCPCSManager..SCADAEventReceived", "aEAFGroupNo = " & m_CDCParam.EAFGroupNo)
				//                Call theCTraceLogger.WriteLog(TraceValue, "CDCPCSManager..SCADAEventReceived", "aPMax = " & m_CDCParam.PMax)
				//
				//                If Not _CPCSInterface.SendLSPTelegram(aEAFGroupNo, aPMax) Then
				//                    Call theCTraceLogger.WriteLog(TraceError, "CDCPCSManager..SCADAEventReceived", "LSP Telegram could not be sent to PCS")
				//                Else
				//                    Call theCTraceLogger.WriteLog(TraceInfo1, "CDCPCSManager..SCADAEventReceived", "OverLoad Condition: LSP Telegram was sent to PCS")
				//                End If
				//            End If
				//        End If
				//        Exit Sub
				//    End If

				//--------------------------------------------------------------------------
				// 4. If "EEC Telegram" was changed:
				if (scadaPoint.Name == "EECTELEGRAM")
				{
					if ((SinglePointStatus)scadaPoint.Value == SinglePointStatus.Appear)
					{ // Telegram is appeared
						_logger.WriteEntry("New EEC Telegram is appeared", LogLevels.Info);
						// TODO: PCS_IsActive FunctionStatus
						///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
						{
							// Processing received telegram
							m_EECTelergramIsUpdated = true;
							_logger.WriteEntry("EEC Telegram will sent to PCS", LogLevels.Info);
						}
					}
					else
					{
						_logger.WriteEntry("New EEC Telegram is not appeared", LogLevels.Info);
					}
					return;
				}

				//--------------------------------------------------------------------------
				// 5. If "LSP Telegram" was changed:
				if( (scadaPoint.Name == "LSPTELEGRAM") && (scadaPoint.Value == (float)eApp_Disapp.Appear))
				{
					//if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("LSPTELEGRAM"), Convert.ToSingle(eApp_Disapp.Disappear)))
					//{
					//	_logger.WriteEntry("Could not write value of LSPACTIVATED for PCS-Telegram, Disappear", LogLevels.Error);
					//}

					_logger.WriteEntry("LSP-Telegram event is received", LogLevels.Info);
					// Telegram is appeared
					if ((SinglePointStatus)scadaPoint.Value == SinglePointStatus.Appear)
					{ 
						// TODO: PCS_IsActive FunctionStatus
						///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
						{
							// EAFGroupNo : EAF group number which its power is limited
							var eafGroupNo = _repository.GetScadaPoint("EAFGROUPNO");
							aEAFGroupNo = (byte)eafGroupNo.Value;

							//******************************************************************
							//MODIFICATION BY HEMATY:7/4/1388
							var pMax = _repository.GetScadaPoint("PMAX");
							aPMax = pMax.Value;
							//END OF MODIFICATION
							//******************************************************************

							//aPMax = m_CDCParam.PMax
							_logger.WriteEntry("aEAFGroupNo = " + eafGroupNo.Value.ToString(), LogLevels.Info);
							_logger.WriteEntry("aPMax = " + aPMax.ToString(), LogLevels.Info);

							if (!_CPCSInterface.SendLSPTelegram(aEAFGroupNo, aPMax))
							{
								_logger.WriteEntry("Could not send LSP Telegram To PCS", LogLevels.Error);
								var lspTelegramError = _repository.GetScadaPoint("LSPTelegramError");
								if (!_updateScadaPointOnServer.SendAlarm(lspTelegramError, SinglePointStatus.Disappear, ""))
									_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
								if (!_updateScadaPointOnServer.SendAlarm(lspTelegramError, SinglePointStatus.Appear, ""))
									_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							}
							else
							{
								_logger.WriteEntry("OverLoad Condition: LSP Telegram was sent to PCS", LogLevels.Error);
							}
						}
						//else
						//{
						//	_logger.WriteEntry("Send to PCS is NotActive or Function is Disabled", LogLevels.Warn);
						//}
					}
					else
					{
						_logger.WriteEntry("New LSP Telegram is not appeared", LogLevels.Info);
					}
					return;
				}

				//--------------------------------------------------------------------------
				// 6. If "RPC Telegram" was changed:
				if (scadaPoint.Name == "RPCTELEGRAM")
				{
					// TODO:
					if ((SinglePointStatus)scadaPoint.Value == SinglePointStatus.Appear)
					{ 
						_logger.WriteEntry("New RPC Telegram is appeared", LogLevels.Info);
						// TODO:
						///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
						{
							// Other actions should be accomplished here
							_logger.WriteEntry("RPC Telegram was sent to PCS", LogLevels.Info);
						}
					}
					else
					{
						_logger.WriteEntry("New RPC Telegram is not appeared", LogLevels.Info);
					}
					return;
				}


				//--------------------------------------------------------------------------
				// 7. If "SVC CONDITION" was changed: HANDLING SVC INFORMATION
				bSVCFound = false;
				bSVCQFound = false;
				for (int I = 1; I <= nSVCPoints; I++)
				{
					// TODO: for RPC only, point names should be checked again
					if (scadaPoint.Name == "SVC" || scadaPoint.Name == "SVCQ")
					{
						if (scadaPoint.Name == "SVC")
						{
							bSVCFound = true;
						}
						else
						{
							bSVCQFound = true;
						}
						_logger.WriteEntry("SVC change is received", LogLevels.Info);
					}
				}

				if (bSVCFound || bSVCQFound)
				{
					if (bSVCFound)
					{ // "One of SVC's"
						_RPCTelegram.m_SVCValue = (int)scadaPoint.Value - 1;
					}
					else
					{
						// "One of SVCQ's"
						if ((int)scadaPoint.Value == 2)
						{
							_RPCTelegram.m_SVCValue = 0;
						}
						else
						{
							_RPCTelegram.m_SVCValue = 1;
						}
					}

					_RPCTelegram.m_OverFlux = 0;
					_RPCTelegram.m_PowerFactor1 = 0;
					_RPCTelegram.m_PowerFactor2 = 0;

					// Sending Telgram to PCS
					// TODO: PCS_IsActive FunctionStatus
					///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
					{
						// Prepare to Send
						_RPCTelegram.PrepareToSend(ref strData);

						// Sending Telegram
						_CPCSInterface.SendRPCTelegram(_RPCTelegram);
						_logger.WriteEntry("SVC change in a RPC Telegram was Sent to PCS", LogLevels.Info);
					}
					//else
					//{
					//	_logger.WriteEntry"PCS is not Active and RPC Telegram could not be Sent to PCS", LogLevels.Info);
					//}

					return;
				}

				//--------------------------------------------------------------------------
				// 8. If "EEC Active" was changed, HANDLING EEC ACTIVATION
				if (scadaPoint.Name == "EECACT")
				{
					if (scadaPoint.Value == 1)
					{
						_logger.WriteEntry("EEC Function is changed to ACTIVE", LogLevels.Info);
					}
					else
					{
						_logger.WriteEntry("EEC Function is changed to INACTIVE", LogLevels.Info);
					}
				}

				//--------------------------------------------------------------------------
				// 9. If "PCS Active" was changed, HANDLING TELEGRAMS TO PCS ACTIVATION
				if (scadaPoint.Name == "PCSACT")
				{
					if (scadaPoint.Value == 1)
					{
						_logger.WriteEntry("PCS System is changed to ACTIVE", LogLevels.Info);
					}
					else
					{
						_logger.WriteEntry("PCS system is changed to INACTIVE", LogLevels.Info);
					}
				}

				//--------------------------------------------------------------------------
				// 10. If "RPC Active" was changed, HANDLING TELEGRAMS TO PCS ACTIVATION
				if (scadaPoint.Name == "RPCACT")
				{
					if (scadaPoint.Value == 1)
					{
						_logger.WriteEntry("RPC Function is changed to ACTIVE", LogLevels.Info);
					}
					else
					{
						_logger.WriteEntry("RPC Function is changed to INACTIVE", LogLevels.Info);
					}
				}

				//--------------------------------------------------------------------------
				// 11. If one of Breakers was changed, HANDLING EAF GROUPS, THE EAF GROUP NUMBER HAS CHANGED
				ProcessEAFGroupJob();

				//--------------------------------------------------------------------------
				// 12. HANDLING EEC AND RPC CONDITION/CONFIGURATION, Check EEC,RPC Configuration based on parameters
				CheckNetworkConfig();

				//--------------------------------------------------------------------------
				// 13. Sending EAFGroup Telegram to PCS,
				//     If MAB breakers, MF1..MF8, DS1..DS2 breakers for EAFGroups was changed,
				//If eDCState = PCS_ACTIVE Or PCS_GOING_ACTIVE Then
				if (scadaPoint.Name == "EAFGrpsChanged")
				{
					// TODO: PCS_IsActive FunctionStatus
					///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
					{
						// Sending EAF Groups Telegram to PCS System
						if (!_CPCSInterface.SendEAFGroupTelegram(_EAFGroupTelegram))
						{
							_logger.WriteEntry("Could not send EAF Group Telegram To PCS", LogLevels.Error);
							var eafGroupTelError = _repository.GetScadaPoint("EAFGroupTelegramError");
							if (!_updateScadaPointOnServer.SendAlarm(eafGroupTelError, SinglePointStatus.Disappear, ""))
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							if (!_updateScadaPointOnServer.SendAlarm(eafGroupTelError, SinglePointStatus.Appear, ""))
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
						}
						else
						{
							var eafGroupChanged = _repository.GetScadaPoint("EAFGrpsChanged");
							if (!_updateScadaPointOnServer.SendAlarm(eafGroupChanged, SinglePointStatus.Disappear, ""))
							{
								_logger.WriteEntry("Clearing EafGroupChanged for DCP-PCS was failed!", LogLevels.Error);
							}

							_logger.WriteEntry("EAFGroups Telegram was sent to PCS", LogLevels.Info);
						}
					}
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}

		}

		// This function adds any points which we need to inform about changes in their values in the SCADA.
		// This function checks if there is any request for EAFGroup, send it to PCS
		public bool EAFGroupRequest()
		{
			bool result = false;
			try
			{
				string strReqDate = "";

				result = false;

				if (_CPCSInterface.IsNewRequestEAFGroup(ref strReqDate))
				{
					//--------------------------------------------------------------------------
					// Prepare the last EAFGroups
					ProcessEAFGroupJob();

					//--------------------------------------------------------------------------
					// 13. Sending EAFGroup Telegram to PCS,
					// TODO: PCS_IsActive FunctionStatus
					///if (m_CDCParam.PCS_IsActive == GeneralModule.STATUS_ON && m_CDCParam.FunctionStatus)
					{
						// Sending EAF Groups Telegram to PCS System
						_CPCSInterface.SendEAFGroupTelegram(_EAFGroupTelegram);
						_logger.WriteEntry("EAFGroups Telegram was sent to PCS", LogLevels.Info);
					}
					//else
					//{
					//	_logger.WriteEntry("EAFGroupRequest Telegram was recieved but DC is not Enabled", LogLevels.Error);
					//}

					//MODIFICATION BY HEMATY:IF MACHINE STANDBY DO NOT UPDATE
					// TODO: IsMachineMaster
					///if (GeneralModule.IsMachineMaster())
					{
						_CPCSInterface.UpdateEAFGroupRequestTelegram(strReqDate);
					}
					//END OF MODIFICATION BY HEMATY
				}
				else
				{
					//_logger.WriteEntry("No new request is received", LogLevels.Info);
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}
			return result;
		}
	}
}