using Irisa.Logger;
using Irisa.Common.Utils;
using System;
using System.Data;

namespace DCP
{
    internal class PCSInterface
    {
        // This factor is used to convert energy or power to appropriate value for PCS
        const int CORRECT_FACTOR = 1000;

        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        public PCSInterface(ILogger logger, IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
        }

        // Sends the EEC Telegram to PCS system,
        // When one telegram of EEC is received, this sub will be called in the CTCPServer by an event.
        // Telegram Structure is: Date ; Time ; RESTIME ; RESENERGY ; OVERL1 ; OVERL2 ; RESPOWER ;
        public bool SendEECTelegram(EECTelegram aEECTelegram)
        {
            bool result = false;
            try
            {
                object strDate = null;
                string strTime = "";
                string strResTime = "";
                string strResEnergy = "";
                string strPOverload1 = "";
                string strPOverload2 = "";
                string strResPower = "";
                string strSQL = "";

                string strSCADAErrorPath = "";

                // -------------------------------------------------------------------------
                // 1.
                _logger.WriteEntry("Sending EEC Telegram for PCS", LogLevels.Info);
                result = false;
                //Call theCTraceLogger.WriteLog(TraceInfo1, "PCSInterface..SendEECTelegram()", " Sending this telegram for PCS:" & Str(aEECTelegram.m_TelegramID))

                // -------------------------------------------------------------------------
                // 2.
                // Check Machine State, if this is StandBy, Exit CyclicActivation
                // TODO :
                ///if (!GeneralModule.IsMachineMaster())
                //{
                //	_logger.WriteEntry("Exit of sendEECTelegramToDC, System mode is: StandBy", LogLevels.Info);
                //	return true;
                //}

                // -------------------------------------------------------------------------					
                // 4. Put in the Database, T_EECTelegram Table
                strSQL = "Insert Into dbo.T_EECTelegram (TELDATETIME, RESIDUALTIME," +
                         " RESIDUALENERGY, OVERLOAD1, OVERLOAD2, RESIDUALENERGYEND) " +
                         "VALUES('" +
                         aEECTelegram.m_Date.ToString() + "', '" +
                         aEECTelegram.m_ResidualTime.Trim() + "', " +
                         (Double.Parse(aEECTelegram.m_ResidualEnergy) * CORRECT_FACTOR).ToString().Trim() + ", " +
                         (Double.Parse(aEECTelegram.m_OverLoad1) * CORRECT_FACTOR).ToString().Trim() + ", " +
                         (Double.Parse(aEECTelegram.m_OverLoad2) * CORRECT_FACTOR).ToString().Trim() + ", " +
                         (Double.Parse(aEECTelegram.m_ResidualEnergyEnd) * CORRECT_FACTOR).ToString().Trim() + ")";
                // -------------------------------------------------------------------------

                // -------------- Modification: Akbari, 2008-11-24 -------------------
                // Trace if we are loosing cycles
                _logger.WriteEntry("Time: " + aEECTelegram.m_Date, LogLevels.Info);

                // -------------------------------------------------------------------
                // 5.
                // On first Link Server

                //  //1401_08_23 Preparing Data For HMI
                try
                {
                    string strSQLHIS = "Insert Into APP_DCP_EECTelegram (\"TELDATETIME\", \"RESIDUALTIME[Min]\"," +
                         " \"RESIDUALENERGY[Mwh]\", \"POWERLIMIT1[MW]\", \"POWERLIMIT2[MW]\", \"RESIDUALENERGYEND[Mwh]\") " +
                         "VALUES('" +
                         aEECTelegram.m_Date.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                         aEECTelegram.m_ResidualTime.Trim() + "', " +
                         Math.Round((Double.Parse(aEECTelegram.m_ResidualEnergy)), 2).ToString().Trim() + ", " +
                         Math.Round((Double.Parse(aEECTelegram.m_OverLoad1)),2).ToString().Trim() + ", " +
                         Math.Round((Double.Parse(aEECTelegram.m_OverLoad2)), 2).ToString().Trim() + ", " +
                         Math.Round((Double.Parse(aEECTelegram.m_ResidualEnergyEnd)), 2).ToString().Trim() + ")";

                    if (!_repository.ModifyOnHistoricalDB(strSQLHIS))

                        _logger.WriteEntry("Archive: Could not insert into APP_DCP_EECTelegram Table in the HIS DB", LogLevels.Error);
                }
                catch (Exception excep)
                {
                    _logger.WriteEntry(excep.Message, LogLevels.Error, excep);


                }

                if (!_repository.InsertTELEGRAM(strSQL))
                {
                    SetAlarmForLinkServer("1_LinkServer: Could not insert into T_EECTelegram Table in the SQLServer.PU10_PCS DB");
                }
                else
                {
                    ClearAlarmForLinkServer("1_LinkServer: Send EECTelegram was accomlished successfully ");
                }
                result = true;

                

            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return result;
        }

        public bool SendEAFGroupTelegram(EAFGrpTelegram aEAFGroupTelegram)
        {
            bool result = false;
            object ADODB = null;
            try
            {
                object strDate = null;
                string strTime = "";
                string strResTime = "";
                string strResEnergy = "";
                string strPOverload1 = "";
                string strPOverload2 = "";
                string strResPower = "";
                string strSQL = "";

                string strSCADAErrorPath = "";
                result = false;

                _logger.WriteEntry("Sending EAFGroup telegram for PCS ", LogLevels.Info);

                // -------------------------------------------------------------------------
                // 2.
                // Check Machine State, if this is StandBy, Exit CyclicActivation
                // TODO: IsMachineMaster
                ///if (!GeneralModule.IsMachineMaster())
                //{
                //	_logger.WriteEntry("Exit of SendEAFGroupTelegram, System mode is: StandBy", LogLevels.Info);
                //	return true;
                //}

                // -------------------------------------------------------------------------
                // 4. Put in the Database, T_EECTelegram Table

                //*******MODIFICATION BY HEMATY:READ EAF GROUP NUMBER FROM TABLE (NOT FROM POWERCC)'
                //strSQL = "SELECT * FROM T_CVMABEAFSGRPNUM WHERE DATEFORMAT=(SELECT MAX(DATEFORMAT) FROM T_CVMABEAFSGRPNUM) AND ROWNUM=1";
                //var EAF1Group = "";
                //var EAF2Group = "";
                //var EAF3Group = "";
                //var EAF4Group = "";
                //var EAF5Group = "";
                //var EAF6Group = "";
                //var EAF7Group = "";
                //var EAF8Group = "";

                //var dtbVMABEAFSGRPNUM = _repository.getVMABEAFSGRPNUM();
                //foreach (DataRow dr in dtbVMABEAFSGRPNUM.Rows)
                //{
                //	EAF1Group = dr["EAF1Group"].ToString();
                //	EAF2Group = dr["EAF2Group"].ToString();
                //	EAF3Group = dr["EAF3Group"].ToString();
                //	EAF4Group = dr["EAF4Group"].ToString();
                //	EAF5Group = dr["EAF5Group"].ToString();
                //	EAF6Group = dr["EAF6Group"].ToString();
                //	EAF7Group = dr["EAF7Group"].ToString();
                //	EAF8Group = dr["EAF8Group"].ToString();
                //}


                //strSQL = "Insert Into T_EAFGroup (TelDateTime, EAF1Group, EAF2Group, EAF3Group, EAF4Group," & _
                //            '" EAF5Group, EAF6Group, EAF7Group, EAF8Group) " & _
                //            '"VALUES('" & _
                //            'Trim(aEAFGroupTelegram.TelDate) & "', " & _
                //            'aEAFGroupTelegram.EAF1Group & ", " & _
                //           'aEAFGroupTelegram.EAF2Group & ", " & _
                //            'aEAFGroupTelegram.EAF3Group & ", " & _
                //            'aEAFGroupTelegram.EAF4Group & ", " & _
                //            'aEAFGroupTelegram.EAF5Group & ", " & _
                //            'aEAFGroupTelegram.EAF6Group & ", " & _
                //            'aEAFGroupTelegram.EAF7Group & ", " & _
                //            'aEAFGroupTelegram.EAF8Group & ")"

                var EAF1Group = aEAFGroupTelegram.EAF1Group;
                var EAF2Group = aEAFGroupTelegram.EAF2Group;
                var EAF3Group = aEAFGroupTelegram.EAF3Group;
                var EAF4Group = aEAFGroupTelegram.EAF4Group;
                var EAF5Group = aEAFGroupTelegram.EAF5Group;
                var EAF6Group = aEAFGroupTelegram.EAF6Group;
                var EAF7Group = aEAFGroupTelegram.EAF7Group;
                var EAF8Group = aEAFGroupTelegram.EAF8Group;

                _logger.WriteEntry("EAF1Group = " + EAF1Group, LogLevels.Info);
                _logger.WriteEntry("EAF2Group = " + EAF2Group, LogLevels.Info);
                _logger.WriteEntry("EAF3Group = " + EAF3Group, LogLevels.Info);
                _logger.WriteEntry("EAF4Group = " + EAF4Group, LogLevels.Info);
                _logger.WriteEntry("EAF5Group = " + EAF5Group, LogLevels.Info);
                _logger.WriteEntry("EAF6Group = " + EAF6Group, LogLevels.Info);
                _logger.WriteEntry("EAF7Group = " + EAF7Group, LogLevels.Info);
                _logger.WriteEntry("EAF8Group = " + EAF8Group, LogLevels.Info);

                strSQL = "Insert Into T_EAFGroup (TelDateTime, EAF1Group, EAF2Group, EAF3Group, EAF4Group," +
                         " EAF5Group, EAF6Group, EAF7Group, EAF8Group) " +
                         "VALUES('" +
                         aEAFGroupTelegram.TelDate + "', " +
                         EAF1Group + ", " +
                         EAF2Group + ", " +
                         EAF3Group + ", " +
                         EAF4Group + ", " +
                         EAF5Group + ", " +
                         EAF6Group + ", " +
                         EAF7Group + ", " +
                         EAF8Group + ")";

                //**************END OF MODIFICATION'

                // -------------------------------------------------------------------------
                // 5.
                // On first Link Server
                //strSQL = "Update T_EAFGroupRequest Set ResponseDateTime = '" + String.Format("{0:d}", DateTime.Now) + "' Where RequestDateTime = (Select MAX(RequestDateTime) From T_EAFGROUPREQUEST)";
                if (!_repository.InsertTELEGRAM(strSQL)) // .updateEAFGroupRequest())
                {
                    SetAlarmForLinkServer("1_LinkServer: Could not insert into T_EAFGroup Table in the SQLServer.PU10_PCS DB");

                    return result;
                }
                else
                {
                    ClearAlarmForLinkServer("1_LinkServer: Send EAFGroupTelegram was accomlished successfully");
                }

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        // This function updates request for EAFGroupTelegram from PCS
        public bool UpdateEAFGroupRequestTelegram(string strReqDate)
        {
            bool result = false;
            try
            {
                string strSQL = "";
                string strSCADAErrorPath = "";

                result = false;

                // -------------------------------------------------------------------------
                // 1.

                // -------------------------------------------------------------------------
                // 2.

                // -------------------------------------------------------------------------
                // 3.
                // On first Link Server
                strSQL = "Update T_EAFGroupRequest Set ResponseDateTime = '" + DateTime.Now.ToString() + "' Where RequestDateTime = (Select MAX(RequestDateTime) From T_EAFGROUPREQUEST)";
                if (!_repository.UpdateEAFGroupRequest())
                {
                    SetAlarmForLinkServer("1_LinkServer: Could not update T_EAFGroupRequest Table in the SQLServer->PU10_PCS DB");

                    return result;
                }
                else
                {
                    ClearAlarmForLinkServer("1_LinkServer: Update EAFGroupRequest was accomlished successfully, ResponseDateTime = " + String.Format("{0:d}", DateTime.Now));
                }

                // On second Link Server
                strSQL = "Update T_EAFGroupRequest Set ResponseDateTime = '" + DateTime.Now.ToString() + "' Where RequestDateTime = (Select MAX(RequestDateTime) From T_EAFGROUPREQUEST)";

                // -------------------------------------------------------------------------
                // 4.
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        // Sends the LSP Telegram to PCS system,
        // Telegram Structure is: Date ; Time ; EAFGroupNo ; PMax
        public bool SendLSPTelegram(byte aEAFGroupNo, float aPMax)
        {
            bool result = false;
            try
            {
                string strDate;
                string strSQL = "";

                result = false;

                // -------------------------------------------------------------------------
                // 1.
                _logger.WriteEntry("Sending LSP telegram for PCS: " + DateTime.Now.ToString(), LogLevels.Info);

                // -------------------------------------------------------------------------
                // 2.
                // Check Machine State, if this is StandBy, Exit CyclicActivation
                //If Not IsMachineMaster() Then
                //    Call theCTraceLogger.WriteLog(TraceInfo1, "PCSInterface..SendLSPTelegram", "Exit of sendEECTelegramToDC, System mode is: StandBy")
                //    Exit Function
                //End If

                //1401.03.24 IranTime
                strDate = DateTime.UtcNow.ToIranDateTime().ToString();

                // -------------------------------------------------------------------------
                // 3. Put in the Database, T_LSPTelegram Table
                //strSQL = "Insert Into T_LSPTELEGRAM (TELDATETIME, EAFGROUPNO, PMAX, POWERREDCMD)" &
                //        "VALUES('" &
                //        strDate & "', '" &
                //        Str(aEAFGroupNo) & "', '" &
                //       Str(aPMax) & "', 1)"

                //*******MODIFICATION BY HEMATY:READ EAF GROUP NUMBER FROM TABLE (NOT FROM POWERCC)'
                //strSQL = "SELECT * FROM T_CVMABEAFSGRPNUM WHERE DATEFORMAT=(SELECT MAX(DATEFORMAT) FROM T_CVMABEAFSGRPNUM) AND ROWNUM=1";
                //var EAF1Group = "";
                //var EAF2Group = "";
                //var EAF3Group = "";
                //var EAF4Group = "";
                //var EAF5Group = "";
                //var EAF6Group = "";
                //var EAF7Group = "";
                //var EAF8Group = "";

                //var dtbVMABEAFSGRPNUM = _repository.getVMABEAFSGRPNUM();
                //foreach (DataRow dr in dtbVMABEAFSGRPNUM.Rows)
                //{
                //	EAF1Group = dr["EAF1Group"].ToString();
                //	EAF2Group = dr["EAF2Group"].ToString();
                //	EAF3Group = dr["EAF3Group"].ToString();
                //	EAF4Group = dr["EAF4Group"].ToString();
                //	EAF5Group = dr["EAF5Group"].ToString();
                //	EAF6Group = dr["EAF6Group"].ToString();
                //	EAF7Group = dr["EAF7Group"].ToString();
                //	EAF8Group = dr["EAF8Group"].ToString();
                //}

                var EAF1Group = _repository.GetScadaPoint("EECGRPEAF1");
                var EAF2Group = _repository.GetScadaPoint("EECGRPEAF2");
                var EAF3Group = _repository.GetScadaPoint("EECGRPEAF3");
                var EAF4Group = _repository.GetScadaPoint("EECGRPEAF4");
                var EAF5Group = _repository.GetScadaPoint("EECGRPEAF5");
                var EAF6Group = _repository.GetScadaPoint("EECGRPEAF6");
                var EAF7Group = _repository.GetScadaPoint("EECGRPEAF7");
                var EAF8Group = _repository.GetScadaPoint("EECGRPEAF8");

                _logger.WriteEntry("EAF1Group = " + EAF1Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF2Group = " + EAF2Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF3Group = " + EAF3Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF4Group = " + EAF4Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF5Group = " + EAF5Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF6Group = " + EAF6Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF7Group = " + EAF7Group.Value, LogLevels.Info);
                _logger.WriteEntry("EAF8Group = " + EAF8Group.Value, LogLevels.Info);

                // TODO: check
                strSQL = "Insert Into T_LSPTELEGRAM (TELDATETIME, EAFGROUPNO, PMAX, POWERREDCMD, " +
                         "EAF1Group, EAF2Group, EAF3Group, EAF4Group," +
                         " EAF5Group, EAF6Group, EAF7Group, EAF8Group) " +
                         "VALUES('" +
                         strDate + "', '" +
                         aEAFGroupNo + "', '" +
                         (aPMax * CORRECT_FACTOR) + "', 1, " +
                         EAF1Group.Value + ", " +
                         EAF2Group.Value + ", " +
                         EAF3Group.Value + ", " +
                         EAF4Group.Value + ", " +
                         EAF5Group.Value + ", " +
                         EAF6Group.Value + ", " +
                         EAF7Group.Value + ", " +
                         EAF8Group.Value + ")";

                //***********END OF MODIFICATION'

                //_logger.WriteEntry(strSQL, LogLevels.Info);

                // -------------------------------------------------------------------------
                // On first Link Server
                if (!_repository.InsertTELEGRAM(strSQL))
                    SetAlarmForLinkServer("1_LinkServer: Could not insert into T_LSPTELEGRAM Table in the SQLServer->PU10_PCS DB");
                else
                    ClearAlarmForLinkServer("1_LinkServer: Insert into LSPTelegram was accomlished successfully");

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        // This function checks is there any request for EAFGroupTelegram from PCS, If yes returns True
        public bool IsNewRequestEAFGroup(ref string strReqDate)
        {
            bool result = false;
            try
            {
                string strSQL = "";

                // -------------------------------------------------------------------------
                // 1. Check the SQL connection

                // --------------------------------
                // For first LinkServer
                // -------------------------------------------------------------------------
                // 2. Retreiving the last requsted record
                var dtbEAFGroupRequest = _repository.GetEAFGROUPREQUEST();
                if (dtbEAFGroupRequest != null)
                {
                    if (dtbEAFGroupRequest.Rows.Count == 0)
                        SetAlarmForLinkServer("1_LinkServer: Could not read from T_EAFGroupRequest Table in the SQLServer->PU10_PCS DB");
                    else
                    {
                        DataRow dr = dtbEAFGroupRequest.Rows[0];

                        ClearAlarmForLinkServer("1_LinkServer: Request Time = " + dr["RequestDateTime"].ToString());

                        // -------------------------------------------------------------------------
                        // 3. Processing the last requsted record
                        if (!string.IsNullOrEmpty(dr["ResponseDateTime"].ToString()))
                        {
                            var RespDate = DateTime.Parse(dr["ResponseDateTime"].ToString());

                            if (RespDate.Year < 2000)
                            {
                                result = true;
                                //_logger.WriteEntry("1_LinkServer: Request Time = " + dr["RequestDateTime"].ToString(), LogLevels.Info);
                            }
                        }
                        else
                            result = true;
                    }
                }

                // -------------------------------------------------------------------------
                // For second LinkServer
                // -------------------------------------------------------------------------
                // 2. Retreiving the last requsted record
                // TODO: check with original code, this part is commented.
                //strSQL = "Select * From T_EAFGROUPREQUEST Where RequestDateTime = (Select MAX(RequestDateTime) From T_EAFGROUPREQUEST)";
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return result;
        }

        //
        public bool SendRPCTelegram(RPCTelegram aRPCTelegram)
        {
            return false;
        }

        private bool SetAlarmForLinkServer(string message)
        {
            try
            {
                // TODO: check 
                _logger.WriteEntry(message, LogLevels.Error);

                string strpatherrorlink = "{FA6F5906-6237-4099-A912-A13A65EFA548}";
                var linkServerError = _repository.GetScadaPoint(Guid.Parse(strpatherrorlink));
                if (!_updateScadaPointOnServer.WriteSCADAPoint(linkServerError, (float)SinglePointStatus.Appear))
                {
                    _logger.WriteEntry("The value could not update in the SCADA", LogLevels.Error);
                }

                // Send Alarm
                linkServerError = _repository.GetScadaPoint("LINKSERVERERROR1");
                if (!_updateScadaPointOnServer.SendAlarm(linkServerError, SinglePointStatus.Appear, message))
                {
                    _logger.WriteEntry("1_LinkServer: Sending alarm was failed.", LogLevels.Error);
                }
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }
        private bool ClearAlarmForLinkServer(string message)
        {
            try
            {
                //_logger.WriteEntry(message, LogLevels.Info);

                // Send Alarm
                var linkServerError = _repository.GetScadaPoint("LINKSERVERERROR1");
                if (!_updateScadaPointOnServer.SendAlarm(linkServerError, SinglePointStatus.Disappear, message))
                {
                    _logger.WriteEntry("1_LinkServer: Sending alarm was failed.", LogLevels.Error);
                }
                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }

        public bool CheckPCSLink()
        {
            if (_repository.GetNRecord("SELECT * FROM T_EAFsPower") > 1 || _repository.GetNRecord("SELECT * FROM T_EECTelegram") > 1 )
                return false;
            else
                return true;
        }
    }
}