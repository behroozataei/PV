using COMMON;
using Irisa.Common.Utils;
using Irisa.Logger;
using Irisa.Message;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Timers;

namespace EEC
{
    class EECSFSCManager
    {
        private const int NUMBER_OF_CYCLES_OVERLAD_CHECK = 8;
        private const int NUMBER_OF_BUSBARS = 2;
        private const int NUMBER_OF_FURNACES = 8;
        private const int TIMER_TICKS_SFSC = 4000;
        private const int ZERO_POWER_ON_BUSBAR = 1; // MW

        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly Timer _timer_4_Seconds;
        private UpdateScadaPointOnServer _updateScadaPointOnServer;

        private float[] _MaxBusbarPowers;           // Maximum power from EEC, On two busbars
        private float[] _BusbarPowers_Limit;        // power Limit for SFSC, On two busbars
        private float[] _BusbarPowers;              // On two busbars
        //private int[] _EAF_BusbarGroups;          // For 8 furnaces; Values: 0, 1 or 2
        private float[] _FurnacePowers;             // For 8 furnaces
        private bool[,] _IsOverloadAppear;          // For 2 busbars, for 6 cycles, check and write overload here
        private bool isWorking = false;
        private int cycle = 0;
        private int previousCycle = 0;
        private readonly DateTime[,] _cycles;  //Array for seconds period
        private readonly int[] _BBG_furnces;   //Array for Busbar group of furnaces
        private readonly int[] _BBG_EEC_furnces;   //Array for Busbar group of furnaces for EEC

        internal EECSFSCManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

            _timer_4_Seconds = new Timer();
            _timer_4_Seconds.Interval = TIMER_TICKS_SFSC;
            _timer_4_Seconds.Elapsed += RunCyclicOperation_SFSC;

            _MaxBusbarPowers = new float[NUMBER_OF_BUSBARS];
            _BusbarPowers_Limit = new float[NUMBER_OF_BUSBARS];
            _BusbarPowers = new float[NUMBER_OF_BUSBARS];
            //_EAF_BusbarGroups = new int[NUMBER_OF_FURNACES];
            _FurnacePowers = new float[NUMBER_OF_FURNACES];
            _IsOverloadAppear = new bool[NUMBER_OF_BUSBARS, NUMBER_OF_CYCLES_OVERLAD_CHECK];
            _cycles = new System.DateTime[NUMBER_OF_BUSBARS, NUMBER_OF_CYCLES_OVERLAD_CHECK];

            _BBG_furnces = new int[NUMBER_OF_FURNACES];
            _BBG_EEC_furnces = new int[NUMBER_OF_FURNACES];
            for (int i = 0; i < NUMBER_OF_FURNACES; i++)
            {
                _BBG_furnces[i] = 0;
                _BBG_EEC_furnces[i] = 0;
            }

            // Reset Status of 'SFSC/STATUS/Sent Warning'
            //Call m_Scada.WriteDigitalData("Network/Model Functions/SFSC/STATUS/Sent Warning", STATUS_DISAPPEARED)

            if (!ClearOverloadAppear())
            {
                _logger.WriteEntry("Error in clearing Overload Appear Flag! ", LogLevels.Error);
                //return;
            }

            //// Load MaxPowers from EEC!
            //if (!ReadEECPMaxFromTableOrSCADA())
            //{
            //    _logger.WriteEntry("Error in reading PMax for Busbars! ", LogLevels.Error);
            //    //return;
            //}

            //// Load EAFGroups from EEC_SFSCEAFSPriority
            //if ( !ReadEAFBusGroup())
            //{
            //    _logger.WriteEntry("Error in running ReadEAFBusGroup! ", LogLevels.Error);
            //    //return;
            //}



            _logger.WriteEntry("Maximum Power 1 = " + _BusbarPowers[0] + " ; Maximum Power 2 = " + _BusbarPowers[1], LogLevels.Info);
        }

        public void Start()
        {
            _timer_4_Seconds.Start();
        }


        private bool ClearOverloadAppear()
        {
            try
            {
                for (int busbar = 0; busbar < NUMBER_OF_BUSBARS; busbar++)
                    for (int cycle = 0; cycle < NUMBER_OF_CYCLES_OVERLAD_CHECK; cycle++)
                    {
                        _IsOverloadAppear[busbar, cycle] = false;
                        _cycles[busbar, cycle] = DateTime.UtcNow;
                    }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        private void RunCyclicOperation_SFSC(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (isWorking)
                {
                    _logger.WriteEntry("Warning: RunCyclicOperation_SFSC is busy!", LogLevels.Warn);
                    return;
                }
                else
                    isWorking = true;
                

                // A.Kaji, 1399.10.09 Adding this code
                // Step 0. Read EAF-Groups from SCADA and Update in EEC_SFSCEAFSPriority
                if (!UpdateFurnaceGroupsInHistorical_EEC_SFSCEAFSPriority())
                {
                    _logger.WriteEntry("Error in Update EAFGroups In Historical.EEC_SFSCEAFSPriority!", LogLevels.Error);
                }

                var functionStatus = (DigitalSingleStatusOnOff)_repository.GetScadaPoint("FSTATUS").Value;
                if (functionStatus == DigitalSingleStatusOnOff.Off)
                {
                    _logger.WriteEntry("SFSC Stop Upadating of Powers, EEC is OFF", LogLevels.Error);

                    isWorking = false;
                    return;
                }

                

                // Step 1. Calculate and update power of furnaces and busbars
                if (!CalculateBusbarAndFurnacePowers())
                {
                    _logger.WriteEntry("Error in Calculation power of Busbar and Furnace!", LogLevels.Error);
                    //return ;
                }

                // Step 2. Updating power of Busbars and Furnaces into SCADA
                if (!UpdatePowersInSCADA())
                {
                    _logger.WriteEntry("Error in updating power of Busbars and Furnaces into SCADA!", LogLevels.Error);
                    //return;
                }

                // Step 3. Writing power of Busbars and Furnaces into SFSCPower Table
                if (!WriteSFSCEAFSPOWER_Cyclic())
                {
                    _logger.WriteEntry("Error in writing power of Busbars and Furnaces into SFSCPower Table!", LogLevels.Error);
                    //return;
                }

                //// step 3.1 Check MAB Status, if MAB is Closed and MAB_EEC is Open force MAB_EEC status to Close
                //if (!Set_EEC_MAB_Status())
                //{
                //    _logger.WriteEntry("Error in setting EEC_MAB Status! ", LogLevels.Error);

                //}

                // Step 4. Load PMax1 and PMax2 from EEC!
                if (!ReadEECPMaxFromTableOrSCADA())
                {
                    _logger.WriteEntry("Error in reading PMax for Busbars! ", LogLevels.Warn);
                    isWorking = false;
                    return;
                }

                // Step 5.0 Calulate cycles
                if (!CalculateCycle())
                {
                    _logger.WriteEntry("Error in calulating cycle numbers! ", LogLevels.Error);
                    //return;
                }
                //_logger.WriteEntry($"cycle = {cycle} , preCycle = {previousCycle} ", LogLevels.Info);

                // Step 5. Check overload of powers
                if (!CheckPowerOverloadCurrentCycle())
                {
                    _logger.WriteEntry("Error in checking Overload of Furnaces on Busbars! ", LogLevels.Error);
                    //return;
                }

                // Step 6. Check full-cycles overload
                if (!ProcessPowerOverloadFullCycles())
                {
                    _logger.WriteEntry("Error in checking Overload of Furnaces on Busbars for full-cycles! ", LogLevels.Error);
                    //return;
                }

                


                //'-------------------------------------------------------------------------
                //' Writing exit message
                //_logger.WriteEntry("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬   End of Running Cycle   ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬", LogLevels.Info);
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            isWorking = false;
        }

        

        private bool ReadEECPMaxFromTableOrSCADA()
        {
            DateTime dTime = DateTime.MinValue;

            try
            {
                EEC_TELEGRAM_Str _eec_tel = new EEC_TELEGRAM_Str();
                // 1401-12-02 delay on redis query
                //if (_repository.GetRedisUtiles().GetKeys(RedisKeyPattern.EEC_TELEGRAM).Length == 0)
                //{
                //    _logger.WriteEntry("There EEC Calculation not Compeleted", LogLevels.Warn);
                //    return false;
                //}

                _eec_tel = JsonConvert.DeserializeObject<EEC_TELEGRAM_Str>(RedisUtils.RedisConn.Get(RedisKeyPattern.EEC_TELEGRAM));

                
                if (!(_eec_tel is null))
                {
                    _MaxBusbarPowers[0] = _eec_tel.MAXOVERLOAD1;
                    _MaxBusbarPowers[1] = _eec_tel.MAXOVERLOAD2;
                    dTime = _eec_tel.TELDATETIME;
                    var deltatime = DateTime.UtcNow.Subtract(dTime).TotalSeconds;
                    if (deltatime > 65)
                    {
                        _logger.WriteEntry("Error: Date and Time  of MAXOVERLOAD1, MAXOVERLOAD2 is not correct! " + "EEC TELDATEtime = " + dTime.ToIranStandardTime() + "  Delta Second = " + deltatime, LogLevels.Warn);
                        return false;
                    }
                }
                //else
                //{
                //    // TODO: read from PSend1 and PSend2 instead of PMax1 and PMax2
                //    if (!ReadEECPMaxFromSCADA())
                //    {
                //        _logger.WriteEntry("Error in loading MAXOVERLOAD1, MAXOVERLOAD2 From EECTELEGRAM and SCADA! ", LogLevels.Error);
                //        return false;
                //    }
                //}

                // Check values of _MaxBusbarPowers
                if ((_MaxBusbarPowers[0] < ZERO_POWER_ON_BUSBAR) &&
                    (_MaxBusbarPowers[1] < ZERO_POWER_ON_BUSBAR))
                {
                    _logger.WriteEntry("Error: both MAXOVERLOAD1, MAXOVERLOAD2 are zero! ", LogLevels.Warn);
                    return false;
                }

                EECScadaPoint _MAB_EEC;
                EECScadaPoint _PMAX1;
                EECScadaPoint _PMAX2;

                _MAB_EEC = _repository.GetScadaPoint("MAB_EEC");
                _PMAX1 = _repository.GetScadaPoint("PMAX1");
                _PMAX2 = _repository.GetScadaPoint("PMAX2");
                // Check MAB Status for _MaxBusbarPowers
                if (((_MaxBusbarPowers[0] == 0.0 && _PMAX1.Value > 0.0) ||
                     (_MaxBusbarPowers[1] == 0.0 && _PMAX2.Value > 0.0)) &&
                    (_MAB_EEC.Value == (float)DigitalSingleStatusOnOff.Off))
                {
                    _logger.WriteEntry("Error: MAB_EEC opened and MAXOVERLOAD1 or MAXOVERLOAD2 is zero! ", LogLevels.Warn);
                    return false;
                }

                if (_MAB_EEC.Value == (float)DigitalSingleStatusOnOff.On && _MaxBusbarPowers[0] > 0.0 && _MaxBusbarPowers[1] > 0.0)
                {
                    float Busbarspowers = _MaxBusbarPowers[0] + _MaxBusbarPowers[1];
                    _MaxBusbarPowers[0] = Busbarspowers;
                    _MaxBusbarPowers[1] = Busbarspowers;
                  //  _logger.WriteEntry("Error: MAB_EEC closed and both MAXOVERLOAD1 and MAXOVERLOAD2 > zero! ", LogLevels.Warn);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        // Instead of Table, using SCADA
        private bool ReadEECPMaxFromSCADA()
        {
            try
            {
                var scadapoint = _repository.GetScadaPoint("PMAX1");
                if (scadapoint is null)
                {
                    _logger.WriteEntry("Error in reading PMax1 from SCADA/EEC! ", LogLevels.Error);
                    return false;
                }
                else
                {
                    _MaxBusbarPowers[0] = scadapoint.Value;
                }

                scadapoint = _repository.GetScadaPoint("PMAX2");
                if (scadapoint is null)
                {
                    _logger.WriteEntry("Error in reading PMax2 from SCADA/EEC! ", LogLevels.Error);
                    return false;
                }
                else
                {
                    _MaxBusbarPowers[1] = scadapoint.Value;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        private bool WriteSFSCEAFSPOWER_Cyclic()
        {
            try
            {
                if (!_repository.ModifyOnHistoricalCache(_BusbarPowers, _FurnacePowers))
                {
                    _logger.WriteEntry($" Error in 'Insert Into APP_SFSCEAFSPOWER'", LogLevels.Error);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        private bool CalculateCycle()
        {
            try
            {
                // Calculating cycle and previous cycle 
                cycle = previousCycle;
                previousCycle++;
                if (previousCycle >= NUMBER_OF_CYCLES_OVERLAD_CHECK)
                    previousCycle = 0;

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }
        private bool CheckPowerOverloadCurrentCycle()
        {
            try
            {
                //int mod = (int)(DateTime.Now.Second / (TIMER_TICKS_SFSC/1000));
                //var cycle = (int)mod % NUMBER_OF_CYCLES_OVERLAD_CHECK;

                for (int busbar = 0; busbar < NUMBER_OF_BUSBARS; busbar++)
                {
                    _BusbarPowers_Limit[busbar] = (_MaxBusbarPowers[busbar] + 10.0f) * 1.02f;
                    if (_BusbarPowers[busbar] > _BusbarPowers_Limit[busbar])
                    {
                        _IsOverloadAppear[busbar, cycle] = true;
                        _cycles[busbar, cycle] = DateTime.UtcNow;
                        _logger.WriteEntry($"Warning: BusbarPowers[{(busbar + 1)}] = {_BusbarPowers[busbar]} ; MaxBusbarPowers[{(busbar + 1)}] = {_MaxBusbarPowers[busbar]} ; CycleNumber = {cycle} ;  CycleTime = {_cycles[busbar, cycle]} "
                                               , LogLevels.Warn);

                        // TODO: check
                        var scadapointAlarm = _repository.GetScadaPoint("SFCWarning");
                        if (!(scadapointAlarm is null))
                        {
                            _updateScadaPointOnServer.SendAlarm(scadapointAlarm, DigitalSingleStatus.Close,
                                "Warning For EAFS Exceed Its Limit On BusBar " + (busbar + 1) + " with value: " + _BusbarPowers[busbar]);
                            _updateScadaPointOnServer.SendAlarm(scadapointAlarm, DigitalSingleStatus.Open,
                                "Warning For EAFS Exceed Its Limit On BusBar " + (busbar + 1) + " with value: " + _BusbarPowers[busbar]);
                        }
                        else
                        {
                            _logger.WriteEntry("Warning For EAFS Exceed its Limit On BusBar 1 with value: " + _BusbarPowers[busbar], LogLevels.Error);
                        }
                    }

                    // Check and clearance of irrelated overload flags
                    var timeDiff = (DateTime.UtcNow - _cycles[busbar, cycle]).TotalMilliseconds;
                    if ((timeDiff >= (NUMBER_OF_CYCLES_OVERLAD_CHECK - 1) * TIMER_TICKS_SFSC) && _IsOverloadAppear[busbar, cycle])
                        _IsOverloadAppear[busbar, cycle] = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            return false;
        }

        private bool ProcessPowerOverloadFullCycles()
        {
            try
            {

                var scadapointAlarm = _repository.GetScadaPoint("SFCWarning");

                // TODO: check condition of overload checking
                // Busbar A/B : TODO: check
                for (int busbar = 0; busbar < NUMBER_OF_BUSBARS; busbar++)
                {
                    if (_IsOverloadAppear[busbar, previousCycle] && _IsOverloadAppear[busbar, cycle])
                    {
                        {
                            _logger.WriteEntry("Start of reporting Overload on busbar " + (busbar + 1) + "; previousCycle = " + previousCycle + "; previousCycleTime =" + _cycles[busbar, previousCycle] + "; Cycle = " + cycle + "; CycleTime =" + _cycles[busbar, cycle], LogLevels.Warn);

                            EEC_SFSCEAFSPRIORITY_Str[] eec_sfsceafprio = new EEC_SFSCEAFSPRIORITY_Str[8];


                            for (int fur = 0; fur < 8; fur++)
                            {
                                // if (_repository.GetRedisUtiles().GetKeys(pattern: RedisKeyPattern.EEC_SFSCEAFSPRIORITY + (fur + 1).ToString()).Length != 0)
                                // 1401-12-02 delay on redis query
                                eec_sfsceafprio[fur] = JsonConvert.DeserializeObject<EEC_SFSCEAFSPRIORITY_Str>(RedisUtils.RedisConn.Get(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + (fur + 1).ToString()));
                                //else
                                //{
                                //    _logger.WriteEntry($"Error in 'SELECT PRIORITY TABLE FROM APP_EEC_SFSCEAFSPRIORITY'", LogLevels.Error);
                                //    return false;
                                //}

                            }

                            var datatable = eec_sfsceafprio.Where(n => n.GROUPNUM_EEC == (busbar + 1).ToString() && n.STATUS_OF_FURNACE == "ON")
                                                        .OrderBy(n => Convert.ToDecimal(n.CONSUMED_ENERGY_PER_HEAT)).ToArray();

                            // DataTable datatable = _repository.GetFromHistoricalDB(sql);
                            if ((datatable is null || datatable.Length == 0))
                            {
                                _logger.WriteEntry($"Error in 'SELECT FURNACE OF GROUP{busbar + 1} FROM APP_EEC_SFSCEAFSPRIORITY'", LogLevels.Error);
                                continue;
                            }
                            else
                            {
                                var furnace = Convert.ToInt32(datatable[0].FURNACE.ToString());
                                var energy = Convert.ToDecimal(datatable[0].CONSUMED_ENERGY_PER_HEAT.ToString());
                                _updateScadaPointOnServer.SendAlarm(scadapointAlarm, DigitalSingleStatus.Close, "SFSC Shedded Furnace " + furnace + " Because EAFS exceed its Limit On BusBar " + (busbar + 1));
                                _updateScadaPointOnServer.SendAlarm(scadapointAlarm, DigitalSingleStatus.Open, "SFSC Shedded Furnace " + furnace + " Because EAFS exceed its Limit On BusBar " + (busbar + 1));

                                // TODO: check
                                var scadapointFurnace = _repository.GetScadaPoint("TOSHED-FURNACE");
                                if (!(scadapointFurnace is null))
                                {
                                    if (!_updateScadaPointOnServer.WriteAnalog(scadapointFurnace, furnace))
                                    {
                                        _logger.WriteEntry("Error in writing Furnace to shed in SCADA, furnace " + furnace, LogLevels.Error);
                                    }
                                }
                                else
                                {
                                    _logger.WriteEntry("Error in finding Furnace to shed in SCADA, furnace " + furnace, LogLevels.Error);
                                }

                                var scadapointGroupPower = _repository.GetScadaPoint("TOSHED-GROUPPOWER");
                                if (!(scadapointGroupPower is null))
                                {
                                    if (!_updateScadaPointOnServer.WriteAnalog(scadapointGroupPower, _BusbarPowers[busbar]))
                                    {
                                        _logger.WriteEntry("Error in writing GroupPower to shed in SCADA, GroupPower " + _BusbarPowers[busbar], LogLevels.Error);
                                    }
                                }
                                else
                                {
                                    _logger.WriteEntry("Error in finding Furnace to shed in SCADA, GroupPower " + _BusbarPowers[busbar], LogLevels.Error);
                                }

                                SFSC_FURNACE_TO_SHED_Str sfsc_furnace_to_shed = new SFSC_FURNACE_TO_SHED_Str();
                                sfsc_furnace_to_shed.TELDATETIME = DateTime.UtcNow;
                                sfsc_furnace_to_shed.FURNACE = furnace.ToString();
                                sfsc_furnace_to_shed.GROUPPOWER = _BusbarPowers[busbar].ToString();
                                sfsc_furnace_to_shed.SHEADTIME = DateTime.Parse("1900-01-01 00:00:00");
                                sfsc_furnace_to_shed.SHEADCOMMAND = true;

                                RedisUtils.RedisConn.Set(RedisKeyPattern.SFSC_FURNACE_TO_SHED, JsonConvert.SerializeObject(sfsc_furnace_to_shed));
                                //_updateScadaPointOnServer.SendAlarm(_repository.GetScadaPoint("SFSCACTIVATED"), DigitalSingleStatus.Open, " ");

                                //1401_08_08 Preparing Data For HMI
                                String Datatime = DateTime.UtcNow.ToIranStandardTime();


                                string sql = $"INSERT INTO APP_SFSC_FURNACE_SHEDDED (DATETIME, FURNACE, ENERGY, EEC_POWER_LIMIT, CONSUMED_POWER, FURNACE_BUS_NUMBER) VALUES(" +
                                                $"'{Datatime}'" +
                                                ",'" +
                                                furnace + "', '" +
                                                Math.Round(energy,2) + "', '" +
                                                Math.Round(_MaxBusbarPowers[busbar],2) + "', '" +
                                                Math.Round(_BusbarPowers[busbar],2) + "', '" +
                                                (busbar+1) + "')";

                                var parameters = new IDbDataParameter[6];
                                parameters[0] = _repository.Get_historicalDataManager().CreateParameter("p_DateTime", Datatime);
                                parameters[1] = _repository.Get_historicalDataManager().CreateParameter("p_Furnace", furnace);
                                parameters[2] = _repository.Get_historicalDataManager().CreateParameter("p_Energy", Math.Round(energy, 2));
                                parameters[3] = _repository.Get_historicalDataManager().CreateParameter("p_EEC_Power_Limit", Math.Round(_MaxBusbarPowers[busbar], 2));
                                parameters[4] = _repository.Get_historicalDataManager().CreateParameter("p_Consumed_Power", Math.Round(_BusbarPowers[busbar], 2));
                                parameters[5] = _repository.Get_historicalDataManager().CreateParameter("p_Furnace_Bus_Num", (busbar + 1));

                                //     if (!_repository.ModifyOnHistoricalDB(sql))
                                if (!_repository.ModifyOnHistoricalDB("APP_SFSC_FURNACE_SHEDDED_INSERT ", parameters))
                                {
                                    _logger.WriteEntry($"Error in INSERT Into APP_SFSC_FURNACE_SHEDDED, Furnace " + furnace, LogLevels.Error);
                                }


                                _logger.WriteEntry("Overloaded furnace " + furnace + " with POWER OVERLOAD " + _BusbarPowers[busbar], LogLevels.Warn);

                                // For "Network/Model Functions/SFSC/STATUS/Sent Warning"
                                ////var scadapoint = _repository.GetScadaPoint("SFSCShedding");
                                var scadapoint = _repository.GetScadaPoint("SFCWarning");

                                if (!_updateScadaPointOnServer.WriteAnalog(scadapoint, (int)DigitalSingleStatus.Open))
                                {
                                    _logger.WriteEntry("Error in write into SCADA for ... ", LogLevels.Error);
                                }

                                // Clear Overload from cycles
                                for (int acycle = 0; acycle < NUMBER_OF_CYCLES_OVERLAD_CHECK; acycle++)
                                    _IsOverloadAppear[busbar, acycle] = false;
                                //}

                                // Log status of furnaces
                                foreach (EEC_SFSCEAFSPRIORITY_Str dr in datatable)
                                    _logger.WriteEntry("SFSC Furnace selection: Furnace " + dr.FURNACE.ToString() + " is ON, and " + "CONSUMED_ENERGY_PER_HEAT = " + dr.CONSUMED_ENERGY_PER_HEAT.ToString(), LogLevels.Warn);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
            }
            return false;
        }

        private bool CalculateBusbarAndFurnacePowers()
        {
            try
            {
                // Calculate power of EAF_Busbars
                _BusbarPowers[0] = 0;
                _BusbarPowers[1] = 0;

                for (int furnace = 1; furnace < NUMBER_OF_FURNACES + 1; furnace++)
                {
                    var scadapointEAFPower = _repository.GetScadaPoint("IPowerEAF" + furnace.ToString());
                    if (!(scadapointEAFPower is null))
                        _FurnacePowers[furnace - 1] = scadapointEAFPower.Value;
                    else
                        _logger.WriteEntry("Error in reading EAFPower from SCADA for Furnace " + furnace, LogLevels.Warn);

                    var scadapointEAFGroup = _repository.GetScadaPoint("EAF" + furnace.ToString() + "-Group-EEC");
                    if (!(scadapointEAFGroup is null) && !(scadapointEAFPower is null))
                    {
                        if ((int)scadapointEAFGroup.Value > 0)
                            _BusbarPowers[(int)scadapointEAFGroup.Value - 1] += scadapointEAFPower.Value;                            
                    }
                    else
                        _logger.WriteEntry("Error in reading EAFGroup-EEC from SCADA for Furnace : " + furnace, LogLevels.Warn);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        private bool UpdatePowersInSCADA()
        {
            try
            {
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("PowerGrp1"), _BusbarPowers[0]))
                {
                    _logger.WriteEntry("Error in write Power EAF Busbar1 into SCADA", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("PowerGrp2"), _BusbarPowers[1]))
                {
                    _logger.WriteEntry("Error in write Power EAF Busbar2 into SCADA", LogLevels.Error);
                }
                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("PowerGrp1_Limit"), _BusbarPowers_Limit[0]))
                {
                    _logger.WriteEntry("Error in write Power Limit EAF Busbar1 into SCADA", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("PowerGrp2_Limit"), _BusbarPowers_Limit[1]))
                {
                    _logger.WriteEntry("Error in write Power Limit EAF Busbar2 into SCADA", LogLevels.Error);
                }

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("Total Power"), _BusbarPowers[0] + _BusbarPowers[1]))
                {
                    _logger.WriteEntry("Error in write Summation of Power EAF Busbars into SCADA", LogLevels.Error);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error, ex);
                return false;
            }
        }

        private bool UpdateFurnaceGroupsInHistorical_EEC_SFSCEAFSPriority()
        {
            try
            {
                for (int furnace = 1; furnace < NUMBER_OF_FURNACES + 1; furnace++)
                {
                    var scadapointEAFGroup = _repository.GetScadaPoint("EAF" + furnace.ToString() + "-Group");
                    var scadapointEAFGroupEEC = _repository.GetScadaPoint("EAF" + furnace.ToString() + "-Group-EEC");
                    if (!String.IsNullOrEmpty(scadapointEAFGroup.Value.ToString()))
                    {
                        int bbg = (int)scadapointEAFGroup.Value;
                        int bbg_eec = (int)scadapointEAFGroupEEC.Value;
                        if ((_BBG_furnces[furnace - 1] != bbg) || (_BBG_EEC_furnces[furnace - 1] != bbg_eec))
                        {
                            EEC_SFSCEAFSPRIORITY_Str eec_sfsceafprio = new EEC_SFSCEAFSPRIORITY_Str();
                            eec_sfsceafprio = JsonConvert.DeserializeObject<EEC_SFSCEAFSPRIORITY_Str>(RedisUtils.RedisConn.Get(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + furnace.ToString()));
                            eec_sfsceafprio.GROUPNUM = scadapointEAFGroup.Value.ToString();
                            eec_sfsceafprio.GROUPNUM_EEC = scadapointEAFGroupEEC.Value.ToString();
                            eec_sfsceafprio.REASON = "EEC.SFSCManager => GROUPNUM is updated";
                            RedisUtils.RedisConn.Set(RedisKeyPattern.EEC_SFSCEAFSPRIORITY + eec_sfsceafprio.FURNACE, JsonConvert.SerializeObject(eec_sfsceafprio));

                            _BBG_furnces[furnace - 1] = Convert.ToInt32(scadapointEAFGroup.Value.ToString());
                            _BBG_EEC_furnces[furnace - 1] = Convert.ToInt32(scadapointEAFGroupEEC.Value.ToString());
                        }
                    }
                    else
                        _logger.WriteEntry("Error in reading EAFGroup from SCADA for Furnace : " + furnace, LogLevels.Warn);


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