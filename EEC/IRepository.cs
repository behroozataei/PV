using System;
using System.Collections.Generic;
using System.Data;

using COM;

namespace EEC
{
    internal interface IRepository
    {

        EECScadaPoint GetScadaPoint(Guid guid);
        EECScadaPoint GetScadaPoint(string name);
        bool SendEECTelegramToDC(float RESTIME, float ER_Cycle, float PSend, float PSend1, float PSend2, float m_EnergyResEnd);
        bool ModifyOnHistoricalDB(string sql);
        DataTable GetFromHistoricalDB(string sql);
        public bool ModifyOnHistoricalCache(float[] _BusbarPowers, float[] _FurnacePowers);
        public RedisUtils GetRedisUtiles();
    }
}
