using Microsoft.VisualBasic;
using System;

namespace RPC_Service_App
{
	internal class CVoltageController
	{


		const int RVOLT = 49;

		private CRPCParameters m_theCRPCParameters = null;
		private CSCADADataInterface _m_theSCADADataInterface = null;
		private CSCADADataInterface m_theSCADADataInterface
		{
			get
			{
				if (_m_theSCADADataInterface == null)
				{
					_m_theSCADADataInterface = new CSCADADataInterface();
				}
				return _m_theSCADADataInterface;
			}
			set
			{
				_m_theSCADADataInterface = value;
			}
		}


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
			TransT5AN
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

		private double[, ] Counter = new double[3, 5]; // Counter(1, x) --> High Counter, Counter(2, x) --> Low Counter, x --> BusBars A, B, E, F
		private bool[, ] OverfluxAppear = new bool[5, 6];


		public CVoltageController()
		{

			for (int i = 1; i <= 4; i++)
			{
				for (int j = 1; j <= 5; j++)
				{
					OverfluxAppear[i, j] = false;
				}
			}

		}

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
				Counter[1, 1] = m_theCRPCParameters.C1_1;
				Counter[1, 2] = m_theCRPCParameters.C1_2;
				Counter[1, 3] = m_theCRPCParameters.C1_3;
				Counter[1, 4] = m_theCRPCParameters.C1_4;
				Counter[2, 1] = m_theCRPCParameters.C2_1;
				Counter[2, 2] = m_theCRPCParameters.C2_2;
				Counter[2, 3] = m_theCRPCParameters.C2_3;
				Counter[2, 4] = m_theCRPCParameters.C2_4;


				// For the 4 busbars --> A, B, E, F:
				//UPGRADE_WARNING: (6021) Casting 'int' to Enum may cause different behaviour. More Information: https://www.mobilize.net/vbtonet/ewis/ewi6021
				for (BusbarBBName Busbar = BusbarBBName.BusbarA; Busbar <= BusbarBBName.BusbarF; Busbar = (BusbarBBName) (((int) Busbar) + 1))
				{

					CheckBusbarInRange = true;
					TransOnBusbar = false;

					switch(Busbar)
					{
						case BusbarBBName.BusbarA : 
							BusVoltage = m_theCRPCParameters.VEAF_A_Avg; 
							break;
						case BusbarBBName.BusbarB : 
							BusVoltage = m_theCRPCParameters.VEAF_B_Avg; 
							break;
						case BusbarBBName.BusbarE : 
							BusVoltage = m_theCRPCParameters.VPP_E_Avg; 
							break;
						case BusbarBBName.BusbarF : 
							BusVoltage = m_theCRPCParameters.VPP_F_Avg; 
							break;
					}

					// For all the Transformers:
					//UPGRADE_WARNING: (6021) Casting 'int' to Enum may cause different behaviour. More Information: https://www.mobilize.net/vbtonet/ewis/ewi6021
					for (TransformerName Trans = TransformerName.TransT1AN; Trans <= TransformerName.TransT5AN; Trans = (TransformerName) (((int) Trans) + 1))
					{

						// Check if a Trans belongs to a bus bar:
						if (!CheckTransStatus(Busbar, Trans))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageColtrol()", "Transformer not on the busbar! --> " + ((int) Busbar).ToString() + ", " + ((int) Trans).ToString());

						}
						else
						{
							TransOnBusbar = true;
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageColtrol()", "Examining Busbar = " + ((int) Busbar).ToString() + ", Transformer = " + ((int) Trans).ToString());
							// Read Actual TAP Position and Nominal Voltage (CRPCParams)
							switch(Trans)
							{
								case TransformerName.TransT1AN : 
									//ActualTAP = m_theCRPCParameters.T1AN_TAP 
									TransActVoltage = m_theCRPCParameters.T1AN_PRIMEVOLT; 
									break;
								case TransformerName.TransT2AN : 
									//ActualTAP = m_theCRPCParameters.T2AN_TAP 
									TransActVoltage = m_theCRPCParameters.T2AN_PRIMEVOLT; 
									break;
								case TransformerName.TransT3AN : 
									//ActualTAP = m_theCRPCParameters.T3AN_TAP 
									TransActVoltage = m_theCRPCParameters.T3AN_PRIMEVOLT; 
									break;
								case TransformerName.TransT4AN : 
									ActualTAP = m_theCRPCParameters.T4AN_TAP; 
									TransActVoltage = m_theCRPCParameters.T4AN_PRIMEVOLT; 
									break;
								case TransformerName.TransT5AN : 
									ActualTAP = m_theCRPCParameters.T5AN_TAP_NEW; 
									TransActVoltage = m_theCRPCParameters.T5AN_PRIMEVOLT; 
									 
									break;
							}

							switch(Convert.ToInt32(ActualTAP))
							{
								case 1 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP1; 
									break;
								case 2 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP2; 
									break;
								case 3 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP3; 
									break;
								case 4 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP4; 
									break;
								case 5 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP5; 
									break;
								case 6 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP6; 
									break;
								case 7 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP7; 
									break;
								case 8 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP8; 
									break;
								case 9 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP9; 
									break;
								case 10 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP10; 
									break;
								case 11 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP11; 
									break;
								case 12 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP12; 
									break;
								case 13 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP13; 
									break;
								case 14 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP14; 
									break;
								case 15 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP15; 
									break;
								case 16 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP16; 
									break;
								case 17 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP17; 
									break;
								case 18 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP18; 
									break;
								case 19 : 
									NomTAPVoltage = m_theCRPCParameters.VTAP19; 
									break;
							}


							if (TransActVoltage < RVOLT)
							{
								// Preset Counters
								//Counter(1, Busbar) = 0
								//Counter(2, Busbar) = 0

								//If Not m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_" & Trim(Str(Busbar))), Str(0)) Then
								//    VoltageControl = False
								//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C1_" & Trim(Str(Busbar)))
								//End If

								//If Not m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_" & Trim(Str(Busbar))), Str(0)) Then
								//    VoltageControl = False
								//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C2_" & Trim(Str(Busbar)))
								//End If

								// Disappear Overflux if it is already appeared
								if (OverfluxAppear[(int) Busbar, (int) Trans])
								{
									OverfluxAppear[(int) Busbar, (int) Trans] = false;
									if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "OVERFLUX Disappeared for the Trans" + ((int) Trans).ToString()))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
									}
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "OVERFLUX Disappeared for the Trans" + ((int) Trans).ToString());
								}
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", ((int) Trans).ToString() + " Actual Voltage < RVOLT!");
								result = false;
							}
							else
							{
								// Check if Trans is in Overflux

								// Not in Overflux
								if (!IsTransOverflux(TransActVoltage, NomTAPVoltage, m_theCRPCParameters.VR_TAV))
								{
									if (OverfluxAppear[(int) Busbar, (int) Trans])
									{
										OverfluxAppear[(int) Busbar, (int) Trans] = false;

										if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "OVERFLUX disappeared for the Trans" + ((int) Trans).ToString()))
										{
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
										}
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "OVERFLUX disappeared for the Trans" + ((int) Trans).ToString());

										if (Busbar == BusbarBBName.BusbarA || Busbar == BusbarBBName.BusbarB)
										{
											if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK2"), "1"))
											{
												result = false;
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " + "MARK2");
											}
										}
										else
										{
											if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK8"), "1"))
											{
												result = false;
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " + "MARK8");
											}
										}
									}

									if (BusVoltage < RVOLT)
									{
										//Counter(1, Busbar) = 0
										//Counter(2, Busbar) = 0
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", ((int) Busbar).ToString() + " Voltage < RVOLT!");

										//If Not m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_" & Trim(Str(Busbar))), Str(0)) Then
										//    VoltageControl = False
										//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C1_" & Trim(Str(Busbar)))
										//End If

										//If Not m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_" & Trim(Str(Busbar))), Str(0)) Then
										//    VoltageControl = False
										//    Call theCTraceLogger.WriteLog(TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " & "C2_" & Trim(Str(Busbar)))
										//End If
									}
								}
								else
								{
									// Trans in Overflux
									CheckBusbarInRange = false;
									OverfluxAppear[(int) Busbar, (int) Trans] = true;
									if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "OVERFLUX appearing for the Trans" + ((int) Trans).ToString()))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
									}
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "OVERFLUX appearing for the Trans" + ((int) Trans).ToString());
									if (Busbar == BusbarBBName.BusbarA || Busbar == BusbarBBName.BusbarB)
									{
										if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK2"), "2"))
										{
											result = false;
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " + "MARK2");
										}
										if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP A/B"))
										{
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
										}
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "SUGGESTION: DECR TAP A/B");
									}
									else
									{
										if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK8"), "2"))
										{
											result = false;
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.VoltageControl()", "Could not update value in SCADA: " + "MARK8");
										}
										if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP PP"))
										{
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
										}
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "SUGGESTION: DECR TAP PP");
									}
								}
							}
						}
					}
					if ((CheckBusbarInRange) && (TransOnBusbar))
					{
						// We're not in overflux --> prepare for the TAP changing
						CheckVoltageInRange(Busbar);
					}
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..VoltageControl()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


			return result;
		}

		// Checks the Switch Status of the Transformer
		private bool CheckTransStatus(BusbarBBName aBusbar, TransformerName aTrans)
		{
			bool result = false;
			try
			{

				result = false;

				switch(aBusbar)
				{
					case BusbarBBName.BusbarA : 
						switch(aTrans)
						{
							case TransformerName.TransT1AN : 
								if (m_theCRPCParameters.MT1A_CB == 2 && m_theCRPCParameters.MT1A_DS3 == 2 && m_theCRPCParameters.MT1A_DS1 == 2)
								{
									result = true;
								} 
								break;
							case TransformerName.TransT2AN : 
								if (m_theCRPCParameters.MT2A_CB == 2 && m_theCRPCParameters.MT2A_DS3 == 2 && m_theCRPCParameters.MT2A_DS1 == 2)
								{
									result = true;
								} 
								break;
							case TransformerName.TransT3AN : 
								if (m_theCRPCParameters.MT3_DS == 2 && (m_theCRPCParameters.MV3_CB == 2) && (m_theCRPCParameters.MV3_DS3 == 2) && (m_theCRPCParameters.MV3_DS1 == 2))
								{
									result = true;
								} 
								break;
							case TransformerName.TransT5AN : 
								if (m_theCRPCParameters.MT5A_DS3 == 2 && (m_theCRPCParameters.M51_CB == 2) && (m_theCRPCParameters.M51_DS1 == 2) && (m_theCRPCParameters.M51_DS2 == 2) && (m_theCRPCParameters.GMF1_CB == 2) && (m_theCRPCParameters.GMF1_DS1 == 2) && (m_theCRPCParameters.GMF1_DS2 == 2) && (m_theCRPCParameters.GMF1_DS3 == 2))
								{
									result = true;
								} 
								break;
						} 
						break;
					case BusbarBBName.BusbarB : 
						switch(aTrans)
						{
							case TransformerName.TransT1AN : 
								if (m_theCRPCParameters.MT1A_CB == 2 && m_theCRPCParameters.MT1A_DS3 == 2 && m_theCRPCParameters.MT1A_DS2 == 2)
								{
									result = true;
								} 
								break;
							case TransformerName.TransT2AN : 
								if (m_theCRPCParameters.MT2A_CB == 2 && m_theCRPCParameters.MT2A_DS3 == 2 && m_theCRPCParameters.MT2A_DS2 == 2)
								{
									result = true;
								} 
								break;
							case TransformerName.TransT3AN : 
								if (m_theCRPCParameters.MT3_DS == 2 && (m_theCRPCParameters.MV3_CB == 2) && (m_theCRPCParameters.MV3_DS3 == 2) && (m_theCRPCParameters.MV3_DS2 == 2))
								{
									result = true;
								} 
								break;
							case TransformerName.TransT5AN : 
								if (m_theCRPCParameters.MT5A_DS3 == 2 && (m_theCRPCParameters.MT5A_CB == 2) && (m_theCRPCParameters.MT5A_DS1 == 2) && (m_theCRPCParameters.MT5A_DS2 == 2))
								{
									result = true;
								} 
								break;
						} 
						break;
					case BusbarBBName.BusbarE : 
						switch(aTrans)
						{
							case TransformerName.TransT3AN : 
								if (m_theCRPCParameters.MT3_DS == 2 && (m_theCRPCParameters.MZ3_CB == 2) && (m_theCRPCParameters.MZ3_DS3 == 2) && (m_theCRPCParameters.MZ3_DS1 == 2))
								{
									result = true;
								} 
								break;
							case TransformerName.TransT4AN : 
								if (m_theCRPCParameters.MT4A_DS3 == 2 && (m_theCRPCParameters.MP4_CB == 2) && (m_theCRPCParameters.MP4_DS1 == 2) && (m_theCRPCParameters.MP4_DS2 == 2) && (m_theCRPCParameters.M1P_CB == 2) && (m_theCRPCParameters.M1P_DS1 == 2) && (m_theCRPCParameters.M1P_DS2 == 2))
								{
									result = true;
								} 
								break;
						} 
						break;
					case BusbarBBName.BusbarF : 
						switch(aTrans)
						{
							case TransformerName.TransT3AN : 
								if (m_theCRPCParameters.MT3_DS == 2 && (m_theCRPCParameters.MZ3_CB == 2) && (m_theCRPCParameters.MZ3_DS3 == 2) && (m_theCRPCParameters.MZ3_DS2 == 2))
								{
									result = true;
								} 
								break;
							case TransformerName.TransT4AN : 
								if (m_theCRPCParameters.MT4A_DS3 == 2 && (m_theCRPCParameters.MT4A_CB == 2) && (m_theCRPCParameters.MT4A_DS1 == 2) && (m_theCRPCParameters.MT4A_DS2 == 2))
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

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..CheckTransStatus()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..IsTransOverflux()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
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
				//If (m_theCRPCParameters.MT5A_CB = 2) And (m_theCRPCParameters.MT5A_DS1 = 2) And _
				//'(m_theCRPCParameters.MT5A_DS2 = 2) And (m_theCRPCParameters.M51_CB = 2) And _
				//'(m_theCRPCParameters.M51_DS1 = 2) And (m_theCRPCParameters.M51_DS2 = 2) And _
				//'(m_theCRPCParameters.GMF1_CB = 2) And (m_theCRPCParameters.GMF1_DS1 = 2) And _
				//'(m_theCRPCParameters.GMF1_DS2 = 2) Then
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
				MAB = (byte) m_theCRPCParameters.MAB;

				// ?????????????????????????
				BusNomVoltage = 63;
				// ?????????????????????????
				switch(aBusbar)
				{
					case BusbarBBName.BusbarA : case BusbarBBName.BusbarB : 
						VRange = m_theCRPCParameters.VR_EAF; 
						break;
					case BusbarBBName.BusbarE : case BusbarBBName.BusbarF : 
						VRange = m_theCRPCParameters.VR_PP; 
						break;
				}

				VRange = (VRange * BusNomVoltage) / 100;
				VLow = BusNomVoltage - VRange;
				VHigh = BusNomVoltage + VRange;

				if (BusVoltage > VHigh)
				{
					Counter[1, (int) aBusbar]++;
					Counter[2, (int) aBusbar] = 0;

					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "BusVoltage=" + BusVoltage.ToString() + " > VHigh=" + VHigh.ToString());
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CVoltageController.CheckVoltageInRange()", "Counter(1, " + ((int) aBusbar).ToString() + ")=" + Counter[1, (int) aBusbar].ToString());

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[1, (int) aBusbar])))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C1_" + Conversion.Str(aBusbar).Trim());
						return;
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[2, (int) aBusbar])))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C2_" + Conversion.Str(aBusbar).Trim());
						return;
					}

					// If BusVoltage is high for more than 3 cycles send decrease suggestions
					if (Counter[1, (int) aBusbar] >= 3)
					{
						if (aBusbar == BusbarBBName.BusbarA || aBusbar == BusbarBBName.BusbarB)
						{
							// Check MAB status
							switch(MAB)
							{
								case 0 : 
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CVoltageController.CheckVoltageInRange()", "MAB Status Invalid!"); 
									return;
								case 1 :  // Open 
									switch(aBusbar)
									{
										case BusbarBBName.BusbarA : 
											if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK4"), "2"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK4");
											} 
											if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP A"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
											} 
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: DECR TAP A"); 
											break;
										case BusbarBBName.BusbarB : 
											if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK6"), "2"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK6");
											} 
											if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP B"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
											} 
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: DECR TAP B"); 
											break;
									} 
									break;
								case 2 :  // Close 
									if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK2"), "2"))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK2");
									} 
									if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP A/B"))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
									} 
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: DECR TAP A/B"); 
									break;
							}
						}
						else
						{
							// PP Busbar
							if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK8"), "2"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK8");
							}
							if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECR TAP PP"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
							}
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: DECR TAP PP");
						}

						// If BusVoltage is high for 5 or more than 5 cycles send suggestion not successfull
						if (Counter[1, (int) aBusbar] >= 5)
						{
							// This delay is used to let the SCADA Alarming having enough time to perform 2 alarms exactly after each other.
							Delay(4);
							if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "SUGGESTION DECR NOT SUCCESSFULL"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.VoltageControl()", "Sending alarm failed.");
							}
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION DECR NOT SUCCESSFULL");
						}
					}
				}
				else
				{
					if (BusVoltage > VLow)
					{
						// Bus voltage is in range, reset the counters
						Counter[1, (int) aBusbar] = 0;
						Counter[2, (int) aBusbar] = 0;

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[1, (int) aBusbar])))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C1_" + Conversion.Str(aBusbar).Trim());
							return;
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[2, (int) aBusbar])))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C2_" + Conversion.Str(aBusbar).Trim());
							return;
						}

					}
					else
					{
						Counter[1, (int) aBusbar] = 0;
						Counter[2, (int) aBusbar]++;

						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "BusVoltage=" + BusVoltage.ToString() + " < VLow=" + VLow.ToString());
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CVoltageController.CheckVoltageInRange()", "Counter(2, " + ((int) aBusbar).ToString() + ")=" + Counter[2, (int) aBusbar].ToString());

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C1_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[1, (int) aBusbar])))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C2_" + Conversion.Str(aBusbar).Trim());
							return;
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C2_" + Conversion.Str(aBusbar).Trim()), Conversion.Str(Counter[2, (int) aBusbar])))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "C2_" + Conversion.Str(aBusbar).Trim());
							return;
						}

						// If BusVoltage is low for 3 or more than 3 cycles send increase suggestions
						if (Counter[2, (int) aBusbar] >= 3)
						{
							NewTAP = ActualTAP + 1;
							if (NewTAP <= 19)
							{
								switch(Convert.ToInt32(NewTAP))
								{
									case 1 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP1; 
										break;
									case 2 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP2; 
										break;
									case 3 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP3; 
										break;
									case 4 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP4; 
										break;
									case 5 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP5; 
										break;
									case 6 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP6; 
										break;
									case 7 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP7; 
										break;
									case 8 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP8; 
										break;
									case 9 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP9; 
										break;
									case 10 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP10; 
										break;
									case 11 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP11; 
										break;
									case 12 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP12; 
										break;
									case 13 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP13; 
										break;
									case 14 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP14; 
										break;
									case 15 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP15; 
										break;
									case 16 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP16; 
										break;
									case 17 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP17; 
										break;
									case 18 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP18; 
										break;
									case 19 : 
										NomNewTAPVoltage = m_theCRPCParameters.VTAP19; 
										break;
								}
							}
							else
							{
								NomNewTAPVoltage = NomTAPVoltage;
							}

							if (!IsTransOverflux(TransActVoltage, NomNewTAPVoltage, m_theCRPCParameters.VR_TAV))
							{
								if (aBusbar == BusbarBBName.BusbarA || aBusbar == BusbarBBName.BusbarB)
								{
									// Check MAB status
									switch(MAB)
									{
										case 0 : 
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceWarning, "CVoltageController.CheckVoltageInRange()", "MAB Status Invalid!"); 
											return;
										case 1 :  // Open 
											switch(aBusbar)
											{
												case BusbarBBName.BusbarA : 
													if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK3"), "2"))
													{
														GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK3");
													} 
													if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCR TAP A"))
													{
														GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
													} 
													GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: INCR TAP A"); 
													break;
												case BusbarBBName.BusbarB : 
													if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK5"), "2"))
													{
														GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK5");
													} 
													if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCR TAP B"))
													{
														GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
													} 
													GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: INCR TAP B"); 
													break;
											} 
											break;
										case 2 :  // Close 
											if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK1"), "2"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK1");
											} 
											if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCR TAP A/B"))
											{
												GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
											} 
											GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: INCR TAP A/B"); 
											break;
									}
								}
								else
								{
									// PP Busbar
									if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK7"), "2"))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController.CheckVoltageInRange()", "Could not update value in SCADA: " + "MARK7");
									}
									if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCR TAP PP"))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
									}
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION: INCR TAP PP");
								}

								if (Counter[2, (int) aBusbar] >= 5)
								{
									// This delay is used to let the SCADA Alarming having enough time to perform 2 alarms exactly after each other.
									Delay(4);
									if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION INCR NOT SUCCESSFULL"))
									{
										GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
									}
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "SUGGESTION INCR NOT SUCCESSFULL");
								}
							}
							else
							{
								// New TAP will produce Overflux
								if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "POSSIBLE OVERFLUX"))
								{
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "Sending alarm failed.");
								}
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckVoltageInRange()", "POSSIBLE OVERFLUX");
								// Send DCO Job (PCS)? (OVFLUX)
							}
						}
					}
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..CheckVoltageInRange()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


		}

		public void SettheRPCParam(CRPCParameters aCRPCParameters)
		{
			m_theCRPCParameters = aCRPCParameters;
		}

		private void Delay(int WaitSecond)
		{

			int StartSecond = DateTime.Now.Second;

			do 
			{
			}
			while(DateTime.Now.Second - StartSecond < WaitSecond);
		}
	}
}