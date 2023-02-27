using Microsoft.VisualBasic;
using System;



namespace RPC_Service_App
{
	internal class CQController
	{


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


		private double NGAuto = 0;
		private bool[] GenAuto = new bool[4];
		private double[] QGen = new double[5];
		private double QGenAuto = 0;

		private int Counter5 = 0;
		private int Counter6 = 0;


		// Checks if the sum of the generators reactive power are in range.
		public bool QControl()
		{
			bool result = false;
			try
			{


				result = true;

				// Read the previous counters values from SCADA
				Counter5 = Convert.ToInt32(m_theCRPCParameters.C5);
				Counter6 = Convert.ToInt32(m_theCRPCParameters.C6);


				NGAuto = 0;
				QGenAuto = 0;

				GenAuto[1] = m_theCRPCParameters.GEN1_AUTO != 0;
				GenAuto[2] = m_theCRPCParameters.GEN2_AUTO != 0;
				GenAuto[3] = m_theCRPCParameters.GEN3_AUTO != 0;

				QGen[1] = m_theCRPCParameters.QGEN1;
				QGen[2] = m_theCRPCParameters.QGEN2;
				QGen[3] = m_theCRPCParameters.QGEN3;
				QGen[4] = m_theCRPCParameters.QGEN4;

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.QControl()", "Q G1 = " + m_theCRPCParameters.QGEN1.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.QControl()", "Q G2 = " + m_theCRPCParameters.QGEN2.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.QControl()", "Q G3 = " + m_theCRPCParameters.QGEN3.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.QControl()", "Q G4 = " + m_theCRPCParameters.QGEN4.ToString());


				for (int i = 1; i <= 3; i++)
				{
					if (GenAuto[i])
					{
						NGAuto++;
						QGenAuto += QGen[i];
					}
				}
				// Gen4 reactive power should be considered.
				QGenAuto += QGen[4];

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("QGEN_AUTO"), Conversion.Str(QGenAuto)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: QGEN_AUTO");
					return result;
				}

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("NGEN_AUTO"), Conversion.Str(NGAuto)))
				{
					result = false;
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: QGEN_AUTO");
					return result;
				}

				if (NGAuto > 0)
				{
					CheckQInRange();
				}
				else
				{
					if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "No Generator in Auto Mode."))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.QControl()", "Sending alarm failed.");
					}
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.QControl()", "No Generator in Auto Mode.");
					Counter5 = 0;
					Counter6 = 0;

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C5"), Conversion.Str(Counter5)))
					{
						result = false;
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: " + "C5");
						return result;
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C6"), Conversion.Str(Counter6)))
					{
						result = false;
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: " + "C6");
						return result;
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "1"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: " + "MARK9");
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_1"), "1"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: " + "MARK9_1");
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK10"), "1"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.QControl()", "Could not update value in SCADA: " + "MARK10");
					}

				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..QControl()", excep.Message);
				result = false;
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}

			return result;
		}

		private void CheckQInRange()
		{
			try
			{

				double K1 = 0;
				double K2 = 0;
				double QMILL = 0;


				K1 = m_theCRPCParameters.K1;
				K2 = m_theCRPCParameters.K2;

				QMILL = m_theCRPCParameters.QFIN + m_theCRPCParameters.QROUGH + m_theCRPCParameters.QTANDEM;

				if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("QMILL"), Conversion.Str(QMILL)))
				{
					GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: QMILL");
				}

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.CheckQInRange()", "Q Fin = " + m_theCRPCParameters.QFIN.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.CheckQInRange()", "Q Rough = " + m_theCRPCParameters.QROUGH.ToString());
				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceValue, "CQController.CheckQInRange()", "Q Tandem = " + m_theCRPCParameters.QTANDEM.ToString());


				if (QGenAuto < NGAuto * K1 + QMILL)
				{
					Counter5++;
					Counter6 = 0;

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C5"), Conversion.Str(Counter5)))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C5");
						return;
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C6"), Conversion.Str(Counter6)))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C6");
						return;
					}

					if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK10"), "1"))
					{
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK10");
					}

					if (Counter5 >= 3)
					{
						if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: INCREASE SET OF GENERATORS"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "Sending alarm failed.");
						}
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "2"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9");
						}
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_1"), "2"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9_1");
						}
						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "SUGGESTION: INCREASE SET OF GENERATORS");
						if (Counter5 >= 5)
						{
							if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "SUGGESTION INCR SET OF GEN NOT SUCCESSFULL"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "Sending alarm failed.");
							}
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "SUGGESTION INCR SET OF GEN NOT SUCCESSFULL");
						}
					}
				}
				else
				{
					if (QGenAuto <= NGAuto * K2 + QMILL)
					{
						Counter5 = 0;
						Counter6 = 0;

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C5"), Conversion.Str(Counter5)))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C5");
							return;
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C6"), Conversion.Str(Counter6)))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C6");
							return;
						}

						// The suggestions should be deleted.
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9");
						}
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_1"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9_1");
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK10"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK10");
						}

						GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CVoltageController.CheckQInRange()", "GENERATORS IN RANGE.");

					}
					else
					{
						Counter5 = 0;
						Counter6++;

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C5"), Conversion.Str(Counter5)))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C5");
							return;
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("C6"), Conversion.Str(Counter6)))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "C6");
							return;
						}

						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9");
						}
						if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK9_1"), "1"))
						{
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK9_1");
						}

						if (Counter6 >= 3)
						{
							if (!m_theSCADADataInterface.SendAlarm("RPCSuggestion", "SUGGESTION: DECREASE SET OF GENERATORS"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "Sending alarm failed.");
							}
							if (!m_theSCADADataInterface.WriteData(m_theCRPCParameters.FindGUID("MARK10"), "2"))
							{
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CQController.CheckQInRange()", "Could not update value in SCADA: " + "MARK10");
							}
							GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "SUGGESTION: DECREASE SET OF GENERATORS");
							if (Counter6 >= 5)
							{
								if (!m_theSCADADataInterface.SendAlarm("RPCAlarm", "SUGGESTION DECR SET OF GEN NOT SUCCESSFULL"))
								{
									GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "Sending alarm failed.");
								}
								GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceInfo1, "CQController.CheckQInRange()", "SUGGESTION DECR SET OF GEN NOT SUCCESSFULL");
							}
						}
					}
				}
			}
			catch (System.Exception excep)
			{

				GeneralModule.theCTraceLogger.WriteLog(CTraceLogger.TraceType.TraceError, "CVoltageController..CheckQInRange()", excep.Message);
				//UPGRADE_TODO: (1065) Error handling statement (Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
				UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("Resume Next Statement");
			}


		}

		public void SettheRPCParam(CRPCParameters aCRPCParameters)
		{
			m_theCRPCParameters = aCRPCParameters;
		}
	}
}