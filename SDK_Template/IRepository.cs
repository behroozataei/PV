using System;

namespace SDK_Template
{
    public interface IRepository
    {
        SDK_Template_ScadaPoint GetScadaPoint(Guid measurementId);
        DigitalStatus GetDigitalStatusByScadaName(string name);
        SDK_Template_ScadaPoint GetScadaPoint(String name);
    }
}
