namespace OCP
{
    public enum OCPCheckPointQuality
    {
        Valid = 1,
        Calculated,
        Previous,
        Invalid
    }

    public enum DigitalDoubleStatus
    {
        Intransit = 0,
        Open = 1,
        Close = 2,
        Disturb = 3
    }

    public enum eResetType
    {
        NotInitialized = 0,
        ResetAll = 1, // Reset IT of all shedponts
        ResetSome = 2 // Reset only ponts which ResetIT falg is set
    }

    public enum TransSideOverload
    {
        Primary = 1,
        Secondary,
        Both
    }

}
