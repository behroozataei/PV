using Irisa.Message.CPS;

namespace EEC
{
    public interface IProcessing
    {
        void Check_Apply_EECConst(MeasurementData measurement);
        void Set_EEC_MAB_Status(EECScadaPoint scadaPoint);
        void AlarmAcked_Processing(EECScadaPoint scadaPoint); 
    }
}
