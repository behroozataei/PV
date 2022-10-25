using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;

namespace MAB
{
    public sealed class MABManager : IProcessing
    {
        private const int EAFs = 8;
        private const float VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK = 5.0f;
        private const float VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK = 3.0f;

        private MABScadaPoint MAB;
        private MABScadaPoint MAB1;
        private MABScadaPoint MAB2;
        private MABScadaPoint MAB3;
        private MABScadaPoint MAB4;
        private MABScadaPoint MAB5;
        private MABScadaPoint MAB6;
        private MABScadaPoint MAB7;

        private MABScadaPoint T1AN_BB;
        private MABScadaPoint T2AN_BB;
        private MABScadaPoint T3AN_BB;
        private MABScadaPoint T5AN_BB;
        private MABScadaPoint T7AN_BB;
        private MABScadaPoint T8AN_BB;
        
        private MABScadaPoint EAF1_Group;
        private MABScadaPoint EAF2_Group;
        private MABScadaPoint EAF3_Group;
        private MABScadaPoint EAF4_Group;
        private MABScadaPoint EAF5_Group;
        private MABScadaPoint EAF6_Group;
        private MABScadaPoint EAF7_Group;
        private MABScadaPoint EAF8_Group;

        private MABScadaPoint EAF1_Group_EEC;
        private MABScadaPoint EAF2_Group_EEC;
        private MABScadaPoint EAF3_Group_EEC;
        private MABScadaPoint EAF4_Group_EEC;
        private MABScadaPoint EAF5_Group_EEC;
        private MABScadaPoint EAF6_Group_EEC;
        private MABScadaPoint EAF7_Group_EEC;
        private MABScadaPoint EAF8_Group_EEC;

        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        private DigitalStatus _oldMAB;
        private bool _initialize_Voltage_Source;
        private bool _mabInitialized;
        private readonly RPCTANVoltage _rpcTanVoltage;

        internal MABManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);
            _rpcTanVoltage = new RPCTANVoltage(_repository, _updateScadaPointOnServer, _logger);
            _oldMAB = DigitalStatus.Open;
        }

        public void Build()
        {
            MAB = _repository.GetScadaPoint("MAB");
            MAB1 = _repository.GetScadaPoint("MAB1");
            MAB2 = _repository.GetScadaPoint("MAB2");
            MAB3 = _repository.GetScadaPoint("MAB3");
            MAB4 = _repository.GetScadaPoint("MAB4");
            MAB5 = _repository.GetScadaPoint("MAB5");
            MAB6 = _repository.GetScadaPoint("MAB6");
            MAB7 = _repository.GetScadaPoint("MAB7");

            T1AN_BB = _repository.GetScadaPoint("T1AN-BB");
            T2AN_BB = _repository.GetScadaPoint("T2AN-BB");
            T3AN_BB = _repository.GetScadaPoint("T3AN-BB");
            T5AN_BB = _repository.GetScadaPoint("T5AN-BB");
            T7AN_BB = _repository.GetScadaPoint("T7AN-BB");
            T8AN_BB = _repository.GetScadaPoint("T8AN-BB");

            EAF1_Group = _repository.GetScadaPoint("EAF1-Group");
            EAF2_Group = _repository.GetScadaPoint("EAF2-Group");
            EAF3_Group = _repository.GetScadaPoint("EAF3-Group");
            EAF4_Group = _repository.GetScadaPoint("EAF4-Group");
            EAF5_Group = _repository.GetScadaPoint("EAF5-Group");
            EAF6_Group = _repository.GetScadaPoint("EAF6-Group");
            EAF7_Group = _repository.GetScadaPoint("EAF7-Group");
            EAF8_Group = _repository.GetScadaPoint("EAF8-Group");

            EAF1_Group_EEC = _repository.GetScadaPoint("EAF1-Group-EEC");
            EAF2_Group_EEC = _repository.GetScadaPoint("EAF2-Group-EEC");
            EAF3_Group_EEC = _repository.GetScadaPoint("EAF3-Group-EEC");
            EAF4_Group_EEC = _repository.GetScadaPoint("EAF4-Group-EEC");
            EAF5_Group_EEC = _repository.GetScadaPoint("EAF5-Group-EEC");
            EAF6_Group_EEC = _repository.GetScadaPoint("EAF6-Group-EEC");
            EAF7_Group_EEC = _repository.GetScadaPoint("EAF7-Group-EEC");
            EAF8_Group_EEC = _repository.GetScadaPoint("EAF8-Group-EEC");
        }

        public void InitializeMAB()
        {
            try
            {
                _logger.WriteEntry($"________________    MAB is Starting to work ...    ______________", LogLevels.Info);
                _logger.WriteEntry($"_________________________________________________________________", LogLevels.Info);
                _logger.WriteEntry($"", LogLevels.Info);
                _logger.WriteEntry($"MAB  \"{MAB.Name} \" with value \"{MAB.Value} \" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB1 \"{MAB1.Name}\" with value \"{MAB1.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB2 \"{MAB2.Name}\" with value \"{MAB2.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB3 \"{MAB3.Name}\" with value \"{MAB3.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB4 \"{MAB4.Name}\" with value \"{MAB4.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB5 \"{MAB5.Name}\" with value \"{MAB5.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB6 \"{MAB6.Name}\" with value \"{MAB6.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"MAB7 \"{MAB7.Name}\" with value \"{MAB7.Value}\" is read.", LogLevels.Info);

                _logger.WriteEntry($"", LogLevels.Info);
                _logger.WriteEntry($"T1AN-BB \"{T1AN_BB.Name}\"T1AN-BB \"{T1AN_BB.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"T2AN-BB \"{T2AN_BB.Name}\"T2AN-BB \"{T2AN_BB.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"T3AN-BB \"{T3AN_BB.Name}\"T3AN-BB \"{T3AN_BB.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"T5AN-BB \"{T5AN_BB.Name}\"T5AN-BB \"{T5AN_BB.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"T7AN-BB \"{T7AN_BB.Name}\"T7AN-BB \"{T7AN_BB.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"T8AN-BB \"{T8AN_BB.Name}\"T8AN-BB \"{T8AN_BB.Value}\" is read.", LogLevels.Info);

                _logger.WriteEntry($"", LogLevels.Info);
                _logger.WriteEntry($"EAF1_Group \"{EAF1_Group.Name}\" with value \"{EAF1_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF2_Group \"{EAF2_Group.Name}\" with value \"{EAF2_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF3_Group \"{EAF3_Group.Name}\" with value \"{EAF3_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF4_Group \"{EAF4_Group.Name}\" with value \"{EAF4_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF5_Group \"{EAF5_Group.Name}\" with value \"{EAF5_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF6_Group \"{EAF6_Group.Name}\" with value \"{EAF6_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF7_Group \"{EAF7_Group.Name}\" with value \"{EAF7_Group.Value}\" is read.", LogLevels.Info);
                _logger.WriteEntry($"EAF8_Group \"{EAF8_Group.Name}\" with value \"{EAF8_Group.Value}\" is read.", LogLevels.Info);

                _mabInitialized = true;
                UpdateMAB();
                Update_VoltageSources();
                Update_TransPrimaryVoltage();
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
        }

        public void CheckCPSStatus()
        {

            while (!GlobalData.CPSStatus)
            {
                System.Threading.Thread.Sleep(5000);
                _logger.WriteEntry("Waiting for Connecting to CPS", LogLevels.Info);

            }
        }

        // TODO : StartupForm.frm -> reset_MAB_Function_Status
        public void ClearEAFGroupsLocal()
        {
            EAF1_Group.Value = 0;
            EAF2_Group.Value = 0;
            EAF3_Group.Value = 0;
            EAF4_Group.Value = 0;
            EAF5_Group.Value = 0;
            EAF6_Group.Value = 0;
            EAF7_Group.Value = 0;
            EAF8_Group.Value = 0;
        }

        public void UpdateMAB()
        {
            if (_mabInitialized == false) return;

            // Updating MAB Status
            _logger.WriteEntry("Updating MAB is started . . . ", LogLevels.Info);

            // ------------------------------------------------------------------------------
            // Step 1.
            if (!Process_MAB())
                _logger.WriteEntry("Updating MAB was failed.", LogLevels.Error);

            // Updating EAF Groups
            _logger.WriteEntry("----------------------------------------- ", LogLevels.Info);
            _logger.WriteEntry("Updating EAFGroups is started . . . ", LogLevels.Info);

            // ------------------------------------------------------------------------------
            // Step 2.
            if (!Update_EAFGroups())
            {
                _logger.WriteEntry("Updating EAFGroups was failed.", LogLevels.Error);
            }
            else
            {
                //_logger.WriteEntry("Updating EAFGroups was accomplished successfully.", LogLevels.Info);

                // Updating "EAFGrpsChanged" to inform DC to send EAFGroup Telegram to PCS
                var eafGroupChanged = _repository.GetScadaPoint("EAFGrpsChanged");
                if (!_updateScadaPointOnServer.WriteDigital(eafGroupChanged, 0, ""))
                {
                    _logger.WriteEntry("Clearing EafGroupChanged for DCP-PCS was failed!", LogLevels.Error);
                }
                if (!_updateScadaPointOnServer.WriteDigital(eafGroupChanged, 1, "EafGroupChanged is triggered to inform DCP-PCS."))
                {
                    _logger.WriteEntry("Informing EafGroupChanged to DCP-PCS was failed!", LogLevels.Error);
                }
            }

            // Updating Transformers Busbar
            _logger.WriteEntry("-----------------------------------------", LogLevels.Info);
            _logger.WriteEntry("Updating TransBusbars is started . . . ", LogLevels.Info);

            // Step 3.
            if (!Update_TransBusbars())
                _logger.WriteEntry("Updating TransBusbars was failed.", LogLevels.Error);

            // Step 4. Updating Voltage Sources
            //  if (!Update_VoltageSources())
            //     _logger.WriteEntry("Updating Voltage Sources was failed.", LogLevels.Error);
            return;
        }

        private bool Process_MAB()
        {
            var result = false;

            try
            {
                MAB.Value = (float)DigitalStatus.Open;
                MAB1.Value = (float)ProcessMAB1();

                MAB2.Value = (float)ProcessMAB2();

                MAB3.Value = (float)ProcessMAB3();

                MAB4.Value = (float)ProcessMAB4();

                MAB5.Value = (float)ProcessMAB5();

                MAB6.Value = (float)ProcessMAB6();

                MAB7.Value = (float)ProcessMAB7();

                // Determining MAB Status:
                if (MAB1.Value == (float)DigitalStatus.Close || MAB2.Value == (float)DigitalStatus.Close ||
                    MAB3.Value == (float)DigitalStatus.Close || MAB4.Value == (float)DigitalStatus.Close ||
                    MAB5.Value == (float)DigitalStatus.Close || MAB6.Value == (float)DigitalStatus.Close || MAB7.Value == (float)DigitalStatus.Close)
                {
                    MAB.Value = (float)DigitalStatus.Close;
                }

                _logger.WriteEntry($"MAB: {MAB.Value} ; OLD MAB was: {(DigitalStatus)_oldMAB}", LogLevels.Info);
                _oldMAB = (DigitalStatus)MAB.Value;

                _updateScadaPointOnServer.SendMAB(MAB, MAB1, MAB2, MAB3, MAB4, MAB5, MAB6, MAB7);

                result = true;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }

            return result;
        }

        private DigitalStatus ProcessMAB1()
        {
            var MAB1 = DigitalStatus.Open;
            var GMF1_CB = _repository.GetDigitalStatusByScadaName("GMF1_CB");
            var GMF1_DS1 = _repository.GetDigitalStatusByScadaName("GMF1_DS1");
            var GMF1_DS2 = _repository.GetDigitalStatusByScadaName("GMF1_DS2");
            var M51_CB = _repository.GetDigitalStatusByScadaName("M51_CB");
            var M51_DS1 = _repository.GetDigitalStatusByScadaName("M51_DS1");
            var M51_DS2 = _repository.GetDigitalStatusByScadaName("M51_DS2");
            var MT5A_CB = _repository.GetDigitalStatusByScadaName("MT5A_CB");
            var MT5A_DS1 = _repository.GetDigitalStatusByScadaName("MT5A_DS1");
            var MT5A_DS2 = _repository.GetDigitalStatusByScadaName("MT5A_DS2");
            var MT5A_DS3 = _repository.GetDigitalStatusByScadaName("MT5A_DS3");

            if (GMF1_CB == DigitalStatus.Close && GMF1_DS1 == DigitalStatus.Close && GMF1_DS2 == DigitalStatus.Close &&
                M51_CB == DigitalStatus.Close && M51_DS1 == DigitalStatus.Close && M51_DS2 == DigitalStatus.Close &&
                MT5A_CB == DigitalStatus.Close && MT5A_DS1 == DigitalStatus.Close && MT5A_DS2 == DigitalStatus.Close && MT5A_DS3 == DigitalStatus.Close)
            {
                MAB1 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 1 ----- ", LogLevels.Info);
            _logger.WriteEntry($"GMF1_CB = {GMF1_CB} ; GMF1_DS1 = {GMF1_DS1} ; GMF1_DS2 = {GMF1_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M51_CB  = {M51_CB} ; M51_DS1 = {M51_DS1} ; M51_DS2 = {M51_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MT5A_CB = {MT5A_CB} ; MT5A_DS1 = {MT5A_DS1} ; MT5A_DS2 = {MT5A_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 1 = {MAB1.ToString()}", LogLevels.Info);

            return MAB1;
        }

        private DigitalStatus ProcessMAB2()
        {
            var MAB2 = DigitalStatus.Open;

            var MF2_CB = _repository.GetDigitalStatusByScadaName("MF2_CB");
            var MF2_DS = _repository.GetDigitalStatusByScadaName("MF2_DS");
            var MF2_DS2 = _repository.GetDigitalStatusByScadaName("MF2_DS2");
            var M25_CB = _repository.GetDigitalStatusByScadaName("M25_CB");
            var M25_DS1 = _repository.GetDigitalStatusByScadaName("M25_DS1");
            var M25_DS2 = _repository.GetDigitalStatusByScadaName("M25_DS2");
            var ML5_CB = _repository.GetDigitalStatusByScadaName("ML5_CB");
            var ML5_DS1 = _repository.GetDigitalStatusByScadaName("ML5_DS1");
            var ML5_DS2 = _repository.GetDigitalStatusByScadaName("ML5_DS2");


            if (MF2_CB == DigitalStatus.Close && MF2_DS == DigitalStatus.Close && MF2_DS2 == DigitalStatus.Close &&
                M25_CB == DigitalStatus.Close && M25_DS1 == DigitalStatus.Close && M25_DS2 == DigitalStatus.Close &&
                ML5_CB == DigitalStatus.Close && ML5_DS1 == DigitalStatus.Close && ML5_DS2 == DigitalStatus.Close)
            {
                MAB2 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 2 ----- ", LogLevels.Info);
            _logger.WriteEntry($"MF2_CB = {MF2_CB} ; MF2_DS  = {MF2_DS} ; MF2_DS2  = {MF2_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M25_CB = {M25_CB} ; M25_DS1 = {M25_DS1} ; M25_DS2 = {M25_DS2}", LogLevels.Info);
            _logger.WriteEntry($"ML5_CB = {ML5_CB} ; ML5_DS1 = {ML5_DS1} ; ML5_DS2 = {ML5_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 2 = {MAB2.ToString()}", LogLevels.Info);

            return MAB2;
        }

        private DigitalStatus ProcessMAB3()
        {
            var MAB3 = DigitalStatus.Open;

            var MF3_CB = _repository.GetDigitalStatusByScadaName("MF3_CB");
            var MF3_DS1 = _repository.GetDigitalStatusByScadaName("MF3_DS1");
            var MF3_DS2 = _repository.GetDigitalStatusByScadaName("MF3_DS2");
            var M3A_CB = _repository.GetDigitalStatusByScadaName("M3A_CB");
            var M3A_DS1 = _repository.GetDigitalStatusByScadaName("M3A_DS1");
            var M3A_DS2 = _repository.GetDigitalStatusByScadaName("M3A_DS2");
            var MSA_CB = _repository.GetDigitalStatusByScadaName("MSA_CB");
            var MSA_DS1 = _repository.GetDigitalStatusByScadaName("MSA_DS1");
            var MSA_DS2 = _repository.GetDigitalStatusByScadaName("MSA_DS2");

            if (MF3_CB == DigitalStatus.Close && MF3_DS1 == DigitalStatus.Close && MF3_DS2 == DigitalStatus.Close &&
                M3A_CB == DigitalStatus.Close && M3A_DS1 == DigitalStatus.Close && M3A_DS2 == DigitalStatus.Close &&
                MSA_CB == DigitalStatus.Close && MSA_DS1 == DigitalStatus.Close && MSA_DS2 == DigitalStatus.Close)
            {
                MAB3 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 3 ----- ", LogLevels.Info);
            _logger.WriteEntry($"MF3_CB = {MF3_CB} ; MF3_DS1 = {MF3_DS1} ; MF3_DS2 = {MF3_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M3A_CB = {M3A_CB} ; M3A_DS1 = {M3A_DS1} ; M3A_DS2 = {M3A_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MSA_CB = {MSA_CB} ; MSA_DS1 = {MSA_DS1} ; MSA_DS2 = {MSA_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 3 = {MAB3.ToString()}", LogLevels.Info);

            return MAB3;
        }

        private DigitalStatus ProcessMAB4()
        {
            var MAB4 = DigitalStatus.Open;

            var GMF4_CB = _repository.GetDigitalStatusByScadaName("GMF4_CB");
            var GMF4_DS1 = _repository.GetDigitalStatusByScadaName("GMF4_DS1");
            var GMF4_DS2 = _repository.GetDigitalStatusByScadaName("GMF4_DS2");
            var M64_CB = _repository.GetDigitalStatusByScadaName("M64_CB");
            var M64_DS1 = _repository.GetDigitalStatusByScadaName("M64_DS1");
            var M64_DS2 = _repository.GetDigitalStatusByScadaName("M64_DS2");
            var ML6_CB = _repository.GetDigitalStatusByScadaName("ML6_CB");
            var ML6_DS1 = _repository.GetDigitalStatusByScadaName("ML6_DS1");
            var ML6_DS2 = _repository.GetDigitalStatusByScadaName("ML6_DS2");

            if (GMF4_CB == DigitalStatus.Close && GMF4_DS1 == DigitalStatus.Close && GMF4_DS2 == DigitalStatus.Close &&
                M64_CB == DigitalStatus.Close && M64_DS1 == DigitalStatus.Close && M64_DS2 == DigitalStatus.Close &&
                ML6_CB == DigitalStatus.Close && ML6_DS1 == DigitalStatus.Close && ML6_DS2 == DigitalStatus.Close)
            {
                MAB4 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 4 ----- ", LogLevels.Info);
            _logger.WriteEntry($"GMF4_CB = {GMF4_CB} ; GMF4_DS1 = {GMF4_DS1} ; GMF4_DS2 = {GMF4_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M64_CB = {M64_CB} ; M64_DS1 = {M64_DS1} ; M64_DS2 = {M64_DS2}", LogLevels.Info);
            _logger.WriteEntry($"ML6_CB = {ML6_CB} ; ML6_DS1 = {ML6_DS1} ; ML6_DS2 = {ML6_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 4 = {MAB4.ToString()}", LogLevels.Info);

            return MAB4;
        }

        private DigitalStatus ProcessMAB5()
        {
            var MAB5 = DigitalStatus.Open;

            var GMF5_CB = _repository.GetDigitalStatusByScadaName("GMF5_CB");
            var GMF5_DS1 = _repository.GetDigitalStatusByScadaName("GMF5_DS1");
            var GMF5_DS2 = _repository.GetDigitalStatusByScadaName("GMF5_DS2");
            var M57_CB = _repository.GetDigitalStatusByScadaName("M57_CB");
            var M57_DS1 = _repository.GetDigitalStatusByScadaName("M57_DS1");
            var M57_DS2 = _repository.GetDigitalStatusByScadaName("M57_DS2");
            var ML7_CB = _repository.GetDigitalStatusByScadaName("ML7_CB");
            var ML7_DS1 = _repository.GetDigitalStatusByScadaName("ML7_DS1");
            var ML7_DS2 = _repository.GetDigitalStatusByScadaName("ML7_DS2");

            if (GMF5_CB == DigitalStatus.Close && GMF5_DS1 == DigitalStatus.Close && GMF5_DS2 == DigitalStatus.Close &&
                M57_CB == DigitalStatus.Close && M57_DS1 == DigitalStatus.Close && M57_DS2 == DigitalStatus.Close &&
                ML7_CB == DigitalStatus.Close && ML7_DS1 == DigitalStatus.Close && ML7_DS2 == DigitalStatus.Close)
            {
                MAB5 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 5 ----- ", LogLevels.Info);
            _logger.WriteEntry($"GMF5_CB = {GMF5_CB} ; GMF5_DS1 = {GMF5_DS1} ; GMF5_DS2 = {GMF5_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M57_CB = {M57_CB} ; M57_DS1 = {M57_DS1} ; M57_DS2 = {M57_DS2}", LogLevels.Info);
            _logger.WriteEntry($"ML7_CB = {ML7_CB} ; ML7_DS1 = {ML7_DS1} ; ML7_DS2 = {ML7_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 5 = {MAB5.ToString()}", LogLevels.Info);

            return MAB5;
        }

        private DigitalStatus ProcessMAB6()
        {
            var MAB6 = DigitalStatus.Open;

            var GMF6_CB = _repository.GetDigitalStatusByScadaName("GMF6_CB");
            var GMF6_DS1 = _repository.GetDigitalStatusByScadaName("GMF6_DS1");
            var GMF6_DS2 = _repository.GetDigitalStatusByScadaName("GMF6_DS2");
            var MB6_CB = _repository.GetDigitalStatusByScadaName("MB6_CB");
            var MB6_DS1 = _repository.GetDigitalStatusByScadaName("MB6_DS1");
            var MB6_DS2 = _repository.GetDigitalStatusByScadaName("MB6_DS2");
            var MSB_CB = _repository.GetDigitalStatusByScadaName("MSB_CB");
            var MSB_DS1 = _repository.GetDigitalStatusByScadaName("MSB_DS1");
            var MSB_DS2 = _repository.GetDigitalStatusByScadaName("MSB_DS2");


            if (GMF6_CB == DigitalStatus.Close && GMF6_DS1 == DigitalStatus.Close && GMF6_DS2 == DigitalStatus.Close &&
                MB6_CB == DigitalStatus.Close && MB6_DS1 == DigitalStatus.Close && MB6_DS2 == DigitalStatus.Close &&
                MSB_CB == DigitalStatus.Close && MSB_DS1 == DigitalStatus.Close && MSB_DS2 == DigitalStatus.Close)
            {
                MAB6 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 6 ----- ", LogLevels.Info);
            _logger.WriteEntry($"GMF6_CB = {GMF6_CB} ; GMF6_DS1 = {GMF6_DS1} ; GMF6_DS2 = {GMF6_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MB6_CB = {MB6_CB} ; MB6_DS1 = {MB6_DS1} ; MB6_DS2 = {MB6_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MSB_CB = {MSB_CB} ; MSB_DS1 = {MSB_DS1} ; MSB_DS2 = {MSB_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 6 = {MAB6.ToString()}", LogLevels.Info);

            return MAB6;
        }

        private DigitalStatus ProcessMAB7()
        {
            var MAB7 = DigitalStatus.Open;

            var MT7A_CB = _repository.GetDigitalStatusByScadaName("MT7A_CB");
            var MT7A_DS1 = _repository.GetDigitalStatusByScadaName("MT7A_DS1");
            var MT7A_DS2 = _repository.GetDigitalStatusByScadaName("MT7A_DS2");
            var M87_CB = _repository.GetDigitalStatusByScadaName("M87_CB");
            var M87_DS1 = _repository.GetDigitalStatusByScadaName("M87_DS1");
            var M87_DS2 = _repository.GetDigitalStatusByScadaName("M87_DS2");
            var ML8_CB = _repository.GetDigitalStatusByScadaName("ML8_CB");
            var ML8_DS1 = _repository.GetDigitalStatusByScadaName("ML8_DS1");
            var ML8_DS2 = _repository.GetDigitalStatusByScadaName("ML8_DS2");

            if (MT7A_CB == DigitalStatus.Close && MT7A_DS1 == DigitalStatus.Close && MT7A_DS2 == DigitalStatus.Close &&
                M87_CB == DigitalStatus.Close && M87_DS1 == DigitalStatus.Close && M87_DS2 == DigitalStatus.Close &&
                ML8_CB == DigitalStatus.Close && ML8_DS1 == DigitalStatus.Close && ML8_DS2 == DigitalStatus.Close)
            {
                MAB7 = DigitalStatus.Close;
            }

            _logger.WriteEntry(" ----- Process MAB 7 ----- ", LogLevels.Info);
            _logger.WriteEntry($"MT7A_CB = {MT7A_CB} ; MT7A_DS1 = {MT7A_DS1} ; MT7A_DS2 = {MT7A_DS2}", LogLevels.Info);
            _logger.WriteEntry($"M87_CB = {M87_CB} ; M87_DS1 = {M87_DS1} ; M87_DS2 = {M87_DS2}", LogLevels.Info);
            _logger.WriteEntry($"ML8_CB = {ML8_CB} ; ML8_DS1 = {ML8_DS1} ; ML8_DS2 = {ML8_DS2}", LogLevels.Info);
            _logger.WriteEntry($"MAB 7 = {MAB7.ToString()}", LogLevels.Info);

            return MAB7;
        }

        public bool Update_EAFGroups()
        {
            bool result = false;

            try
            {
                var OneGroup = 0;

                if (_oldMAB == DigitalStatus.Close)
                {
                    OneGroup = 1;
                    _logger.WriteEntry("MAB Is Close", LogLevels.Info);
                }
                else
                {
                    OneGroup = 2;
                    _logger.WriteEntry("MAB Is Open", LogLevels.Info);
                }

                _logger.WriteEntry($"EAFs are in {OneGroup} Group(s) ", LogLevels.Info);

                var MF1_Group = ProcessEAFGroup1(OneGroup);
                _logger.WriteEntry($"MF1 is connected to Busbar : {MF1_Group}", LogLevels.Info);

                var MF2_Group = ProcessEAFGroup2(OneGroup);
                _logger.WriteEntry($"MF2 is connected to Busbar : {MF2_Group}", LogLevels.Info);

                var MF3_Group = ProcessEAFGroup3(OneGroup);
                _logger.WriteEntry($"MF3 is connected to Busbar : {MF3_Group}", LogLevels.Info);

                var MF4_Group = ProcessEAFGroup4(OneGroup);
                _logger.WriteEntry($"MF4 is connected to Busbar : {MF4_Group}", LogLevels.Info);

                var MF5_Group = ProcessEAFGroup5(OneGroup);
                _logger.WriteEntry($"MF5 is connected to Busbar : {MF5_Group}", LogLevels.Info);

                var MF6_Group = ProcessEAFGroup6(OneGroup);
                _logger.WriteEntry($"MF6 is connected to Busbar : {MF6_Group}", LogLevels.Info);

                var MF7_Group = ProcessEAFGroup7(OneGroup);
                _logger.WriteEntry($"MF7 is connected to Busbar : {MF7_Group}", LogLevels.Info);

                var MF8_Group = ProcessEAFGroup8(OneGroup);
                _logger.WriteEntry($"MF8 is connected to Busbar : {MF8_Group}", LogLevels.Info);

                var _MAB_EEC = _repository.DigitalSingleStatusOnOffByScadaName("MAB_EEC");

                bool _groupsWasChanged = false;
                if (((int)EAF1_Group.Value != MF1_Group) ||
                        ((int)EAF2_Group.Value != MF2_Group) ||
                        ((int)EAF3_Group.Value != MF3_Group) ||
                        ((int)EAF4_Group.Value != MF4_Group) ||
                        ((int)EAF5_Group.Value != MF5_Group) ||
                        ((int)EAF6_Group.Value != MF6_Group) ||
                        ((int)EAF7_Group.Value != MF7_Group) ||
                        ((int)EAF8_Group.Value != MF8_Group))
                {
                    _groupsWasChanged = true;
                    EAF1_Group.Value = (float)MF1_Group;
                    EAF2_Group.Value = (float)MF2_Group;
                    EAF3_Group.Value = (float)MF3_Group;
                    EAF4_Group.Value = (float)MF4_Group;
                    EAF5_Group.Value = (float)MF5_Group;
                    EAF6_Group.Value = (float)MF6_Group;
                    EAF7_Group.Value = (float)MF7_Group;
                    EAF8_Group.Value = (float)MF8_Group;
                }

                EAF1_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF1_Group > 0.0) ? (float)1.0 : (float)MF1_Group;
                EAF2_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF2_Group > 0.0) ? (float)1.0 : (float)MF2_Group;
                EAF3_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF3_Group > 0.0) ? (float)1.0 : (float)MF3_Group;
                EAF4_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF4_Group > 0.0) ? (float)1.0 : (float)MF4_Group;
                EAF5_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF5_Group > 0.0) ? (float)1.0 : (float)MF5_Group;
                EAF6_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF6_Group > 0.0) ? (float)1.0 : (float)MF6_Group;
                EAF7_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF7_Group > 0.0) ? (float)1.0 : (float)MF7_Group;
                EAF8_Group_EEC.Value = (_MAB_EEC == DigitalSingleStatusOnOff.On && (float)MF8_Group > 0.0) ? (float)1.0 : (float)MF8_Group;
                _updateScadaPointOnServer.SendEAFGroups(EAF1_Group_EEC, EAF2_Group_EEC, EAF3_Group_EEC, EAF4_Group_EEC,
                           EAF5_Group_EEC, EAF6_Group_EEC, EAF7_Group_EEC, EAF8_Group_EEC);


                /*
                var Group = 0;
                var DS1 = DigitalStatus.Intransit;
                var DS2 = DigitalStatus.Intransit;
                var CB = DigitalStatus.Intransit;
                var EAFBreakers = new int[EAFs + 1, 4]; // The Breaker status for EAF's, DS1: DS for Bus 1 or group 1, DS2: DS for Bus 2 or group 2

                // -------------------------------------------------------------------------------
                // For other EAFes
                // -------------------------------------------------------------------------------
                //EAFBreakers(2, 1) = m_CMABParams.getValuebyName("MF2_DS1
                //EAFBreakers(2, 2) = m_CMABParams.getValuebyName("MF2_DS2

                //EAFBreakers(3, 1) = m_CMABParams.getValuebyName("MF3_DS1
                //EAFBreakers(3, 2) = m_CMABParams.getValuebyName("MF3_DS2

                //EAFBreakers(4, 1) = m_CMABParams.getValuebyName("MF4_DS1
                //EAFBreakers(4, 2) = m_CMABParams.getValuebyName("MF4_DS2
                //EAFBreakers(4, 3) = m_CMABParams.getValuebyName("MF4_CB

                //EAFBreakers(5, 1) = m_CMABParams.getValuebyName("MF5_DS1
                //EAFBreakers(5, 2) = m_CMABParams.getValuebyName("MF5_DS2
                //EAFBreakers(5, 3) = m_CMABParams.getValuebyName("MF5_CB

                //EAFBreakers(6, 1) = m_CMABParams.getValuebyName("MF6_DS1
                //EAFBreakers(6, 2) = m_CMABParams.getValuebyName("MF6_DS2
                //EAFBreakers(6, 3) = m_CMABParams.getValuebyName("MF6_CB

                EAFBreakers[7, 1] = (int)_repository.GetDigitalStatusByScadaName("MF7_DS1");
                EAFBreakers[7, 2] = (int)_repository.GetDigitalStatusByScadaName("MF7_DS2");
                EAFBreakers[7, 3] = (int)_repository.GetDigitalStatusByScadaName("MF7_CB");

                EAFBreakers[8, 1] = (int)_repository.GetDigitalStatusByScadaName("MF8_DS1");
                EAFBreakers[8, 2] = (int)_repository.GetDigitalStatusByScadaName("MF8_DS2");
                EAFBreakers[8, 3] = (int)_repository.GetDigitalStatusByScadaName("MF8_CB");

                for (int I = 7; I <= EAFs; I++)
                {
                    //"I" MUST BE INCREASED AFTER ADDING A NEW MAB
                    // Determining the EAFGroup

                    Group = 0;
                    DS1 = (DigitalStatus)EAFBreakers[I, 1];
                    DS2 = (DigitalStatus)EAFBreakers[I, 2];
                    CB = (DigitalStatus)EAFBreakers[I, 3];
                    if (DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                        CB == DigitalStatus.Close && _currentMAB == DigitalStatus.Open)
                    { //   BY LOOKING TO THE CB STATUS, DS1
                        Group = 2;
                    }

                    if ((DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                         CB == DigitalStatus.Close && _currentMAB == DigitalStatus.Close) ||

                        (DS1 == DigitalStatus.Close && DS2 == DigitalStatus.Open &&
                         CB == DigitalStatus.Close && _currentMAB == DigitalStatus.Open) ||

                        (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Open) &&
                        CB == (DigitalStatus.Close) && _currentMAB == DigitalStatus.Close) ||

                        (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Close) &&
                         CB == (DigitalStatus.Close) && _currentMAB == DigitalStatus.Close))
                    { //   BY LOOKING TO THE CB STATUS, DS1
                        Group = 1;
                    }

                    //If aDS2 = Breaker_Status.GeneralModule.bClose Then
                    //aGroup = OneGroup
                    //End If

                    var EAFGroup = I; // _MABParams.GetEAFGroups(I)
                    // _MABParams.SetEAFGroups(I, Group);

                    if (EAFGroup != Group)
                        _groupsWasChanged = true;

                   _logger.WriteEntry($"EAF {I}  Group is: {EAFGroup}", LogLevels.Info);
                }
                */

                // If at least one of EAFes Group was changed, ...
                // Updating "EAFGrpsChanged" to inform DC to send EAFGroup Telegram to PCS
                if (_groupsWasChanged)
                {
                    _updateScadaPointOnServer.SendEAFGroups(EAF1_Group, EAF2_Group, EAF3_Group, EAF4_Group,
                            EAF5_Group, EAF6_Group, EAF7_Group, EAF8_Group);
                }

                result = true;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }

            return result;
        }

        public int ProcessEAFGroup1(int oneGroup)
        {
            var MT5A_CB = _repository.GetDigitalStatusByScadaName("MT5A_CB");
            var M51_CB = _repository.GetDigitalStatusByScadaName("M51_CB");
            var GMF1_CB = _repository.GetDigitalStatusByScadaName("GMF1_CB");
            var GMF1_DS3 = _repository.GetDigitalStatusByScadaName("GMF1_DS3");
            var MF1_Group = (int)EAF1_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup1", LogLevels.Info);
            _logger.WriteEntry($"GMF1_CB= {GMF1_CB} ; GMF1_DS3= {GMF1_DS3} ; M51_CB= {M51_CB} ; MT5A_CB= {MT5A_CB}", LogLevels.Info);

            if (GMF1_DS3 == DigitalStatus.Open)
            {
                MF1_Group = 0;
            }
            else
            {
                if (GMF1_DS3 == DigitalStatus.Close)
                {
                    if (GMF1_CB == DigitalStatus.Close)
                    {
                        MF1_Group = 1;
                    }
                    else
                    {
                        if (GMF1_CB == DigitalStatus.Open)
                        {
                            if (M51_CB == DigitalStatus.Open)
                            {
                                MF1_Group = 0;
                            }
                            else
                            {
                                if (M51_CB == DigitalStatus.Close)
                                {

                                    if (MT5A_CB == DigitalStatus.Close)
                                    {
                                        MF1_Group = oneGroup;
                                    }
                                    else
                                    {
                                        if (MT5A_CB == DigitalStatus.Open)
                                        {
                                            MF1_Group = 0;
                                            _logger.WriteEntry("Warning: MF1 and MT5A are directly connected!", LogLevels.Warn);
                                        }
                                        else
                                        {
                                            _logger.WriteEntry($"Warning: MT5A_CB value is not valid, Value = {MT5A_CB}", LogLevels.Warn);
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.WriteEntry($"M51_CB value is not valid, Value = {M51_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"GMF1_CB value is not valid, Value = {GMF1_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"GMF1_DS3 value is not valid, Value = {GMF1_DS3}", LogLevels.Warn);
                }
            }

            return MF1_Group;
        }

        public int ProcessEAFGroup2(int oneGroup)
        {
            var ML5_CB = _repository.GetDigitalStatusByScadaName("ML5_CB");
            var MF2_CB = _repository.GetDigitalStatusByScadaName("MF2_CB");
            var M25_CB = _repository.GetDigitalStatusByScadaName("M25_CB");
            var MF2_DS3 = _repository.GetDigitalStatusByScadaName("MF2_DS3");
            var MF2_Group = (int)EAF2_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup2", LogLevels.Info);
            _logger.WriteEntry($"MF2_CB= {MF2_CB} ; MF2_DS3= {MF2_DS3} ; M25_CB= {M25_CB} ; ML5_CB= {ML5_CB}", LogLevels.Info);

            if (MF2_DS3 == DigitalStatus.Open)
            {
                MF2_Group = 0;
            }
            else
            {
                if (MF2_DS3 == DigitalStatus.Close)
                {
                    if (MF2_CB == DigitalStatus.Close)
                    {
                        MF2_Group = oneGroup;
                    }
                    else
                    {
                        if (MF2_CB == DigitalStatus.Open)
                        {
                            if (M25_CB == DigitalStatus.Close)
                            {
                                if (ML5_CB == DigitalStatus.Close)
                                {
                                    MF2_Group = 1;
                                }
                                else
                                {
                                    MF2_Group = 0;
                                    _logger.WriteEntry("Warning: MF2 and LF5 are directly connected!", LogLevels.Warn);
                                }
                            }
                            else
                            {
                                if (M25_CB == DigitalStatus.Open)
                                {
                                    MF2_Group = 0;
                                }
                                else
                                {
                                    _logger.WriteEntry($"M25_CB value is not valid, Value= {M25_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"MF2_CB value is not valid, Value= {MF2_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"MF2_DS3 value is not valid, Value= {MF2_DS3}", LogLevels.Warn);
                }
            }

            return MF2_Group;
        }

        public int ProcessEAFGroup3(int oneGroup)
        {
            var MSA_CB = _repository.GetDigitalStatusByScadaName("MSA_CB");
            var MF3_CB = _repository.GetDigitalStatusByScadaName("MF3_CB");
            var M3A_CB = _repository.GetDigitalStatusByScadaName("M3A_CB");
            var MF3_DS3 = _repository.GetDigitalStatusByScadaName("MF3_DS3");
            var MF3_Group = (int)EAF3_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup3", LogLevels.Info);
            _logger.WriteEntry($"MF3_CB= {MF3_CB} ; MF3_DS3= {MF3_DS3} ; M3A_CB= {M3A_CB} ; MSA_CB= {MSA_CB}", LogLevels.Info);

            if (MF3_DS3 == DigitalStatus.Open)
            {
                MF3_Group = 0;
            }
            else
            {
                if (MF3_DS3 == DigitalStatus.Close)
                {
                    if (MF3_CB == DigitalStatus.Close)
                    {
                        MF3_Group = oneGroup;
                    }
                    else
                    {
                        if (MF3_CB == DigitalStatus.Open)
                        {
                            if (M3A_CB == DigitalStatus.Close)
                            {
                                if (MSA_CB == DigitalStatus.Close)
                                {
                                    MF3_Group = 1;
                                }
                                else
                                {
                                    MF3_Group = 0;
                                    _logger.WriteEntry("Warning: MF3 and SVCA are directly connected!", LogLevels.Warn);
                                }
                            }
                            else
                            {
                                if (M3A_CB == DigitalStatus.Open)
                                {
                                    MF3_Group = 0;
                                }
                                else
                                {
                                    _logger.WriteEntry($"M3A_CB value is not valid, Value= {M3A_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"MF3_CB value is not valid, Value= {MF3_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"MF3_DS3 value is not valid, Value= {MF3_DS3}", LogLevels.Warn);
                }
            }

            return MF3_Group;
        }

        public int ProcessEAFGroup4(int oneGroup)
        {
            var GMF4_DS3 = _repository.GetDigitalStatusByScadaName("GMF4_DS3");
            var GMF4_CB = _repository.GetDigitalStatusByScadaName("GMF4_CB");
            var ML6_CB = _repository.GetDigitalStatusByScadaName("ML6_CB");
            var M64_CB = _repository.GetDigitalStatusByScadaName("M64_CB");
            var MF4_Group = (int)EAF4_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup4", LogLevels.Info);
            _logger.WriteEntry($"GMF4_CB= {GMF4_CB} ; GMF4_DS3= {GMF4_DS3} ; ML6_CB= {ML6_CB} ; M64_CB= {M64_CB}", LogLevels.Info);

            if (GMF4_DS3 == DigitalStatus.Open)
            {
                MF4_Group = 0;
            }
            else
            {
                if (GMF4_DS3 == DigitalStatus.Close)
                {
                    if (GMF4_CB == DigitalStatus.Close)
                    {
                        MF4_Group = 1;
                    }
                    else
                    {
                        if (GMF4_CB == DigitalStatus.Open)
                        {
                            if (ML6_CB == DigitalStatus.Close)
                            {
                                if (M64_CB == DigitalStatus.Close)
                                {
                                    MF4_Group = oneGroup;
                                }
                                else
                                {
                                    MF4_Group = 0;
                                    _logger.WriteEntry("Warning: MF4 and LF6 are directly connected!", LogLevels.Warn);
                                }
                            }
                            else
                            {
                                if (M64_CB == DigitalStatus.Open)
                                {
                                    MF4_Group = 0;
                                }
                                else
                                {
                                    _logger.WriteEntry($"M64_CB value is not valid, Value= {M64_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"GMF4_CB value is not valid, Value= {GMF4_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"GMF4_DS3 value is not valid, Value= {GMF4_DS3}", LogLevels.Warn);
                }
            }

            return MF4_Group;
        }

        public int ProcessEAFGroup5(int oneGroup)
        {
            _logger.WriteEntry("====== > ProcessEAFGroup5", LogLevels.Info);
            _logger.WriteEntry($"GMF5_CB= {_repository.GetDigitalStatusByScadaName("GMF5_CB")} ; GMF5_DS3= {_repository.GetDigitalStatusByScadaName("GMF5_DS3")} ; ML7_CB= {_repository.GetDigitalStatusByScadaName("ML7_CB")} ; M57_CB= {_repository.GetDigitalStatusByScadaName("M57_CB")}", LogLevels.Info);

            var GMF5_DS3 = _repository.GetDigitalStatusByScadaName("GMF5_DS3");
            var MF5_Group = (int)EAF5_Group.Value;
            var GMF5_CB = _repository.GetDigitalStatusByScadaName("GMF5_CB");
            var ML7_CB = _repository.GetDigitalStatusByScadaName("ML7_CB");
            var M57_CB = _repository.GetDigitalStatusByScadaName("M57_CB");

            if (GMF5_DS3 == DigitalStatus.Open)
            {
                MF5_Group = 0;
            }
            else
            {
                if (GMF5_DS3 == DigitalStatus.Close)
                {
                    if (GMF5_CB == DigitalStatus.Close)
                    {
                        MF5_Group = oneGroup;
                    }
                    else
                    {
                        if (GMF5_CB == DigitalStatus.Open)
                        {
                            if (ML7_CB == DigitalStatus.Close)
                            {
                                if (M57_CB == DigitalStatus.Close)
                                {
                                    MF5_Group = 1;
                                }
                                else
                                {
                                    MF5_Group = 0;
                                    _logger.WriteEntry("Warning: MF5 and LF7 are directly connected!", LogLevels.Warn);
                                }
                            }
                            else
                            {
                                if (M57_CB == DigitalStatus.Open)
                                {
                                    MF5_Group = 0;
                                }
                                else
                                {
                                    _logger.WriteEntry($"M57_CB value is not valid, Value= {M57_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"GMF5_CB value is not valid, Value= {GMF5_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"GMF5_DS3 value is not valid, Value= {GMF5_DS3}", LogLevels.Warn);
                }
            }

            return MF5_Group;
        }

        public int ProcessEAFGroup6(int oneGroup)
        {
            var GMF6_DS3 = _repository.GetDigitalStatusByScadaName("GMF6_DS3");
            var GMF6_CB = _repository.GetDigitalStatusByScadaName("GMF6_CB");
            var MSB_CB = _repository.GetDigitalStatusByScadaName("MSB_CB");
            var MB6_CB = _repository.GetDigitalStatusByScadaName("MB6_CB");
            var MF6_Group = (int)EAF6_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup6", LogLevels.Info);
            _logger.WriteEntry($"GMF6_CB= {GMF6_CB} ; GMF6_DS3= {GMF6_DS3} ; MSB_CB= {MSB_CB} ; MB6_CB= {MB6_CB}", LogLevels.Info);

            if (GMF6_DS3 == DigitalStatus.Open)
            {
                MF6_Group = 0;
            }
            else
            {
                if (GMF6_DS3 == DigitalStatus.Close)
                {
                    if (GMF6_CB == DigitalStatus.Close)
                    {
                        MF6_Group = 1;
                    }
                    else
                    {
                        if (GMF6_CB == DigitalStatus.Open)
                        {
                            if (MSB_CB == DigitalStatus.Close)
                            {
                                if (MB6_CB == DigitalStatus.Close)
                                {
                                    MF6_Group = oneGroup;
                                }
                                else
                                {
                                    MF6_Group = 0;
                                    _logger.WriteEntry("Warning: MF6 and SVCB are directly connected!", LogLevels.Warn);
                                }
                            }
                            else
                            {
                                if (MB6_CB == DigitalStatus.Open)
                                {
                                    MF6_Group = 0;
                                }
                                else
                                {
                                    _logger.WriteEntry($"MB6_CB value is not valid, Value= {MB6_CB}", LogLevels.Warn);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteEntry($"GMF6_CB value is not valid, Value = {GMF6_CB}", LogLevels.Warn);
                        }
                    }
                }
                else
                {
                    _logger.WriteEntry($"GMF6_DS3 value is not valid, Value = {GMF6_DS3}", LogLevels.Warn);
                }
            }

            return MF6_Group;
        }

        public int ProcessEAFGroup7(int oneGroup)
        {
            var MF7_DS1 = _repository.GetDigitalStatusByScadaName("MF7_DS1");
            var MF7_DS2 = _repository.GetDigitalStatusByScadaName("MF7_DS2");
            var MF7_CB = _repository.GetDigitalStatusByScadaName("MF7_CB");
            var MF7_Group = (int)EAF7_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup7", LogLevels.Info);
            _logger.WriteEntry($"MF7_DS1= {MF7_DS1} ; MF7_CB= {MF7_CB} ; MF7_DS2= {MF7_DS2} ", LogLevels.Info);

            var Group = 0;
            var DS1 = DigitalStatus.Intransit;
            var DS2 = DigitalStatus.Intransit;
            var CB = DigitalStatus.Intransit;
            var EAFBreakers = new int[EAFs + 1, 4]; // The Breaker status for EAF's, DS1: DS for Bus 1 or group 1, DS2: DS for Bus 2 or group 2

            // -------------------------------------------------------------------------------
            // For other EAFes
            // -------------------------------------------------------------------------------
            EAFBreakers[7, 1] = (int)_repository.GetDigitalStatusByScadaName("MF7_DS1");
            EAFBreakers[7, 2] = (int)_repository.GetDigitalStatusByScadaName("MF7_DS2");
            EAFBreakers[7, 3] = (int)_repository.GetDigitalStatusByScadaName("MF7_CB");

            int I = 7;
            {
                //"I" MUST BE INCREASED AFTER ADDING A NEW MAB
                // Determining the EAFGroup

                Group = 0;
                DS1 = (DigitalStatus)EAFBreakers[I, 1];
                DS2 = (DigitalStatus)EAFBreakers[I, 2];
                CB = (DigitalStatus)EAFBreakers[I, 3];

                if (DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                    CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Open)
                { //   BY LOOKING TO THE CB STATUS, DS1
                    Group = 2;
                }

                if ((DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                     CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Close) ||

                    (DS1 == DigitalStatus.Close && DS2 == DigitalStatus.Open &&
                     CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Open) ||

                    (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Open) &&
                    CB == (DigitalStatus.Close) && _oldMAB == DigitalStatus.Close) ||

                    (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Close) &&
                     CB == (DigitalStatus.Close) && _oldMAB == DigitalStatus.Close))
                { //   BY LOOKING TO THE CB STATUS, DS1
                    Group = 1;
                }
            }

            MF7_Group = Group;

            return MF7_Group;
        }

        public int ProcessEAFGroup8(int oneGroup)
        {
            var MF8_DS1 = _repository.GetDigitalStatusByScadaName("MF8_DS1");
            var MF8_DS2 = _repository.GetDigitalStatusByScadaName("MF8_DS2");
            var MF8_CB = _repository.GetDigitalStatusByScadaName("MF8_CB");
            var MF8_Group = (int)EAF8_Group.Value;

            _logger.WriteEntry("====== > ProcessEAFGroup8", LogLevels.Info);
            _logger.WriteEntry($"MF8_DS1= {MF8_DS1} ; MF8_CB= {MF8_CB} ; MF8_DS2= {MF8_DS2} ", LogLevels.Info);

            var Group = 0;
            var DS1 = DigitalStatus.Intransit;
            var DS2 = DigitalStatus.Intransit;
            var CB = DigitalStatus.Intransit;
            var EAFBreakers = new int[EAFs + 1, 4]; // The Breaker status for EAF's, DS1: DS for Bus 1 or group 1, DS2: DS for Bus 2 or group 2

            // -------------------------------------------------------------------------------
            // For other EAFes
            // -------------------------------------------------------------------------------
            EAFBreakers[8, 1] = (int)_repository.GetDigitalStatusByScadaName("MF8_DS1");
            EAFBreakers[8, 2] = (int)_repository.GetDigitalStatusByScadaName("MF8_DS2");
            EAFBreakers[8, 3] = (int)_repository.GetDigitalStatusByScadaName("MF8_CB");

            int I = 8;
            {
                //"I" MUST BE INCREASED AFTER ADDING A NEW MAB
                // Determining the EAFGroup

                Group = 0;
                DS1 = (DigitalStatus)EAFBreakers[I, 1];
                DS2 = (DigitalStatus)EAFBreakers[I, 2];
                CB = (DigitalStatus)EAFBreakers[I, 3];
                if (DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                    CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Open)
                { //   BY LOOKING TO THE CB STATUS, DS1
                    Group = 2;
                }

                if ((DS1 == DigitalStatus.Open && DS2 == DigitalStatus.Close &&
                     CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Close) ||

                    (DS1 == DigitalStatus.Close && DS2 == DigitalStatus.Open &&
                     CB == DigitalStatus.Close && _oldMAB == DigitalStatus.Open) ||

                    (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Open) &&
                    CB == (DigitalStatus.Close) && _oldMAB == DigitalStatus.Close) ||

                    (DS1 == (DigitalStatus.Close) && DS2 == (DigitalStatus.Close) &&
                     CB == (DigitalStatus.Close) && _oldMAB == DigitalStatus.Close))
                { //   BY LOOKING TO THE CB STATUS, DS1
                    Group = 1;
                }
            }

            MF8_Group = Group;

            return MF8_Group;
        }

        private bool Update_TransBusbars()
        {
            bool result = false;
            try
            {
                T1AN_BB.Value = (float)ProcessTransBusbar1();
                _logger.WriteEntry($"T1AN is connected to Busbar : {(int)T1AN_BB.Value}", LogLevels.Info);

                T2AN_BB.Value = (float)ProcessTransBusbar2();
                _logger.WriteEntry($"T2AN is connected to Busbar : {(int)T2AN_BB.Value}", LogLevels.Info);

                T3AN_BB.Value = (float)ProcessTransBusbar3();
                _logger.WriteEntry($"T3AN is connected to Busbar : {(int)T3AN_BB.Value}", LogLevels.Info);

                T5AN_BB.Value = (float)ProcessTransBusbar5();
                _logger.WriteEntry($"T5AN is connected to Busbar : {(int)T5AN_BB.Value}", LogLevels.Info);

                T7AN_BB.Value = (float)ProcessTransBusbar7();
                _logger.WriteEntry($"T7AN is connected to Busbar : {(int)T7AN_BB.Value}", LogLevels.Info);

                T8AN_BB.Value = (float)ProcessTransBusbar8();
                _logger.WriteEntry($"T8AN is connected to Busbar : {(int)T8AN_BB.Value}", LogLevels.Info);

                _updateScadaPointOnServer.SendTransBusbars(T1AN_BB, T2AN_BB, T3AN_BB, T5AN_BB, T7AN_BB, T8AN_BB);

                result = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }

            return result;
        }

        public DigitalStatus ProcessTransBusbar1()
        {
            var MT1A_CB = _repository.GetDigitalStatusByScadaName("MT1A_CB");
            var MT1A_DS1 = _repository.GetDigitalStatusByScadaName("MT1A_DS1");
            var MT1A_DS2 = _repository.GetDigitalStatusByScadaName("MT1A_DS2");

            if (MT1A_CB == DigitalStatus.Disturb || MT1A_CB == DigitalStatus.Intransit)
            {
                _logger.WriteEntry($"Status of MT1A_CB is invalid, {MT1A_CB}", LogLevels.Warn);
                // TODO: Sending alarm is required here
            }

            if (MT1A_DS1 == DigitalStatus.Disturb || MT1A_DS1 == DigitalStatus.Intransit)
            {
                _logger.WriteEntry($"Status of MT1A_DS1 is invalid, {MT1A_DS1}", LogLevels.Warn);
                // TODO: Sending alarm is required here
            }

            if (MT1A_DS2 == DigitalStatus.Disturb || MT1A_DS2 == DigitalStatus.Intransit)
            {
                _logger.WriteEntry($"Status of MT1A_DS2 is invalid, {MT1A_DS2}", LogLevels.Warn);
                // TODO: Sending alarm is required here
            }

            if (MT1A_CB != DigitalStatus.Close)
            {
                T1AN_BB.Value = (float)0;
            }
            else
            {
                if (MT1A_DS1 == DigitalStatus.Close && MT1A_DS2 == DigitalStatus.Open)
                {
                    T1AN_BB.Value = (float)1;
                }
                else
                {
                    if (MT1A_DS1 == DigitalStatus.Open && MT1A_DS2 == DigitalStatus.Close)
                    {
                        T1AN_BB.Value = (float)2;
                    }
                    else
                    {
                        T1AN_BB.Value = (float)0;
                    }
                }
            }

            if (((int)T1AN_BB.Value == 2) && ((int)MAB.Value == 2))
            {
                T1AN_BB.Value = (float)1;
            }

            return (DigitalStatus)T1AN_BB.Value;
        }

        public DigitalStatus ProcessTransBusbar2()
        {
            var MT2A_CB = _repository.GetDigitalStatusByScadaName("MT2A_CB");
            var MT2A_DS1 = _repository.GetDigitalStatusByScadaName("MT2A_DS1");
            var MT2A_DS2 = _repository.GetDigitalStatusByScadaName("MT2A_DS2");

            if (MT2A_CB == DigitalStatus.Disturb || MT2A_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT2A_CB is invalid, {MT2A_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT2A_DS1 == DigitalStatus.Disturb || MT2A_DS1 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT2A_DS1 is invalid, {MT2A_DS1}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT2A_DS2 == DigitalStatus.Disturb || MT2A_DS2 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT2A_DS2 is invalid, {MT2A_DS2}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT2A_CB != DigitalStatus.Close)
            {
                T2AN_BB.Value = 0;
            }
            else
            {
                if (MT2A_DS1 == DigitalStatus.Close && MT2A_DS2 == DigitalStatus.Open)
                {
                    T2AN_BB.Value = (float)1;
                }
                else
                {
                    if (MT2A_DS1 == DigitalStatus.Open && MT2A_DS2 == DigitalStatus.Close)
                    {
                        T2AN_BB.Value = (float)2;
                    }
                    else
                    {
                        T2AN_BB.Value = (float)0;
                    }
                }
            }

            if (((int)T2AN_BB.Value == 2) && ((int)MAB.Value == 2))
            {
                T2AN_BB.Value = (float)1;
            }

            return (DigitalStatus)T2AN_BB.Value;
        }

        public DigitalStatus ProcessTransBusbar3()
        {
            var MV3_CB = _repository.GetDigitalStatusByScadaName("MV3_CB");
            var MV3_DS1 = _repository.GetDigitalStatusByScadaName("MV3_DS1");
            var MV3_DS2 = _repository.GetDigitalStatusByScadaName("MV3_DS2");

            if (MV3_CB == DigitalStatus.Disturb || MV3_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MV3_CB is invalid, {MV3_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MV3_DS1 == DigitalStatus.Disturb || MV3_DS1 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MV3_DS1 is invalid, {MV3_DS1}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MV3_DS2 == DigitalStatus.Disturb || MV3_DS2 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MV3_DS2 is invalid, {MV3_DS2}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MV3_CB != DigitalStatus.Close)
            {
                T3AN_BB.Value = (float)0;
            }
            else
            {
                if (MV3_DS1 == DigitalStatus.Close && MV3_DS2 == DigitalStatus.Open)
                {
                    T3AN_BB.Value = (float)1;
                }
                else
                {
                    if (MV3_DS1 == DigitalStatus.Open && MV3_DS2 == DigitalStatus.Close)
                        T3AN_BB.Value = (float)2;
                    else
                        T3AN_BB.Value = (float)0;
                }
            }

            if (((int)T3AN_BB.Value == 2) && ((int)MAB.Value == 2))
                T3AN_BB.Value = (float)1;

            return (DigitalStatus)T3AN_BB.Value;
        }

        public DigitalStatus ProcessTransBusbar5()
        {
            var MT5A_DS3 = _repository.GetDigitalStatusByScadaName("MT5A_DS3");
            var MT5A_CB = _repository.GetDigitalStatusByScadaName("MT5A_CB");
            var M51_CB = _repository.GetDigitalStatusByScadaName("M51_CB");
            var GMF1_CB = _repository.GetDigitalStatusByScadaName("GMF1_CB");

            if (MT5A_DS3 == DigitalStatus.Disturb || MT5A_DS3 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT5A_DS3 is invalid, {MT5A_DS3}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT5A_CB == DigitalStatus.Disturb || MT5A_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT5A_CB is invalid, {MT5A_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (M51_CB == DigitalStatus.Disturb || M51_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of M51_CB is invalid, {M51_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (GMF1_CB == DigitalStatus.Disturb || GMF1_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of GMF1_CB is invalid, {GMF1_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT5A_DS3 == DigitalStatus.Open)
            {
                T5AN_BB.Value = (float)0;
            }
            else
            {
                if (M51_CB == DigitalStatus.Close && GMF1_CB == DigitalStatus.Close)
                {
                    T5AN_BB.Value = (float)1;
                }
                else
                {
                    if (MT5A_CB == DigitalStatus.Close)
                        T5AN_BB.Value = (float)2;
                }
            }

            if (((int)T5AN_BB.Value == 2) && ((int)MAB.Value == 2))
            {
                T5AN_BB.Value = (float)1;
            }

            return (DigitalStatus)T5AN_BB.Value;
        }

        public DigitalStatus ProcessTransBusbar7()
        {
            var MT7A_DS3 = _repository.GetDigitalStatusByScadaName("MT7A_DS3");
            var MT7A_CB = _repository.GetDigitalStatusByScadaName("MT7A_CB");
            var M87_CB = _repository.GetDigitalStatusByScadaName("M87_CB");
            var ML8_CB = _repository.GetDigitalStatusByScadaName("ML8_CB");

            if (MT7A_DS3 == DigitalStatus.Disturb || MT7A_DS3 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT7A_DS3 is invalid, {MT7A_DS3}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT7A_CB == DigitalStatus.Disturb || MT7A_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of M87_CB is invalid, {MT7A_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (M87_CB == DigitalStatus.Disturb || M87_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of M87_CB is invalid, {M87_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (ML8_CB == DigitalStatus.Disturb || ML8_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of ML8_CB is invalid, {ML8_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT7A_DS3 == DigitalStatus.Open)
            {
                T7AN_BB.Value = (float)0;
            }
            else
            {
                if (M87_CB == DigitalStatus.Close && ML8_CB == DigitalStatus.Close)
                {
                    T7AN_BB.Value = (float)2;
                }
                else
                {
                    if (MT7A_CB == DigitalStatus.Close)
                        T7AN_BB.Value = (float)1;
                }
            }

            if (((int)T7AN_BB.Value == 2) && ((int)MAB.Value == 2))
            {
                T7AN_BB.Value = (float)1;
            }

            return (DigitalStatus)T7AN_BB.Value;
        }

        public DigitalStatus ProcessTransBusbar8()
        {
            var MT8A_DS3 = _repository.GetDigitalStatusByScadaName("MT8A_DS3");
            var MT8A_CB = _repository.GetDigitalStatusByScadaName("MT8A_CB");
            var MT8A_DS1 = _repository.GetDigitalStatusByScadaName("MT8A_DS1");
            var MT8A_DS2 = _repository.GetDigitalStatusByScadaName("MT8A_DS2");

            if (MT8A_DS3 == DigitalStatus.Disturb || MT8A_DS3 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT5A_DS3 is invalid, {MT8A_DS3}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT8A_CB == DigitalStatus.Disturb || MT8A_CB == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT5A_CB is invalid, {MT8A_CB}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT8A_DS1 == DigitalStatus.Disturb || MT8A_DS1 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT8A_DS1 is invalid, {MT8A_DS1}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT8A_DS2 == DigitalStatus.Disturb || MT8A_DS2 == DigitalStatus.Intransit)
                _logger.WriteEntry($"Status of MT8A_DS2 is invalid, {MT8A_DS2}", LogLevels.Warn);
            // TODO: Sending alarm is required here

            if (MT8A_DS3 == DigitalStatus.Open || MT8A_CB == DigitalStatus.Open)
            {
                T8AN_BB.Value = (float)0;
            }
            else
            {
                if (MT8A_DS2 == DigitalStatus.Close)
                    T8AN_BB.Value = (float)2;
                if (MT8A_DS1 == DigitalStatus.Close)
                    T8AN_BB.Value = (float)1;
            }

            if (((int)T8AN_BB.Value == 2) && ((int)MAB.Value == 2))
            {
                T8AN_BB.Value = (float)1;
            }

            return (DigitalStatus)T8AN_BB.Value;
        }

        public void Update_VoltageSources()
        {
            if (_mabInitialized == false) return;

            try
            {
                // For CB_914
                var CB_914 = _repository.GetScadaPoint("CB_914");
                var CB_914_Status = _repository.GetDigitalStatusByScadaName("CB_914");
                var NIS1VT = _repository.GetScadaPoint("NIS1VT").Value;
                if ((NIS1VT > VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_914.Value == (float)DigitalStatus.Open) || CB_914.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_914 is connected, {CB_914.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_914, (int)DigitalStatus.Close, "Line_914 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_914.NetworkPath}", LogLevels.Error);
                }
                else if ((NIS1VT <= VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_914.Value == (float)DigitalStatus.Close) || CB_914.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_914 is disconnected, {CB_914.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_914, (int)DigitalStatus.Open, "Line_914 is disconnected "))
                        _logger.WriteEntry($"It is not possible to Open, {CB_914.NetworkPath}", LogLevels.Error);
                }

                //For CB_915
                var CB_915 = _repository.GetScadaPoint("CB_915");
                var CB_915_Status = _repository.GetDigitalStatusByScadaName("CB_915");
                var NIS2VT = _repository.GetScadaPoint("NIS2VT").Value;
                if ((NIS2VT > VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_915.Value == (float)DigitalStatus.Open) || CB_915.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_915 is connected, {CB_915.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_915, (int)DigitalStatus.Close, "Line_915 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_915.NetworkPath}", LogLevels.Error);
                }
                else if ((NIS2VT <= VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_915.Value == (float)DigitalStatus.Close) || CB_915.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"Line_915 is disconnected, {CB_915.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_915, (int)DigitalStatus.Open, "Line_915 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_915.NetworkPath}", LogLevels.Error);
                }

                //For CB_MBSM
                var CB_BMSM = _repository.GetScadaPoint("CB_MBSM");
                var CB_BMSM_Status = _repository.GetDigitalStatusByScadaName("CB_MBSM");
                var MIS2VT = _repository.GetScadaPoint("MIS2VT").Value;
                if ((MIS2VT > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_BMSM.Value == (float)DigitalStatus.Open) || CB_BMSM.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"MBSM is connected, {CB_BMSM.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_BMSM, (int)DigitalStatus.Close, "MBSM is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_BMSM.NetworkPath}", LogLevels.Error);
                }
                else if ((MIS2VT <= VOLTAGLE_LIMIT_FOR_LINE_SOURCE_CHECK) && ((CB_BMSM.Value == (float)DigitalStatus.Close) || CB_BMSM.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"MBSM is disconnected, {CB_BMSM.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_BMSM, (int)DigitalStatus.Open, "MBSM is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_BMSM.NetworkPath}", LogLevels.Error);
                }

                //For CB_GEN1
                var CB_GEN1 = _repository.GetScadaPoint("GEN1");
                var CB_GEN1_Status = _repository.GetDigitalStatusByScadaName("GEN1");
                var GENVT1 = _repository.GetScadaPoint("GENVT1").Value;
                if ((GENVT1 > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN1.Value == (float)DigitalStatus.Open) || CB_GEN1.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN1 is connected, {CB_GEN1.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN1, (int)DigitalStatus.Close, "GEN1 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_GEN1.NetworkPath}", LogLevels.Error);
                }
                else if ((GENVT1 <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN1.Value == (float)DigitalStatus.Close) || CB_GEN1.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN1 is disconnected, {CB_GEN1.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN1, (int)DigitalStatus.Open, "GEN1 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_GEN1.NetworkPath}", LogLevels.Error);
                }

                //For CB_GEN2
                var CB_GEN2 = _repository.GetScadaPoint("GEN2");
                var CB_GEN2_Status = _repository.GetDigitalStatusByScadaName("GEN2");
                var GENVT2 = _repository.GetScadaPoint("GENVT2").Value;
                if ((GENVT2 > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN2.Value == (float)DigitalStatus.Open) || CB_GEN2.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN2 is connected, {CB_GEN2.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN2, (int)DigitalStatus.Close, "GEN2 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_GEN2.NetworkPath}", LogLevels.Error);
                }
                else if ((GENVT2 <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN2.Value == (float)DigitalStatus.Close) || CB_GEN2.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN2 is disconnected, {CB_GEN2.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN2, (int)DigitalStatus.Open, "GEN2 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_GEN2.NetworkPath}", LogLevels.Error);
                }

                //For CB_GEN3
                var CB_GEN3 = _repository.GetScadaPoint("GEN3");
                var CB_GEN3_Status = _repository.GetDigitalStatusByScadaName("GEN3");
                var GENVT3 = _repository.GetScadaPoint("GENVT3").Value;
                if ((GENVT3 > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN3.Value == (float)DigitalStatus.Open) || CB_GEN3.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN3 is connected, {CB_GEN3.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN3, (int)DigitalStatus.Close, "GEN3 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_GEN3.NetworkPath}", LogLevels.Error);
                }
                else if ((GENVT3 <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN3.Value == (float)DigitalStatus.Close) || CB_GEN3.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN3 is disconnected, {CB_GEN3.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN3, (int)DigitalStatus.Open, "GEN3 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_GEN3.NetworkPath}", LogLevels.Error);
                }

                //For CB_GEN4
                var CB_GEN4 = _repository.GetScadaPoint("GEN4");
                var CB_GEN4_Status = _repository.GetDigitalStatusByScadaName("GEN4");
                var GENVT4 = _repository.GetScadaPoint("GENVT4").Value;
                if ((GENVT4 > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN4.Value == (float)DigitalStatus.Open) || CB_GEN4.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN4 is connected, {CB_GEN4.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN4, (int)DigitalStatus.Close, "GEN4 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_GEN4.NetworkPath}", LogLevels.Error);
                }
                else if ((GENVT4 <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_GEN4.Value == (float)DigitalStatus.Close) || CB_GEN4.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"GEN4 is disconnected, {CB_GEN4.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_GEN4, (int)DigitalStatus.Open, "GEN4 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_GEN4.NetworkPath}", LogLevels.Error);
                }

                //For CB_M2_L601
                var CB_M2_601 = _repository.GetScadaPoint("CB_M2_601");
                var CB_M2_601_Status = _repository.GetDigitalStatusByScadaName("CB_M2_601");
                var L601VT = _repository.GetScadaPoint("L601VT").Value;
                if ((L601VT > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_M2_601.Value == (float)DigitalStatus.Open) || CB_M2_601.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"L601 is connected, {CB_M2_601.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_M2_601, (int)DigitalStatus.Close, "L601 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_M2_601.NetworkPath}", LogLevels.Error);
                }
                else if ((L601VT <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_M2_601.Value == (float)DigitalStatus.Close) || CB_M2_601.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"L601 is disconnected, {CB_M2_601.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_M2_601, (int)DigitalStatus.Open, "L601 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_M2_601.NetworkPath}", LogLevels.Error);
                }

                //For CB_M2_L614
                var CB_M2_614 = _repository.GetScadaPoint("CB_M2_614");
                var CB_M2_614_Status = _repository.GetDigitalStatusByScadaName("CB_M2_614");
                var L614VT = _repository.GetScadaPoint("L614VT").Value;
                if ((L614VT > VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_M2_614.Value == (float)DigitalStatus.Open) || CB_M2_614.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"L614 is connected, {CB_M2_614.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_M2_614, (int)DigitalStatus.Close, "L614 is connected"))
                        _logger.WriteEntry($"It is not possible to Close, {CB_M2_614.NetworkPath}", LogLevels.Error);
                }
                else if ((L614VT <= VOLTAGLE_LIMIT_FOR_GEN_SOURCE_CHECK) && ((CB_M2_614.Value == (float)DigitalStatus.Close) || CB_M2_614.Qulity == (int)QualityCodes.LocalNotRenewed))
                {
                    _logger.WriteEntry($"L614 is disconnected, {CB_M2_614.NetworkPath}", LogLevels.Info);
                    if (!_updateScadaPointOnServer.WriteDigital(CB_M2_614, (int)DigitalStatus.Open, "L614 is disconnected"))
                        _logger.WriteEntry($"It is not possible to Open, {CB_M2_614.NetworkPath}", LogLevels.Error);
                }

                if (_initialize_Voltage_Source == false)
                    _logger.WriteEntry(" ----- Initialize_VoltageSources -> Process VoltageSources is finished ---- ", LogLevels.Info);
                _initialize_Voltage_Source = true;
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                _logger.WriteEntry("Updating Voltage Sources was failed.", LogLevels.Error);
            }
        }

        public void Update_TransPrimaryVoltage()
        {
           try
            {
                _rpcTanVoltage.TransPrimeVoltageCalc();
            }
            catch(Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                _logger.WriteEntry("Updating Transformer Primary Voltage was failed.", LogLevels.Error);

            }
        }
    }
}