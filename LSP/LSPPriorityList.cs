using System;
using System.Data;
using System.Linq;

using Irisa.Common;
using Irisa.Logger;

namespace LSP
{
    internal class CPriorityList
    {
        //==============================================================================
        //MEMBER VARIABLES
        //==============================================================================

        // The maximum number of breakers in one priority list
        const int MAXCBINPRIORITYLIST = 100;

        // The number of priority list
        public byte _priorityNo { get; set; }

        // Value of SumIt received in job from LSP, It on primary side of trans/line
        public float _sumIt { get; set; }

        // Shed value
        public float _shedValue { get; set; }

        // Shed type
        public eShedType _shedType { get; set; } = eShedType.None;

        // The number of breakers in this priority list
        public byte _nBreakers { get; set; }

        public string _description1 { get; set; }

        // List of all breakers to shed, in this priotiy list
        public LSPBreakerToShed[] _breakersToShed { get; set; } = new LSPBreakerToShed[MAXCBINPRIORITYLIST + 1];

        //==============================================================================
        //MEMBER FUNCTIONS
        //==============================================================================
        
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private bool isCompleted = true;

        private readonly IRepository _repository;

        public CPriorityList(IRepository repository, ILogger logger)
        {
            try
            {
                _priorityNo = 0;
                _sumIt = 0;
                _nBreakers = 0;
                _shedType = eShedType.None;
                _description1 = "";

                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                for (int I = 0; I <= MAXCBINPRIORITYLIST; I++)
                {
                    _breakersToShed[I] = null;
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

        }

        public bool ResetPriolData()
        {
            bool result = false;
            try
            {
                int I = 0;

                result = false;

                _sumIt = 0;
                _shedValue = 0;

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        public bool LoadBreakersToShed()
        {
            int I;
            //DataTable dtbMeasurements = new DataTable();

            try
            {
                var dtbMeasurements = _repository.FetchBreakersToShed(_priorityNo);
                var rows = dtbMeasurements.Where(n => n.PRIORITYLISTNO == _priorityNo)
                                                                .OrderBy(n => n.ITEMNO).ToArray();

                // -------------------------------------------------------------------------
                // Read all detail info about decision tables from three tables
                _logger.WriteEntry($"-----------------------   Load Items for Priority List {_priorityNo}  ------------------- ", LogLevels.Info);
                I = 1;
                foreach (var dr in rows)
                {
                    if (_nBreakers > 0)
                    {
                        _breakersToShed[I] = new LSPBreakerToShed(_logger);
                        _breakersToShed[I].BreakerNo = Convert.ToByte(dr.ITEMNO);
                        _breakersToShed[I].NetworkPath_Cur = dr.NETWORKPATH_CURR;
                        _breakersToShed[I].NetworkPath_Item = dr.NETWORKPATH_ITEM;
                        _breakersToShed[I].HasPartner = dr.HASPARTNER;
                        _breakersToShed[I].AddressPartner = dr.ADDRESSPARTNER;
                        _breakersToShed[I].guid_curr = Guid.Parse( dr.ID_CURR.ToString());
                        _breakersToShed[I].guid_item = Guid.Parse(dr.ID_CB.ToString());
                        // TODO: Check 
                        _breakersToShed[I].FurnaceIndex = "";  // dr["Furnace"].ToString();
                        
                        if(_breakersToShed[I].AddressPartner!="NULL")
                            _breakersToShed[I].addressPartner_guid = Guid.Parse(dr.ID_CB_PARTNER.ToString());

                        _logger.WriteEntry("  BreakerNo = " + _breakersToShed[I].BreakerNo.ToString() +
                            "; NetworkPath_Cur = " + _breakersToShed[I].NetworkPath_Cur +
                            "; NetworkPath_Item = " + _breakersToShed[I].NetworkPath_Item +
                            "; HasPartner=" + _breakersToShed[I].HasPartner +
                            "; AddressPartner=" + _breakersToShed[I].AddressPartner, LogLevels.Info);
                        I++;
                    }
                }
                return true;
            }
            catch (Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
                return false;
            }
        }

        public bool InitializeIt()
        {
            bool result = false;
            try
            {
                result = false;

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        // Read all status and currents of Shed Brekaers for this Priority List
        public bool ReadPriorityItems()
        {
            bool result = false;
            try
            {
                result = false;

                byte tempForEndVar = _nBreakers;
                for (byte idxBreaker = 1; idxBreaker <= tempForEndVar; idxBreaker = (byte)(idxBreaker + 1))
                {
                    // ---------------------------------------------------------------------------------
                    // Reading status value
                    Guid id;
                    if (Guid.TryParse(_breakersToShed[idxBreaker].guid_item.ToString(), out id))
                    {
                        var guid_cb = _breakersToShed[idxBreaker].guid_item;

                        // TODO :
                        var cb_point = _repository.GetLSPScadaPoint(guid_cb);
                        if( !(cb_point is null))
                        {
                            //_breakersToShed[idxBreaker].Status = (Breaker_Status)cb_point.Value;
                            if (cb_point.Quality == QualityCodes.None)
                                _breakersToShed[idxBreaker].StatusQuality = true;
                            else
                                _breakersToShed[idxBreaker].StatusQuality = false;

                            // ---------------------------------------------------------------------------------
                            // Reading current value
                            var guid_curr = _breakersToShed[idxBreaker].guid_curr;

                            var curr_point = _repository.GetLSPScadaPoint(guid_curr);
                            if(!(curr_point is null))
                            {
                                // TODO :
                                //_breakersToShed[idxBreaker].Current = curr_point.Value;
                                if (curr_point.Quality != QualityCodes.None)
                                {
                                    _logger.WriteEntry("The value is not valid for current of : " + cb_point.NetworkPath, LogLevels.Error);
                                    // TODO :
                                    //_breakersToShed[idxBreaker].Current = 0;
                                }
                            }
                            else
                            {
                                _logger.WriteEntry("Current of BreakerToShed is not valid for : " + cb_point.NetworkPath + " ; GUID = " + guid_cb.ToString(), LogLevels.Error);
                            }

                            // Logging the read values
                            // TODO :
                            //_logger.WriteEntry("BreakerToShed : " + cb_point.NetworkPath + " ; Status=" + ((int)_breakersToShed[idxBreaker].Status).ToString() + " ; Current=" + _breakersToShed[idxBreaker].Current.ToString(), LogLevels.Info);
                        }
                        else
                        {
                            _logger.WriteEntry("Error Breaker of BreakerToShed is not valid for : " + _breakersToShed[idxBreaker].NetworkPath_Item + " ; GUID = " + guid_cb.ToString(), LogLevels.Error);
                        }
                    }
                    else
                    {
                        _logger.WriteEntry("Error GUID " + _breakersToShed[idxBreaker].guid_item + " ITEM " + _breakersToShed[idxBreaker].NetworkPath_Item, LogLevels.Error);
                    }
                }

                return true;
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }
            return result;
        }

        public LSPBreakerToShed GetArrBreakers(byte Index)
        {
            LSPBreakerToShed result = null;
            try
            {
                // 2016.02.17 A.K
                //Call theCTraceLogger.WriteLog(TraceInfo1, "CPriorityList..Get_arrBreakers()", Str(Index) + Str(m_nBreakers))

                if (Index < 1 || Index > _nBreakers)
                {
                    // Error message
                    _logger.WriteEntry("Index is out of range", LogLevels.Error);
                }
                else
                {
                    if (_breakersToShed[Index] == null)
                    {
                        _logger.WriteEntry("arrBreakers(" + Index.ToString() + ") is nothing", LogLevels.Error);
                    }
                    else
                    {
                        result = _breakersToShed[Index];
                    }
                }
            }
            catch (System.Exception excep)
            {
                _logger.WriteEntry(excep.Message, LogLevels.Error, excep);
            }

            return result;
        }
    }
}