namespace RPC
{
    public interface IProcessing
    {
        void SCADAEventRaised(RPCScadaPoint scadaPoint);
        void Integrator(RPCScadaPoint scadaPoint);
        void AlarmAcked_Processing(RPCScadaPoint scadaPoint);
    }
}
