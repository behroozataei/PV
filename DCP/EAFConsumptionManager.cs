using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Data;
using Newtonsoft.Json;

using COM;
using Irisa.Logger;
using Irisa.Message;

namespace DCP
{
	internal class EAFConsumptionManager
	{
		// Instead of Temp, we use this one
		private float[] SumEAFConsumptionPerHeat;
		//private int[] ArrayPreviousStatusEAFs;
		//private string PowerSumation = "";
		private int MaxF_Key = 0;

		private const int NUMBER_OF_FURNACES = 8;
		private const int EAFS_TIMER_1_Minute_TICKS = 60000;
		private const int EAFS_TIMER_4_Seconds_TICKS = 4000;
		private const float EAFS_ON_OFF_CURRENT_LIMIT = 50.0f;
		private readonly Timer _timer_1_Minute;
		private readonly Timer _timer_4_Seconds;

		private readonly ILogger _logger;
		private readonly IRepository _repository;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
		private readonly string[] _OnOff_furnces_New;  //Array for On/Off of furnaces;		
		private readonly string[] _OnOff_furnces_Old;  //Array for On/Off of furnaces;		

		internal EAFConsumptionManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
		{
			// TODO : 
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

			try
			{
				// Delete old rows from T_Furnace with Null in EndTime(Not finished heat in EAF)
				string sql = "DELETE FROM T_Furnace WHERE ENDTIME IS NULL";
				if (!_repository.ModifyOnLinkDB(sql))
				{
					_logger.WriteEntry($"'DELETE FROM T_Furnace WHERE ENDTIME IS NULL' is not possible!", LogLevels.Error);
				}

				// Insert new rows for furnaces into T_Furnace with current time as Start
				for (int furnace = 1; furnace <= 8; furnace++)
				{
					sql = "INSERT INTO T_Furnace(Start, FurnaceNumber) values('" + DateTime.Now.ToString() + "', " + furnace.ToString() + ")";
					if (!_repository.ModifyOnLinkDB(sql))
					{
						_logger.WriteEntry($"'INSERT INTO T_Furnace(Start, {furnace}) ' is not possible!", LogLevels.Error);
					}
				}

				// Get Max F_key
				MaxF_Key = _repository.GetMAxFKeyEndedFurnace(MaxF_Key);

				// For I = 0 To 7:		ArrayEAFsConsumption(I) 
				//						PowerSumation
				// For I = 8 To 15:		ArrayEAFsEnergy(I - 8)
				// For I = 16 To 23:	ArrayEAFsCurrent(I - 16, 0)

				// Reset ... in SCADA 
				SumEAFConsumptionPerHeat = new float[8];
				for (int furnace = 1; furnace <= 8; furnace++)
				{
					SumEAFConsumptionPerHeat[furnace - 1] = 0;
					var scadaPointEAFSConsumption = _repository.GetScadaPoint("ConsEnergy_EAF" + furnace.ToString());
					if (!_updateScadaPointOnServer.WriteSCADAPoint(scadaPointEAFSConsumption, 0))
					{
						_logger.WriteEntry($"'Reset value for 'ConsEnergy_EAF'+ {furnace.ToString()} ' is not possible!", LogLevels.Error);
					}
				}

				_OnOff_furnces_New = new string[NUMBER_OF_FURNACES];
				_OnOff_furnces_Old = new string[NUMBER_OF_FURNACES];
				for (int i = 0; i < NUMBER_OF_FURNACES; i++)
				{
					_OnOff_furnces_New[i] = "";
					_OnOff_furnces_Old[i] = "  ";
				}

				//----------------------------------------------------------------
				_timer_1_Minute = new Timer();
				_timer_1_Minute.Interval = EAFS_TIMER_1_Minute_TICKS;
				_timer_1_Minute.Elapsed += Timer_1_Minute_Tick;

				_timer_4_Seconds = new Timer();
				_timer_4_Seconds.Interval = EAFS_TIMER_4_Seconds_TICKS;
				_timer_4_Seconds.Elapsed += Timer_4_Seconds_Tick;

                _timer_1_Minute.Start();
                _timer_4_Seconds.Start();
            }
			catch (Exception ex)
			{
				_logger.WriteEntry(ex.Message, LogLevels.Error, ex);
			}
		}
		
		private void Timer_1_Minute_Tick(Object eventSender, EventArgs eventArgs)
		{
			// TODO : should be fixed, now it was commented!
			try
			{
				//----------------------------------------------------------------------------------------
				// Step 1: Ending heat of one EAF:
				// TODO: check with PCS: T_EndedFurnace_Backup or T_EndedFurnace!
				string sql = "SELECT F_Key, Furnace from dbo.T_EndedFurnace_Backup WHERE F_Key > '" +
						 MaxF_Key.ToString() + "' ORDER BY F_Key DESC";
				var dataTable = _repository.GetFromLinkDB(sql);
				if (dataTable is null)
				{
					_logger.WriteEntry("'SELECT F_Key, Furnace from link.     ' is not possible!", LogLevels.Error);
					return;
				}

				foreach (DataRow dr in dataTable.Rows)
				{
					MaxF_Key = Convert.ToInt32(dr["F_Key"].ToString());
					int furnace = Convert.ToInt32(dr["Furnace"].ToString());
					for (int J = 1; J <= 8; J++)
					{
						if (furnace == J)
						{
							var scadaPointEAFSConsumption = _repository.GetScadaPoint("ConsEnergy_EAF" + furnace.ToString());
							if (scadaPointEAFSConsumption is null)
							{
								_logger.WriteEntry($"'Find SCADAPoint 'ConsEnergy_EAF' + {furnace.ToString()} ' is not possible!", LogLevels.Error);
							}
							else
							{
								sql = "INSERT INTO PU10_PCS.dbo.T_FURNACE(FurnaceNumber, EndTime, ConsumedEnergy)" +
										"VALUES(" + furnace.ToString() + ", '" + DateTime.Now.ToString() + "', '" + SumEAFConsumptionPerHeat[furnace - 1].ToString() + "');";

								//sql = "UPDATE dbo.T_FURNACE SET EndTime='" + DateTime.Now.ToString() +
								//		"',ConsumedEnergy='" + scadaPointEAFSConsumption.Value.ToString() +
								//		"' WHERE F_Key = (SELECT MAX(F_Key) FROM dbo.T_FURNACE where FurnaceNumber = " + 
								//		furnace.ToString() + 
								//		") AND " +                                                                                                                                                                                                  
								//		" FurnaceNumber = " + 
								//		furnace.ToString();

								if (!_repository.ModifyOnLinkDB(sql))
								{
									_logger.WriteEntry("'UPDATE dbo.T_FURNACE SET EndTime' is not possible!", LogLevels.Error);
								}
								//Module1.ArrayEAFsCurrent[J - 1, 1] = "Standby";
								//WriteData(ArrayEAFsConsumption[J - 1], "0");
								//EAFCONSUMPTION[J - 1] = 0;
								_repository.UpdateFurnace(furnace, DateTime.Now.ToString(), SumEAFConsumptionPerHeat[furnace - 1].ToString());
								SumEAFConsumptionPerHeat[furnace - 1] = 0;
								// TODO: check if we have put 0 for this furnace for 'ConsEnergy_EAF'

								break;
							}
						}
					}
				}

				//----------------------------------------------------------------------------------------
				// Step 2:
				// TODO: check or completion
				///Module1.CHECK_ACTIVE_FURNACE();

				//----------------------------------------------------------------------------------------
				// Step 3: Updating 1-Minute energy of EAFs
				for (int I = 1; I <= 8; I++)
				{
					var EAFConsumption = _repository.GetScadaPoint("Energy_EAF" + I.ToString()).Value;
					SumEAFConsumptionPerHeat[I - 1] = SumEAFConsumptionPerHeat[I - 1] + EAFConsumption;

					var eafEnergyConsumption = _repository.GetScadaPoint("ConsEnergy_EAF" + I.ToString());
					if (eafEnergyConsumption is null)
					{
						_logger.WriteEntry($"'Find SCADAPoint 'ConsEnergy_EAF' + {I.ToString()} ' is not possible!", LogLevels.Error);
						continue;
					}

					if (!_updateScadaPointOnServer.WriteSCADAPoint(eafEnergyConsumption, SumEAFConsumptionPerHeat[I - 1]))
					{
						_logger.WriteEntry("'Write value for ... ' is not possible!", LogLevels.Error);
					}

					var currentValue = _repository.GetScadaPoint("Current_EAF" + I.ToString()).Value;
					if (currentValue > EAFS_ON_OFF_CURRENT_LIMIT)
					{
						sql = "UPDATE dbo.T_EAFsEnergyConsumption SET [Consumed Energy Per Heat] = '" +
								SumEAFConsumptionPerHeat[I - 1].ToString() +
								"' WHERE Furnace='" +
								I.ToString() +
								"'";
						if (!_repository.ModifyOnLinkDB(sql))
						{
							_logger.WriteEntry("'UPDATE dbo.T_EAFsEnergyConsumption' is not possible!", LogLevels.Error);
						}
					}
				}

				//----------------------------------------------------------------------------------------
				// Step 4:
				//sql = "UPDATE app.EEC_SFSCEAFSPriority SET STATUS_OF_FURNACE = 'OFF'";
				//if( !_repository.modifyOnHistoricalDB(sql))
				//{
				//	_logger.WriteEntry("'UPDATE app.EEC_SFSCEAFSPriority' is not possible!", LogLevels.Error);
				//}

				sql = "SELECT [Furnace], [Consumed Energy Per Heat], [Status] " +
						"FROM dbo.T_EAFsEnergyConsumption " +
						"WHERE Status='ON' " +
						"ORDER BY [Consumed Energy Per Heat]";
				dataTable = _repository.GetFromLinkDB(sql);
                foreach (DataRow dr in dataTable.Rows)
                {
                    //sql = $"UPDATE APP_EEC_SFSCEAFSPRIORITY SET CONSUMED_ENERGY_PER_HEAT='" +
                    //        dr["Consumed Energy Per Heat"].ToString() +
                    //        "',Reason = 'DCP.EAFsConsumption => CONSUMED_ENERGY_PER_HEAT is updated, 1-Minute', STATUS_OF_FURNACE='" +
                    //        dr["Status"].ToString() +
                    //        "' WHERE FURNACE='" +
                    //        dr["Furnace"].ToString() +
                    //        "'";
                    //if (!_repository.ModifyOnHistoricalDB(sql))
                    //{
                    //    _logger.WriteEntry($"UPDATE APP_EEC_SFSCEAFSPriority SET CONSUMED_ENERGY_PER_HEAT is not possible!", LogLevels.Error);
                    //}

                    EEC_SFSCEAFSPRIORITY_Str eec_sfsceafprio = new EEC_SFSCEAFSPRIORITY_Str();
					eec_sfsceafprio = JsonConvert.DeserializeObject<EEC_SFSCEAFSPRIORITY_Str>(_repository.GetRedisUtiles().DataBase.StringGet(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + dr["Furnace"].ToString()));
					eec_sfsceafprio.CONSUMED_ENERGY_PER_HEAT = dr["Consumed Energy Per Heat"].ToString();
					eec_sfsceafprio.STATUS_OF_FURNACE = dr["Status"].ToString();
					eec_sfsceafprio.REASON = "DCP.EAFsConsumption => CONSUMED_ENERGY_PER_HEAT is updated, 1-Minute";
					_repository.GetRedisUtiles().DataBase.StringSet(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + eec_sfsceafprio.FURNACE, JsonConvert.SerializeObject(eec_sfsceafprio));

				}
			}
			catch (Exception ex)
			{
				_logger.WriteEntry(ex.Message, LogLevels.Error, ex);
			}
		}

		private void Timer_4_Seconds_Tick(Object eventSender, EventArgs eventArgs)
		{
			// TODO : should be fixed, now it was commented!
			try
			{
				string sql_T_EAFsEnergyConsumption;
				//string sql_EEC_SFSCEAFSPriority;
				string sql;

				// Step 1: ON/OFF for furnaces
				for (int nFurnace = 1; nFurnace <= 8; nFurnace++)
				{
					var scadaPoint = _repository.GetScadaPoint("Current_EAF" + nFurnace.ToString());
					EEC_SFSCEAFSPRIORITY_Str eec_sfsceafprio = new EEC_SFSCEAFSPRIORITY_Str();
					eec_sfsceafprio = JsonConvert.DeserializeObject<EEC_SFSCEAFSPRIORITY_Str>(_repository.GetRedisUtiles().DataBase.StringGet(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + nFurnace.ToString()));

					if (scadaPoint.Value > EAFS_ON_OFF_CURRENT_LIMIT)
					{
						sql_T_EAFsEnergyConsumption = "UPDATE [PU10_PCS].[dbo].[T_EAFsEnergyConsumption] SET [Status] ='ON' WHERE [Furnace]='" + nFurnace.ToString() + "'";
						//sql_EEC_SFSCEAFSPriority = $"UPDATE APP_EEC_SFSCEAFSPRIORITY SET STATUS_OF_FURNACE='ON', Reason = 'DCP.EAFsConsumption => STATUS_OF_FURNACE goes ON' WHERE FURNACE='" + nFurnace.ToString() + "'";
						_OnOff_furnces_New[nFurnace - 1] = "ON";
						eec_sfsceafprio.STATUS_OF_FURNACE = "ON";
					}
					else
					{
						sql_T_EAFsEnergyConsumption = "UPDATE [PU10_PCS].[dbo].[T_EAFsEnergyConsumption] SET [Status] ='OFF' WHERE [Furnace]='" + nFurnace.ToString() + "'";
						//sql_EEC_SFSCEAFSPriority = $"UPDATE APP_EEC_SFSCEAFSPRIORITY SET STATUS_OF_FURNACE='OFF', Reason = 'DCP.EAFsConsumption => STATUS_OF_FURNACE goes OFF' WHERE FURNACE='" + nFurnace.ToString() + "'";
						_OnOff_furnces_New[nFurnace - 1] = "OFF";
						eec_sfsceafprio.STATUS_OF_FURNACE = "OFF";
					}

					if (_OnOff_furnces_Old[nFurnace - 1] != _OnOff_furnces_New[nFurnace - 1])
					{
						if (!_repository.ModifyOnLinkDB(sql_T_EAFsEnergyConsumption))
						{
							_logger.WriteEntry("'UPDATE [PU10_PCS].[dbo].[T_EAFsEnergyConsumption]' is not possible!", LogLevels.Error);
						}

						//if (!_repository.ModifyOnHistoricalDB(sql_EEC_SFSCEAFSPriority))
						//{
						//	_logger.WriteEntry("'UPDATE APP_EEC_SFSCEAFSPriority SET STATUS_OF_FURNACE' is not possible!", LogLevels.Error);
						//}

						_repository.GetRedisUtiles().DataBase.StringSet(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + eec_sfsceafprio.FURNACE, JsonConvert.SerializeObject(eec_sfsceafprio));

					}
					_OnOff_furnces_Old[nFurnace - 1] = _OnOff_furnces_New[nFurnace - 1];
				}

				// Step 2: Update PowerSumation in SCADA from PCS-Table
				sql = "SELECT Sumation FROM dbo.T_EAFsPower_Backup WHERE F_Key=(SELECT MAX(F_Key) FROM dbo.T_EAFsPower_Backup)";
				float summation = _repository.GetPowerSumationFromT_EAFsPower(sql);
				if (summation < 0)
				{
					_logger.WriteEntry("'SELECT Sumation FROM [PU10_PCS].[dbo].[T_EAFsPower_Backup] ' is not possible!", LogLevels.Error);
				}
				else
				{
					if (!_updateScadaPointOnServer.WriteSCADAPoint(_repository.GetScadaPoint("SummationPower"), summation))
					{
						_logger.WriteEntry("'Write PowerSummation to SCADA' is not possible!", LogLevels.Error);
					}
				}

			}
			catch (Exception ex)
			{
				_logger.WriteEntry(ex.Message, LogLevels.Error, ex);
			}
		}
		//Public Sub CHECK_ACTIVE_FURNACE()
		//Dim J As Integer
		//Dim CurrentTemp As String
		//Dim qc As Boolean
		//Dim STRSQL1 As String
		//Dim m_CWRFROMSCADA1 As New WRFROMSCADA

		//For J = 0 To 7
		//Call m_CWRFROMSCADA1.ReadData(ArrayEAFsCurrent(J, 0), CurrentTemp, qc)
		//If ArrayEAFsCurrent(J, 1) = "Standby" And CurrentTemp > 0 Then
		//STRSQL1 = "INSERT INTO T_Furnace" & J + 1 & "(Start) values('" & Now & "')"
		//CONNECTION_AND_INSERTDATA_INTO_SQLSERVER(STRSQL1)
		//ArrayEAFsCurrent(J, 1) = ""
		//End If
		//Next

		//End Sub
	}
}