using Irisa.Common;
using Irisa.Logger;
using Irisa.Message;
using System;
using System.Timers;

namespace SDK_Template
{
    public class SDK_Template_Manager : IProcessing
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;
        private const int TIMER_4Sec_TICKS = 4000;
        private readonly Timer _timer_4_Sec;

        internal SDK_Template_Manager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);
            _timer_4_Sec = new Timer();
            _timer_4_Sec.Interval = TIMER_4Sec_TICKS;
            _timer_4_Sec.Elapsed += Function4;
            _timer_4_Sec.Start();
        }

        // Write to Scada
        public void Function1()
        {
            //var scada_point_test1 = _repository.GetScadaPoint("SCADA_POINT_TEST1");
            //if (!_updateScadaPointOnServer.WriteSample(scada_point_test1, 1.0f))
            //{
            //    _logger.WriteEntry("Write to SCADA_POINT_TEST fail!", LogLevels.Error);
            //}

        }

        // Read From Scada
        public void Function2()
        {
            SDK_Template_ScadaPoint scada_point_test2 ;
            //scada_point_test2 = _repository.GetScadaPoint("SCADA_POINT_TEST2");
            //var test2 = scada_point_test2.Value;
        }

       
        public void Function3()
        {

        }

        // Timer handler
        public void Function4(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("time = "+DateTime.Now);

        }

        // Receive changed data from CPS
        void IProcessing.Process_Function1()
        {
            Function2();
            Function1();
        }
    }
}
