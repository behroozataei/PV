using COMMON;
using Irisa.Logger;
using Irisa.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace LSP
{
    class LSPSFSCManager
    {
        private readonly ILogger _logger;
        private readonly RedisUtils _RTDBManager;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly ChangeControlStateOnServer _changeControlStateOnServer;
        private readonly IRepository _repository;
        private List<CPriorityList> _priorityList;

        private const int TIMER_1_TICKS = 60000;
        private const int TIMER_SHEDFURNACE_TICKS = 5000;
        private const int TIMER_UPDATESHEDLIST_TICKS = 1000;

        private readonly Timer _timer_1_Minute;
        private readonly Timer _timer_ShedingFurnace;
        private readonly Timer _timer_UpdateShedList;

        private bool isWorking_Shedding_To_SafeConsumption = false;
        private bool isWorking_CheckLSPActivationFromSFSC = false;
        SFSC_FURNACE_TO_SHED_Str _sfsc_furnace_to_shed = null;
        Stopwatch _stopwatch1, _stopwatch2;

        internal LSPSFSCManager(ILogger logger, IRepository repository, ICpsCommandService scadaCommand, List<CPriorityList> priorityList, RedisUtils RTDBManager)
        {
            try
            {
                _repository = repository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _RTDBManager = RTDBManager ?? throw new ArgumentNullException(nameof(RTDBManager));
                _updateScadaPointOnServer = new UpdateScadaPointOnServer(logger, scadaCommand);
                _changeControlStateOnServer = new ChangeControlStateOnServer(logger, scadaCommand);
                _priorityList = priorityList;
                _sfsc_furnace_to_shed = new SFSC_FURNACE_TO_SHED_Str();
                
                //------------------------------------------------------------
                // Clearing pending shed commands
                ClearSFSCTrigger(_sfsc_furnace_to_shed);
                
                _stopwatch1 = new Stopwatch();
                _stopwatch2 = new Stopwatch();
                _timer_UpdateShedList = new Timer();
                _timer_UpdateShedList.Interval = TIMER_UPDATESHEDLIST_TICKS;
                _timer_UpdateShedList.Elapsed += CheckLSPActivationFromSFSC;
                _timer_UpdateShedList.Start();
                _stopwatch1.Start();
                _stopwatch2.Start();

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
        }


        // TODO: It should be called in Timer2!!!!, maybe it is in SFSC 
        // R.Hemmaty
        private void Shedding_To_SafeConsumption(object sender, ElapsedEventArgs e)
        {
            //try
            //{
            //	if (isWorking_Shedding_To_SafeConsumption)
            //		return;
            //	else
            //		isWorking_Shedding_To_SafeConsumption = true;

            //	var scadapointAlarm_LSPACTIVATED = _repository.GetLSPScadaPoint("LSPACTIVATED");
            //	if (scadapointAlarm_LSPACTIVATED is null)
            //	{
            //		_logger.WriteEntry("Error in finding 'LSPACTIVATED' in repository.", LogLevels.Error);
            //		return;
            //	}

            //	var sql = "SELECT F_Key, TelDateTime, Sumation FROM dbo.T_EAFsPower WHERE F_Key=(SELECT MAX(F_Key) FROM [PU10_PCS].[dbo].[T_EAFsPower])";
            //	DataTable datatableEAFSPower = _repository.getFromLinkDB(sql);
            //	if ((datatableEAFSPower is null) || (datatableEAFSPower.Rows.Count == 0))
            //	{
            //		_logger.WriteEntry("Error in running 'SELECT F_Key, TelDateTime, Sumation FROM dbo.T_EAFsPower '.", LogLevels.Error);
            //	}
            //	float sumOverloadEAFSPower = Convert.ToSingle(datatableEAFSPower.Rows[0][0]) / 1000;

            //	sql = "SELECT ([OverLoad1] + [OverLoad2])/1000 FROM dbo.T_EECTelegram WHERE TelDateTime=(SELECT MAX(TELDATETIME) FROM dbo.T_EECTelegram)";
            //	DataTable datatbleEECTelegram = _repository.getFromLinkDB(sql);
            //	if ((datatbleEECTelegram is null) || (datatbleEECTelegram.Rows.Count == 0))
            //	{
            //		_logger.WriteEntry("Error in running 'SELECT ([OverLoad1] + [OverLoad2])/1000 FROM dbo.T_EECTelegram '.", LogLevels.Error);
            //	}
            //	float sumOverloadEEC = Convert.ToSingle(datatbleEECTelegram.Rows[0][0]);
            //	//_logger.WriteEntry("Limited Power For EAFS Is : " + (sumOverloadEEC + 10) * 1.02d, LogLevels.Info);

            //	if (sumOverloadEAFSPower > ((sumOverloadEEC + 10) * 1.02d))
            //	{
            //		//Create Alarm
            //		if (!_updateScadaPointOnServer.WriteDigital(scadapointAlarm_LSPACTIVATED, EventStatus.Appear, "Warning For PCS Exceed Its Power Limit"))
            //		{
            //			_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
            //		}

            //		_logger.WriteEntry("Warning Alarm Was Sent.", LogLevels.Info);

            //		System.Threading.Thread.Sleep(30000);

            //		sql = "SELECT F_Key, TelDateTime, Sumation FROM dbo.T_EAFsPower WHERE F_Key=(SELECT MAX(F_Key) FROM dbo.T_EAFsPower)";
            //		DataTable datatableEAFSPowerNew = _repository.getFromLinkDB(sql);
            //		if ((datatableEAFSPowerNew is null) || (datatableEAFSPowerNew.Rows.Count == 0))
            //		{
            //			_logger.WriteEntry("Error in running 'SELECT F_Key, TelDateTime, Sumation FROM dbo.T_EAFsPower '.", LogLevels.Error);
            //		}
            //		float sumOverloadEAFSPowerNew = Convert.ToSingle(datatableEAFSPowerNew.Rows[0][0])/1000;

            //		if (sumOverloadEAFSPowerNew > ((sumOverloadEEC + 10) * 1.02d))
            //		{
            //			_logger.WriteEntry("... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ...", LogLevels.Info);
            //			_logger.WriteEntry("PCS Exceeds Its Limit", LogLevels.Info);
            //			_logger.WriteEntry("PCS Limit = " + ((sumOverloadEEC + 10) * 1.02d), LogLevels.Info);
            //			_logger.WriteEntry("Current PCS Power = " + sumOverloadEAFSPower, LogLevels.Info);

            //			var priol = _priorityList.Find(priol => priol._priorityNo == Constants.PRIORITYLISTNO_EAF);

            //			_logger.WriteEntry("Selected Furnace To Break='" + priol.GetArrBreakers(1).FurnaceIndex + "'", LogLevels.Info);

            //			if (!SendCommandToSCADA(priol.GetArrBreakers(1).guid_item, priol.GetArrBreakers(1).NetworkPath_Item))
            //			{
            //				_logger.WriteEntry("Error in Sending Open command for :" + priol.GetArrBreakers(1).NetworkPath_Item, LogLevels.Error);
            //			}

            //			if (priol.GetArrBreakers(1).HasPartner == "YES")
            //			{
            //				if( !SendCommandToSCADA(priol.GetArrBreakers(1).addressPartner_guid, priol.GetArrBreakers(1).AddressPartner))
            //				{
            //					_logger.WriteEntry("Error in Sending Open command for partner :" + priol.GetArrBreakers(1).AddressPartner, LogLevels.Error);
            //				}
            //			}

            //			if (!_updateScadaPointOnServer.WriteDigital(scadapointAlarm_LSPACTIVATED, EventStatus.Appear, "Shedding The Furnace For PCS Exceed Its Power Limit That Is " + ((sumOverloadEEC + 10) * 1.02d)))
            //			{
            //				_logger.WriteEntry("Sending alarm was failed.", LogLevels.Error);
            //			}
            //		}
            //	}
            //	isWorking_Shedding_To_SafeConsumption = false;
            //}
            //catch (System.Exception excep)
            //{
            //	_logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            //	isWorking_Shedding_To_SafeConsumption = false;
            //}
        }

        //' R.Hemmaty
        //' Timer1 : 60000 msecond
        //Private Sub Timer1_Timer()
        //'    Call m_CLSPManager.Update_m_arrPriol_IdxPriolsEAF
        //End Sub
        public void CheckCPSStatus()
        {

            while (!GlobalData.CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
                _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);

            }
        }
        private void CheckLSPActivationFromSFSC(object sender, EventArgs e)
        {
            try
            {
                CheckCPSStatus();

                if (isWorking_CheckLSPActivationFromSFSC)
                {
                    _logger.WriteEntry("Warning: CheckLSPActivationFromSFSC is busy!", LogLevels.Warn);
                    return;
                }
                else
                    isWorking_CheckLSPActivationFromSFSC = true;

                _stopwatch2.Stop();
                if (_stopwatch2.Elapsed.TotalMilliseconds > TIMER_UPDATESHEDLIST_TICKS+100)
                    _logger.WriteEntry($"Warning: CheckLSPActivationFromSFSC Processing take {_stopwatch2.Elapsed.TotalMilliseconds} ms!", LogLevels.Warn);
                _stopwatch2.Restart();

                // TODO: for SFSC
                var scadapointWarning = _repository.GetLSPScadaPoint("SFSC/STATUS/Sent Warning");
                if (scadapointWarning is null)
                {
                    _logger.WriteEntry("Error in finding SFSC/STATUS/Sent Warning", LogLevels.Error);
                }

                //var key_shed = _repository.GetRedisUtiles().GetKeys(RedisKeyPattern.SFSC_FURNACE_TO_SHED);

                //if (key_shed.Length == 0)
                //{
                //    _logger.WriteEntry("no record is available in SFSCSELECTEDFURNACETOSHED!, RedisKey length = 0", LogLevels.Warn);
                //    isWorking_CheckLSPActivationFromSFSC = false;
                //    return;
                //}

                // Exit of method, because there is no Furnace to SHED 
               



                _stopwatch1.Restart();
                if (!_RTDBManager.StringGet(RedisKeyPattern.SFSC_FURNACE_TO_SHED, ref _sfsc_furnace_to_shed))
                {
                    _logger.WriteEntry("can not read SFSCSELECTEDFURNACETOSHED record from Redis!", LogLevels.Warn);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }
                _stopwatch1.Stop();
                if(_stopwatch1.ElapsedMilliseconds>1500)
                    _logger.WriteEntry($"Get Data From Redis Take {_stopwatch1.ElapsedMilliseconds} ms!", LogLevels.Warn);


                //var SFSC_ATIVATED = _repository.GetLSPScadaPoint("SFSCACTIVATED");
                //if (SFSC_ATIVATED is null)
                //{
                //    _logger.WriteEntry("Error in finding SFSCACTIVATED", LogLevels.Error);
                //    isWorking_CheckLSPActivationFromSFSC = false;
                //    return;

                //}
                //if (SFSC_ATIVATED.Value == 0.0f)
                //{
                //    isWorking_CheckLSPActivationFromSFSC = false;
                //    return;
                //}

                if (_sfsc_furnace_to_shed == null)
                {
                    _logger.WriteEntry("no record is available in SFSCSELECTEDFURNACETOSHED!, Redis Key not found", LogLevels.Warn);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }

                if (_sfsc_furnace_to_shed.SHEADCOMMAND == false)
                {
                    //_logger.WriteEntry("SFSC not Sent any Shedding Command", LogLevels.Info);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }

                var SFSC_STATUS = _repository.GetLSPScadaPoint("SFSC_STATUS");
                if (SFSC_STATUS is null)
                {
                    _logger.WriteEntry("Error in finding SFSC_STATUS", LogLevels.Error);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }

                if (SFSC_STATUS.Value == 0.0)
                {
                    _logger.WriteEntry("LSP_SFSC Triggered, LSP_SFSC Function is OFF!", LogLevels.Error);
                    ClearSFSCTrigger(_sfsc_furnace_to_shed);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }

                _logger.WriteEntry(". .. ... .... ..... ...... SFSC is ACTIVATED ...... ..... .... ... .. . . .. ... .... ..... ...... ", LogLevels.Warn);

                var scadapointLSPACTIVATED = _repository.GetLSPScadaPoint("LSPACTIVATED");
                if (scadapointLSPACTIVATED is null)
                {
                    _logger.WriteEntry("Error in finding LSPACTIVATED", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.SendAlarm(scadapointLSPACTIVATED, SinglePointStatus.Disappear, " "))
                {
                    _logger.WriteEntry("Error in diappearing LSPACTIVATED ", LogLevels.Error);
                }
                if (!_updateScadaPointOnServer.SendAlarm(scadapointLSPACTIVATED, SinglePointStatus.Appear, "LSP is activated by SFSC for furnace " + _sfsc_furnace_to_shed.FURNACE + " with PowerGroup : " + _sfsc_furnace_to_shed.GROUPPOWER))
                {
                    _logger.WriteEntry("Error in send Alarm for LSPACTIVATED, " +
                        "Activated by SFSC for Furnace " +
                        _sfsc_furnace_to_shed.FURNACE +
                        " with PowerGroup : " +
                        _sfsc_furnace_to_shed.GROUPPOWER, LogLevels.Error);
                }

                _logger.WriteEntry("SELECTED FURNACE IS:" + _sfsc_furnace_to_shed.FURNACE, LogLevels.Info);

                var keys = _RTDBManager.GetKeys(pattern: RedisKeyPattern.EEC_EAFSPriority);
                if (keys.Length == 0)
                {
                    _logger.WriteEntry("Error in running get furnce number from cache", LogLevels.Error);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }

                var datatable = _RTDBManager.StringGet<EEC_EAFSPRIORITY_Str>(keys);
                EEC_EAFSPRIORITY_Str dr_EEC_EAFSPriority = datatable.Where(n => n.FURNACE == _sfsc_furnace_to_shed.FURNACE).First();

                if (dr_EEC_EAFSPriority is null)
                {
                    _logger.WriteEntry("Error in running get furnace number from cache", LogLevels.Error);
                    isWorking_CheckLSPActivationFromSFSC = false;
                    return;
                }


                _logger.WriteEntry("CB ADDRESS IS for sheding Furnace is: " + dr_EEC_EAFSPriority.CB_NETWORKPATH, LogLevels.Info);
                var guid = Guid.Parse(dr_EEC_EAFSPriority.ID_CB.ToString());

                var cbToShed = _repository.GetLSPScadaPoint(guid);
                if (cbToShed != null)
                {
                    if ((DigitalDoubleStatus)cbToShed.Value == DigitalDoubleStatus.Close)
                    {
                        if (!SendCommandToSCADA(guid, dr_EEC_EAFSPriority.CB_NETWORKPATH.ToString()))
                        {
                            _logger.WriteEntry("Error in SendCommandToSCADA for " + dr_EEC_EAFSPriority.CB_NETWORKPATH.ToString(), LogLevels.Error);

                        }

                    }
                    else
                    {
                        _logger.WriteEntry("CB Status is OPEN: " + dr_EEC_EAFSPriority.CB_NETWORKPATH.ToString(), LogLevels.Info);
                    }
                }
                else
                {
                    _logger.WriteEntry("Error to find CB ADDRESS in Repository: " + dr_EEC_EAFSPriority.CB_NETWORKPATH.ToString(), LogLevels.Error);
                }


                if (dr_EEC_EAFSPriority.HASPARTNER.ToString() == "YES")
                {
                    _logger.WriteEntry("Sending Shed command for Partner CB ADDRESS :" + dr_EEC_EAFSPriority.PARTNERADDRESS.ToString(), LogLevels.Info);
                    var guidPartner = Guid.Parse(dr_EEC_EAFSPriority.ID_CB_PARTNER.ToString());

                    var cbPartnerToShed = _repository.GetLSPScadaPoint(guidPartner);
                    if (cbPartnerToShed != null)
                    {
                        if ((DigitalDoubleStatus)cbPartnerToShed.Value == DigitalDoubleStatus.Close)
                        {
                            if (!SendCommandToSCADA(guidPartner, dr_EEC_EAFSPriority.PARTNERADDRESS.ToString()))
                            {
                                _logger.WriteEntry("Error in SendCommandToSCADA for Partner " + dr_EEC_EAFSPriority.PARTNERADDRESS.ToString(), LogLevels.Error);

                            }
                        }
                        else
                        {
                            _logger.WriteEntry("CB Status is OPEN: " + dr_EEC_EAFSPriority.PARTNERADDRESS.ToString(), LogLevels.Info);
                        }
                    }
                    else
                    {
                        _logger.WriteEntry("Error to find CB ADDRESS in Repository: " + dr_EEC_EAFSPriority.PARTNERADDRESS.ToString(), LogLevels.Error);
                    }
                }
                _sfsc_furnace_to_shed.SHEADTIME = DateTime.UtcNow;
                ClearSFSCTrigger(_sfsc_furnace_to_shed);               
                isWorking_CheckLSPActivationFromSFSC = false;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                isWorking_CheckLSPActivationFromSFSC = false;
                ClearSFSCTrigger(_sfsc_furnace_to_shed);

            }
        }

        private void ClearSFSCTrigger(SFSC_FURNACE_TO_SHED_Str sfsc_furnace_to_shed)
        {
            try
            {


                _logger.WriteEntry("Clear SFSC Trigger", LogLevels.Info);
                var scadapointLSPACTIVATED = _repository.GetLSPScadaPoint("LSPACTIVATED");
                if (scadapointLSPACTIVATED is null)
                {
                    _logger.WriteEntry("Error in finding LSPACTIVATED", LogLevels.Error);
                }
                if ((SinglePointStatus)(int)scadapointLSPACTIVATED.Value == SinglePointStatus.Appear)
                    if (!_updateScadaPointOnServer.SendAlarm(scadapointLSPACTIVATED, SinglePointStatus.Disappear, " "))
                    {
                        _logger.WriteEntry("Error in diappearing LSPACTIVATED ", LogLevels.Error);
                    }

                sfsc_furnace_to_shed.SHEADCOMMAND = false;
                _RTDBManager.RedisConn.Set(RedisKeyPattern.SFSC_FURNACE_TO_SHED, JsonConvert.SerializeObject(sfsc_furnace_to_shed));
                //_updateScadaPointOnServer.SendAlarm(_repository.GetLSPScadaPoint("SFSCACTIVATED"), SinglePointStatus.Disappear, "");
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }

        }

        private bool SendCommandToSCADA(Guid guid, string path)
        {
            try
            {
                var scadapointCB = _repository.GetLSPScadaPoint(guid);
                if (scadapointCB is null)
                {
                    _logger.WriteEntry("Error in finding Breaker : " + path + " ; " + guid, LogLevels.Error);
                    return false;
                }

                if (!_updateScadaPointOnServer.SendCommandSFSC(scadapointCB, (int)Breaker_Status.BOpen))
                {
                    _logger.WriteEntry("SFSC-Sending OPEN command is failed for Breaker : " + scadapointCB.NetworkPath, LogLevels.Error);
                    // Send Alarm
                    return false;
                }
                else
                {
                    _logger.WriteEntry("SFSC-Sending OPEN command was accomlished for Breaker : " + scadapointCB.NetworkPath, LogLevels.Info);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }
    }
}
