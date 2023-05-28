using COMMON;
using Irisa.DataLayer;
using System;
using System.Data;

namespace EEC
{
    internal interface IRepository
    {

        EECScadaPoint GetScadaPoint(Guid guid);
        EECScadaPoint GetScadaPoint(string name);
        bool SendEECTelegramToDC(float RESTIME, float ER_Cycle, float PSend, float PSend1, float PSend2, float m_EnergyResEnd);
        bool ModifyOnHistoricalDB(string sql);
        public bool ModifyOnHistoricalDB(string StoredProcedure, IDbDataParameter[] dbDataParameter);
        DataTable GetFromHistoricalDB(string sql);
        public bool ModifyOnHistoricalCache(float[] _BusbarPowers, float[] _FurnacePowers);
        public DataManager Get_historicalDataManager();
        public RedisUtils GetRedisUtiles();
        
    }
}
