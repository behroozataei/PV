using Irisa.Message.CPS;


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
