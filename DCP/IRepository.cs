using COM;
using System;
using System.Data;

namespace DCP
{
    public interface IRepository
    {
        DCPScadaPoint GetScadaPoint(Guid measurementId);
        DigitalStatus GetDigitalStatusByScadaName(string name);
        DCPScadaPoint GetScadaPoint(string name);
        int GetMAxFKeyEndedFurnace(int oldMAxFKey);
        bool WriteTimeNowToFurnace(int I);
        bool ClearFromFurnace(int I);
        bool UpdateFurnace(int I, string time1, string TotalConsumption);
        bool UpdateEAFSComnsumption(int I, string eafConsumption);
        //DataTable GetFromMasterDB(string sql);
        DataTable GetFromLinkDB(string sql);
        //DataTable GetEECTELEGRAM();
        //bool UpdateEECTELEGRAM(string atime, string telDate);
        bool InsertTELEGRAM(string sql);
        DataTable GetVMABEAFSGRPNUM();
        DataTable GetEAFGROUPREQUEST();
        bool UpdateEAFGroupRequest();
        bool ModifyOnLinkDB(string sql);
        float GetPowerSumationFromT_EAFsPower(string sql);
        bool ModifyOnStaticDB(string sql);
        bool ModifyOnHistoricalDB(string sql);
        //DataTable GetFromHistoricalDB(string sql);
        SFSC_EAFSPOWER_Str GetFromHistoricalCache();
        public RedisUtils GetRedisUtiles();


    }
}