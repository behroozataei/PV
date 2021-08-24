using Irisa.Common;

namespace OCP
{
    public interface IProcessing
    {
        void QualityError(OCPCheckPoint checkpoint, QualityCodes Quality, SinglePointStatus Status);
    }
}
