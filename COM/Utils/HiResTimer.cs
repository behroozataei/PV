using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace COM
{
    public class HiResTimer
    {
        // The number of ticks per one millisecond.
        private static readonly float tickFrequency = 1000f / Stopwatch.Frequency;

        public event EventHandler<HiResTimerElapsedEventArgs> Elapsed;

        private volatile float interval;
        private volatile bool isRunning;
        public float ignoreElapsedThreshold = 10f;
        private int fallouts;
        private bool activation_in_specified_second = false;

        public HiResTimer() : this(1f)
        {
        }

        public HiResTimer(float interval)
        {
            if (interval < 0f || Single.IsNaN(interval))
                throw new ArgumentOutOfRangeException(nameof(interval));
            this.interval = interval;
        }

        public HiResTimer(float interval, bool Activation_in_specified_second)
        {
            if (interval < 0f || Single.IsNaN(interval))
                throw new ArgumentOutOfRangeException(nameof(interval));
            this.interval = interval;
            this.activation_in_specified_second = Activation_in_specified_second;
        }

        // The interval in milliseconds. Fractions are allowed so 0.001 is one microsecond.
        public float Interval
        {
            get { return interval; }
            set
            {
                if (value < 0f || Single.IsNaN(value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                interval = value;
            }
        }

        public bool Enabled
        {
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
            get { return isRunning; }
        }

        public void Start()
        {
            if (isRunning)
                return;

            isRunning = true;
            Thread thread = new Thread(ExecuteTimer);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void ExecuteTimer()
        {
            float nextTrigger = 0f;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            float delta = 0;

            if (activation_in_specified_second)
            {
                var sec = DateTime.Now.Second;
                var millisec = DateTime.Now.Millisecond;
                delta = interval - (sec % (interval / 1000)) * 1000 - millisec + 1;
                nextTrigger = delta;
            }
            // Console.WriteLine($"DateTime = {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --------------- delta = {delta}");

            bool firstcycle = true;
            bool cycle_time_corr = false;
            while (isRunning)
            {
                float intervalLocal = interval;
                nextTrigger += intervalLocal;
                float elapsed = 0;
                float diff = 0f;
                float diff1 = 0f;
                float diff2 = 0f;

                //elapsed = ElapsedHiRes(stopwatch);
                //diff1 = nextTrigger - elapsed;

                //var sec = DateTime.Now.Second;
                //var millisec = DateTime.Now.Millisecond;
                //diff2 = intervalLocal - (sec % (intervalLocal / 1000)) * 1000 - millisec;

                //if ((Math.Abs(diff2) > 30) && Math.Abs(diff1) < intervalLocal && !firstcycle && activation_in_specified_second)
                //    nextTrigger -= diff2 - diff1;

                while (true)
                {

                    elapsed = ElapsedHiRes(stopwatch);
                    diff = nextTrigger - elapsed;

                   // Console.WriteLine($"DateTime = {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --------------- Diff1 = {diff1}, diff2 = {diff2}, elapsed = {elapsed}, nextTrigger = {nextTrigger}");

                    if (diff <= 0f)
                        break;


                    if (diff < 1f)
                        Thread.SpinWait(10);
                    else if (diff < 10f)
                        Thread.SpinWait(100);
                    else
                    {
                        // By default Sleep(1) lasts about 15.5 ms (if not configured otherwise for the application by WinMM, for example)
                        // so not allowing sleeping under 16 ms. Not sleeping for more than 50 ms so interval changes/stopping can be detected.
                        if (diff >= 16f)
                        {
                            Thread.Sleep(diff >= 100f ? 50 : 1);

                        }
                        else
                        {
                            Thread.SpinWait(1000);
                            Thread.Sleep(0);
                        }

                        // if we have a larger time to wait, we check if the interval has been changed in the meantime
                        float newInterval = interval;

                        if (intervalLocal != newInterval)
                        {
                            nextTrigger += newInterval - intervalLocal;
                            intervalLocal = newInterval;
                        }
                    }

                    if (!isRunning)
                        return;
                }
                firstcycle = false;

                float delay = elapsed - nextTrigger;

                if (delay >= ignoreElapsedThreshold)
                {
                    fallouts += 1;
                    //  Elapsed?.Invoke(this, new HiResTimerElapsedEventArgs(delay, fallouts));
                    continue;
                }

                Elapsed?.Invoke(this, new HiResTimerElapsedEventArgs(delay, fallouts));
                fallouts = 0;

                // restarting the timer in every hour to prevent precision problems
                if (stopwatch.Elapsed.TotalHours >= 1d)
                {
                    stopwatch.Restart();
                    nextTrigger = 0f;
                }
            }

            stopwatch.Stop();
        }

        private static float ElapsedHiRes(Stopwatch stopwatch)
        {
            return stopwatch.ElapsedTicks * tickFrequency;
        }
    }

    public class HiResTimerElapsedEventArgs : EventArgs
    {
        public float Delay { get; }
        public int Fallouts { get; }

        internal HiResTimerElapsedEventArgs(float delay, int fallouts)
        {
            Delay = delay;
            Fallouts = fallouts;
        }
    }
}