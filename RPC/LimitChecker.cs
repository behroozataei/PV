using System;
using System.Collections.Generic;
using System.Text;


using Irisa.Logger;
using Irisa.Message.CPS;

namespace RPC
{
    internal class LimitChecker
    {
		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
		private bool[] QLimitViolApp = new bool[3];


		internal LimitChecker(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
		}


		// Busbars Voltage Limit Checking --> EAF, PP, 400kV
		// This method reads the 3Min averages values and writes to SCADA.
		// The PowerCC will itself check the limit checking.
		public bool VoltageLimitChecking()
		{
			bool result = false;
			try
			{
				float[] VoltageValue = new float[7];
				DateTime dtTo = new DateTime();
				          
				result = true;

				// Calculate time range:

				//TODO: 1401-11-02
				// The difference is for Greenwich Mean Time (GMT) differential
				dtTo = DateTime.UtcNow;
				//dtTo = DateTime.Now.AddHours(GeneralModule.GMTHourDiff);
				//dtTo = dtTo.AddMinutes(GeneralModule.GMTMinuteDiff);
				//dtTo = dtTo.AddSeconds(GeneralModule.GMTSecondDiff);
				// Adjust the time to the begining of the period
				//dtTo = dtTo.AddSeconds(-dtTo.Second);

				IntervalTime intervaltime = new IntervalTime(DateTime.UtcNow.AddMinutes(-3.0), DateTime.UtcNow);

				// Retreiving a value for the previous 3 minutes from HIS:
				_logger.WriteEntry("Average Sampling Time=" + dtTo.ToString(),LogLevels.Trace);


				// TODO: 1401-11-02
				//VoltageValue[1] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("V400_1"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				//VoltageValue[2] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("V400_2"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				//VoltageValue[3] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VEAF_A"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				//VoltageValue[4] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VEAF_B"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				//VoltageValue[5] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VPP_E"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);
				//VoltageValue[6] = m_theCSCADADataInterface.GetHISValue(m_theCRPCParameters.FindGUID("VPP_F"), "HIS_ANALOG_3_Min_A", dtTo, dtTo);

				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("V400_1"), intervaltime, out VoltageValue[1]))
                {
					_logger.WriteEntry("Could not read avarage value V400_1", LogLevels.Error);
				}
				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("V400_2"), intervaltime, out VoltageValue[2]))
				{
					_logger.WriteEntry("Could not read avarage value V400_2", LogLevels.Error);
				}
				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("VEAF_A"), intervaltime, out VoltageValue[3]))
				{
					_logger.WriteEntry("Could not read avarage value VEAF_A", LogLevels.Error);
				}
				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("VEAF_B"), intervaltime, out VoltageValue[4]))
				{
					_logger.WriteEntry("Could not read avarage value VEAF_B", LogLevels.Error);
				}
				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("VPP_E"), intervaltime, out VoltageValue[5]))
				{
					_logger.WriteEntry("Could not read avarage value VPP_E", LogLevels.Error);
				}
				if(!_repository.TryGetHISAverageinIntervalTime(_repository.GetRPCScadaPoint("VPP_F"), intervaltime, out VoltageValue[6]))
				{
					_logger.WriteEntry("Could not read avarage value VPP_F", LogLevels.Error);
				}

				// The Alarming in the Alarm List is done by the SCADA.

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("V400_1_Avg"), (float)VoltageValue[1]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: V400_1_Avg", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("V400_2_Avg"), (float)VoltageValue[2]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: V400_2_Avg", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VEAF_A_Avg"), (float)VoltageValue[3]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: VEAF_A_Avg", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VEAF_B_Avg"), (float)VoltageValue[4]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: VEAF_B_Avg", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VPP_E_Avg"), (float)VoltageValue[5]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: VPP_E_Avg", LogLevels.Error);
				}
				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("VPP_F_Avg"),(float)VoltageValue[6]))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: VPP_F_Avg",LogLevels.Error);
				}


				_logger.WriteEntry("----- 3 Min Average Voltages -----",LogLevels.Info);
				_logger.WriteEntry("V400_1_Avg = "  + VoltageValue[1].ToString() ,LogLevels.Trace);
				_logger.WriteEntry("V400_2_Avg = "  + VoltageValue[2].ToString() , LogLevels.Trace);
				_logger.WriteEntry("VEAF_A_Avg = "  + VoltageValue[3].ToString() , LogLevels.Trace);
				_logger.WriteEntry("VEAF_B_Avg = "  + VoltageValue[4].ToString() , LogLevels.Trace);
				_logger.WriteEntry("VPP_E_Avg  =  " + VoltageValue[5].ToString() , LogLevels.Trace);
				_logger.WriteEntry("VPP_F_Avg  =  " + VoltageValue[6].ToString() , LogLevels.Trace);
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message,LogLevels.Error);
				result = false;
			}

			return result;
		}

		// Q-SVC Checking
		public bool QLimitChecking()
		{
			bool result = false;
			try
			{
				double K = 0;
				double M = 0;
				double Er_EAF =0;
				double Er_SVC = 0;
				double Er_BANK = 0;
				double Er_LF = 0;

				result = true;

				Er_EAF = _repository.GetRPCScadaPoint("Er_EAF_3Min").Value; 
				Er_SVC = _repository.GetRPCScadaPoint("Er_SVC_3Min").Value; ;
				K = _repository.GetRPCScadaPoint("K").Value; ;

				if (Er_SVC > Er_EAF + K)
				{
					QLimitViolApp[1] = true;
					// Sending Alarm
					if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear,$"Er_SVC=" + Er_SVC.ToString() + ">" + "Er_EAF + K=" + (Er_EAF + K).ToString()))
					{
						_logger.WriteEntry("Sending \"RPCAlarm\" alarm  failed.", LogLevels.Warn);
					}
					_logger.WriteEntry($"Limit Violation Appeared --> Er_SVC = " + Er_SVC.ToString() + " > Er_EAF + K = " + (Er_EAF + K).ToString(),LogLevels.Info);
				}
				else
				{
					if (QLimitViolApp[1])
					{
						QLimitViolApp[1] = false;
						_logger.WriteEntry("Limit Violation Disappeared --> Er_SVC = " + Er_SVC.ToString() + " <= Er_EAF + K = " + (Er_EAF + K).ToString(),LogLevels.Info);
					}
				}

				// In the documents ther's a section that checks Capacitor Bank with LFS,
				// but in the perl codes there's not such a section.
				Er_BANK = _repository.GetRPCScadaPoint("Er_BANK_3Min").Value;
				Er_LF = _repository.GetRPCScadaPoint("Er_LF_3Min").Value;
				M = Er_LF = _repository.GetRPCScadaPoint("M").Value; ;

				if (Er_BANK > Er_LF + M)
				{
					QLimitViolApp[2] = true;
					// Sending Alarm
					if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "Er_BANK=" + Er_BANK + ">" + "Er_LF + M=" + (Er_LF + M).ToString()))
					{
						_logger.WriteEntry("Sending \"RPCAlarm\" alarm  failed.", LogLevels.Warn);
					}
					_logger.WriteEntry("Limit Violation Appeared --> Er_BANK = " + Er_BANK + " > Er_LF + M = " + (Er_LF + M).ToString(),LogLevels.Info);
				}
				else
				{
					if (QLimitViolApp[2])
					{
						QLimitViolApp[2] = false;
						_logger.WriteEntry("Limit Violation Disappeared --> Er_BANK = " + Er_BANK + " <= Er_LF + M = " + (Er_LF + M).ToString(),LogLevels.Info);
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
	}
}
