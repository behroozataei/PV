namespace LSP
{
    public enum CheckPointQuality
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

    public enum LSPActivation
    {
        NotInitialized = 0,
        ResetOverload = 1,          // Only by LSP
        SetOverload = 2             // Only by OCP
    }
}
