using COMMON;
using System;
using System.Collections.Generic;
using System.Data;

using Irisa.DataLayer;

namespace LSP
{
    internal interface IRepository
    {
        OCPCheckPoint GetCheckPoint(Guid measurementId);
        OCPCheckPoint GetCheckPoint(String name);
        OCPCheckPoint GetCheckPoint(int checkPointNumber);
        IEnumerable<OCPCheckPoint> GetCheckPoints();

        LSPScadaPoint GetLSPScadaPoint(Guid measurementId);
        LSPScadaPoint GetLSPScadaPoint(String name);

        IEnumerable<LSPScadaPoint> GetLSPScadaPoints();

        DataTable FetchDecisionTables();

        IEnumerable<LSP_DECTITEMS_Str> FetchItems(byte decisionTableNo);

        DataTable FetchCombinations(byte decisionTableNo);

        DataTable FetchPriorityListsNoForCombinations(byte decisionTableNo);

        IEnumerable<LSP_PRIORITYITEMS_Str> FetchBreakersToShed(byte priorityListNo);

        float GetTANSecondaryActivePower(byte Index);
        int GetTANBusbarPosition(byte Index);
        DataTable FetchPriorityLists();
        FetchEAFSPriority_Str[] FetchEAFSPriority(string grpNumber, string FurnaceStatus, List<string> Exception);
        DataTable FetchReducedPower(int furnaceIndex);
        DataTable FetchEAFsGroup(string sqlQuery);
        DataTable GetFromLinkDB(string sql);
        bool ModifyOnLinkDB(string sql);
        DataTable GetFromHistoricalDB(string sql);
        DataTable GetFromMasterDB(string sql);
        bool ModifyOnHistoricalDB(string sql);
        public bool ModifyOnHistoricalDB(string StoredProcedure, IDbDataParameter[] dbDataParameter);
        Guid GetGuid(String networkpath);
        public RedisUtils GetRedisUtiles();
        public SFSC_EAFSPOWER_Str GetFromHistoricalCache();
        public DataManager Get_historicalDataManager();




    }
}
