using System;

namespace MAB
{
    public interface IRepository
    {
        MABScadaPoint GetScadaPoint(Guid measurementId);
        DigitalStatus GetDigitalStatusByScadaName(string name);
        DigitalSingleStatusOnOff DigitalSingleStatusOnOffByScadaName(string name);
        MABScadaPoint GetScadaPoint(String name);
    }
}
