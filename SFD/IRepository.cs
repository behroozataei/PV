using System;

namespace SRC_FEED_DETECTION
{
    public interface IRepository
    {
        ScadaPoint GetScadaPoint(Guid measurementId);
        DigitalStatus GetDigitalStatusByScadaName(string name);
        DigitalSingleStatusOnOff DigitalSingleStatusOnOffByScadaName(string name);
        ScadaPoint GetScadaPoint(String name);
    }
}
