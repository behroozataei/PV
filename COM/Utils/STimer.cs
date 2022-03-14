using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace COM
{

    public delegate void Tdelegate(object sender, ElapsedEventArgs e);
    public class STimer
    {
        public event Tdelegate Elapsed;
        private double deviation;
        private bool timerstarted;
        public double Interval { get; set; }
        public STimer()
        {
            timer = new Timer();    
        }

        private Timer timer;
       
        public void Start()
        {
            timer.Start();
            timer.Interval = Interval;
            timer.Elapsed += OnTimerElapsed;
        }

        public void Stop()
        {
            timerstarted = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {

            timer.Interval = Interval;
            if (timerstarted)
            {
                var ms = System.DateTime.Now.Millisecond;
                if (ms > 10 && ms < 200)
                    deviation = ms;
                if (ms > 800 && ms < 1000)
                    deviation = ms - 1000;

                if (Math.Abs(deviation) > 0 && Math.Abs(deviation) < 200)
                {
                    timer.Interval = Interval - deviation; ;
                }

            }
            timerstarted = true;
            if (Elapsed != null)
                Elapsed( sender, e);
        }

        


    }
}
