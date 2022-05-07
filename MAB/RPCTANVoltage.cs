using System;
using System.Collections.Generic;
using System.Text;

using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;

namespace MAB
{
    internal class RPCTANVoltage
    {
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly IRepository _repository;

        internal RPCTANVoltage(IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
        }

        //' Update the Transformers Primary Voltages
        public bool TransPrimeVoltageCalc()
        {
            List<double> TAN_PRIMVOLTS;


            double T1AN_PRIMEBUS;
            double T2AN_PRIMEBUS;
            double T3AN_PRIMEBUS;
            double T4AN_PRIMEBUS;
            double T5AN_PRIMEBUS;
            double T6AN_PRIMEBUS;
            double T7AN_PRIMEBUS;

            //'' KAJI T8AN Definition
            double T8AN_PRIMEBUS;

            //Dim C01A_CB, C01A_DS1, C01A_DS2, C01A_DS3 As Long
            //Dim C01B_CB, C01B_DS1, C01B_DS2 As Long
            //Dim C01C_CB, C01C_DS1, C01C_DS2, C01C_DS3 As Long

            //Dim C02A_CB, C02A_DS1, C02A_DS2 As Long
            //Dim C02B_CB, C02B_DS1, C02B_DS2 As Long
            //Dim C02C_CB, C02C_DS1, C02C_DS2, C02C_DS3 As Long

            //Dim C03A_CB, C03A_DS1, C03A_DS2 As Long
            //Dim C03B_CB, C03B_DS1, C03B_DS2 As Long
            //Dim C03C_CB, C03C_DS1, C03C_DS2, C03C_DS3 As Long

            //Dim C04A_CB, C04A_DS1, C04A_DS2 As Long
            //Dim C04B_CB, C04B_DS1, C04B_DS2 As Long
            //Dim C04C_CB, C04C_DS1, C04C_DS2, C04C_DS3, C04A_DS3 As Long

            //Dim C05A_CB, C05A_DS1, C05A_DS2, C05A_DS3 As Long
            //Dim C05B_CB, C05B_DS1, C05B_DS2 As Long
            //Dim C05C_CB, C05C_DS1, C05C_DS2, C05C_DS3 As Long

            //TransPrimeVoltageCalc = True

            try
            {
                var C01A_CB = _repository.GetScadaPoint("C01A_CB");
                var C01A_DS1 = _repository.GetScadaPoint("C01A_DS1");
                var C01A_DS2 = _repository.GetScadaPoint("C01A_DS2");
                var C01A_DS3 = _repository.GetScadaPoint("C01A_DS3");
                var C01B_CB = _repository.GetScadaPoint("C01B_CB");
                var C01B_DS1 = _repository.GetScadaPoint("C01B_DS1");
                var C01B_DS2 = _repository.GetScadaPoint("C01B_DS2");
                var C01C_CB = _repository.GetScadaPoint("C01C_CB");
                var C01C_DS1 = _repository.GetScadaPoint("C01C_DS1");
                var C01C_DS2 = _repository.GetScadaPoint("C01C_DS2");
                var C01C_DS3 = _repository.GetScadaPoint("C01C_DS3");

                var C02A_CB = _repository.GetScadaPoint("C02A_CB");
                var C02A_DS1 = _repository.GetScadaPoint("C02A_DS1");
                var C02A_DS2 = _repository.GetScadaPoint("C02A_DS2");
                var C02B_CB = _repository.GetScadaPoint("C02B_CB");
                var C02B_DS1 = _repository.GetScadaPoint("C02B_DS1");
                var C02B_DS2 = _repository.GetScadaPoint("C02B_DS2");
                var C02C_CB = _repository.GetScadaPoint("C02C_CB");
                var C02C_DS1 = _repository.GetScadaPoint("C02C_DS1");
                var C02C_DS2 = _repository.GetScadaPoint("C02C_DS2");
                var C02C_DS3 = _repository.GetScadaPoint("C02C_DS3");

                var C03A_CB = _repository.GetScadaPoint("C03A_CB");
                var C03A_DS1 = _repository.GetScadaPoint("C03A_DS1");
                var C03A_DS2 = _repository.GetScadaPoint("C03A_DS2");
                var C03B_CB = _repository.GetScadaPoint("C03B_CB");
                var C03B_DS1 = _repository.GetScadaPoint("C03B_DS1");
                var C03B_DS2 = _repository.GetScadaPoint("C03B_DS2");
                var C03C_CB = _repository.GetScadaPoint("C03C_CB");
                var C03C_DS1 = _repository.GetScadaPoint("C03C_DS1");
                var C03C_DS2 = _repository.GetScadaPoint("C03C_DS2");
                var C03C_DS3 = _repository.GetScadaPoint("C03C_DS3");

                var C04A_CB = _repository.GetScadaPoint("C04A_CB");
                var C04A_DS1 = _repository.GetScadaPoint("C04A_DS1");
                var C04A_DS2 = _repository.GetScadaPoint("C04A_DS2");
                var C04A_DS3 = _repository.GetScadaPoint("C04A_DS3");
                var C04B_CB = _repository.GetScadaPoint("C04B_CB");
                var C04B_DS1 = _repository.GetScadaPoint("C04B_DS1");
                var C04B_DS2 = _repository.GetScadaPoint("C04B_DS2");
                var C04C_CB = _repository.GetScadaPoint("C04C_CB");
                var C04C_DS1 = _repository.GetScadaPoint("C04C_DS1");
                var C04C_DS2 = _repository.GetScadaPoint("C04C_DS2");
                var C04C_DS3 = _repository.GetScadaPoint("C04C_DS3");

                var C05A_CB = _repository.GetScadaPoint("C05A_CB");
                var C05A_DS1 = _repository.GetScadaPoint("C05A_DS1");
                var C05A_DS2 = _repository.GetScadaPoint("C05A_DS2");
                var C05A_DS3 = _repository.GetScadaPoint("C05A_DS3");

                var C05B_CB = _repository.GetScadaPoint("C05B_CB");
                var C05B_DS1 = _repository.GetScadaPoint("C05B_DS1");
                var C05B_DS2 = _repository.GetScadaPoint("C05B_DS2");
                var C05C_CB = _repository.GetScadaPoint("C05C_CB");
                var C05C_DS1 = _repository.GetScadaPoint("C05C_DS1");
                var C05C_DS2 = _repository.GetScadaPoint("C05C_DS2");
                var C05C_DS3 = _repository.GetScadaPoint("C05C_DS3");

                //' Bus 400 voltages
                var V400_1 = _repository.GetScadaPoint("V400_1");
                var V400_2 = _repository.GetScadaPoint("V400_2");

                int BUS1 = 1;
                int BUS2 = 2;

                //' Set the default value
                T1AN_PRIMEBUS = 0;
                T2AN_PRIMEBUS = 0;
                T3AN_PRIMEBUS = 0;
                T4AN_PRIMEBUS = 0;
                T5AN_PRIMEBUS = 0;
                T6AN_PRIMEBUS = 0;
                T7AN_PRIMEBUS = 0;

                //'' KAJI T8AN Definition
                T8AN_PRIMEBUS = 0;

                // Feeder C01A, T1AN
                if (C01A_DS3.IsTransient() || C01A_DS3.IsOpen() || C01A_DS3.IsClose())
                {
                    if (C01A_CB.IsClose() && C01A_DS1.IsClose() && C01A_DS2.IsClose())
                    {
                        T1AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T1AN_PRIMEVOLT (In Logic) : " + "BUS"+ T1AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if (C01B_CB.IsClose() && C01B_DS1.IsClose() && C01B_DS2.IsClose() &&
                            C01C_CB.IsClose() && C01C_DS1.IsClose() && C01C_DS2.IsClose())
                        {
                            T1AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T1AN_PRIMEVOLT (In Logic) : " + "BUS" + T1AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C01C, T2AN
                if ((C01C_DS3.IsTransient()) || (C01C_DS3.IsOpen()) || (C01C_DS3.IsClose()))
                {
                    if ((C01A_CB.IsClose()) && (C01A_DS1.IsClose()) && (C01A_DS2.IsClose()) &&
                        (C01B_CB.IsClose()) && (C01B_DS1.IsClose()) && (C01B_DS2.IsClose()))
                    {
                        T2AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T2AN_PRIMEVOLT (In Logic) : " + "BUS" + T2AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C01C_CB.IsClose()) && (C01C_DS1.IsClose()) && (C01C_DS2.IsClose()))
                        {
                            T2AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T2AN_PRIMEVOLT (In Logic) : " + "BUS" + T2AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C02C, T3AN
                if ((C02C_DS3.IsTransient()) || (C02C_DS3.IsOpen()) || (C02C_DS3.IsClose()))
                {
                    if ((C02A_CB.IsClose()) && (C02A_DS1.IsClose()) && (C02A_DS2.IsClose()) &&
                        (C02B_CB.IsClose()) && (C02B_DS1.IsClose()) && (C02B_DS2.IsClose()))
                    {
                        T3AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T3AN_PRIMEVOLT (In Logic) : " + "BUS" + T3AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C02C_CB.IsClose()) && (C02C_DS1.IsClose()) && (C02C_DS2.IsClose()))
                        {
                            T3AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T3AN_PRIMEVOLT (In Logic) : " + "BUS" + T3AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C03C, T4AN
                if ((C03C_DS3.IsTransient()) || (C03C_DS3.IsOpen()) || (C03C_DS3.IsClose()))
                {
                    if ((C03A_CB.IsClose()) && (C03A_DS1.IsClose()) && (C03A_DS2.IsClose()) &&
                        (C03B_CB.IsClose()) && (C03B_DS1.IsClose()) && (C03B_DS2.IsClose()))
                    {
                        T4AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T4AN_PRIMEVOLT (In Logic) : " + "BUS" + T4AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C03C_CB.IsClose()) && (C03C_DS1.IsClose()) && (C03C_DS2.IsClose()))
                        {
                            T4AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T4AN_PRIMEVOLT (In Logic) : " + "BUS" + T4AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C04C, T5AN
                if ((C04C_DS3.IsTransient()) || (C04C_DS3.IsOpen()) || (C04C_DS3.IsClose()))
                {
                    if ((C04A_CB.IsClose()) && (C04A_DS1.IsClose()) && (C04A_DS2.IsClose()) &&
                        (C04B_CB.IsClose()) && (C04B_DS1.IsClose()) && (C04B_DS2.IsClose()))
                    {
                        T5AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T5AN_PRIMEVOLT (In Logic) : " + "BUS" + T5AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C04C_CB.IsClose()) && (C04C_DS1.IsClose()) && (C04C_DS2.IsClose()))
                        {
                            T5AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T5AN_PRIMEVOLT (In Logic) : " + "BUS" + T5AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C05C, T6AN
                if ((C05C_DS3.IsTransient()) || (C05C_DS3.IsOpen()) || (C05C_DS3.IsClose()))
                {
                    if ((C05B_CB.IsClose()) && (C05B_DS1.IsClose()) && (C05B_DS2.IsClose()) &&
                        (C05A_CB.IsClose()) && (C05A_DS1.IsClose()) && (C05A_DS2.IsClose()))
                    {
                        T6AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T6AN_PRIMEVOLT (In Logic) : " + "BUS" + T6AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C05C_CB.IsClose()) && (C05C_DS1.IsClose()) && (C05C_DS2.IsClose()))
                        {
                            T6AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T6AN_PRIMEVOLT (In Logic) : " + "BUS" + T6AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C04A, T7AN
                if ((C04A_DS3.IsTransient()) || (C04A_DS3.IsOpen()) || (C04A_DS3.IsClose()))
                {
                    if ((C04A_CB.IsClose()) && (C04A_DS1.IsClose()) && (C04A_DS2.IsClose()))
                    {
                        T7AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T7AN_PRIMEVOLT (In Logic) : " + "BUS" + T7AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C04B_CB.IsClose()) && (C04B_DS1.IsClose()) && (C04B_DS2.IsClose()) &&
                            (C04C_CB.IsClose()) && (C04C_DS1.IsClose()) && (C04C_DS2.IsClose()))
                        {
                            T7AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T7AN_PRIMEVOLT (In Logic) : " + "BUS" + T7AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }

                // Feeder C05A, T8AN
                // '' KAJI START   T8AN Definition
                if ((C05A_DS3.IsTransient()) || (C05A_DS3.IsOpen()) || (C05A_DS3.IsClose()))
                {
                    if ((C05A_CB.IsClose()) && (C05A_DS1.IsClose()) && (C05A_DS2.IsClose()))
                    {
                        T8AN_PRIMEBUS = BUS1;
                        _logger.WriteEntry("T8AN_PRIMEVOLT (In Logic) : " + "BUS" + T8AN_PRIMEBUS, LogLevels.Info);
                    }
                    else
                    {
                        if ((C05B_CB.IsClose()) && (C05B_DS1.IsClose()) && (C05B_DS2.IsClose()) &&
                            (C05C_CB.IsClose()) && (C05C_DS1.IsClose()) && (C05C_DS2.IsClose()))
                        {
                            T8AN_PRIMEBUS = BUS2;
                            _logger.WriteEntry("T8AN_PRIMEVOLT (In Logic) : " + "BUS" + T8AN_PRIMEBUS, LogLevels.Info);
                        }
                    }
                }
                //'' KAJI END


                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T1AN_PRIMEBUS"), (float)T1AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T1AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T2AN_PRIMEBUS"), (float)T2AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T2AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T3AN_PRIMEBUS"), (float)T3AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T3AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T4AN_PRIMEBUS"), (float)T4AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T4AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T5AN_PRIMEBUS"), (float)T5AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T5AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T6AN_PRIMEBUS"), (float)T6AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T6AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T7AN_PRIMEBUS"), (float)T7AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T7AN_PRIMEBUS", LogLevels.Error);

                if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetScadaPoint("T8AN_PRIMEBUS"), (float)T8AN_PRIMEBUS))
                    _logger.WriteEntry("Could not update value in SCADA: T8AN_PRIMEBUS", LogLevels.Error);

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
                return false;
            }
        }

    }
}
