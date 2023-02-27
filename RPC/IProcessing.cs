namespace RPC
{
    public interface IProcessing
    {
        void SCADAEventRaised(RPCScadaPoint scadaPoint);
    }
}
