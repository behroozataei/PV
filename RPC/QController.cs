using System;
using System.Collections.Generic;
using System.Text;


using Irisa.Logger;

namespace RPC
{
    internal class QController
    {
		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
		private bool[] QLimitViolApp = new bool[3];


		internal QController(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
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
				Counter5 = Convert.ToInt32(_repository.GetRPCScadaPoint("C5").Value);
				Counter6 = Convert.ToInt32(_repository.GetRPCScadaPoint("C6").Value);


				NGAuto = 0;
				QGenAuto = 0;

				GenAuto[1] = _repository.GetRPCScadaPoint("GEN1_AUTO").Value != 0;
				GenAuto[2] = _repository.GetRPCScadaPoint("GEN2_AUTO").Value != 0;
				GenAuto[3] = _repository.GetRPCScadaPoint("GEN3_AUTO").Value != 0;

				QGen[1] = _repository.GetRPCScadaPoint("QGEN1").Value;
				QGen[2] = _repository.GetRPCScadaPoint("QGEN2").Value;
				QGen[3] = _repository.GetRPCScadaPoint("QGEN3").Value;
				QGen[4] = _repository.GetRPCScadaPoint("QGEN4").Value;

				_logger.WriteEntry("Q G1 = " + (_repository.GetRPCScadaPoint("QGEN1").Value).ToString(),LogLevels.Trace);
				_logger.WriteEntry("Q G2 = " + (_repository.GetRPCScadaPoint("QGEN2").Value).ToString(), LogLevels.Trace);
				_logger.WriteEntry("Q G3 = " + (_repository.GetRPCScadaPoint("QGEN3").Value).ToString(), LogLevels.Trace);
				_logger.WriteEntry("Q G4 = " + (_repository.GetRPCScadaPoint("QGEN4").Value).ToString(), LogLevels.Trace);

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

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("QGEN_AUTO"), (float)QGenAuto))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: QGEN_AUTO",LogLevels.Error);
					return result;
				}

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("NGEN_AUTO"), (float)NGAuto))
				{
					result = false;
					_logger.WriteEntry( "Could not update value in SCADA: QGEN_AUTO", LogLevels.Error);
					return result;
				}

				if (NGAuto > 0)
				{
					CheckQInRange();
				}
				else
				{
					if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"),SinglePointStatus.Appear, "No Generator in Auto Mode."))
					{
						_logger.WriteEntry("Sending alarm failed.",LogLevels.Error);
					}
					_logger.WriteEntry("No Generator in Auto Mode.",LogLevels.Info);
					Counter5 = 0;
					Counter6 = 0;

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C5"), (float)Counter5))
					{
						result = false;
						_logger.WriteEntry( "Could not update value in SCADA: " + "C5", LogLevels.Error);
						return result;
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C6"), (float)Counter6))
					{
						result = false;
						_logger.WriteEntry( "Could not update value in SCADA: " + "C6", LogLevels.Error);
						return result;
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 1))
					{
						_logger.WriteEntry( "Could not update value in SCADA: " + "MARK9", LogLevels.Error);
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_1"), 1))
					{
						_logger.WriteEntry( "Could not update value in SCADA: " + "MARK9_1", LogLevels.Error);
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), 1))
					{
						_logger.WriteEntry( "Could not update value in SCADA: " + "MARK10", LogLevels.Error);
					}

				}
			}
			catch (System.Exception excep)
			{

				_logger.WriteEntry(excep.Message,LogLevels.Error);
				result = false;
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


				K1 = _repository.GetRPCScadaPoint("K1").Value;
				K2 = _repository.GetRPCScadaPoint("K2").Value;

				QMILL = _repository.GetRPCScadaPoint("QFIN").Value + _repository.GetRPCScadaPoint("QROUGH").Value + _repository.GetRPCScadaPoint("QTANDEM").Value;

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("QMILL"), (float)QMILL))
				{
					_logger.WriteEntry("Could not update value in SCADA: QMILL",LogLevels.Error);
				}

				_logger.WriteEntry("Q Fin = " + _repository.GetRPCScadaPoint("QFIN").Value.ToString(),LogLevels.Trace);
				_logger.WriteEntry("Q Rough = " + _repository.GetRPCScadaPoint("QROUGH").Value.ToString(), LogLevels.Trace);
				_logger.WriteEntry("Q Tandem = " + _repository.GetRPCScadaPoint("QTANDEM").Value.ToString(), LogLevels.Trace);


				if (QGenAuto < NGAuto * K1 + QMILL)
				{
					Counter5++;
					Counter6 = 0;

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C5"), (float)Counter5))
					{
						_logger.WriteEntry("Could not update value in SCADA: " + "C5",LogLevels.Error);
						return;
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C6"), (float)Counter6))
					{
						_logger.WriteEntry("Could not update value in SCADA: " + "C6", LogLevels.Error);
						return;
					}

					if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), 1))
					{
						_logger.WriteEntry("Could not update value in SCADA: " + "MARK10", LogLevels.Error);
					}

					if (Counter5 >= 3)
					{
						if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"),SinglePointStatus.Appear, "SUGGESTION: INCREASE SET OF GENERATORS"))
						{
							_logger.WriteEntry("Sending alarm failed.",LogLevels.Error);
						}
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 2))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9",LogLevels.Error);
						}
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_1"), 2))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9_1",LogLevels.Error);
						}
						_logger.WriteEntry("SUGGESTION: INCREASE SET OF GENERATORS",LogLevels.Info);
						if (Counter5 >= 5)
						{
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"),SinglePointStatus.Appear, "SUGGESTION INCR SET OF GEN NOT SUCCESSFULL"))
							{
								_logger.WriteEntry("Sending alarm failed.", LogLevels.Info);
							}
							_logger.WriteEntry("SUGGESTION INCR SET OF GEN NOT SUCCESSFULL",LogLevels.Info);
						}
					}
				}
				else
				{
					if (QGenAuto <= NGAuto * K2 + QMILL)
					{
						Counter5 = 0;
						Counter6 = 0;

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C5"), (float)Counter5))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C5",LogLevels.Error);
							return;
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C6"), (float)Counter6))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C6",LogLevels.Error);
							return;
						}

						// The suggestions should be deleted.
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 1))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9",LogLevels.Error);
						}
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_1"), 1))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9_1",LogLevels.Error);
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), 1))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK10",LogLevels.Error);
						}

						_logger.WriteEntry("GENERATORS IN RANGE.",LogLevels.Info);

					}
					else
					{
						Counter5 = 0;
						Counter6++;

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C5"), (float)Counter5))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C5",LogLevels.Error);
							return;
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("C6"), (float)Counter6))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "C6",LogLevels.Error);
							return;
						}

						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9"), 1))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9",LogLevels.Error);
						}
						if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK9_1"), 1))
						{
							_logger.WriteEntry("Could not update value in SCADA: " + "MARK9_1", LogLevels.Error);
						}

						if (Counter6 >= 3)
						{
							if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCSuggestion"),SinglePointStatus.Appear, "SUGGESTION: DECREASE SET OF GENERATORS"))
							{
								_logger.WriteEntry("Sending alarm failed.",LogLevels.Info);
							}
							if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("MARK10"), 2))
							{
								_logger.WriteEntry("Could not update value in SCADA: " + "MARK10",LogLevels.Error);
							}
							_logger.WriteEntry("SUGGESTION: DECREASE SET OF GENERATORS",LogLevels.Info);
							if (Counter6 >= 5)
							{
								if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "SUGGESTION DECR SET OF GEN NOT SUCCESSFULL"))
								{
									_logger.WriteEntry("Sending alarm failed.",LogLevels.Info);
								}
								_logger.WriteEntry("SUGGESTION DECR SET OF GEN NOT SUCCESSFULL", LogLevels.Info);
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
	}
}
