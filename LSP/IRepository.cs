using System;
using System.Collections.Generic;
using System.Data;

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

        DataTable FetchItems(byte decisionTableNo);

        DataTable FetchCombinations(byte decisionTableNo);

        DataTable FetchPriorityListsNoForCombinations(byte decisionTableNo);

        DataTable FetchBreakersToShed(byte priorityListNo);

        float GetTANSecondaryActivePower(byte Index);
        int GetTANBusbarPosition(byte Index);
        DataTable FetchPriorityLists();
        DataTable FetchEAFSPriority(string grpNumber, string FurnaceStatus, string strSQLException);
        DataTable FetchReducedPower(int furnaceIndex);
        DataTable FetchEAFsGroup(string sqlQuery);
        DataTable GetFromLinkDB(string sql);
        bool ModifyOnLinkDB(string sql);
        DataTable GetFromHistoricalDB(string sql);
        DataTable GetFromMasterDB(string sql);
        bool ModifyOnHistoricalDB(string sql);
        Guid GetGuid(String networkpath);



    }
}
