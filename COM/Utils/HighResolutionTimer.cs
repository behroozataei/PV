using System;
using System.Collections.Generic;
using System.Text;

namespace COMMON
{
    public class HighResolutionTimer
    {
        public HighResolutionTimer()
        {
            // Initialize the timer.
            timer = new System.Threading.Timer(TimerCallback,
                null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1));
        }

        private System.Threading.Timer timer;
        private long startTime;
        private long prevTime;

        public void Start()
        {
            // Store the start time.
            startTime = DateTime.Now.Ticks;
            prevTime = startTime;
        }

        public float Stop()
        {
            // Calculate the elapsed time.
            long currentTime = DateTime.Now.Ticks;
            float elapsedTime = (currentTime - startTime) / (float)TimeSpan.TicksPerSecond;

            // Stop the timer.
            timer.Dispose();

            return elapsedTime;
        }

        private void TimerCallback(object state)
        {
            // Calculate the time since the last tick.
            long currentTime = DateTime.Now.Ticks;
            float elapsedTime = (currentTime - prevTime) / (float)TimeSpan.TicksPerSecond;
            prevTime = currentTime;

            // Invoke the tick event handler.
            OnTick(elapsedTime);
        }

        public event Action<float> Tick;

        protected virtual void OnTick(float elapsedTime)
        {
            Tick?.Invoke(elapsedTime);
        }

    }
}
