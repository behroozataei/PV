using System;
using System.Collections.Generic;
using System.Text;

using Irisa.Logger;

namespace RPC
{
    internal class VoltageController
    {
		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
		private bool[] QLimitViolApp = new bool[3];


		internal VoltageController(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));

			for (int i = 1; i <= 4; i++)
			{
				for (int j = 1; j <= 5; j++)
				{
					OverfluxAppear[i, j] = false;
				}
			}
		}
		const int RVOLT = 49;

		public enum BusbarBBName
		{
			BusbarA = 1,
			BusbarB,
			BusbarE,
			BusbarF
		}

		public enum TransformerName
		{
			TransT1AN = 1,
			TransT2AN,
			TransT3AN,
			TransT4AN,
			TransT5AN,
			TransT6AN,
			TransT7AN,
			TransT8AN
		}

		//
		public enum Breaker_Status
		{
			BIntransient = 0,
			BOpen = 1,
			BClose = 2,
			BDisturbed = 3
		}

		private double ActualTAP = 0;
		private double TransActVoltage = 0;
		private double NomTAPVoltage = 0;
		private double BusVoltage = 0;

		private double[,] Counter = new double[3, 5]; // Counter(1, x) --> High Counter, Counter(2, x) --> Low Counter, x --> BusBars A, B, E, F
		private bool[,] OverfluxAppear = new bool[5, 6];
		private int [,] TransformerStatus = new int [5,9];



		public bool VoltageControl()
		{
			bool result = false;
			try
			{

				bool CheckBusbarInRange = false;
				bool TransOnBusbar = false;
				//Dim VRange As double


				result = true;

				// Read the previous counters values from SCADA
				Counter[1, 1] = _repository.GetRPCScadaPoint("C1_1").Value;
				Counter[1, 2] = _repository.GetRPCScadaPoint("C1_2").Value;
				Counter[1, 3] = _repository.GetRPCScadaPoint("C1_3").Value;
				Counter[1, 4] = _repository.GetRPCScadaPoint("C1_4").Value;
				Counter[2, 1] = _repository.GetRPCScadaPoint("C2_1").Value;
				Counter[2, 2] = _repository.GetRPCScadaPoint("C2_2").Value;
				Counter[2, 3] = _repository.GetRPCScadaPoint("C2_3").Value;
				Counter[2, 4] = _repository.GetRPCScadaPoint("C2_4").Value;


				// For the 4 busbars --> A, B, E, F:
				for (BusbarBBName Busbar = BusbarBBName.BusbarA; Busbar <= BusbarBBName.BusbarF; Busbar = (BusbarBBName)(((int)Busbar) + 1))
				{

					CheckBusbarInRange = true;
					TransOnBusbar = false;

					switch (Busbar)
					{
						case BusbarBBName.BusbarA:
							BusVoltage = _repository.GetRPCScadaPoint("VEAF_A_Avg").Value;
							break;
						case BusbarBBName.BusbarB:
							BusVoltage = _repository.GetRPCScadaPoint("VEAF_B_Avg").Value;
							break;
						case BusbarBBName.BusbarE:
							BusVoltage = _repository.GetRPCScadaPoint("VPP_E_Avg").Value;
							break;
						case BusbarBBName.BusbarF:
							BusVoltage = _repository.GetRPCScadaPoint("VPP_F_Avg").Value;
							break;
					}

					// For all the Transformers:
					for (TransformerName Trans = TransformerName.TransT1AN; Trans <= TransformerName.TransT8AN; Trans = (TransformerName)(((int)Trans) + 1))
					{

						// Check if a Trans belongs to a bus bar:
						if (!CheckTransStatus(Busbar, Trans))
						{ 
							
							_logger.WriteEntry("Transformer not on the busbar! --> " + ((int)Busbar).ToString() + ", " + ((int)Trans).ToString(),LogLevels.Info);

						}
						else
						{
							TransOnBusbar = true;
							_logger.WriteEntry("Examining Busbar = " + ((int)Busbar).ToString() + ", Transformer = " + ((int)Trans).ToString(),LogLevels.Info);
							// Read Actual TAP Position and Nominal Voltage 
							switch (Trans)
							{
								case TransformerName.TransT1AN:
									ActualTAP = _repository.GetRPCScadaPoint("T1AN_TAP").Value; 
									TransActVoltage = _repository.GetRPCScadaPoint("T1AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT2AN:
									ActualTAP = _repository.GetRPCScadaPoint("T2AN_TAP").Value; 
									TransActVoltage = _repository.GetRPCScadaPoint("T2AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT3AN:
									ActualTAP = _repository.GetRPCScadaPoint("T3AN_TAP").Value; 
									TransActVoltage = _repository.GetRPCScadaPoint("T3AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT4AN:
									ActualTAP = _repository.GetRPCScadaPoint("T4AN_TAP").Value;
									TransActVoltage = _repository.GetRPCScadaPoint("T4AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT5AN:
									ActualTAP = _repository.GetRPCScadaPoint("T5AN_TAP").Value;
									TransActVoltage = _repository.GetRPCScadaPoint("T5AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT6AN:
									ActualTAP = _repository.GetRPCScadaPoint("T6AN_TAP_NEW").Value;
									TransActVoltage = _repository.GetRPCScadaPoint("T6AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT7AN:
									ActualTAP = _repository.GetRPCScadaPoint("T7AN_TAP_NEW").Value;
									TransActVoltage = _repository.GetRPCScadaPoint("T7AN_PRIMEVOLT").Value;
									break;
								case TransformerName.TransT8AN:
									ActualTAP = _repository.GetRPCScadaPoint("T8AN_TAP_NEW").Value;
									TransActVoltage = _repository.GetRPCScadaPoint("T8AN_PRIMEVOLT").Value;
									break;
							}

							switch (Convert.ToInt32(ActualTAP))
							{
								case 1:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP1").Value;
									break;
								case 2:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP2").Value;
									break;
								case 3:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP3").Value;
									break;
								case 4:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP4").Value;
									break;
								case 5:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP5").Value;
									break;
								case 6:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP6").Value;
									break;
								case 7:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP7").Value;
									break;
								case 8:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP8").Value;
									break;
								case 9:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP9").Value;
									break;
								case 10:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP10").Value;
									break;
								case 11:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP11").Value;
									break;
								case 12:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP12").Value;
									break;
								case 13:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP13").Value;
									break;
								case 14:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP14").Value;
									break;
								case 15:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP15").Value;
									break;
								case 16:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP16").Value;
									break;
								case 17:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP17").Value;
									break;
								case 18:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP18").Value;
									break;
								case 19:
									NomTAPVoltage = _repository.GetRPCScadaPoint("VTAP19").Value;
									break;
							}


							if (TransActVoltage < RVOLT)
							{
								// Preset Counters
								//Counter[1, (int)Busbar] = 0;
								//Counter[2, (int)Busbar] = 0;

								//If Not m_theSCADADataInterface.WriteData(_repository.GetRPCScadaPoint("FindGUID("C1_" & Trim(Str(Busbar))), Str(0)) Then
								//    VoltageControl = False
								//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C1_" & Trim(Str(Busbar)))
								//End If

								//If Not m_theSCADADataInterface.WriteData(_repository.GetRPCScadaPoint("FindGUID("C2_" & Trim(Str(Busbar))), Str(0)) Then
								//    VoltageControl = False
								//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C2_" & Trim(Str(Busbar)))
								//End If

								// Disappear Overflux if it is already appeared
								if (OverfluxAppear[(int)Busbar, (int)Trans])
								{
									OverfluxAppear[(int)Busbar, (int)Trans] = false;
									if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"),SinglePointStatus.Appear, "OVERFLUX Disappeared for the Trans" + ((int)Trans).ToString()))
									{
										_logger.WriteEntry( "Sending alarm failed.",LogLevels.Info);
									}
									_logger.WriteEntry( "OVERFLUX Disappeared for the Trans" + ((int)Trans).ToString(),LogLevels.Info);
								}
								_logger.WriteEntry( ((int)Trans).ToString() + " Actual Voltage < RVOLT!",LogLevels.Info);
								result = false;
							}
							else
							{
								// Check if Trans is in Overflux

								// Not in Overflux
								if (!IsTransOverflux(TransActVoltage, NomTAPVoltage, _repository.GetRPCScadaPoint("VR_TAV").Value))
								{
									if (OverfluxAppear[(int)Busbar, (int)Trans])
									{
										OverfluxAppear[(int)Busbar, (int)Trans] = false;

										if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"),SinglePointStatus.Disappear, "OVERFLUX disappeared for the Trans" + ((int)Trans).ToString()))
										{
											_logger.WriteEntry( "Sending alarm failed.",LogLevels.Info);
										}
										_logger.WriteEntry( "OVERFLUX disappeared for the Trans" + ((int)Trans).ToString(),LogLevels.Info);

										if (Busbar == BusbarBBName.BusbarA || Busbar == BusbarBBName.BusbarB)
										{
											if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK2"), 0))
											{
												result = false;
												_logger.WriteEntry("Could not update value in SCADA: " + "MARK2",LogLevels.Error);
											}
										}
										else
										{
											if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK8"), 0))
											{
												result = false;
												_logger.WriteEntry("Could not update value in SCADA: " + "MARK8", LogLevels.Error);
											}
										}
									}

									if (BusVoltage < RVOLT)
									{
										//Counter(1, Busbar) = 0
										//Counter(2, Busbar) = 0
										_logger.WriteEntry( ((int)Busbar).ToString() + " Voltage < RVOLT!",LogLevels.Info);

										//If Not m_theSCADADataInterface.WriteData(_repository.GetRPCScadaPoint("FindGUID("C1_" & Trim(Str(Busbar))), Str(0)) Then
										//    VoltageControl = False
										//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C1_" & Trim(Str(Busbar)))
										//End If

										//If Not m_theSCADADataInterface.WriteData(_repository.GetRPCScadaPoint("FindGUID("C2_" & Trim(Str(Busbar))), Str(0)) Then
										//    VoltageControl = False
										//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C2_" & Trim(Str(Busbar)))
										//End If
									}
								} // Not in Overflux
								else
								{
									// Trans in Overflux
									CheckBusbarInRange = false;
									OverfluxAppear[(int)Busbar, (int)Trans] = true;
									if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"),SinglePointStatus.Appear, "OVERFLUX appearing for the Trans" + ((int)Trans).ToString()))
									{
										_logger.WriteEntry( "Sending alarm failed.",LogLevels.Info);
									}
									_logger.WriteEntry("OVERFLUX appearing for the Trans" + ((int)Trans).ToString() + ", PrimaryVoltage = "+ TransActVoltage + ",TAPVolatage = " + NomTAPVoltage, LogLevels.Info);
									if (Busbar == BusbarBBName.BusbarA || Busbar == BusbarBBName.BusbarB)
									{
										if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK2"), 1))
										{
											result = false;
											_logger.WriteEntry("Could not update value in SCADA: " + "MARK2", LogLevels.Error);
										}
										if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"),SinglePointStatus.Appear, "SUGGESTION: DECR TAP A/B"))
										{
											_logger.WriteEntry("Sending alarm failed.",LogLevels.Info);
										}
										_logger.WriteEntry( "SUGGESTION: DECR TAP A/B", LogLevels.Info);
									}
									else
									{
										if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK8"), 1))
										{
											result = false;
											_logger.WriteEntry("Could not update value in SCADA: " + "MARK8", LogLevels.Error);
										}
										if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear , "SUGGESTION: DECR TAP PP"))
										{
											_logger.WriteEntry("Sending alarm failed.", LogLevels.Info);
										}
										_logger.WriteEntry("SUGGESTION: DECR TAP PP", LogLevels.Info);
									}
								} // Trans in Overflux
							} // Transformer Primary actual Voltage > RVOLT  
						} // Transformer belong to Busbar
					} // transformer Loop
					if ((CheckBusbarInRange) && (TransOnBusbar))
					{
						// We're not in overflux --> prepare for the TAP changing
						CheckVoltageInRange(Busbar);
					}
				} //busbar Loop
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
				result = false;
			}


			return result;
		}

   //     private void TransStatustoScada(BusbarBBName busbar, TransformerName trans)
   //     {
			//try
			//{
			//	switch (busbar)
			//	{
			//		case BusbarBBName.BusbarA:
			//			if (trans == TransformerName.TransT1AN || trans == TransformerName.TransT2AN || trans == TransformerName.TransT3AN || trans == TransformerName.TransT5AN ||
			//				trans == TransformerName.TransT7AN || trans == TransformerName.TransT8AN)
			//				_updateScadaPointOnServer.WriteDigital(_repository.GetRPCScadaPoint($"BUSA_T{trans}AN"), (SinglePointStatus)TransformerStatus[(int)busbar, (int)trans]);
			//			break;
			//		case BusbarBBName.BusbarB:
			//			if (trans == TransformerName.TransT1AN || trans == TransformerName.TransT2AN || trans == TransformerName.TransT3AN || trans == TransformerName.TransT5AN ||
			//				trans == TransformerName.TransT7AN || trans == TransformerName.TransT8AN)
			//				_updateScadaPointOnServer.WriteDigital(_repository.GetRPCScadaPoint($"BUSB_T{trans}AN"), (SinglePointStatus)TransformerStatus[(int)busbar, (int)trans]);
			//			break;
			//		case BusbarBBName.BusbarE:
			//			if (trans == TransformerName.TransT3AN || trans == TransformerName.TransT4AN || trans == TransformerName.TransT6AN)
			//				_updateScadaPointOnServer.WriteDigital(_repository.GetRPCScadaPoint($"BUSC_T{trans}AN"), (SinglePointStatus)TransformerStatus[(int)busbar, (int)trans]);
			//			break;
			//		case BusbarBBName.BusbarF:
			//			if (trans == TransformerName.TransT3AN || trans == TransformerName.TransT4AN || trans == TransformerName.TransT6AN)
			//				_updateScadaPointOnServer.WriteDigital(_repository.GetRPCScadaPoint($"BUSD_T{trans}AN"), (SinglePointStatus)TransformerStatus[(int)busbar, (int)trans]);
			//			break;
			//	}
			//}
			//catch(System.Exception excep)
   //         {
			//	_logger.WriteEntry(excep.Message, LogLevels.Error);

			//}
            
   //     }

        // Checks the Switch Status of the Transformer
        private bool CheckTransStatus(BusbarBBName aBusbar, TransformerName aTrans)
		{
			bool result = false;
			try
			{

				result = false;

				switch (aBusbar)
				{
					case BusbarBBName.BusbarA:
						switch (aTrans)
						{
							case TransformerName.TransT1AN:
								if (_repository.GetRPCScadaPoint("MT1A_CB").Value == 2 && _repository.GetRPCScadaPoint("MT1A_DS3").Value == 2 
									&& _repository.GetRPCScadaPoint("MT1A_DS1").Value == 2)
								{
									result = true;
								}
								break;
							case TransformerName.TransT2AN:
								if (_repository.GetRPCScadaPoint("MT2A_CB").Value == 2 && _repository.GetRPCScadaPoint("MT2A_DS3").Value == 2 &&
									_repository.GetRPCScadaPoint("MT2A_DS1").Value == 2)
								{
									result = true;
								}
								break;
							case TransformerName.TransT3AN:
								if (_repository.GetRPCScadaPoint("MT3_DS").Value == 2 && (_repository.GetRPCScadaPoint("MV3_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MV3_DS3").Value == 2) && (_repository.GetRPCScadaPoint("MV3_DS1").Value == 2))
								{
									result = true;
								}
								break;
							case TransformerName.TransT5AN:
								if (_repository.GetRPCScadaPoint("MT5A_DS3").Value == 2 && (_repository.GetRPCScadaPoint("M51_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("M51_DS1").Value == 2) && (_repository.GetRPCScadaPoint("M51_DS2").Value == 2) &&
									(_repository.GetRPCScadaPoint("GMF1_CB").Value == 2) && (_repository.GetRPCScadaPoint("GMF1_DS1").Value == 2) &&
									(_repository.GetRPCScadaPoint("GMF1_DS2").Value == 2) && (_repository.GetRPCScadaPoint("GMF1_DS3").Value == 2))
								{
									result = true;
								}
								break;
						}
						break;
					case BusbarBBName.BusbarB:
						switch (aTrans)
						{
							case TransformerName.TransT1AN:
								if (_repository.GetRPCScadaPoint("MT1A_CB").Value == 2 && _repository.GetRPCScadaPoint("MT1A_DS3").Value == 2 &&
									_repository.GetRPCScadaPoint("MT1A_DS2").Value == 2)
								{
									result = true;
								}
								break;
							case TransformerName.TransT2AN:
								if (_repository.GetRPCScadaPoint("MT2A_CB").Value == 2 && _repository.GetRPCScadaPoint("MT2A_DS3").Value == 2 &&
									_repository.GetRPCScadaPoint("MT2A_DS2").Value == 2)
								{
									result = true;
								}
								break;
							case TransformerName.TransT3AN:
								if (_repository.GetRPCScadaPoint("MT3_DS").Value == 2 && (_repository.GetRPCScadaPoint("MV3_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MV3_DS3").Value == 2) && (_repository.GetRPCScadaPoint("MV3_DS2").Value == 2))
								{
									result = true;
								}
								break;
							case TransformerName.TransT5AN:
								if (_repository.GetRPCScadaPoint("MT5A_DS3").Value == 2 && (_repository.GetRPCScadaPoint("MT5A_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MT5A_DS1").Value == 2) && (_repository.GetRPCScadaPoint("MT5A_DS2").Value == 2))
								{
									result = true;
								}
								break;
						}
						break;
					case BusbarBBName.BusbarE:
						switch (aTrans)
						{
							case TransformerName.TransT3AN:
								if (_repository.GetRPCScadaPoint("MT3_DS").Value == 2 && (_repository.GetRPCScadaPoint("MZ3_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MZ3_DS3").Value == 2) && (_repository.GetRPCScadaPoint("MZ3_DS1").Value == 2))
								{
									result = true;
								}
								break;
							case TransformerName.TransT4AN:
								if (_repository.GetRPCScadaPoint("MT4A_DS3").Value == 2 && (_repository.GetRPCScadaPoint("MP4_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MP4_DS1").Value == 2) && (_repository.GetRPCScadaPoint("MP4_DS2").Value == 2) &&
									(_repository.GetRPCScadaPoint("M1P_CB").Value == 2) && (_repository.GetRPCScadaPoint("M1P_DS1").Value == 2) &&
									(_repository.GetRPCScadaPoint("M1P_DS2").Value == 2))
								{
									result = true;
								}
								break;
						}
						break;
					case BusbarBBName.BusbarF:
						switch (aTrans)
						{
							case TransformerName.TransT3AN:
								if (_repository.GetRPCScadaPoint("MT3_DS").Value == 2 && (_repository.GetRPCScadaPoint("MZ3_CB").Value == 2) &&
									(_repository.GetRPCScadaPoint("MZ3_DS3").Value == 2) && (_repository.GetRPCScadaPoint("MZ3_DS2").Value == 2))
								{
									result = true;
								}
								break;
							case TransformerName.TransT4AN:
								if (_repository.GetRPCScadaPoint("MT4A_DS3").Value == 2 && (_repository.GetRPCScadaPoint("MT4A_CB").Value == 2) && 
									(_repository.GetRPCScadaPoint("MT4A_DS1").Value == 2) && (_repository.GetRPCScadaPoint("MT4A_DS2").Value == 2))
								{
									result = true;
								}
								break;
						}
						break;
				}
			}
			catch (System.Exception excep)
			{

				_logger.WriteEntry(excep.Message, LogLevels.Error);
				result = false;
			}


			return result;
		}

		private bool IsTransOverflux(double VActual, double VTAP, double VRange)
		{
			bool result = false;
			try
			{

				double Range = 0;

				result = true;

				Range = Math.Abs(((VActual - VTAP) / VTAP) * 100);

				if (Range < VRange)
				{
					result = false;
				}
			}
			catch (System.Exception excep)
			{

				_logger.WriteEntry("IsTransOverflux()..."+ excep.Message,LogLevels.Error);
				result = false;
			}

			return result;
		}

		private void CheckVoltageInRange(BusbarBBName aBusbar)
		{
			try
			{

				double BusNomVoltage = 0;
				double VRange = 0;
				double VLow = 0;
				double VHigh = 0;
				double NewTAP = 0;
				double NomNewTAPVoltage = 0;
				byte MAB = 0;


				// MAB status definition
				//If (_repository.GetRPCScadaPoint("MT5A_CB = 2) And (_repository.GetRPCScadaPoint("MT5A_DS1 = 2) And _
				//'(_repository.GetRPCScadaPoint("MT5A_DS2 = 2) And (_repository.GetRPCScadaPoint("M51_CB = 2) And _
				//'(_repository.GetRPCScadaPoint("M51_DS1 = 2) And (_repository.GetRPCScadaPoint("M51_DS2 = 2) And _
				//'(_repository.GetRPCScadaPoint("GMF1_CB = 2) And (_repository.GetRPCScadaPoint("GMF1_DS1 = 2) And _
				//'(_repository.GetRPCScadaPoint("GMF1_DS2 = 2) Then
				//MAB = 2 ' Close
				//Else
				//MAB = 1 ' Open
				//End If

				// Reading the data from the HIS was not successfull
				if (BusVoltage == -1)
				{
					return;
				}

				// MAB status is read from the application VMAB.
				MAB = (byte)_repository.GetRPCScadaPoint("MAB").Value;

				// ?????????????????????????
				BusNomVoltage = 63;
				// ?????????????????????????
				switch (aBusbar)
				{
					case BusbarBBName.BusbarA:
					case BusbarBBName.BusbarB:
						VRange = _repository.GetRPCScadaPoint("VR_EAF").Value;
						break;
					case BusbarBBName.BusbarE:
					case BusbarBBName.BusbarF:
						VRange = _repository.GetRPCScadaPoint("VR_PP").Value;
						break;
				}

				VRange = (VRange * BusNomVoltage) / 100;
				VLow = BusNomVoltage - VRange;
				VHigh = BusNomVoltage + VRange;

				if (BusVoltage > VHigh)
				{
					Counter[1, (int)aBusbar]++;
					Counter[2, (int)aBusbar] = 0;

					_logger.WriteEntry("BusVoltage=" + BusVoltage.ToString() + " > VHigh=" + VHigh.ToString(), LogLevels.Info);
					_logger.WriteEntry("Counter(1, " + ((int)aBusbar).ToString() + ")=" + Counter[1, (int)aBusbar].ToString(), LogLevels.Trace);

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_" + ((uint)aBusbar)), (float)Counter[1, (int)aBusbar]))
					{
						_logger.WriteEntry("Could not update value in SCADA: " + "C1_" + aBusbar,LogLevels.Info);
						return;
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_" + ((uint)aBusbar)), (float)Counter[2, (int)aBusbar]))
					{
						_logger.WriteEntry( "Could not update value in SCADA: " + "C2_" + aBusbar,LogLevels.Info);
						return;
					}

					// If BusVoltage is high for more than 3 cycles send decrease suggestions
					if (Counter[1, (int)aBusbar] >= 3)
					{
						if (aBusbar == BusbarBBName.BusbarA || aBusbar == BusbarBBName.BusbarB)
						{
							// Check MAB status
							switch (MAB)
							{
								case 0:
									_logger.WriteEntry("MAB Status Invalid!",LogLevels.Warn);
									return;
								case 1:  // Open 
									switch (aBusbar)
									{
										case BusbarBBName.BusbarA:
											if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK4"), 1))
											{
												_logger.WriteEntry("Could not update value in SCADA: " + "MARK4",LogLevels.Error);
											}
											if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"),SinglePointStatus.Appear, "SUGGESTION: DECR TAP A"))
											{
												_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
											}
											_logger.WriteEntry( "SUGGESTION: DECR TAP A", LogLevels.Info);
											break;
										case BusbarBBName.BusbarB:
											if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK6"), 1))
											{
												_logger.WriteEntry("Could not update value in SCADA: " + "MARK6",LogLevels.Error);
											}
											if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: DECR TAP B"))
											{
												_logger.WriteEntry( "Sending alarm failed.",LogLevels.Info);
											}
											_logger.WriteEntry( "SUGGESTION: DECR TAP B",LogLevels.Info);
											break;
									}
									break;
								case 2:  // Close 
									if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK2"), 1))
									{
										_logger.WriteEntry("Could not update value in SCADA: " + "MARK2",LogLevels.Error);
									}
									if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: DECR TAP A/B"))
									{
										_logger.WriteEntry("Sending alarm failed.", LogLevels.Info);
									}
									_logger.WriteEntry("SUGGESTION: DECR TAP A/B", LogLevels.Info);
									break;
							}
						}
						else
						{
							// PP Busbar
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK8"), 1))
							{
								_logger.WriteEntry("Could not update value in SCADA: " + "MARK8", LogLevels.Error);
							}
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: DECR TAP PP"))
							{
								_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
							}
							_logger.WriteEntry( "SUGGESTION: DECR TAP PP", LogLevels.Info);
						}

						// If BusVoltage is high for 5 or more than 5 cycles send suggestion not successfull
						if (Counter[1, (int)aBusbar] >= 5)
						{
							// This delay is used to let the SCADA Alarming having enough time to perform 2 alarms exactly after each other.
							Delay(4);
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "SUGGESTION DECR NOT SUCCESSFULL"))
							{
								_logger.WriteEntry( "Sending alarm failed.",LogLevels.Info);
							}
							_logger.WriteEntry("SUGGESTION DECR NOT SUCCESSFULL", LogLevels.Info);
						}
					}
				}
				else
				{
					if (BusVoltage > VLow)
					{
						// Bus voltage is in range, reset the counters
						Counter[1, (int)aBusbar] = 0;
						Counter[2, (int)aBusbar] = 0;

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_" + ((int)aBusbar)), (float)Counter[1, (int)aBusbar]))
						{
							_logger.WriteEntry( "Could not update value in SCADA: " + "C1_" + aBusbar, LogLevels.Info);
							return;
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_" + ((int)aBusbar)), (float)Counter[2, (int)aBusbar]))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C2_" + aBusbar,LogLevels.Info);
							return;
						}

					}
					else
					{
						Counter[1, (int)aBusbar] = 0;
						Counter[2, (int)aBusbar]++;

						_logger.WriteEntry( "BusVoltage=" + BusVoltage.ToString() + " < VLow=" + VLow.ToString(),LogLevels.Info);
						_logger.WriteEntry("Counter(2, " + ((int)aBusbar).ToString() + ")=" + Counter[2, (int)aBusbar].ToString(), LogLevels.Trace);

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C1_" + ((int)aBusbar)), (float)(Counter[1, (int)aBusbar])))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C2_" + aBusbar, LogLevels.Error);
							return;
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C2_" + ((int)aBusbar)), (float)Counter[2, (int)aBusbar]))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C2_" + aBusbar, LogLevels.Error);
							return;
						}

						// If BusVoltage is low for 3 or more than 3 cycles send increase suggestions
						if (Counter[2, (int)aBusbar] >= 3)
						{
							NewTAP = ActualTAP + 1;
							if (NewTAP <= 19)
							{
								switch (Convert.ToInt32(NewTAP))
								{
									case 1:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP1").Value;
										break;
									case 2:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP2").Value;
										break;
									case 3:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP3").Value;
										break;
									case 4:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP4").Value;
										break;
									case 5:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP5").Value;
										break;
									case 6:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP6").Value;
										break;
									case 7:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP7").Value;
										break;
									case 8:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP8").Value;
										break;
									case 9:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP9").Value;
										break;
									case 10:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP10").Value;
										break;
									case 11:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP11").Value;
										break;
									case 12:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP12").Value;
										break;
									case 13:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP13").Value;
										break;
									case 14:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP14").Value;
										break;
									case 15:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP15").Value;
										break;
									case 16:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP16").Value;
										break;
									case 17:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP17").Value;
										break;
									case 18:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP18").Value;
										break;
									case 19:
										NomNewTAPVoltage = _repository.GetRPCScadaPoint("VTAP19").Value;
										break;
								}
							}
							else
							{
								NomNewTAPVoltage = NomTAPVoltage;
							}

							if (!IsTransOverflux(TransActVoltage, NomNewTAPVoltage, _repository.GetRPCScadaPoint("VR_TAV").Value))
							{
								if (aBusbar == BusbarBBName.BusbarA || aBusbar == BusbarBBName.BusbarB)
								{
									// Check MAB status
									switch (MAB)
									{
										case 0:
											_logger.WriteEntry("MAB Status Invalid!", LogLevels.Warn);
											return;
										case 1:  // Open 
											switch (aBusbar)
											{
												case BusbarBBName.BusbarA:
													if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK3"), 1))
													{
														_logger.WriteEntry("Could not update value in SCADA: " + "MARK3", LogLevels.Error);
													}
													if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: INCR TAP A"))
													{
														_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
													}
													_logger.WriteEntry( "SUGGESTION: INCR TAP A", LogLevels.Info);
													break;
												case BusbarBBName.BusbarB:
													if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK5"), 1))
													{
														_logger.WriteEntry("Could not update value in SCADA: " + "MARK5", LogLevels.Error);
													}
													if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: INCR TAP B"))
													{
														_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
													}
													_logger.WriteEntry("SUGGESTION: INCR TAP B",LogLevels.Info);
													break;
											}
											break;
										case 2:  // Close 
											if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK1"), 1))
											{
												_logger.WriteEntry("Could not update value in SCADA: " + "MARK1", LogLevels.Error);
											}
											if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: INCR TAP A/B"))
											{
												_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
											}
											_logger.WriteEntry( "SUGGESTION: INCR TAP A/B", LogLevels.Info);
											break;
									}
								}
								else
								{
									// PP Busbar
									if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK7"), 1))
									{
										_logger.WriteEntry("Could not update value in SCADA: " + "MARK7", LogLevels.Error);
									}
									if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION: INCR TAP PP"))
									{
										_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
									}
									_logger.WriteEntry( "SUGGESTION: INCR TAP PP", LogLevels.Info);
								}

								if (Counter[2, (int)aBusbar] >= 5)
								{
									// This delay is used to let the SCADA Alarming having enough time to perform 2 alarms exactly after each other.
									Delay(4);
									if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "SUGGESTION INCR NOT SUCCESSFULL"))
									{
										_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
									}
									_logger.WriteEntry("SUGGESTION INCR NOT SUCCESSFULL",LogLevels.Info );
								}
							}
							else
							{
								// New TAP will produce Overflux
								if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"), SinglePointStatus.Appear, "POSSIBLE OVERFLUX"))
								{
									_logger.WriteEntry( "Sending alarm failed.", LogLevels.Info);
								}
								_logger.WriteEntry( "POSSIBLE OVERFLUX", LogLevels.Info);
								// Send DCO Job (PCS)? (OVFLUX)
							}
						}
					}
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
			}


		}

		private void Delay(int WaitSecond)
		{

			int StartSecond = DateTime.UtcNow.Second;

			do
			{
			}
			while (DateTime.UtcNow.Second - StartSecond < WaitSecond);
		}
	}
}
