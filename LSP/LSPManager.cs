using System;
using System.Collections.Generic;
using System.Data;

using Irisa.Logger;
using Irisa.Common;
using Irisa.Message;
using Irisa.Message.CPS;
namespace LSP
{
	// TODO: all TODO from CLSPManager.cs

	internal class LSPManager : IProcessing
	{
		private readonly ILogger _logger;
		private readonly ICpsCommandService _scadaCommand;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
		private readonly ChangeControlStateOnServer _changeControlStateOnServer;
		private bool isCompleted = true;
		private LSPSFSCManager _sfscManager;

		// The number of jobs is sent from LSP
		private byte _activatedJobs = 0;

		private byte m_nCheckPoints = 0;

		// Array for storing a list Analog Object should be added to receiving their value changes
		// List of digital points subscribed for receiving any change on their values
		// A list of all points should be monitored if any changed, do some actions ...
		private string[] m_arrChangeableDPoints = null;

		// List of all decision tables
		public List<CDecisionTable> _DecisionTableList = new List<CDecisionTable>();

		// List of all priority lists
		public List<CPriorityList> _PriorityList = new List<CPriorityList>();

		// List of all received Jobs
		public List<CLSPJob> _LSPJobList = new List<CLSPJob>();

		// A list of EAF-Busbars for Power Calculations
		private CEAFBusbar[] _EAFBusbars = new CEAFBusbar[3];

		// A list of Transformers could be set on EAF-Busbars for Power Calculations
		private bool[] m_arrOLTransOnEAFBB = new bool[Constants.MaxNoOfTransformers + 1];

		//A Bit For Checking To Reduce Current Only Once more
		private bool m_CheckReduce = false;

		// The number of priority lists in-use
		private byte m_nPriols = 0;

		public List<LSPScadaPoint> _LSPSCADAPoints = new List<LSPScadaPoint>();

		private readonly IRepository _repository;
		//==============================================================================
		//MEMBER FUNCTIONS
		//==============================================================================
		internal LSPManager(ILogger logger, IRepository repository, ICpsCommandService scadaCommand)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_updateScadaPointOnServer = new UpdateScadaPointOnServer(logger, scadaCommand);
			_changeControlStateOnServer = new ChangeControlStateOnServer(logger, scadaCommand);
			_repository = repository;
			_scadaCommand = scadaCommand;


			
			//SendCommandTestRetry();
			//SendCommandTest2();
			//SendCommandTest();

			//return;

			//-----------------------------------------------------------------------------------
			// An array for Tag of all Digital Points in SCADA may be changed
			// Important Note: These names should match with TAG field of T_CDCPARAMS
			m_arrChangeableDPoints = new string[] { "OVERLCOND", "OVERLOADRESET", "" };

			//-------------------------------------------------------------------------
				
		}

		public void Initialize()
        {
			try
			{
				_logger.WriteEntry("----------------------  FetchDecisionTables  ----------------------", LogLevels.Info);
				var dtbMeasurements = _repository.FetchDecisionTables();
				foreach (DataRow dr in dtbMeasurements.Rows)
				{
					var aDect = new CDecisionTable(_repository, _logger);
					aDect.DectNo = Convert.ToByte(dr["DectNo"].ToString());
					aDect.nItems = Convert.ToByte(dr["nItems"].ToString());
					aDect.nCombinations = Convert.ToByte(dr["nCombinations"].ToString());

					_DecisionTableList.Add(aDect);

					_logger.WriteEntry(" ", LogLevels.Info);
					_logger.WriteEntry(" Decision Table Number = " + aDect.DectNo.ToString() + "; Decision Table Name = " + dr["Name"].ToString(), LogLevels.Info);
					_logger.WriteEntry(" Number of Items = " + aDect.nItems.ToString() + " and Number of Combinations = " + aDect.nCombinations.ToString(), LogLevels.Info);
				}

				foreach (var aDect in _DecisionTableList)
				{
					if (!aDect.LoadDecisionTable())
					{
						_logger.WriteEntry("Error in running LoadDecisionTables" + aDect.DectNo.ToString(), LogLevels.Error);
						return;
					}
				}

				//-------------------------------------------------------------------------
				dtbMeasurements = _repository.FetchPriorityLists();
				foreach (DataRow dr in dtbMeasurements.Rows)
				{
					var priol = new CPriorityList(_repository, _logger)
					{
						_priorityNo = Convert.ToByte(dr["PRIORITYLISTNO"].ToString()),
						_nBreakers = Convert.ToByte(dr["NITEMS"].ToString()),
						_description1 = dr["DESCRITPION1"].ToString()
					};

					// Loading all CBes for this Priority List
					_logger.WriteEntry(" --------------------  Load Breakers to Shed  --------------------- ", LogLevels.Info);
					if (!priol.LoadBreakersToShed())
					{
						_logger.WriteEntry("Could not load priority list for PriolNo : " + priol._priorityNo.ToString(), LogLevels.Error);
					}

					_PriorityList.Add(priol);

					_logger.WriteEntry(" -------------------------------------------- ", LogLevels.Info);
					_logger.WriteEntry(" PriolNo = " + priol._priorityNo.ToString() + "; N_CBes = " + priol._nBreakers.ToString(), LogLevels.Info);
				}

				//-------------------------------------------------------------------------
				// moved to Repository
				//if (!LoadLSPCheckPoints())
				//{
				//	_logger.WriteEntry("CLSPManager..Class_Initialize()" + "Error in running LoadLSPCheckPoints", LogLevels.Error);
				//	return;
				//}

				//-------------------------------------------------------------------------
				for (int I = 1; I <= 2; I++)
				{
					_EAFBusbars[I] = new CEAFBusbar(_logger);
				}

				//-------------------------------------------------------------------------
				// We disappear all Alarm points here, or ...
				if (_repository.GetLSPScadaPoint("LSPACTIVATED").Value == (float)(SinglePointStatus.Appear))
					if (!_updateScadaPointOnServer.WriteDigital(_repository.GetLSPScadaPoint("LSPACTIVATED"), SinglePointStatus.Disappear, string.Empty))
					{
						_logger.WriteEntry("Reset alarm was failed for LSPACTIVATED.", LogLevels.Error);
					}

				if (_repository.GetLSPScadaPoint("PCSPowerLimit").Value == (float)(SinglePointStatus.Appear))
					if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("PCSPowerLimit"), SinglePointStatus.Disappear, string.Empty))
					{
						_logger.WriteEntry("Reset alarm was failed for PCSPowerLimit.", LogLevels.Error);
					}

				if (_repository.GetLSPScadaPoint("WRONGNETWORKSTATUS").Value == (float)(SinglePointStatus.Appear))

					if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("WRONGNETWORKSTATUS"), SinglePointStatus.Disappear, string.Empty))
					{
						_logger.WriteEntry("Reset alarm was failed for WRONGNETWORKSTATUS.", LogLevels.Error);
					}

				//if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("ALARMLIST"), SinglePointStatus.Disappear, ""))
				//{
				//		_logger.WriteEntry("Reset alarm was failed for ALARMLIST.", LogLevels.Error);
				//}

				if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("OVERLCOND"), SinglePointStatus.Disappear,
					"LSP try to clear remained invalid Appear on OVERLOAD"))
					_logger.WriteEntry("Error in LSP try to clear remaned invalid Appear on OVERLOAD!'", LogLevels.Error);

				var overLReset = _repository.GetLSPScadaPoint("OVERLOADRESET");
				if (!_updateScadaPointOnServer.WriteDigital(overLReset, SinglePointStatus.Disappear, "Reset 'OverLoad Reset Control' for LSP"))
				{
					_logger.WriteEntry("Reset OverLoad Reset Control for LSP.", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.ApplyMarkerCommand(overLReset))
					_logger.WriteEntry("Error in LSP in 'trying to remove Blocked Marker from OVERLOAD!'", LogLevels.Error);

				// Create and run SFSCManager
				_sfscManager = new LSPSFSCManager(_logger, _repository, _scadaCommand, _PriorityList);


				//_logger.WriteEntry(" INITILIZE: Before SendCommandTestRetry ", LogLevels.Info);
				//SendCommandTestRetry();
				//_logger.WriteEntry(" INITILIZE: After SendCommandTestRetry", LogLevels.Info);

			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		~LSPManager()
		{
			try
			{
				_logger.WriteEntry(" Terminating is started... ", LogLevels.Info);

				// Destroy objects
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		// This sub receive all changes or updates on the added points for events to ScadaEvent
		public void SCADAEventRaised(MeasurementData measurement)
		{
			try
			{
				string strGUID = measurement.MeasurementId.ToString();
				string strValue = measurement.Value.ToString();

				if (isCompleted == false)
					return;

				isCompleted = false;
				//_logger.WriteEntry($"Guid = {measurementId} is received with value {value}", LogLevels.Info);

				//--------------------------------------------------------------------------
				// Prepare TAG of received point
				var aTag = _repository.GetLSPScadaPoint(Guid.Parse(strGUID));

				if (aTag is null)
				{
					//_logger.WriteEntry("GUID is not found! , " + strGUID, LogLevels.Warn);
					isCompleted = true;
					return;
				}

				//Check SFSC FUnction Status
				if (aTag.Name == "SFSC_STATUS" && (aTag.Value == 0.0))
				{
					_logger.WriteEntry("LSP_SFSC function is Disabled", LogLevels.Warn);
				}

				if (aTag.Name == "SFSC_STATUS" && (aTag.Value == 1.0))
				{
					_logger.WriteEntry("LSP_SFSC function is Enabled", LogLevels.Warn);
				}


				//--------------------------------------------------------------------------
				// Call related function
				// Check status of function, If function is not enabled, Return
				// TODO: check status of function
				if (aTag.Name == "FunctionSatus" && (aTag.Value == 1.0))
				{
					_logger.WriteEntry("LSP function is Enabled", LogLevels.Warn);
				}

				if (aTag.Name == "FunctionSatus" && (aTag.Value == 0))
				{
					_logger.WriteEntry("LSP function is Disabled", LogLevels.Warn);
				}
				else
				{
					// TODO: check LSPACTIVATED or OVERLCOND?
					// OVERLCOND:		OCP to LSP trigerring
					// LSPACTIVATED:	LSP informs Operator by this Alarm
					// LSPTELEGRAM:		LSP to DC  trigerring
					// TODO: LSPACTIVATED	=>	"INPUTOUTPUT" in table app.LSP_PARAMS
					if ((aTag.Name == "OVERLCOND") && ((SinglePointStatus)aTag.Value == SinglePointStatus.Appear))
					{
						_logger.WriteEntry("... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ...", LogLevels.Info);
						_logger.WriteEntry("Processing OVERLOAD is started . . . ", LogLevels.Info);

						if (_repository.GetLSPScadaPoint("FunctionSatus").Value == 1.0)
						{
							if (!ProcessOverload(strValue))
							{
								_logger.WriteEntry("Process OVERLOAD was failed", LogLevels.Error);
							}
							else
							{
								_logger.WriteEntry("Processing OVERLOAD was accomplished succesfully", LogLevels.Info);
							}
						}
						else
						{
							_logger.WriteEntry("Processing OVERLOAD not accomplished because 'LSP Function for OCP' was disabled ", LogLevels.Warn);
						}


						// 1399.09.25 KAJI    001_Added
						var overLCond = _repository.GetLSPScadaPoint("OVERLCOND");
						if (!_updateScadaPointOnServer.WriteDigital(overLCond, SinglePointStatus.Disappear,
							"LSP has processed OVERLOAD from OCP, successfull processing."))
							_logger.WriteEntry("Error in sending Alarm for 'LSP has received OVERLOAD, Clear flag after processing, Succesfull processing.'", LogLevels.Error);

						//--------------------------------------------------------------------------
						// Log successful exit of LSP
						//_logger.WriteEntry($"Exit of processing SCADAEventRaised with {aTag.Name.ToString()}, " +
						//	$"{aTag.Value.ToString()}, {measurement.SelectedMeasurementSource.ToString()}, . . . ", LogLevels.Info);
						//_logger.WriteEntry("..........................................................................................................", LogLevels.Info);
					}
					else
					{
						if (aTag.Name == "OVERLOADRESET")
						{
							if (((SinglePointStatus)aTag.Value == SinglePointStatus.Disappear) &&
								(measurement.SelectedMeasurementSource != (int)MeasurementSource.Calculated))
							{
								if (!ResetOverload(strValue))
								{
									_logger.WriteEntry("Processing RESET OVERLOAD Button was failed", LogLevels.Error);
								}
								else
								{
									_logger.WriteEntry("Processing RESET OVERLOAD Button was accomplished succesfully", LogLevels.Info);
								}
							}
							//--------------------------------------------------------------------------
							// Log succesful exit of LSP
							//_logger.WriteEntry($"Exit of processing SCADAEventRaised with {aTag.Name.ToString()}, " +
							//	$"{aTag.Value.ToString()}, {measurement.SelectedMeasurementSource.ToString()}, . . . ", LogLevels.Info);
							//_logger.WriteEntry("..........................................................................................................", LogLevels.Info);
						}
						else
						{
							//_logger.WriteEntry("LSP was triggered by wrong Point, point name : " + aTag.Name + "Value = " + aTag.Value, LogLevels.Warn);
						}
					}
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			isCompleted = true;
		}

		private bool ProcessOverload(string strValue)
		{
			bool result = false;
			try
			{
				float aBBPower = 0;
				byte aEAFGroup = 0;

				result = false;
				//--------------------------------------------------------------------------
				//
				_logger.WriteEntry("----------------- Processing Overload -------------------", LogLevels.Info);

				// TODO: check
				var value = Convert.ToInt32(strValue);

				if (value != ((int)eApp_Disapp.Appear))
				{
					_logger.WriteEntry("Value of OVERLCOND is not appeared, value = " + strValue, LogLevels.Info);
					return true;
				}

				//--------------------------------------------------------------------------
				// 1. Open a connection in CDBInterface

				//--------------------------------------------------------------------------
				// 2. Preparing Busbar data
				for (byte I = 1; I <= 2; I = (byte)(I + 1))
				{
					_EAFBusbars[I].ResetBusbarData();
				}

				// Reset all transformers from Overload
				for (byte I = 1; I <= Constants.MaxNoOfTransformers; I = (byte)(I + 1))
				{
					m_arrOLTransOnEAFBB[I] = false;
				}

				//--------------------------------------------------------------------------
				// 3. If function is enabled, do all required actions in response to an event
				// Checking changed value is required here, before calling ProcessJobs
				if (!ProcessJobs())
				{
					_logger.WriteEntry("Processing Jobs was failed", LogLevels.Error);
					//' Send Alarm
				}
				else
				{
					_logger.WriteEntry("Processing Jobs was accomplished succesfully", LogLevels.Info);

					//--------------------------------------------------------------------------
					// 4. Processing Busbars for Maximum powers
					if (_EAFBusbars[1].BusbarPower > 0 || _EAFBusbars[2].BusbarPower > 0)
					{
						_logger.WriteEntry("----------------------  ProcessBusbars  ----------------------", LogLevels.Info);
						if (!ProcessBusbars())
						{
							_logger.WriteEntry("Processing Busbars was failed", LogLevels.Error);
							//' Send Alarm
						}
						else
						{
							//---------------------------------------
							// Preparing and sending overload job to PCS
							if (_EAFBusbars[1].BusbarPower > 0)
							{
								aBBPower = _EAFBusbars[1].BusbarPower;

								//Modification By Mr.Hematy For Offer From Boss To Setting Max Power To PCS Less tHAN Or Equal 500 MW
								if (aBBPower > 500)
								{
									_logger.WriteEntry("Calculated Power=" + aBBPower.ToString() + " So Power Should Be Reduce!", LogLevels.Info);
									aBBPower = (float)ReducePower(aBBPower);
								}
								//End Of Modification

								if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P1MAX_CALC"), aBBPower))
								{
									_logger.WriteEntry("Could not write value of OVERLOAD1 for EEC", LogLevels.Error);
								}
								else
								{
									_logger.WriteEntry("EEC-OVERLOAD1 = " + aBBPower.ToString(), LogLevels.Info);
								}

								_logger.WriteEntry("Busbar-A Max-Power = " + _EAFBusbars[1].BusbarPower.ToString(), LogLevels.Info);
								aEAFGroup = 1;
							}
							else
							{
								aBBPower = _EAFBusbars[2].BusbarPower;

								//Modification By Mr.Hematy For Offer From Boss To Setting Max Power To PCS Less tHAN Or Equal 500 MW
								if (aBBPower > 500)
								{
									_logger.WriteEntry("Calculated Power=" + aBBPower.ToString() + " So Power Should Be Reduce!", LogLevels.Info);
									aBBPower = (float)ReducePower(aBBPower);
								}
								//End Of Modification

								if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P2MAX_CALC"), aBBPower))
								{
									_logger.WriteEntry("Could not write value of OVERLOAD2 for EEC", LogLevels.Error);
								}
								else
								{
									_logger.WriteEntry("EEC-OVERLOAD2 = " + aBBPower.ToString(), LogLevels.Info);
								}

								_logger.WriteEntry("Busbar-B Max-Power = " + _EAFBusbars[2].BusbarPower.ToString(), LogLevels.Info);
								aEAFGroup = 2;
							}

							//---------------------------------------------------------------------------------------------------------------------
							// Sending overload job to PCS
							_logger.WriteEntry("--------------  Sending overload job to PCS  ----------------", LogLevels.Info);
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("EAFGROUPNO"), aEAFGroup))
							{
								_logger.WriteEntry("Could not write value of EAFGroupNo for PCS-Telegram", LogLevels.Error);
							}
							else
							{
								//Call theCTraceLogger.WriteLog(TraceInfo1, "EEC-OVERLOAD1 = " & aBBPower)
							}

							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("PMAX"), aBBPower))
							{
								_logger.WriteEntry("Could not write value of PMAX for PCS-Telegram", LogLevels.Error);
							}
							else
							{
								//Call theCTraceLogger.WriteLog(TraceInfo1, "EEC-OVERLOAD1 = " & aBBPower)
							}

							//--------------------------------------------------------------------------------------------------------------------
							// Informing DC-Function
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("LSPTELEGRAM"), Convert.ToSingle(eApp_Disapp.Disappear)))
							{
								_logger.WriteEntry("Could not write value of LSPTELEGRAM for PCS-Telegram, Disappear", LogLevels.Error);
							}

							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("LSPTELEGRAM"), Convert.ToSingle(eApp_Disapp.Appear)))
							{
								_logger.WriteEntry("Could not write value of LSPTELEGRAM for PCS-Telegram, Appear", LogLevels.Error);
							}
							else
							{
								_logger.WriteEntry("Trigger of PCS-Telegram was sent to SCADA, LSPTELEGRAM, Appear", LogLevels.Info);
							}

							//---------------------------------------
							_logger.WriteEntry("Busbar-A Max-Power = " + _EAFBusbars[1].BusbarPower.ToString(), LogLevels.Info);

							//---------------------------------------------------------------------------------------------------------------------
							// Send Alarm To Operator
							
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("PCSPowerLimit"), SinglePointStatus.Appear, "Sending Power Limit to PCS for EAF-Group: " + aEAFGroup.ToString() + " ; Power Limit for PCS: " + aBBPower.ToString()))
							{
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							}
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("PCSPowerLimit"), SinglePointStatus.Disappear, ""))
							{
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							}
						}

						//----------------------------------------------------------------------------------------------------------------------
						// Reset Overload Control
						if (!_updateScadaPointOnServer.WriteDigital(_repository.GetLSPScadaPoint("OVERLOADRESET"), SinglePointStatus.Appear, "Reset PCS Power Limit after going to Normal condition in EEC page . . . "))
						{
							_logger.WriteEntry("Fail to set OverLoad for LSP ...", LogLevels.Error);
						}
						//if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("OVERLOADRESETCONTROL"), 1.0f))
						//{ // Appearing
						//	_logger.WriteEntry("Could not write value of OVERLOADRESETCONTROL", LogLevels.Error);
						//}
						//else
						//{
						//	_logger.WriteEntry("OVERLOADRESETCONTROL = " + "0", LogLevels.Info);
						//}
					}
					else
					{
						// Send reset job to PCS
						_logger.WriteEntry("Busbar-A Power = 0 ; Busbar-B Power = 0", LogLevels.Info);
					}

					//-----------------------------------------------------------------------------------------------------------------
					// 5. Processing Activated Priority Lists
					_logger.WriteEntry("----------------------  ProcessPriols  ----------------------", LogLevels.Info);
					if (!ProcessPriols())
					{
						_logger.WriteEntry("Processing Priority Lists was failed", LogLevels.Error);
						// Send Alarm
					}
					else
					{
						//-----------------------------------------------------------------------------------------------------------------
						// 6. Reset the Overload Tag
						// Removing SumIt of ShedPoints
						_logger.WriteEntry("--------------    Reset IT of Check Points    -----------------", LogLevels.Info);
						if (!ResetCheckPoints())
						{
							_logger.WriteEntry("Resetting IT of Activated Points by LSP was failed!", LogLevels.Error);
						}
						else
						{
							// TODO: Check
							if (!_updateScadaPointOnServer.WriteDigital(_repository.GetLSPScadaPoint("LSPACTIVATED"), SinglePointStatus.Disappear, ""))
							//if (!m_theCDBInterface.WriteLSPActivation(Conversion.Str(GeneralModule.eLSPActivation.ResetOverload)))
							{
								_logger.WriteEntry("Resetting LSPACTIVATED in SCADA was failed", LogLevels.Error);
							}
						}
						_logger.WriteEntry("All Activated Priority lists was processed succesfully", LogLevels.Info);
					}
				}

				//-----------------------------------------------------------------------------------------------------------------
				// 7. Close connection to DB in CDBInterface

				//-----------------------------------------------------------------------------------------------------------------
				// 8. Log succesful exit of LSP
				_logger.WriteEntry("End of Processing overload . . . ", LogLevels.Info);

				return true;
			}
			catch (System.Exception excep)
			{
				{
					_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
				}
			}
			return result;
		}

		private bool ResetOverload(string strValue)
		{
			try
			{
				byte aEAFGroupNo = 0;

				aEAFGroupNo = Convert.ToByte(_repository.GetLSPScadaPoint("EAFGROUPNO").Value);
				//---------------------------------------
				// Preparing and sending overload job to PCS
				//
				if (aEAFGroupNo == 1)
				{
					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P1MAX_CALC"), 0f))
					{
						_logger.WriteEntry("Could not write value of OVERLOAD1 for EEC", LogLevels.Error);
					}
					else
					{
						_logger.WriteEntry("EEC-OVERLOAD1 = " + "0", LogLevels.Info);
					}
				}
				else
				{
					if (aEAFGroupNo == 2)
					{
						//
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P2MAX_CALC"), 0f))
						{
							_logger.WriteEntry("Could not write value of OVERLOAD2 for EEC for P2MAX_CALC", LogLevels.Error);
						}
						else
						{
							_logger.WriteEntry("EEC-OVERLOAD2 = " + "0", LogLevels.Info);
						}
					}
					else
					{
						if (aEAFGroupNo == 0)
						{
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P1MAX_CALC"), 0f))
							{
								_logger.WriteEntry("Could not write value of OVERLOAD1 for EEC", LogLevels.Error);
							}
							else
							{
								_logger.WriteEntry("EEC-OVERLOAD1 = " + "0", LogLevels.Info);
							}

							//
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("P2MAX_CALC"), 0f))
							{
								_logger.WriteEntry("Could not write value of OVERLOAD2 for EEC", LogLevels.Error);
							}
							else
							{
								_logger.WriteEntry("EEC-OVERLOAD2 = " + "0", LogLevels.Info);
							}
						}
						else
						{
							// Error
						}
					}
				}

				//---------------------------------------
				// Sending overload job to PCS
				_logger.WriteEntry("ResetOverload", LogLevels.Info);
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("EAFGROUPNO"), 0f))
				{
					_logger.WriteEntry("Could not write value of EAFGroupNo for PCS-Telegram", LogLevels.Error);
					//!!!!!!!
				}
				else
				{
					//Call theCTraceLogger.WriteLog(TraceInfo1, "CLSPManager..ResetOverload()", "EEC-OVERLOAD1 = " & aBBPower)
				}

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetLSPScadaPoint("PMAX"), 0f))
				{
					_logger.WriteEntry("Could not write value of PMAX for PCS-Telegram", LogLevels.Error);
					//!!!!!!!
				}
				else
				{
					//Call theCTraceLogger.WriteLog(TraceInfo1, "CLSPManager..ResetOverload()", "EEC-OVERLOAD1 = " & aBBPower)
				}

				//---------------------------------------
				// Informing DC-Function
				if (!_updateScadaPointOnServer.WriteDigital(_repository.GetLSPScadaPoint("LSPTELEGRAM"), SinglePointStatus.Disappear, ""))
				{
					_logger.WriteEntry("Could not write value of LSPTELEGRAM for PCS-Telegram", LogLevels.Error);
				}
				else
				{
					_logger.WriteEntry("PCS-Telegram was sent to DC", LogLevels.Info);
				}

				//---------------------------------------
				// TODO: Send Alarm To Operator
				
				if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("PCSPowerLimit"), SinglePointStatus.Appear, "Sending reset Overload-Power to PCS for EAF-Group: " + aEAFGroupNo.ToString()))
				{
					_logger.WriteEntry("Sending alarm was failed for PCSPowerLimit.", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("PCSPowerLimit"), SinglePointStatus.Disappear, ""))
				{
					_logger.WriteEntry("Sending alarm was failed for PCSPowerLimit.", LogLevels.Error);
				}

				//----------------------------------------
				// Reset Overload Control
				var overLReset = _repository.GetLSPScadaPoint("OVERLOADRESET");
				if (!_updateScadaPointOnServer.WriteDigital(overLReset, SinglePointStatus.Disappear, "Reset OverLoad Reset Control for LSP"))
				{
					_logger.WriteEntry("Reset OverLoad Reset Control for LSP.", LogLevels.Error);
				}

				if (!_updateScadaPointOnServer.ApplyMarkerCommand(overLReset))
					_logger.WriteEntry("Error in LSP in 'trying to remove Blocked Marker from OVERLOAD!'", LogLevels.Error);


				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
				return false;
			}
		}

		// Processing all received Jobs of LSP
		private bool ProcessJobs()
		{
			bool result = false;
			byte idxPriol = 0;
			byte aCombNo = 0;
			string strTemp = "";
			bool bBigTrans = false;

			//"'IMANIAN  1395.12  ADDING L914,L915
			byte I = 0;
			//"' END IMANIAN  1395.12  ADDING L914,L915

			try
			{
				//------------------------------------------------------------------------
				// 0. Reseting all priols
				foreach (var aPriols in _PriorityList)
				{
					if (aPriols != null)
					{
						if (!aPriols.ResetPriolData())
						{
							_logger.WriteEntry("Error in Reset Priority List Data", LogLevels.Error);
						}
					}
				}

				//------------------------------------------------------------------------
				// 1. Finding activated jobs
				//'KAJI START of CycVal
				if (!FindActivatedJobs())
				{
					//'KAJI END of CycVal
					_logger.WriteEntry("Error in running FindActivatedJobs", LogLevels.Error);
				}

				if (_activatedJobs < 1)
				{
					_logger.WriteEntry("There is not any activated Job from OCP", LogLevels.Warn);
				}

				foreach (var aJob in _LSPJobList)
				{
					aJob.ResetJobValues();

					OCPCheckPoint checkPoint = _repository.GetCheckPoint(aJob.CheckPointNo);
					aJob.DectNo = Convert.ToByte(checkPoint.DecisionTable.ToString());
					aJob.PrimaryVoltage = Convert.ToInt32(checkPoint.VoltageEnom);
					aJob.SecondaryVoltage = Convert.ToInt32(checkPoint.VoltageDenom);

					if (checkPoint.ShedType == "Some")
						aJob.ShedType = eShedType.SomeLoads;
					else
						if (checkPoint.ShedType == "All")
						aJob.ShedType = eShedType.AllLoads;
					else
						aJob.ShedType = eShedType.None;

					// 2016.02.16    A.K Modeification for ...
					var aDect = _DecisionTableList.Find(dect => dect.DectNo == aJob.DectNo);

					_logger.WriteEntry(" --------------------- ProcessJobs ------------------------- ", LogLevels.Info);
					_logger.WriteEntry("CheckPointNo: " + aJob.CheckPointNo.ToString() + " ; CheckPointNo = " + aJob.CheckPointNo.ToString() + " ; DectNo= " + aJob.DectNo.ToString() + " ; NetworkPath = " + checkPoint.NetworkPath, LogLevels.Info);

					//------------------------------------------------------------------------
					// 2. Reading the CBes and DSes values for this Job/Dect
					if (!aDect.ReadDectItemValues())
					{
						_logger.WriteEntry("Status of CBs and DSs could not be read.", LogLevels.Error);
						continue;
					}

					//------------------------------------------------------------------------
					// 5.
					if (!aDect.ConvertItToSV(aJob))
					{
						_logger.WriteEntry("ConvertItToSV was failed", LogLevels.Error);
						// Sending Alarm to Operator is required here
						continue;
					}

					//------------------------------------------------------------------------
					// 3. Finding matched combination of network for this Decision Table
					if (!aDect.FindCombination(ref aCombNo))
					{
						_logger.WriteEntry("Find combination was failed", LogLevels.Error);
						// Sending Alarm to Operator is required here
						continue;
					}

					if (aCombNo == 0)
					{
						_logger.WriteEntry("Found combination is not valid", LogLevels.Error);

						// Sending Alarm to Operator is required here
						if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("WRONGNETWORKSTATUS"), SinglePointStatus.Disappear, ""))
						{
							_logger.WriteEntry("Sending alarm was failed for WRONGNETWORKSTATUS.", LogLevels.Error);
						}
						if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("WRONGNETWORKSTATUS"), SinglePointStatus.Appear, "Network Status Error for Transformer"))
						{
							_logger.WriteEntry("Sending alarm was failed for WRONGNETWORKSTATUS.", LogLevels.Error);
						}
						continue;
					}

					//------------------------------------------------------------------------
					// 4. Finding related priol to this Combination
					if (!aDect.FindPriorityListNo(aCombNo, ref idxPriol))
					{
						_logger.WriteEntry("FindPriorityListNo was failed", LogLevels.Error);
						continue;
					}
					else
					{
						aJob.PriolNo = idxPriol;
					}

					if (idxPriol == 0)
					{
						_logger.WriteEntry("Found PriorityListNo is not valid", LogLevels.Error);
						continue;
					}

					//------------------------------------------------------------------------
					// 5. Checking Priority List No
					// TODO: check
					var aPriolFound = _PriorityList.Find(priol => priol._priorityNo == idxPriol);

					// Note: Previuos Shedtype should be considered here
					//If m_arrPriols(idxPriolRowNo).ShedType <> eShedType.AllLoads Then
					aPriolFound._shedType = aJob.ShedType;
					//End If

					// Check big trans( except T4 and T6) on PP Busbar
					bBigTrans = false;
					if (aJob.DectNo == Constants.T1AN_DectNo)
					{
						bBigTrans = true;
					}
					if (aJob.DectNo == Constants.T2AN_DectNo)
					{
						bBigTrans = true;
					}
					if (aJob.DectNo == Constants.T3AN_DectNo)
					{
						bBigTrans = true;
					}
					if (aJob.DectNo == Constants.T5AN_DectNo)
					{
						bBigTrans = true;
					}
					if (aJob.DectNo == Constants.T7AN_DectNo)
					{
						bBigTrans = true;
					}

					//' KAJI START fo T8AN
					if (aJob.DectNo == Constants.T8AN_DectNo)
					{
						bBigTrans = true;
					}
					//' KAJI END fo T8AN

					if (aPriolFound._priorityNo == Constants.PRIORITYLISTNO_PP && bBigTrans)
					{
						// For other cases, conversion rate is considered properly
						aPriolFound._sumIt += aJob.SumIt;
						aPriolFound._shedValue += aJob.ShedValue * Constants.BigTransOnPP_VoltageNum / Constants.BigTransOnPP_VoltageDenom;
					}
					else
					{
						aPriolFound._sumIt += aJob.SumIt;
						aPriolFound._shedValue += aJob.ShedValue;
					}

					//'Call theCTraceLogger.WriteLog(TraceInfo4, "CLSPManager..ProcessJobs()", "Shed value of Priority List = " & m_arrPriols(idxPriolRowNo).ShedValue)

					//------------------------------------------------------------------------
					// 6. Sending LSP Activation message to Operator
					strTemp = "Priority List No. ";
					strTemp = strTemp + aPriolFound._priorityNo.ToString();
					strTemp = strTemp + "; Priority List: ";
					strTemp = strTemp + aPriolFound._description1.ToString();
					strTemp = strTemp + "; SumIt: ";
					strTemp = strTemp + aPriolFound._sumIt.ToString();
					strTemp = strTemp + "; For ";
					strTemp = strTemp + checkPoint.NetworkPath;

					var lspActivated = _repository.GetLSPScadaPoint("LSPACTIVATED");

					if (!_updateScadaPointOnServer.SendAlarm(lspActivated, SinglePointStatus.Disappear, " "))
					{
						_logger.WriteEntry("Writing Disappear was failed for LSPACTIVATED!", LogLevels.Error);
					}

					if (!_updateScadaPointOnServer.SendAlarm(lspActivated, SinglePointStatus.Appear, strTemp))
					{
						_logger.WriteEntry("Writing Appear was failed for LSPACTIVATED!", LogLevels.Error);
					}

					//If Not m_theCSCADAInterface.SendAlarm(strAlarmPoint, m_arrPriols(idxPriolRowNo).ShedValue, "Current Value for shedding") Then
					//    Call theCTraceLogger.WriteLog(TraceInfo1, "CLSPManager..ProcessJobs()", "Sending alarm was failed.")
					//End If

					//------------------------------------------------------------------------
					// 7. Checking Overload on Big-Transfromers
					if (aPriolFound._priorityNo == Constants.PRIORITYLISTNO_EAF)
					{
						if (!ProcessOverloadedTrans(aDect, aJob.AllowedActivePower))
						{
							_logger.WriteEntry("Processing of Transfomer Powers was failed", LogLevels.Error);
						}
						else
						{
							_logger.WriteEntry("Busbar power calculations for overloaded transforemr was accomlished", LogLevels.Info);
						}
					}

					//------------------------------------------------------------------------
					//"'IMANIAN  1395.12  ADDING L914,L915
					if (aPriolFound._priorityNo == Constants.PRIORITYLISTNO_NISLINES)
					{
						if (aJob.DectNo == Constants.L914_DectNo)
						{
							_logger.WriteEntry("Overloaded LINE is 914 (TIRAN)", LogLevels.Info);
						}
						else
						{
							_logger.WriteEntry("Overloaded LINE is 915 (CHEHELSTON)", LogLevels.Info);
						}

						//var priolNISLines = _PriorityList.Find(priol => priol._priorityNo == Constants.L914_DectNo);
						var priolNISLines = _PriorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_NISLINES);
						priolNISLines._nBreakers = 3;
						for (byte idPriol = 1; idPriol <= 3; idPriol++)
						{
							priolNISLines.GetArrBreakers(idPriol).NetworkPath_Item = " ";
							priolNISLines.GetArrBreakers(idPriol).NetworkPath_Cur = "0";
							priolNISLines.GetArrBreakers(idPriol).HasPartner = " ";
							priolNISLines.GetArrBreakers(idPriol).AddressPartner = " ";
							priolNISLines.GetArrBreakers(idPriol).FurnaceIndex = "0";
						}
						priolNISLines._nBreakers = 0;

						string strSQLException = "";
						int iExcpType = 0;
						if (!CheckExceptionInSheddingFurnaces(ref iExcpType))
						{
							_logger.WriteEntry("Call CheckExceptionInSheddingFurnaces was Failed, All exceptions are considered.", LogLevels.Error);
							iExcpType = 1;
						}

						//"IMANIAN 1396.11.02. EXCLUDE MF8 FROM SHEDDING LIST
						strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF8%' ";
						//"IMANIAN 1396.11.02. EXCLUDE MF8 FROM SHEDDING LIST

						if (iExcpType == 1)
						{
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF1%' ";
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF3%' ";
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF6%' ";
						}

						if (iExcpType == 2)
						{
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF3%' ";
						}

						if (iExcpType == 3)
						{
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF1%' ";
							strSQLException += " AND CB_NETWORKPATH NOT LIKE '%MF6%' ";
						}

						//string strSql = "SELECT * FROM app.EEC_SFSCEAFSPriority  WHERE STATUS_OF_FURNACE='ON' ORDER BY CONSUMED_ENERGY_PER_HEAT ASC";
						// TODO: Fetch data from its table
						//var dtbEAFsGroup = _repository.FetchEAFsGroup(strSql);
						var dtbEAFsGroup = _repository.FetchEAFSPriority("", "ON", strSQLException);
						var priorityNISLines = _PriorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_NISLINES);
						if (priorityNISLines is null)
						{
							_logger.WriteEntry("Error in finding PRIORITYLISTNO_NISLINES for ProcessJobs . . . ", LogLevels.Error);
						}
						I = 1;
						foreach (DataRow dr in dtbEAFsGroup.Rows)
						{
							_logger.WriteEntry("--------------- Update_m_arrPriol_IdxPriolsEAF -------------------", LogLevels.Info);

							// TODO: process data here . . . 


							priorityNISLines._nBreakers = (byte)(priorityNISLines._nBreakers + 1);
							_logger.WriteEntry(" ----------------- Update_m_arrPriol_IdxPriolsEAF -----------------", LogLevels.Info);
							priorityNISLines._breakersToShed[I].NetworkPath_Item = dr["CB_NETWORKPATH"].ToString();
							_logger.WriteEntry("m_arrPriols(IdxPriolsLINES).arrBreakers('" + I.ToString() + "').NetworkPath_Item = " + priorityNISLines._breakersToShed[I].NetworkPath_Item, LogLevels.Info);
							priorityNISLines._breakersToShed[I].NetworkPath_Cur = dr["CT_NetworkPath"].ToString();
							_logger.WriteEntry("m_arrPriols(IdxPriolsLINES).arrBreakers('" + I.ToString() + "').NetworkPath_Cur = " + priorityNISLines._breakersToShed[I].NetworkPath_Cur, LogLevels.Info);
							priorityNISLines._breakersToShed[I].HasPartner = dr["HasPartner"].ToString();
							_logger.WriteEntry("m_arrPriols(IdxPriolsLINES).arrBreakers('" + I.ToString() + "').HasPartner = " + priorityNISLines._breakersToShed[I].HasPartner, LogLevels.Info);
							priorityNISLines._breakersToShed[I].AddressPartner = dr["PartnerAddress"].ToString();
							_logger.WriteEntry("m_arrPriols(IdxPriolsLINES).arrBreakers('" + I.ToString() + "').AddressPartner = " + priorityNISLines._breakersToShed[I].AddressPartner, LogLevels.Info);
							priorityNISLines._breakersToShed[I].FurnaceIndex = dr["Furnace"].ToString();
							_logger.WriteEntry("m_arrPriols(IdxPriolsLINES).arrBreakers('" + I.ToString() + "').FurnaceIndex  = " + priorityNISLines._breakersToShed[I].FurnaceIndex, LogLevels.Info);

							if(priorityNISLines._breakersToShed[I].HasPartner == "YES")
								//priorityNISLines._breakersToShed[I].addressPartner_guid = Guid.Parse(dr["PARTNER_GUID"].ToString());
								priorityNISLines._breakersToShed[I].addressPartner_guid = _repository.GetGuid(priorityNISLines._breakersToShed[I].AddressPartner);
							//priorityNISLines._breakersToShed[I].guid_item = Guid.Parse(dr["CB_GUID"].ToString());
							priorityNISLines._breakersToShed[I].guid_item = _repository.GetGuid(priorityNISLines._breakersToShed[I].NetworkPath_Cur);
							//priorityNISLines._breakersToShed[I].guid_curr = Guid.Parse(dr["CT_GUID"].ToString());
							priorityNISLines._breakersToShed[I].guid_curr = _repository.GetGuid(priorityNISLines._breakersToShed[I].NetworkPath_Item);


							if (I < 3)
							{
								I = (byte)(I + 1);
							}
							else
							{
								goto aaa;
							}
						}
						//****************
					}
					aaa:;

					//"' END IMANIAN  1395.12  ADDING L914,L915
					//If m_arrPriols(idxPriolRowNo).PriorityNo = PRIORITYLISTNO_NISLINES Then
					//    If Not ProcessOverloadedNISLines(idxDect) Then
					//        Call theCTraceLogger.WriteLog(TraceError, "CLSPManager..ProcessJobs()", "Processing of Overloded in NIS Lines (914-915) was failed")
					//    Else
					//         Call theCTraceLogger.WriteLog(TraceInfo2, "CLSPManager..ProcessJobs()", "Prority list selection for overloaded NIS LINES (914-915) was accomlished")
					//   End If
					//End If

					//------------------------------------------------------------------------
					//10.
					_logger.WriteEntry("Processing Job for CheckPoint " + aJob.CheckPointNo.ToString() + " was accomplished", LogLevels.Info);

					result = true;
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			return result;
		}

		//
		private bool ProcessPriols()
		{
			bool result = false;
			Guid shedItem;

			try
			{
				_logger.WriteEntry("--------------------------------------------", LogLevels.Info);
				_logger.WriteEntry("Processing priority lists is started ", LogLevels.Info);

				foreach (var aPriol in _PriorityList)
				{
					var dtCurrShedTime = DateTime.Now;

					//Call theCTraceLogger.WriteLog(TraceError, "idxPriol = " & idxPriol & " ; PriorityList = " & m_arrPriols(idxPriol).PriorityNo)

					// ---------------------------------------------------------------------------
					// Shedding the Breaker
					if (aPriol._shedValue > 0)
					{
						_logger.WriteEntry("--------------------------------------------", LogLevels.Info);
						_logger.WriteEntry("PriolNo = " + aPriol._priorityNo.ToString() + " has been overloaded with current= " + aPriol._shedValue.ToString(), LogLevels.Info);

						if (!aPriol.ReadPriorityItems())
						{
							_logger.WriteEntry("Reading currents and status of items is failed for PriorityList = " + aPriol._priorityNo.ToString(), LogLevels.Error);
							//GoTo NextPriols
							// !!!!!!!
							continue;
						}

						// ---------------------------------------------------------------------------
						// Shedding all breakers
						if (aPriol._shedType == eShedType.AllLoads)
						{
							_logger.WriteEntry("Shed All Loads for Priority List is starting ...", LogLevels.Info);

							for (byte idxItem = 1; idxItem <= aPriol._nBreakers; idxItem++)
							{
								//If m_arrPriols(idxPriol).arrBreakers(idxItem).Status = Breaker_Status.bClose And _
								//'    m_arrPriols(idxPriol).arrBreakers(idxItem).Current > 0 Then
								// TODO:
								shedItem = aPriol.GetArrBreakers(idxItem).guid_item;
								var cbToShed = _repository.GetLSPScadaPoint(shedItem);
								if (!_updateScadaPointOnServer.SendCommand(cbToShed, (int)Breaker_Status.BOpen))
								{
									_logger.WriteEntry("Sending OPEN command is failed for Breaker : " + cbToShed.NetworkPath, LogLevels.Error);
									// Send Alarm
								}
								else
								{
									aPriol.GetArrBreakers(idxItem).LastShedTime = DateTime.Now.AddDays(-1000);
									_logger.WriteEntry("Sending OPEN command was accomlished for Breaker : " + cbToShed.NetworkPath, LogLevels.Info);
								}
							}
						}

						// ---------------------------------------------------------------------------
						// Shedding some breakers
						if (aPriol._shedType == eShedType.SomeLoads)
						{
							_logger.WriteEntry("Shed Some Loads for Priority List is starting ...", LogLevels.Info);
							for (byte idxItem = 1; idxItem <= aPriol._nBreakers; idxItem++)
                            {
								shedItem = aPriol.GetArrBreakers(idxItem).guid_item;
								var cbToShed = _repository.GetLSPScadaPoint(shedItem);
								_logger.WriteEntry("PriorityNumber = "+ aPriol._priorityNo+" ; BreakerNo = "+idxItem+" ;Network_Item = "+ cbToShed.NetworkPath, LogLevels.Info);
								
							}


							for (byte idxItem = 1; idxItem <= aPriol._nBreakers; idxItem++)
							{
								_logger.WriteEntry("idxItem = " + idxItem.ToString(), LogLevels.Info);
								// -------------------------------------------------------------------
								// Checking remained shed value
								if (aPriol._shedValue <= 0)
								{
									_logger.WriteEntry("Sheding breakers is finished before : " + aPriol.GetArrBreakers(idxItem).NetworkPath_Item, LogLevels.Info);
									goto NextPriols;
								}

								// -------------------------------------------------------------------
								// Checking last shed time for this CB
								//TODO: check
								var dtLastShedTime = aPriol.GetArrBreakers(idxItem).LastShedTime;
								dtCurrShedTime = DateTime.Now;

								var diffInSeconds = (dtCurrShedTime - dtLastShedTime).TotalSeconds;
								if ((dtLastShedTime == DateTime.MinValue) ||
									(diffInSeconds > 20))
								{
									// -------------------------------------------------------------------
									// TODO : 
									// Checking quality of status, if it's not valid, don't continue this Priol!
									shedItem = aPriol.GetArrBreakers(idxItem).guid_item;									
									var cbToShed = _repository.GetLSPScadaPoint(shedItem);									
									var breaker_current = _repository.GetLSPScadaPoint(aPriol.GetArrBreakers(idxItem).guid_curr);
									

									var shedItemPartner = aPriol.GetArrBreakers(idxItem).addressPartner_guid;
									var cbToShedPartner = _repository.GetLSPScadaPoint(shedItemPartner);
																			
									

									if (cbToShed is null)
									{
										_logger.WriteEntry("Error: Breaker is null : " + aPriol.GetArrBreakers(idxItem).NetworkPath_Item, LogLevels.Error);
										goto NextItemInPriol;
									}

									if ((cbToShedPartner is null) && aPriol.GetArrBreakers(idxItem).HasPartner == "YES")
									{
										_logger.WriteEntry("Error: Partner Breaker is null : " + aPriol.GetArrBreakers(idxItem).NetworkPath_Item, LogLevels.Error);
										goto NextItemInPriol;
									}

									if (breaker_current is null)
									{
										_logger.WriteEntry("Error: Breaker is null : " + aPriol.GetArrBreakers(idxItem).NetworkPath_Cur, LogLevels.Error);
										goto NextItemInPriol;
									}
									

									// TODO : check
									if (cbToShed.Quality != QualityCodes.None)
									{
										_logger.WriteEntry("Breaker status is not valid: " + cbToShed.NetworkPath + " ; Quality is: " + cbToShed.Quality.ToString(), LogLevels.Error);
										//GoTo LastChecksForPriol
										goto NextItemInPriol;
									}



									else
									{
										// -------------------------------------------------------------------
										// Checking status and current of CB to shed
										var aItemCurrent = breaker_current.Value;

										//**********************************************************************************
										// Modification idea , 14/9/2008
										// If strPath <> "Network/Substations/MIS1/63kV/MF2/CB/STATE" Then
										//
										//**********************************************************************************
										
										_logger.WriteEntry("cbToShed is : " + cbToShed.NetworkPath, LogLevels.Info);
										_logger.WriteEntry("Breaker Status Quality = " + cbToShed.Quality.ToString(), LogLevels.Info);
										_logger.WriteEntry("Breaker Status = " + CDecisionTable.SwitchStatusbyName((int)cbToShed.Value), LogLevels.Info);
										_logger.WriteEntry("Breaker Current = " + breaker_current.Value.ToString(), LogLevels.Info);

										// CASE 1:
										if (((DigitalDoubleStatus)cbToShed.Value == DigitalDoubleStatus.Close) && (aPriol.GetArrBreakers(idxItem).HasPartner == "YES") && aItemCurrent > 0)
										{
											if (!_updateScadaPointOnServer.SendCommand(cbToShed, (int)Breaker_Status.BOpen))
											{
												_logger.WriteEntry("OPEN command was failed for : " + cbToShed.NetworkPath, LogLevels.Error);
											}
											else
											{
												_logger.WriteEntry("OPEN command was sent for : " + cbToShed.NetworkPath, LogLevels.Info);

												_logger.WriteEntry("Partner cbToShed is : " + cbToShedPartner.NetworkPath, LogLevels.Info);
												_logger.WriteEntry("Breaker Status Quality = " + cbToShedPartner.Quality.ToString(), LogLevels.Info);
												_logger.WriteEntry("Breaker Status = " + CDecisionTable.SwitchStatusbyName((int)cbToShedPartner.Value), LogLevels.Info);

												if ((DigitalDoubleStatus)cbToShedPartner.Value == DigitalDoubleStatus.Close)
												{
													if (!_updateScadaPointOnServer.SendCommand(cbToShedPartner, (int)Breaker_Status.BOpen))
													{
														_logger.WriteEntry("OPEN command was failed for : " + cbToShedPartner.NetworkPath, LogLevels.Error);
													}
													else
													{
														_logger.WriteEntry("OPEN command was sent for : " + cbToShedPartner.NetworkPath, LogLevels.Info);

														_logger.WriteEntry("Sheded Current is: " + aItemCurrent.ToString(), LogLevels.Info);

														aPriol._shedValue -= aItemCurrent;
														aPriol.GetArrBreakers(idxItem).LastShedTime = dtCurrShedTime;

														_logger.WriteEntry("Reamined shed value = " + aPriol._shedValue.ToString(), LogLevels.Info);
													}
												}
                                                else
                                                {
													_logger.WriteEntry("Sheded Current is: " + aItemCurrent.ToString(), LogLevels.Info);

													aPriol._shedValue -= aItemCurrent;
													aPriol.GetArrBreakers(idxItem).LastShedTime = dtCurrShedTime;

													_logger.WriteEntry("Reamined shed value = " + aPriol._shedValue.ToString(), LogLevels.Info);
												}
											}
										}

										// CASE 2:
										if (((DigitalDoubleStatus)cbToShed.Value == DigitalDoubleStatus.Close) && (aPriol.GetArrBreakers(idxItem).HasPartner != "YES") && aItemCurrent > 0)
										{
											if (!_updateScadaPointOnServer.SendCommand(cbToShed, (int)Breaker_Status.BOpen))
											{
												_logger.WriteEntry("OPEN command was failed for : " + cbToShed.NetworkPath, LogLevels.Error);
											}
											else
											{
												_logger.WriteEntry("OPEN command was sent for : " + cbToShed.NetworkPath, LogLevels.Info);


												_logger.WriteEntry("Sheded Current is: " + aItemCurrent.ToString(), LogLevels.Info);

												aPriol._shedValue -= aItemCurrent;
												aPriol.GetArrBreakers(idxItem).LastShedTime = dtCurrShedTime;

												_logger.WriteEntry("Reamined shed value = " + aPriol._shedValue.ToString(), LogLevels.Info);
											}
										}

										// CASE 3:
										if (((DigitalDoubleStatus)cbToShed.Value != DigitalDoubleStatus.Close) && (aPriol.GetArrBreakers(idxItem).HasPartner == "YES") && aItemCurrent > 0)
										{
											_logger.WriteEntry("Partner cbToShed is : " + cbToShedPartner.NetworkPath, LogLevels.Info);
											_logger.WriteEntry("Breaker Status Quality = " + cbToShedPartner.Quality.ToString(), LogLevels.Info);
											_logger.WriteEntry("Breaker Status = " + CDecisionTable.SwitchStatusbyName((int)cbToShedPartner.Value), LogLevels.Info);

											if ((DigitalDoubleStatus)cbToShedPartner.Value == DigitalDoubleStatus.Close)
											{

												if (!_updateScadaPointOnServer.SendCommand(cbToShedPartner, (int)Breaker_Status.BOpen))
												{
													_logger.WriteEntry("OPEN command was failed for : " + cbToShedPartner.NetworkPath, LogLevels.Error);
												}
												else
												{
													_logger.WriteEntry("OPEN command was sent for : " + cbToShedPartner.NetworkPath, LogLevels.Info);

													_logger.WriteEntry("Sheded Current is: " + aItemCurrent.ToString(), LogLevels.Info);

													aPriol._shedValue -= aItemCurrent;
													aPriol.GetArrBreakers(idxItem).LastShedTime = dtCurrShedTime;

													_logger.WriteEntry("Reamined shed value = " + aPriol._shedValue.ToString(), LogLevels.Info);
												}
											}
										}
									}
								}
								else
								{
									_logger.WriteEntry("Last shed time is below than 20 seconds than Now, LastShedTime = " + dtLastShedTime.ToString(), LogLevels.Info);
								}
								NextItemInPriol:;
							}

							// -------------------------------------------------------------------
							// Checking if CBes were finished, but ShedValue isn't Zero yet
							if (aPriol._shedValue > 0)
							{
								_logger.WriteEntry("All CBes were sheded, but the Shed Value is remained for Priority List No.: " + aPriol._priorityNo.ToString(), LogLevels.Info);
								//---------------------------------------
								// Send Alarm To Operator
								if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("LSPACTIVATED"), SinglePointStatus.Disappear, ""))
								{
									_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
								}
								if (!_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("LSPACTIVATED"), SinglePointStatus.Appear, "Shed Value = " + aPriol._shedValue.ToString() + " ; Remained It to shed in Priority List No. " + aPriol._priorityNo.ToString()))
								{
									_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
								}
							}
						}
					}
					NextPriols:;
					result = true;
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			return result;
		}

		// Finding actived jobs by LSP, reads passed values and stored them into a job
		private bool FindActivatedJobs()
		{
			try
			{
				// TODO: complete check is required.
				_activatedJobs = 0;
				_LSPJobList.Clear();

				byte idxCP = 1;
				foreach (var checkpoint in _repository.GetCheckPoints())
				{
					var valueIT = checkpoint.OverloadIT.Value;
					if (valueIT > 0)
					{
						_logger.WriteEntry("Check point name is : " + checkpoint.Name + " ; " + " IT = " + valueIT, LogLevels.Info);

						var valueAP = checkpoint.ActivePower.Value;

						CLSPJob job = new CLSPJob(_logger);
						job.CheckPointNo = checkpoint.CheckPointNumber;
						job.SumIt = valueIT;
						job.AllowedActivePower = valueAP;

						_LSPJobList.Add(job);
						_activatedJobs++;

						idxCP = (byte)(idxCP + 1);
					}
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Info);
				return false;
			}
		}

		// Only for ine transformer is mentioned in the Job, this process will be done.
		private bool ProcessOverloadedTrans(CDecisionTable aDecisionTable, float aAllowedActivePower)
		{
			bool result = true;
			int iTransPos = 0;
			float aCosPHI = 0;
			float aPower = 0;
			string strTransName = "";
			LSPScadaPoint aP1;
			LSPScadaPoint aQ2;
			LSPScadaPoint aQ3;
			int indexTrans = 0;


			byte I = 0;
			int aDectNo = aDecisionTable.DectNo;

			// TODO: check is required
			try
			{
				// 2016.02.17    A.K Modification
				var priol = _PriorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_EAF);
				priol._nBreakers = 3;
				for (byte idxPriol = 1; idxPriol <= 3; idxPriol = (byte)(idxPriol + 1))
				{
					var breaker = priol.GetArrBreakers(idxPriol);
					if (!(breaker == null))
					{
						//breaker.NetworkPath_Item = " ";
						breaker.NetworkPath_Cur = "0";
						//breaker.HasPartner = " ";
						//breaker.AddressPartner = " ";
						//breaker._FurnaceIndex = "0";
					}
				}
				priol._nBreakers = 0;

				aCosPHI = 0.7f;

				//' KAJI START of T8AN
				int iExcpType = 0;
				string strSQLException = "";

				strSQLException = " ";

				if (!CheckExceptionInSheddingFurnaces(ref iExcpType))
				{
					_logger.WriteEntry("CheckExceptionInSheddingFurnaces was failed, All exceptions will be considered.", LogLevels.Error);
					iExcpType = 1;
				}

				if (iExcpType == 1)
				{
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF1%' ";
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF3%' ";
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF6%' ";
				}

				if (iExcpType == 2)
				{
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF3%' ";
				}

				if (iExcpType == 3)
				{
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF1%' ";
					strSQLException = strSQLException + " AND CB_NETWORKPATH NOT LIKE '%MF6%' ";
				}
				//' KAJI END of T8AN

				//---------------------------------------------------------
				// Only for transformers 1, 2, 3, 5, 7
				// KAJI T8AN, and for 8

				string TAN_BB = "";
				string strP1 = "";
				string strQ2 = "";
				string strQ3 = "";
				string TAN_name = "";

				// Because we don't have primary powers for transes, we use secondary vlaues!
				//"AN_PRIM_P";
				//"AN_SEC_P";
				if (aDectNo == Constants.T1AN_DectNo)
				{
					TAN_name = "T1AN";
					TAN_BB = "T1AN-BB";
					strP1 = "T1AN_SEC_P";
					strQ2 = "T1AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 1;
				}

				if (aDectNo == Constants.T2AN_DectNo)
				{
					TAN_name = "T2AN";
					TAN_BB = "T2AN-BB";
					strP1 = "T2AN_SEC_P";
					strQ2 = "T2AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 2;
				}

				if (aDectNo == Constants.T3AN_DectNo)
				{
					TAN_name = "T3AN";
					TAN_BB = "T3AN-BB";
					strP1 = "T3AN_SEC_P";
					strQ2 = "T3AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 3;
				}

				if (aDectNo == Constants.T5AN_DectNo)
				{
					TAN_name = "T5AN";
					TAN_BB = "T5AN-BB";
					strP1 = "T5AN_SEC_P";
					strQ2 = "T5AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 5;
				}

				if (aDectNo == Constants.T7AN_DectNo)
				{
					TAN_name = "T7AN";
					TAN_BB = "T7AN-BB";
					strP1 = "T7AN_SEC_P";
					strQ2 = "T7AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 7;
				}

				if (aDectNo == Constants.T8AN_DectNo)
				{
					TAN_name = "T8AN";
					TAN_BB = "T8AN-BB";
					strP1 = "T8AN_SEC_P";
					strQ2 = "T8AN_SEC_Q";
					strQ3 = " ";
					indexTrans = 8;
				}

				if (TAN_name.Trim().Length > 0)
				{
					_logger.WriteEntry("Overloaded Transformer is " + TAN_name, LogLevels.Info);
					var scadaPoint = _repository.GetLSPScadaPoint(TAN_BB);
					_logger.WriteEntry(TAN_name + " TAN_BB = " + scadaPoint.Value.ToString(), LogLevels.Info);

					var eafsPriolDT = _repository.FetchEAFSPriority(scadaPoint.Value.ToString(), "ON", strSQLException);

					if (scadaPoint.Value == 0)
					{
						_logger.WriteEntry("Status Of " + TAN_name + " Is Not Correct!", LogLevels.Error);
						// TODO: ' KAJI Exit is required or no?????!!!!
						_logger.WriteEntry(TAN_name + " is in OVERLOAD but it's not connected to a EAF-Busbar!", LogLevels.Error);
					}

					_logger.WriteEntry("Update_m_arrPriol_IdxPriolsEAF ----------------------------------", LogLevels.Info);

					I = 1;
					// TODO:
					//var dtbMeasurements = _repository.FetchPriorityLists();
					foreach (DataRow dr in eafsPriolDT.Rows)
					{
						// 2016.02.17 A.K
						priol = _PriorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_EAF);

						priol._nBreakers++;

						_logger.WriteEntry("Update_m_arrPriol_IdxPriolsEAF for TAN ----------------------------------", LogLevels.Info);
						priol.GetArrBreakers(I).NetworkPath_Item = dr["CB_NETWORKPATH"].ToString();
						_logger.WriteEntry("Breakers(" + I.ToString() + ").NetworkPath_Item = " + priol.GetArrBreakers(I).NetworkPath_Item.ToString(), LogLevels.Info);

						priol.GetArrBreakers(I).NetworkPath_Cur = dr["CT_NETWORKPATH"].ToString();
						_logger.WriteEntry("Breakers(" + I.ToString() + ").NetworkPath_Cur = " + priol.GetArrBreakers(I).NetworkPath_Cur.ToString(), LogLevels.Info);

						priol.GetArrBreakers(I).HasPartner = dr["HASPARTNER"].ToString();
						_logger.WriteEntry("Breakers(" + I.ToString() + ").HasPartner = " + priol.GetArrBreakers(I).HasPartner.ToString(), LogLevels.Info);

						priol.GetArrBreakers(I).AddressPartner = dr["PARTNERADDRESS"].ToString();
						_logger.WriteEntry("Breakers(" + I.ToString() + ").AddressPartner = " + priol.GetArrBreakers(I).AddressPartner.ToString(), LogLevels.Info);

						//priol.GetArrBreakers(I).guid_item = Guid.Parse(dr["CB_GUID"].ToString());
						priol.GetArrBreakers(I).guid_item = _repository.GetGuid(priol.GetArrBreakers(I).NetworkPath_Item);

						//priol.GetArrBreakers(I).guid_curr = Guid.Parse(dr["CT_GUID"].ToString());
						priol.GetArrBreakers(I).guid_curr = _repository.GetGuid(priol.GetArrBreakers(I).NetworkPath_Cur);

						priol.GetArrBreakers(I).FurnaceIndex = dr["FURNACE"].ToString();
						_logger.WriteEntry("Breakers(" + I.ToString() + ").FurnaceIndex = " + priol.GetArrBreakers(I).FurnaceIndex.ToString(), LogLevels.Info);

						if(priol.GetArrBreakers(I).HasPartner == "YES")
							//priol.GetArrBreakers(I).addressPartner_guid = Guid.Parse(dr["PARTNER_GUID"].ToString());
							priol.GetArrBreakers(I).addressPartner_guid = _repository.GetGuid(priol.GetArrBreakers(I).AddressPartner);

						if (I < 3)
							I = (byte)(I + 1);
						else
							goto aaa;
					}

					//****************
					aaa:
					aP1 = _repository.GetLSPScadaPoint(strP1);
					if (aP1==null)
						_logger.WriteEntry("Error to read "+strP1 + " from Repository", LogLevels.Error);

					aQ2 = _repository.GetLSPScadaPoint(strQ2);
					if (aQ2 == null)
						_logger.WriteEntry("Error to read " + strQ2 + " from Repository", LogLevels.Error);

					aQ3 = _repository.GetLSPScadaPoint(strQ3);

					// TODO: check, because aQ3 is always null, I use (aQ3_Value = 0) instead of aQ3.Value
					var aQ3_Value = 0;

					_logger.WriteEntry(strP1 + " = " + aP1.Value.ToString(), LogLevels.Info);
					_logger.WriteEntry(strQ2 + " = " + aQ2.Value.ToString(), LogLevels.Info);
					_logger.WriteEntry(strQ3 + " aQ3_Value = " + aQ3_Value.ToString(), LogLevels.Info);

					iTransPos = (int)_repository.GetLSPScadaPoint(TAN_BB).Value;
					//
					m_arrOLTransOnEAFBB[indexTrans] = true;
					strTransName = TAN_name;

					//-------------------------------
					// CosPhi calculation

					if (aP1.Value <= 0 || aQ2.Value <= 0 || aQ3_Value < 0)
					{
						// Error message
						_logger.WriteEntry("Value of Active or Reactive Power is not valid", LogLevels.Info);
					}
					else
					{
						aCosPHI = (float)(aP1.Value / Math.Sqrt(aP1.Value * aP1.Value + (aQ2.Value + aQ3_Value) * (aQ2.Value + aQ3_Value)));
						_logger.WriteEntry("CosPHI = " + aCosPHI.ToString(), LogLevels.Info);
					}

					_logger.WriteEntry("AllowedActivePower = " + aAllowedActivePower.ToString(), LogLevels.Info);

					aPower = aAllowedActivePower * aCosPHI / 1000;
					_logger.WriteEntry("Power = " + aPower.ToString(), LogLevels.Info);

					//------------------------------
					// Power calculation on Busbar
					if (iTransPos == 1)
					{
						_EAFBusbars[1].BusbarPower += aPower;
						_logger.WriteEntry("Busbar-A Power = " + _EAFBusbars[1].BusbarPower.ToString(), LogLevels.Info);
					}
					else
					{
						if (iTransPos == 2)
						{
							_EAFBusbars[2].BusbarPower += aPower;
							_logger.WriteEntry("Busbar-B Power = " + _EAFBusbars[2].BusbarPower.ToString(), LogLevels.Info);
						}
						else
						{
							// Error Message
							_logger.WriteEntry("Transformer Position on busbar is not valid, TANBusbarPosition = " + iTransPos.ToString(), LogLevels.Info);

							// Send Alarm To Operator
							var aAlarmPoint = _repository.GetLSPScadaPoint("WRONGNETWORKSTATUS");
							if (!_updateScadaPointOnServer.SendAlarm(aAlarmPoint, SinglePointStatus.Disappear, ""))
							{
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							}
							if (!_updateScadaPointOnServer.SendAlarm(aAlarmPoint, SinglePointStatus.Appear, "Network Status Error : Overloaded Transfromer Position is not Valid"))
							{
								_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
							}

							result = false;
						}
					}
				}
			}
			catch (Exception excep)
			{
				_logger.WriteEntry(excep.Message.ToString(), LogLevels.Info);
				result = false;
			}

			return result;
		}

		//
		// For all transformers if there is any overload on one of them, this process will be done.
		private bool ProcessBusbars()
		{
			bool result = false;
			try
			{
				byte iTransPos = 0;
				float aPower = 0;

				result = false;

				//--------------------------------------------------------------
				// Only for transformers 1, 2, 3, 5 ,7, 8; except overloaded transformer
				for (byte idxTransformer = 1; idxTransformer <= Constants.MaxNoOfTransformers; idxTransformer = (byte)(idxTransformer + 1))
				{

					//--------------------------------------------------------------
					// Excluding transformer 4,6
					if (!(idxTransformer == 4 || idxTransformer == 6))
					{

						//--------------------------------------------------------------
						// Excluding Overloaded transformer, Allowed Active Power for this transformer was calculated previuosly
						if (!(m_arrOLTransOnEAFBB[idxTransformer]))
						{

							//--------------------------------------------------------------
							// Calculating not overloaded transformers actual power
							aPower = _repository.GetTANSecondaryActivePower(idxTransformer);
							_logger.WriteEntry("Transformer-" + idxTransformer.ToString() + " ; Active Power =" + aPower.ToString(), LogLevels.Info);

							// 1399.09.29 KAJI    002_Commented
							//iTransPos = (byte)_repository.GetTANBusbarPosition(idxTransformer);

							// 1399.09.29 KAJI    002_Added
							string TAN_BB = "T" + idxTransformer.ToString() + "AN-BB";
							var transBB = _repository.GetLSPScadaPoint(TAN_BB);
							_logger.WriteEntry($" Overloaded Transformer{idxTransformer} Busbar = " + transBB.Value.ToString(), LogLevels.Info);
							iTransPos = (byte)transBB.Value;
							// 1399.09.29 KAJI    002_Added_End

							// Important note: This part does not implemented in the OLD SYSTEM
							// If MAB is closed
							var mab = _repository.GetLSPScadaPoint("MAB");
							//if (m_CLSPParams.MABStatus == ((byte) Breaker_Status.bClose))
							if ((byte)mab.Value == ((byte)Breaker_Status.bClose))
							{
								if (_EAFBusbars[1].BusbarPower > 0 || _EAFBusbars[2].BusbarPower > 0)
								{
									if (iTransPos == 1)
									{
										_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is on Overloaded Busbar-A", LogLevels.Info);
										_EAFBusbars[1].BusbarPower += aPower;
									}
									else
									{
										if (iTransPos == 2)
										{
											_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is on Overloaded Busbar-B", LogLevels.Info);
											_EAFBusbars[2].BusbarPower += aPower;
										}
										else
										{
											_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is not on Overloaded Busbar", LogLevels.Warn);
										}
									}
								}
							}
							else
							{
								// MAB is Open
								if (iTransPos == 1 && _EAFBusbars[1].BusbarPower > 0)
								{
									_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is on Overloaded Busbar-A", LogLevels.Info);
									_EAFBusbars[1].BusbarPower += aPower;
								}
								else
								{
									if (iTransPos == 2 && _EAFBusbars[2].BusbarPower > 0)
									{
										_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is on Overloaded Busbar-B", LogLevels.Info);
										_EAFBusbars[2].BusbarPower += aPower;
									}
									else
									{
										_logger.WriteEntry("Transformer no.:" + idxTransformer.ToString() + " is not on Overloaded Busbar", LogLevels.Info);
									}
								}
							}
						}
					}
				}

				var mabValue = _repository.GetLSPScadaPoint("MAB").Value;
				//if (m_CLSPParams.MABStatus == ((byte) Breaker_Status.bClose))
				if (mabValue == ((byte)Breaker_Status.bClose))
				{
					_EAFBusbars[1].BusbarPower += _EAFBusbars[2].BusbarPower;
					_EAFBusbars[2].BusbarPower = 0;

					// Why only for Busbar 1?
					if (_EAFBusbars[1].BusbarPower > 0)
					{
						_EAFBusbars[1].BusbarPower += _repository.GetLSPScadaPoint("DELTAP").Value;
					}
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Info);
			}

			return result;
		}

		// Reset the SumIT on shed points
		private bool ResetCheckPoints()
		{
			bool result = false;
			try
			{
				string strPerfix = "";
				string strItemPath = "";

				result = false;

				//
				foreach (var job in _LSPJobList)
				//for (byte idxJob = 1; idxJob <= tempForEndVar; idxJob = (byte) (idxJob + 1))
				{
					if (job.SumIt > 0)
					{
						var checkPoint = _repository.GetCheckPoint(job.CheckPointNo);
						strPerfix = Constants.SHEDPOINTADDRESS + checkPoint.Name + "/";
						// TODO: tag address should be filled correctly
						strItemPath = strPerfix + "IT";

						//'KAJI Commented below line
						//'If Not m_theCSCADAInterface.WriteData(strItemPath, 0) Then
						//'KAJI Inserted below line
						if (!_updateScadaPointOnServer.WriteAnalog(checkPoint.OverloadIT, 0f))
						{
							_logger.WriteEntry("Could not reset SumIT for : " + strItemPath, LogLevels.Error);
						}
						else
						{
							_logger.WriteEntry("SumIt was reset succesfully for : " + strItemPath, LogLevels.Info);
						}
					}
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			return result;
		}

		// R.Hemmaty
		private double ReducePower(double PowerMax)
		{
			int result = Constants.ReducePowerPrefer;
			try
			{

				var priol = _PriorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_EAF);
				var FurnaceNumber = System.Convert.ToInt32(priol.GetArrBreakers(1).FurnaceIndex);
				var reducePowerData = _repository.FetchReducedPower(FurnaceNumber);

				if (reducePowerData.Rows.Count > 0)
				{
					var dr = reducePowerData.Rows[0];

					_logger.WriteEntry("ReducePower" + " TelDateTime = " + dr["TelDateTime"].ToString(), LogLevels.Info);
					_logger.WriteEntry("ReducePower" + " PowerFurnace = " + dr["Furnace" + FurnaceNumber].ToString(), LogLevels.Info);

					// TODO: test is required 
					result = (int)PowerMax - (int)Convert.ToDouble(dr["Furnace" + FurnaceNumber].ToString());
					_logger.WriteEntry("ReducePower = " + result.ToString(), LogLevels.Info);
				}
				else
				{
					_logger.WriteEntry("There is not any data in table for reducePowerData, default will be used . . . ", LogLevels.Warn);
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}

			return result;
		}

		//'KAJI START fo Definition T8AN
		// For all big-transformers if there is any exception in shedding loads, this process will be done.
		// Exceptions are including three cases:
		//   0. No Exception, All loads can be shed
		//   1. If (MAC=OPEN) and (MBD=OPEN) and (MAB=CLOSE) Then MF1, MF3, MF6 are not permitted to Shed!
		//   2. If (MAC=CLOSE) and (MBD=OPEN) and (MAB=OPEN) Then MF3 is not permitted to Shed!
		//   3. If (MAC=OPEN) and (MBD=CLOSE) and (MAB=OPEN) Then MF1, MF6 are not permitted to Shed!
		// For all the points which are selected for exception list, the partners also never permitted to shed.
		// The partners will automatically are excluded from shed in ProcessPriols.
		//
		private bool CheckExceptionInSheddingFurnaces(ref int iExceptionType)
		{
			bool result = false;
			try
			{
				result = false;

				// Case 0: No exception
				iExceptionType = 0;

				// Case 1: MF1, MF3, MF6 are exceptions
				var MACStatus = _repository.GetLSPScadaPoint("MAC_DS").Value;
				var MBDStatus = _repository.GetLSPScadaPoint("MBD_DS").Value;
				var MABStatus = _repository.GetLSPScadaPoint("MAB").Value;
				var MZ3Status = _repository.GetLSPScadaPoint("MZ3_CB").Value;
				var MV3Status = _repository.GetLSPScadaPoint("MV3_CB").Value;

				if (MACStatus == ((byte)Breaker_Status.BOpen) &&
					MBDStatus == ((byte)Breaker_Status.BOpen) &&
					MABStatus == ((byte)Breaker_Status.bClose))
				{
					iExceptionType = 1;
				}

				if (MZ3Status == ((byte)Breaker_Status.BOpen) &&
					MV3Status == ((byte)Breaker_Status.bClose) &&
					MABStatus == ((byte)Breaker_Status.bClose))
				{
					iExceptionType = 1;
				}

				// Case 2: MF3 is exception
				if (MACStatus == ((byte)Breaker_Status.bClose) &&
					MBDStatus == ((byte)Breaker_Status.BOpen) &&
					MABStatus == ((byte)Breaker_Status.BOpen))
				{
					iExceptionType = 2;
				}

				// Case 3: MF1, MF6 are exceptions
				if (MACStatus == ((byte)Breaker_Status.BOpen) &&
					MBDStatus == ((byte)Breaker_Status.bClose) &&
					MABStatus == ((byte)Breaker_Status.BOpen))
				{
					iExceptionType = 3;
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}
			return result;
		}
		//'KAJI END fo Definition T8AN

		private bool SendCommandTestRetry()
		{
			bool result = false;
			try
			{
				result = false;

				
				if (!_updateScadaPointOnServer.SendCommandTestRetry())
				{
					_logger.WriteEntry("SendCommandTestRetry failed " , LogLevels.Error);
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}
			return result;
		}

		private bool SendCommandTest()
		{
			bool result = false;
			try
			{
				result = false;

				var scadaPointTest = _repository.GetLSPScadaPoint("RED_6.6_C.59F_CB");
				if (!_updateScadaPointOnServer.SendCommandTest(scadaPointTest, (int)Breaker_Status.BOpen))
				{
					_logger.WriteEntry("Sending OPEN command is failed for Breaker : " + scadaPointTest.NetworkPath, LogLevels.Error);
				}

				return true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}
			return result;
		}

		private bool SendCommandTest2()
		{
			bool result = false;
			try
			{
				result = false;

				var controls = new System.Collections.ObjectModel.Collection<Tuple<string, string>>
				{
					new Tuple<string, string>("73fe7bc5-4d1d-4276-8ce4-5025a73c0f29", "Network/Substations/EFS-B/6.6kV/C.60"),
					new Tuple<string, string>("ce638f5a-d25c-4216-a3d4-121457684ba2", "Network/Substations/EFS-B/6.6kV/C.61"),
					new Tuple<string, string>("fcf18fc2-ee8c-423a-aaf0-802988e03289", "Network/Substations/EFS-B/6.6kV/C.62"),
				    new Tuple<string, string>("7948d845-bb7c-4130-93d6-5f0d88817f24", "Network/Substations/EFS-B/6.6kV/C.63"),
					new Tuple<string, string>("5bc05bf5-26ac-4b8b-a557-42c3fef4f536", "Network/Substations/EFS-B/6.6kV/C.64"),
					new Tuple<string, string>("9622a017-9999-41c6-8633-f34f7ae78d4d", "Network/Substations/EFS-B/6.6kV/C.65"),
					new Tuple<string, string>("673190e1-d9c5-41ec-ad2b-43e9ab796e43", "Network/Substations/EFS-B/6.6kV/C.66"),
					new Tuple<string, string>("5643b67f-0c0f-4a6b-9e3b-b06db1cd8e47", "Network/Substations/EFS-B/6.6kV/C.67"),
					new Tuple<string, string>("1694cd89-684a-4b54-b7d9-013e20dba0e4", "Network/Substations/EFS-B/6.6kV/C.68"),
					new Tuple<string, string>("9ee472ce-e3bf-489f-b889-70a543ca82fb", "Network/Substations/EFS-B/6.6kV/C.70"),
					new Tuple<string, string>("2d51b536-dddc-4d7c-b3f9-ba3dd0ca2aec", "Network/Substations/EFS-B/6.6kV/C.72"),
					new Tuple<string, string>("e55dd32d-ddd0-47ae-a3c3-1e877be34016", "Network/Substations/EFS-B/6.6kV/C.73"),
					new Tuple<string, string>("ae4db9bd-bf3f-4158-8a4b-bd92feaf43d2", "Network/Substations/EFS-B/6.6kV/C.74"),
					new Tuple<string, string>("6144add4-a099-4e9c-9c6b-2efbae653d07", "Network/Substations/EFS-B/6.6kV/C.75"),
					new Tuple<string, string>("8e2621fe-32fa-4d5b-bf2a-60c26a13311a", "Network/Substations/EFS-B/6.6kV/C.76"),
					new Tuple<string, string>("a7c1a03c-09d3-4bc9-ada0-1db9f6661b84", "Network/Substations/EFS-B/6.6kV/C.77"),
					new Tuple<string, string>("9c2993bd-c73b-41e9-85f9-8bf35f454dd1", "Network/Substations/EFS-B/6.6kV/C.78")
				};

				foreach (var commandItem in controls)
				{
					ControlStateRequest controlRequest = new ControlStateRequest
					{
						MeasurementId = commandItem.Item1,
						Console = "LSPTEST",
						Force = true,
						User = "mscfunction",    // It is a constant value with "mscfunction"
						Value = (int)Breaker_Status.bClose
					};

					_ = _updateScadaPointOnServer.SendCommandAsync(controlRequest);
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