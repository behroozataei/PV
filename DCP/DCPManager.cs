using System;
using System.Runtime.InteropServices;
using System.Data;
using System.Timers;

using Irisa.Logger;
using Irisa.Message;

namespace DCP
{
	internal enum eApp_Disapp
	{
		Disappear = 0,
		Appear = 1
	}

	public sealed class DCPManager : IProcessing
	{
		private const int EAFs = 8;
		private readonly IRepository _repository;
		private readonly ILogger _logger;
		private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

		// In second
		private int TIMER_CYCLE_LoadPowerForEAFS = 4000;
		private Timer _timer_LoadPowerForEAFS;

		// TODO : StartupForm.frm -> reset_DCP_Function_Status

		//==============================================================================
		//MEMBER VARIABLES
		//==============================================================================
		// An object for handling EAFs Consumptions
		private EAFConsumptionManager _eafConsumptonManager;

		// An object for handling PCS Telegrams
		private PCSManager _pcsManager = null;

		public DCPManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

			//--------------------------------------------------------
			// Step 1: For DC-EAFS Consumption:
			_eafConsumptonManager = new EAFConsumptionManager(_logger, _repository, cpsCommandService);

			//--------------------------------------------------------
			// Step 2: For DC-PCS:
			_pcsManager = new PCSManager(_logger, _repository, _updateScadaPointOnServer);

			//--------------------------------------------------------
			// Step 3: For LoadPowerForEAFS:
			DCManager_start();

			//--------------------------------------------------------
			_logger.WriteEntry($"=================================================================", LogLevels.Info);
			_logger.WriteEntry($" DCP is started . . . ", LogLevels.Info);
		}

		public void SCADAEventRaised(DCPScadaPoint scadaPoint)
		{
			_pcsManager.SCADAEventReceived(scadaPoint);
		}

		public void DCManager_start()
		{
			try
			{
				_timer_LoadPowerForEAFS = new Timer();
				_timer_LoadPowerForEAFS.Interval = TIMER_CYCLE_LoadPowerForEAFS;
				_timer_LoadPowerForEAFS.Elapsed += RunCyclicOperation4Second;

				_timer_LoadPowerForEAFS.Start();
				
				// Create a theCTraceLogger for any Logging, and Send dbConnection to TraceLogger
				_logger.WriteEntry($"Start of running LoadPowerForEAFS ... ", LogLevels.Info);
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}
		private static string GetEndStringCommand()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return "app.";
				// return string.Empty;

			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{

				return "APP_";

			}

			return string.Empty;
		}

		public void RunCyclicOperation4Second(object sender, ElapsedEventArgs e)
		{
			try
			{
				Retrive_Power();
			}
			catch (System.Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
		}

		public bool Retrive_Power()
		{
			try
			{
				var sql = "SELECT TelDateTime, Sumation, PowerGrp1, PowerGrp2, Furnace1, Furnace2, " +
					"Furnace3, Furnace4, Furnace5, Furnace6, Furnace7, Furnace8 " +
					$"from {GetEndStringCommand()}SFSC_EAFsPower Where TELDATETIME = (Select Max(TELDATETIME) From {GetEndStringCommand()}SFSC_EAFsPower)";
				DataTable dataTable = _repository.GetFromHistoricalDB(sql);
				if ((dataTable is null) || (dataTable.Rows.Count == 0))
				{
					_logger.WriteEntry($"Error: Select from Historical.{GetEndStringCommand()}SFSC_EAFsPower is null or no Row ", LogLevels.Error);
					return false;
				}
				else
				{
					DataRow dr = dataTable.Rows[0];
					sql = "INSERT INTO [PU10_PCS].[dbo].[T_EAFsPower](TelDateTime, Sumation, " +
						"PowerGrp1, PowerGrp2, Furnace1, Furnace2, Furnace3, Furnace4, Furnace5, " +
						"Furnace6, Furnace7, Furnace8) Values('" +
						dr["TelDateTime"].ToString() + "', " +
						dr["Sumation"].ToString() + ", " +
						dr["PowerGrp1"].ToString() + ", " +
						dr["PowerGrp2"].ToString() + ", " +
						dr["Furnace1"].ToString() + ", " +
						dr["Furnace2"].ToString() + ", " +
						dr["Furnace3"].ToString() + ", " +
						dr["Furnace4"].ToString() + ", " +
						dr["Furnace5"].ToString() + ", " +
						dr["Furnace6"].ToString() + ", " +
						dr["Furnace7"].ToString() + ", " +
						dr["Furnace8"].ToString() + " " +
						" );";
					if (!_repository.ModifyOnLinkDB(sql))
					{
						_logger.WriteEntry("1_LinkServer: INSERT INTO [PU10_PCS].[dbo].[T_EAFsPower] ", LogLevels.Error);
					}
				}
				return true;
			}
			catch (Exception excep)
			{
				_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
			}
			return false;
		}
	}
}
