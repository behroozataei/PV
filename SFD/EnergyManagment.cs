using Irisa.Message;
using SRC_FEED_DETECTION;
using System;
using System.Collections.Generic;
using System.Text;

using Irisa.Logger;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Timers;
using Google.Protobuf.WellKnownTypes;
using static Grpc.Core.Metadata;
using System.ComponentModel.DataAnnotations;
using Irisa.Common.Utils;
using Aspose.Cells;
using Microsoft.Identity.Client;
using System.Collections;

namespace SFD
{
    internal class EnergyManagment
    {
        private const int TIMER_TICKS = 60000;
        private readonly Timer _timer_1_Min;
        private readonly Timer _timer_5_Min;
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private const float LIMIT_FOR_1MIN_IRR = 0.023f;
        Queue<float> IIR_5_Min;
        Queue<float> Energy_5_Min;
        private UpdateScadaPointOnServer _updateScadaPointOnServer;
        internal EnergyManagment(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

            _timer_1_Min = new Timer();
            _timer_1_Min.Interval = TIMER_TICKS;
            _timer_1_Min.Elapsed += Energy_Cal;

            _timer_5_Min = new Timer();
            _timer_5_Min.Interval = TIMER_TICKS;
            _timer_5_Min.Elapsed += Energy_Cal;
            IIR_5_Min=new Queue<float>();
            Energy_5_Min = new Queue<float>();
        }

        ScadaPoint DailyEnergy, TotalEnergy, _1MinEnergy, PerviousDayEnergy;
        ScadaPoint _1MinIrradiance, DailyIrradiance, TotalIrradiance, PerviousDayIrradiance;
        ScadaPoint PR_1Min;
        ScadaPoint PR_daily;
        ScadaPoint PV_Panels;
        public void ReadLastData()
        {

            try
            {
                string sql = $"SELECT   *  FROM   SCADAHIS.APP_ENERGY  ORDER BY  DATETIME DESC";
                var datatable = _repository.GetFromHistoricalDB(sql);
                if (datatable != null)
                {
                    var _e_daily = (double)(datatable.Rows[0]["DAILY"]);
                    var _e_total = (double)(datatable.Rows[0]["TOTAL"]);
                    var _ir_daily = (double)(datatable.Rows[0]["IR_DAILY"]);
                    var _ir_total = (double)(datatable.Rows[0]["IR_TOTAL"]);

                    DailyEnergy = _repository.GetScadaPoint("DailyEnergy");
                    TotalEnergy = _repository.GetScadaPoint("TotalEnergy");
                    DailyIrradiance = _repository.GetScadaPoint("DailyIrradiance");
                    TotalIrradiance = _repository.GetScadaPoint("TotalIrradiance");
                    
                    _updateScadaPointOnServer.WriteAnalog(DailyEnergy, (float)_e_daily);
                    _updateScadaPointOnServer.WriteAnalog(TotalEnergy, (float)_e_total);
                    _updateScadaPointOnServer.WriteAnalog(DailyIrradiance, (float)_ir_daily);
                    _updateScadaPointOnServer.WriteAnalog(TotalIrradiance, (float)_ir_total);
                    _logger.WriteEntry($"LastValue Restored E_Daily = {_e_daily}   E_Total= {_e_total}", LogLevels.Info);
                    _logger.WriteEntry($"LastValue Restored IR_Daily = {_ir_daily}  IR_Total= {_ir_total}", LogLevels.Info);


                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }
        }




        public void Start()
        {
            _timer_1_Min.Start();
        }
        private void Energy_Cal(object sender, ElapsedEventArgs e)
        {

            try
            {
                _1MinEnergy = _repository.GetScadaPoint("_1MinEnergy");
                DailyEnergy = _repository.GetScadaPoint("DailyEnergy");
                PerviousDayEnergy = _repository.GetScadaPoint("PerviousDayEnergy");
                TotalEnergy = _repository.GetScadaPoint("TotalEnergy");

                _1MinIrradiance = _repository.GetScadaPoint("_1MinIrradiance");
                DailyIrradiance = _repository.GetScadaPoint("DailyIrradiance");
                TotalIrradiance = _repository.GetScadaPoint("TotalIrradiance");
                PerviousDayIrradiance = _repository.GetScadaPoint("PerviousDayIrradiance");


                DailyEnergy.Value = _1MinEnergy.Value + DailyEnergy.Value;
                TotalEnergy.Value = _1MinEnergy.Value + TotalEnergy.Value;
                if (_1MinIrradiance.Value < LIMIT_FOR_1MIN_IRR)
                    _1MinIrradiance.Value = 0;


                DailyIrradiance.Value = _1MinIrradiance.Value + DailyIrradiance.Value;
                TotalIrradiance.Value = _1MinIrradiance.Value + TotalIrradiance.Value;


                _updateScadaPointOnServer.WriteAnalog(TotalEnergy, TotalEnergy.Value);

                _updateScadaPointOnServer.WriteAnalog(TotalIrradiance, TotalIrradiance.Value);

                var CurrentTime = DateTime.Now;

                if (CurrentTime.Hour == 0 && CurrentTime.Minute == 0)
                {
                    _updateScadaPointOnServer.WriteAnalog(PerviousDayEnergy, DailyEnergy.Value);
                    _updateScadaPointOnServer.WriteAnalog(PerviousDayIrradiance, PerviousDayIrradiance.Value);
                    DailyEnergy.Value = 0;
                    DailyIrradiance.Value = 0;

                }
                _updateScadaPointOnServer.WriteAnalog(DailyEnergy, DailyEnergy.Value);
                _updateScadaPointOnServer.WriteAnalog(DailyIrradiance, DailyIrradiance.Value);

                _logger.WriteEntry($"1MinEnergy: {_1MinEnergy.Value} , DailyEnergy: {DailyEnergy.Value}", LogLevels.Info);
                _logger.WriteEntry($"1MinIrradiance: {_1MinIrradiance.Value},  DailyIrradiance: {DailyIrradiance.Value}", LogLevels.Info);

                PR_Calc();


                String Datatime = DateTime.Now.ToString($"yyyy-MM-dd HH:mm:ss.ff");



                //string sql = $"INSERT INTO APP_ENERGY (DATETIME, IR_DAILY, IR_TOTAL) VALUES (" +
                //                                $"TIMESTAMP '{Datatime}'" +
                //                                ",'" +
                //                                DailyIrradiance.Value + "', '" +
                //                                TotalIrradiance.Value + "')";

                string sql2 = $"UPDATE  APP_ENERGY  SET DATETIME = TIMESTAMP '{Datatime}' ," +
                                                        $"DAILY =  {DailyEnergy.Value} ," +
                                                        $"TOTAL =  {TotalEnergy.Value} ," +
                                                        $"IR_DAILY =  {DailyIrradiance.Value} ," +
                                                        $"IR_TOTAL =  {TotalIrradiance.Value}"; 
                                                        
                if (!_repository.ModifyOnHistoricalDB(sql2))

                {
                    _logger.WriteEntry($"Error in INSERT Into APP_ENERGY ", LogLevels.Error);
                }

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }


        }


        //The Performance Ratio is the ratio of the energy effectively produced(used), with respect to the energy which would be produced
        //if the system was continuously working at its nominal STC efficiency.The PR is defined in the norm IEC EN 61724.
        //In usual Grid-connected systems, the available energy is E_Grid.In stand-alone systems, it is the PV energy effectively delivered to the user,
        //The energy potentially produced at STC conditions is indeed equal to GlobInc * PnomPV, where PnomPV is the STC installed power (manufacturer's nameplate value).
        //This equivalence is explained by the fact that at STC (1000 W/m², 25°C) each kWh/m² of incident irradiation will produce 1 kWh of electricity.
        // Therefore for a grid-connected system:
        //   PR  = E_Grid / (GlobInc * PnomPV)

        const double PnomPV = 660.0 * 0.000001; //W ---> MW
       

        //Performance Ratio Calculation
        private void PR_Calc()
        {
            try
            {
                
                var POA = _repository.GetScadaPoint("PV_Panels").Value;
              

                if (_1MinEnergy.Value > 0 && _1MinIrradiance.Value > 0)
                {
                    IIR_5_Min.Enqueue(_1MinIrradiance.Value);
                    Energy_5_Min.Enqueue(_1MinEnergy.Value);
                    while(IIR_5_Min.Count>=6)
                    {
                        IIR_5_Min.Dequeue();
                        Energy_5_Min.Dequeue();
                    }
                    if (IIR_5_Min.Count == 5)
                    {
                        var _pr_1Min = (Energy_5_Min.AsEnumerable<float>().Sum() / (POA * PnomPV * (IIR_5_Min.AsEnumerable<float>().Sum() / 1000.0)));
                        PR_1Min = _repository.GetScadaPoint("PR_1Min");
                        _logger.WriteEntry($"PR:  {_pr_1Min}", LogLevels.Info);
                        _updateScadaPointOnServer.WriteAnalog(PR_1Min, (float)_pr_1Min * 100.0f); // Percent

                    }


                  
                   
                }
                if (DailyEnergy.Value > 0 && DailyIrradiance.Value > 0)
                {
                    var _pr_daily = (DailyEnergy.Value / (POA * PnomPV * (DailyIrradiance.Value / 1000.0)));
                    PR_daily = _repository.GetScadaPoint("PR_daily");
                    _logger.WriteEntry($"PR_Daily:  {_pr_daily}", LogLevels.Info);
                    _updateScadaPointOnServer.WriteAnalog(PR_daily, (float)_pr_daily * 100.0f);  // Percent
                }

            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }

        }
    }
}
