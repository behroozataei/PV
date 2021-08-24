using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using Irisa.Logger;

namespace EEC
{
    class PowerCalculation
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private UpdateScadaPointOnServer _updateScadaPointOnServer;

        // SFSC required memebers:
        private const int TIMER_TICKS_SFSC = 4000;
        private readonly Timer _timer_SFSC;
        private const int NUMBER_OF_EAFS = 8;
        private const int NUMBER_OF_EAFBUSBARS = 2;
        private const int NUMBER_OF_CYCLES_TO_POWER_CHECK = 6;
        private float[] _sumBBPowers = new float[NUMBER_OF_EAFBUSBARS + 1];
        private int[] _eafBusbarGroups = new int[NUMBER_OF_EAFS];
        private float[] _eafPowers = new float[NUMBER_OF_EAFS];
        private DateTime[,] _overloadCycles = new DateTime[NUMBER_OF_EAFBUSBARS, NUMBER_OF_CYCLES_TO_POWER_CHECK];
        private int _powerCycleCounter = 0;

        internal PowerCalculation(ILogger logger, IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));

            // SFSC initiation
            _timer_SFSC = new Timer();
            _timer_SFSC.Interval = TIMER_TICKS_SFSC;
            _timer_SFSC.Elapsed += RunCyclicSFSC;
            ResetOverloadCycles();

            //StartCyclicOperation();
        }

        public void StartCyclicOperation()
        {
            //_energyCalculator.InitialValues();

            // SFSC initiation
            _timer_SFSC.Start();
        }
        private void RunCyclicSFSC(object sender, ElapsedEventArgs e)
        {
            try
            {
                //_timer_SFSC.Stop();
                _logger.WriteEntry("------------------------------  SFSC Cyclic running ---------------------------------", LogLevels.Info);

                // 1. Read instant power from EAF1 to EAF8, write them into SCADA points for SFSC:
                EECScadaPoint scadaPoint;
                for (int eaf = 0; eaf < NUMBER_OF_EAFS; eaf++)
                {
                    // Reading from instant power points in SCADA and Initialize EAF powers
                    _eafPowers[eaf] = _repository.GetScadaPoint("IPowerEAF" + (eaf + 1).ToString()).Value;

                    // Updating power for EAFs in SFSC section
                    string eafName = "PowerEAF" + (eaf + 1).ToString();
                    scadaPoint = _repository.GetScadaPoint(eafName);
                    if (!_updateScadaPointOnServer.WriteAnalog(scadaPoint, _eafPowers[eaf]))
                        _logger.WriteEntry("Could not write value in SCADA for " + eafName.ToString(), LogLevels.Error);

                    // Initialize EAFs group on busbars
                    string eafGroup = "EAF" + (eaf + 1).ToString() + "-Group";
                    _eafBusbarGroups[eaf] = (int)_repository.GetScadaPoint(eafGroup).Value;

                }

                // 2. Processing sum of power for every busbar:
                // 2. Preparing sum of powers on busbars
                // NUMBER_OF_EAF_BUSBARS + 1 : 1 more for case of not connected EAF to Busbar( Busbar == 0 )
                _sumBBPowers[0] = 0;
                _sumBBPowers[1] = 0;
                _sumBBPowers[2] = 0;

                for (int eaf = 0; eaf < NUMBER_OF_EAFS; eaf++)
                    _sumBBPowers[_eafBusbarGroups[eaf]] += _eafPowers[eaf];

                _logger.WriteEntry($" _sumBBPowers[0] = {_sumBBPowers[0]} ", LogLevels.Info);
                _logger.WriteEntry($" _sumBBPowers[1] = {_sumBBPowers[1]} ", LogLevels.Info);
                _logger.WriteEntry($" _sumBBPowers[2] = {_sumBBPowers[2]} ", LogLevels.Info);

                // 3. Checking Power of Busbars with maximum values from EEC
                // Pmax1, Pmax2, Pmax_BB1, Pmax_BB2, EAF1_power .. 
                for (int busbar = 0; busbar < NUMBER_OF_EAFBUSBARS; busbar++)
                {
                    var BusbarOverloaded = CheclPowerIsInOverloadCycle(busbar);

                    // 4. Find proper EAF to send for sheding, Lowest consumed energy from active EAFs on busbar

                    List<EECEAFPoint> eECEAFPoints = new List<EECEAFPoint>();
                    for (int eaf = 0; eaf < NUMBER_OF_EAFS; eaf++)
                        eECEAFPoints.Add(_repository.GetEAFPoint(eaf + 1));

                    EECEAFPoint eafSelectedToShed = eECEAFPoints
                        .Where(eafSelected => (eafSelected.GROUPNUM == (busbar + 1).ToString()) &&
                                                (eafSelected.STATUS_OF_FURNACE == "ON"))
                        .OrderBy(eafOrder => eafOrder.CONSUMED_ENERGY_PER_HEAT).FirstOrDefault();

                    _logger.WriteEntry($" busbar = {busbar + 1} ", LogLevels.Info);
                    _logger.WriteEntry($" Furnace to shed is: {eafSelectedToShed.FurnaceNumber}, {eafSelectedToShed.CONSUMED_ENERGY_PER_HEAT}, {eafSelectedToShed.GROUPNUM} ", LogLevels.Info);

                    // 5. Send EAF number for sheding to LSP

                    // 6. Storing current snapshot in required tables or SCADA
                }

                // 7. Cycle counter cycling and progressing
                CycleCounterProgressing();

                //_timer_SFSC.Start();
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
        }

        void CycleCounterProgressing()
        {
            if (_powerCycleCounter < NUMBER_OF_CYCLES_TO_POWER_CHECK - 1)
                _powerCycleCounter++;
            else
                _powerCycleCounter = 0;
        }

        bool CheclPowerIsInOverloadCycle(int busbar)
        {
            try
            {
                // Check cycle number
                if ((busbar < 0) || (busbar >= NUMBER_OF_EAFBUSBARS))
                {
                    _logger.WriteEntry("Busbar number to check power overload is incorrect!, invalid busbar is: " + busbar.ToString(), LogLevels.Error);
                    return false;
                }

                // Overload condition in the current cycle
                EECScadaPoint pMax = _repository.GetScadaPoint("PMAX" + (busbar + 1).ToString());
                // TODO : Formula should be applied
                _logger.WriteEntry($"{pMax.Name} --> {pMax.Value}", LogLevels.Info);
                if (_sumBBPowers[busbar + 1] > (pMax.Value * 1.04 + 10))
                {
                    _logger.WriteEntry($"Sum of power on Busbar {busbar + 1} is exceeded than {pMax.Value}!", LogLevels.Warn);
                    _overloadCycles[busbar, _powerCycleCounter] = DateTime.MinValue;
                }

                // Check overload condition totally
                int previousCycleToCheck = _powerCycleCounter + 1;

                if (previousCycleToCheck >= NUMBER_OF_CYCLES_TO_POWER_CHECK)
                    previousCycleToCheck = 0;

                if (_overloadCycles[busbar, _powerCycleCounter] > DateTime.MinValue &&
                    _overloadCycles[busbar, previousCycleToCheck] > DateTime.MinValue)
                {
                    _logger.WriteEntry($"POWER OVERLOAD status is detected: Busbar {busbar + 1}; Current cycle {_overloadCycles[busbar, _powerCycleCounter]}; Prevoius cycle {_overloadCycles[busbar, previousCycleToCheck]}!", LogLevels.Error);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
            return false;
        }

        void ResetOverloadCycles()
        {
            bool[,] _overloadCycles = new bool[NUMBER_OF_EAFBUSBARS, NUMBER_OF_CYCLES_TO_POWER_CHECK];
            for (int busbar = 0; busbar < NUMBER_OF_EAFBUSBARS; busbar++)
                for (int cycle = 0; cycle < NUMBER_OF_CYCLES_TO_POWER_CHECK; cycle++)
                    _overloadCycles[busbar, cycle] = false;
        }

    }
}