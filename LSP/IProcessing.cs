using Irisa.Message.CPS;
using System;


namespace LSP
{
    public interface IProcessing
    {
        //void AnalogMeasurements(AnalogRuntimeData analogRuntimeData);
        //void CounterMeasurements(CounterRuntimeData counterRuntimeData);
        //void DigitalMeasurement(DigitalRuntimeData digitalRuntimeData);
        public void SCADAEventRaised(MeasurementData measurement);

    }
}
