namespace SRC_FEED_DETECTION
{
    public interface IProcessing
    {
        void Update_VoltageSources();
        bool GetCPSStatus();
        void SetCPSStatus(bool State);
    }
}
