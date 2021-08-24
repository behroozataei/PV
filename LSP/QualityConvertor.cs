using Irisa.Common;

namespace LSP
{
    internal static class QualityConvertorCPSToCom
    {
        internal static CheckPointQuality GetCheckPointQuality(QualityCodes quality)
        {
            if ((quality & QualityCodes.RemoteInvalid) == QualityCodes.RemoteInvalid)
                return CheckPointQuality.Invalid;
            if ((quality & QualityCodes.RemoteOverflow) == QualityCodes.RemoteOverflow)
                return CheckPointQuality.Invalid;
            if ((quality & QualityCodes.LocalOutOfRange) == QualityCodes.LocalOutOfRange)
                return CheckPointQuality.Invalid;
            if ((quality & QualityCodes.OperandIsSuspect) == QualityCodes.OperandIsSuspect)
                return CheckPointQuality.Invalid;

            return CheckPointQuality.Valid;
        }
    }
}
