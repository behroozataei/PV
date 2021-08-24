using System;
using System.Collections.Generic;
using System.Data;

namespace EEC
{
    internal interface IRepository
    {
        Dictionary<int, EECEAFPoint> _dEAFsPriority { get; }

        EECEAFPoint GetEAFPoint(int furnace);
        EECScadaPoint GetScadaPoint(Guid guid);
        EECScadaPoint GetScadaPoint(string name);
        bool SendEECTelegramToDC(float RESTIME, float ER_Cycle, float PSend, float PSend1, float PSend2, float m_EnergyResEnd);
        bool ModifyOnHistoricalDB(string sql);
        DataTable GetFromHistoricalDB(string sql);
    }
}
