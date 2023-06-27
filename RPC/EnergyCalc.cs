using Google.Protobuf.WellKnownTypes;
using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using Irisa.Message.CPS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace RPC
{
    internal class EnergyCalc
    {

        private readonly IRepository _repository;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private readonly ILogger _logger;
        private readonly Timer _timer;

        private const int ACC_TIMER_TICKS = 60000;

        public EnergyCalc(ILogger logger, IRepository repository, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _logger = logger;
            _updateScadaPointOnServer = updateScadaPointOnServer;
            _repository = repository;
            _timer = new Timer();
            _timer.Interval = ACC_TIMER_TICKS;
            _timer.Elapsed += RunCyclicOperation;

        }

        public void Start()
        {
            _timer.Start();
        }

        private void RunCyclicOperation(object sender, ElapsedEventArgs e)
        {
            foreach (var accScadaPoint in _repository.accScadaPoint)
                Energy1Min(accScadaPoint.Value);
        }

        public void TotalEnergy(ACCScadaPoint accScadaPoint)
        {
            if (accScadaPoint.t1.Date.Year == 1900)
            {
                accScadaPoint.t1 = DateTime.UtcNow;
                accScadaPoint.t2 = DateTime.UtcNow;
                accScadaPoint.Value_t1 = accScadaPoint.sPCScadaPoint.Value;
                accScadaPoint.Value_t2 = accScadaPoint.sPCScadaPoint.Value;
            }
            else
            {
                accScadaPoint.Value_t1 = accScadaPoint.Value_t2;
                accScadaPoint.t1 = accScadaPoint.t2;
                accScadaPoint.Value_t2 = accScadaPoint.sPCScadaPoint.Value;
                accScadaPoint.t2 = DateTime.UtcNow;
                TimeSpan ts = accScadaPoint.t2 - accScadaPoint.t1;
                accScadaPoint.Energy += (accScadaPoint.Value_t1 + accScadaPoint.Value_t2) * (ts.TotalMilliseconds) / (2 * 3600000);
            }

        }

        public void Energy1Min(ACCScadaPoint accScadaPoint)
        {

            TotalEnergy(accScadaPoint);
            if (!_updateScadaPointOnServer.WriteAnalog(_repository.GetRPCScadaPoint(accScadaPoint.aPCScadaPoint.Name), (float)accScadaPoint.Energy))
            {
                _logger.WriteEntry($"Could not update value in SCADA:{accScadaPoint.aPCScadaPoint.Name} ", LogLevels.Error);
            }
            accScadaPoint.Energy = 0.0;
        }


    }
}
