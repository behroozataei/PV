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

namespace SFD
{
    internal class EnergyManagment
    {
        private const int TIMER_TICKS = 60000;
        private readonly Timer _timer_1_Min;
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        private UpdateScadaPointOnServer _updateScadaPointOnServer;
        internal EnergyManagment(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

            _timer_1_Min = new Timer();
            _timer_1_Min.Interval = TIMER_TICKS;
            _timer_1_Min.Elapsed += Energy_Cal;
        }

        public void Start()
        {
            _timer_1_Min.Start();
        }
        private void Energy_Cal(object sender, ElapsedEventArgs e)
        {
            try
            {
                var _1MinEnergy = _repository.GetScadaPoint("_1MinEnergy");
                var DailyEnergy = _repository.GetScadaPoint("DailyEnergy");
                var PerviousDayEnergy = _repository.GetScadaPoint("PerviousDayEnergy");
                var TotalEnergy = _repository.GetScadaPoint("TotalEnergy");
                DailyEnergy.Value = _1MinEnergy.Value + DailyEnergy.Value;//;+3245.746f;

                TotalEnergy.Value = _1MinEnergy.Value + TotalEnergy.Value;
                //TotalEnergy.Value = 3281.0f;
                //DailyEnergy.Value = 17.0f;
                //PerviousDayEnergy.Value = 100.068f;


                _updateScadaPointOnServer.WriteAnalog(DailyEnergy, DailyEnergy.Value);
                _updateScadaPointOnServer.WriteAnalog(TotalEnergy, TotalEnergy.Value);
                _updateScadaPointOnServer.WriteAnalog(PerviousDayEnergy, PerviousDayEnergy.Value);
                //     _updateScadaPointOnServer.WriteAnalog(AnnualEnergy, AnnualEnergy.Value);
                var CurrentTime = DateTime.Now;

                if (CurrentTime.Hour == 0 && CurrentTime.Minute == 0)
                {
                    _updateScadaPointOnServer.WriteAnalog(PerviousDayEnergy, DailyEnergy.Value);
                    DailyEnergy.Value = 0;

                }
                _logger.WriteEntry($"Energy at {CurrentTime.ToString()} : {DailyEnergy.Value}",LogLevels.Info);

                //if(_repository.GetScadaPoint("ResetDailyEnergy").Value ==1.0 ||( CurrentTime.Hour==0  && CurrentTime.Minute==0))
                //{
                //    DailyEnergy.Value = 0;

                //}
                //if (_repository.GetScadaPoint("ResetAnnualEnergy").Value == 1.0)
                //{
                //    AnnualEnergy.Value = 0;
                //}
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex.Message, LogLevels.Error);
            }


            }
    }
}
