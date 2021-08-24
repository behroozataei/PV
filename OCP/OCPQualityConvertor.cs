using Irisa.Common;
using Irisa.Common.Utils;

namespace OCP
{
    internal static class OCPQualityConvertor
    {
        internal static OCPCheckPointQuality GetCheckPointQuality(QualityCodes quality)
        {
            if (QualityCodesProcessing.CheckValueFatal(quality))
                return OCPCheckPointQuality.Invalid;
            else
                return OCPCheckPointQuality.Valid;

            if ((quality & QualityCodes.RemoteInvalid) == QualityCodes.RemoteInvalid)
                return OCPCheckPointQuality.Invalid;
            if ((quality & QualityCodes.RemoteOverflow) == QualityCodes.RemoteOverflow)
                return OCPCheckPointQuality.Invalid;
            if ((quality & QualityCodes.LocalOutOfRange) == QualityCodes.LocalOutOfRange)
                return OCPCheckPointQuality.Invalid;
            if ((quality & QualityCodes.OperandIsSuspect) == QualityCodes.OperandIsSuspect)
                return OCPCheckPointQuality.Invalid;

            // 2021.04.24 A.K and B.A, added these lines:
            if ((quality & QualityCodes.None) == QualityCodes.None)
                return OCPCheckPointQuality.Valid;
            if ((quality & QualityCodes.LocalEntered) == QualityCodes.LocalEntered)
                return OCPCheckPointQuality.Valid;
            if ((quality & QualityCodes.RemoteEntered) == QualityCodes.RemoteEntered)
                return OCPCheckPointQuality.Valid;
            if ((quality & QualityCodes.PowerSubstituted) == QualityCodes.LocalEstimated)
                return OCPCheckPointQuality.Valid;
            if ((quality & QualityCodes.PowerSubstituted) == QualityCodes.PowerSubstituted)
                return OCPCheckPointQuality.Valid;

            // 2021.04.24 A.K and B.A, change from Valid to Invalid:
            return OCPCheckPointQuality.Invalid;
        }
    }
}
