using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using Irisa.Logger;
using Irisa.Message.CPS;
using COMMON;

namespace RPC
{
	internal class CycleValidator
	{
		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

		private DateTime[] m_Cycles = new DateTime[16]; //Array for 15 minutes period
		private int m_CycleNo = 0;

		internal CycleValidator(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
		}

		private RPCScadaPoint _FuncCycle;

		public bool GetRPCCycleNo()
		{
			bool result = false;
			try
			{
				int FuncCycle = 0;
				// Suppose everything is Ok.
				result = true;
				// Load previous values into array
				if (!LoadRPCCycles())
				{
					_logger.WriteEntry("Could not load data from T_CRPCCycles!", LogLevels.Warn);
					return false;
				}
				var vTime = DateTime.Now;
				m_CycleNo = vTime.Minute % 15 + 1; // Cycles begin from 1 to 15

				FuncCycle = ((m_CycleNo - 1) / 3) * 3;
				if (FuncCycle == 0)
				{
					FuncCycle = 15;
				}

				if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint("FuncCycle"), (float)FuncCycle))
				{
					result = false;
					_logger.WriteEntry("Could not update value in SCADA: FuncCycle", LogLevels.Error);
				}

				if (m_CycleNo == 1)
				{
					_logger.WriteEntry("In the first cycle (Cycle 1),   " + vTime.ToString("t"), LogLevels.Info);
				}
				_logger.WriteEntry("   Cycle Number is:                   " + m_CycleNo.ToString(), LogLevels.Info);

				if (m_CycleNo > 1)
				{
					result = IsContiniuousCycles(m_CycleNo);
					if (!result)
					{
						if (!_updateScadaPointOnServer.SendAlarm(_repository.GetRPCScadaPoint("RPCAlarm"), SinglePointStatus.Appear, "RPC Function is not running continuouslyx"))
						{
							_logger.WriteEntry("Sending RPCAlarm failed.", LogLevels.Error);
						}
						_logger.WriteEntry("Function is not running continuously!", LogLevels.Warn);
						return result;
					}
				}

				// Save the last values into array
				if (!SaveRPCCycles())
				{
					_logger.WriteEntry("Could not save data into T_CRPCCycles!", LogLevels.Error);
					return false;
				}
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
				result = false;
			}
			return result;
		}
		public int CycleNo
		{
			get
			{

				return m_CycleNo;

			}
		}

		private bool IsContiniuousCycles(int nCurCycle)
		{
			bool result = false;
			int CurrHour = 0;
			int PrevHour = 0;
			int PrevMin = 0;
			int CurrMin = 0;
			DateTime PrevDate = new DateTime();
			DateTime CurrDate = new DateTime();

			result = true;

			// Check continuosely in activation times
			int tempForEndVar = nCurCycle;
			for (int i = 2; i <= tempForEndVar; i++)
			{ // m_CycleNo
				PrevDate = m_Cycles[i - 1].Date;
				CurrDate = m_Cycles[i].Date;

				PrevHour = m_Cycles[i - 1].Hour;
				PrevMin = m_Cycles[i - 1].Minute;

				CurrHour = m_Cycles[i].Hour;
				CurrMin = m_Cycles[i].Minute;

				if ((PrevHour != CurrHour) || ((PrevMin + 1) != CurrMin) || (PrevDate != CurrDate))
				{
					result = false;
				}
			}
			_logger.WriteEntry("IsContiniuousCycles(),  " + result, LogLevels.Info);

			return result;
		}

		private bool LoadRPCCycles()
		{
			bool result = false;
			try
			{
				T_RPCCycles_Str _t_rpcCycles = new T_RPCCycles_Str();
				_t_rpcCycles = JsonConvert.DeserializeObject<T_RPCCycles_Str>(RedisUtils.RedisConn.Get(RedisKeyPattern.T_RPCCycles.ToString()));

				for (int i = 1; i <= 15; i++)
				{
					m_Cycles[i] = _t_rpcCycles.CYCLE[i];
				}
				result = true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
				result = false;
			}
			return result;
		}
		private bool SaveRPCCycles()
		{
			bool result = false;
			try
			{
				T_RPCCycles_Str _t_rpcCycles = new T_RPCCycles_Str();
				_t_rpcCycles = JsonConvert.DeserializeObject<T_RPCCycles_Str>(RedisUtils.RedisConn.Get(RedisKeyPattern.T_RPCCycles.ToString()));

				for (int i = 1; i <= 15; i++)
				{
					_t_rpcCycles.CYCLE[i] = m_Cycles[i];
				}
				RedisUtils.RedisConn.Set(RedisKeyPattern.T_RPCCycles, JsonConvert.SerializeObject(_t_rpcCycles));
				result = true;
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error);
				result = false;
			}
			return result;
		}
	}

	//

}

