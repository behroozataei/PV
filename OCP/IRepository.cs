using Irisa.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;


namespace OCP
{
    internal interface IRepository
    {
        OCPCheckPoint GetCheckPoint(Guid measurementId);
        OCPCheckPoint GetCheckPoint(String name);
        IEnumerable<OCPCheckPoint> GetCheckPoints();

        OCPScadaPoint GetOCPScadaPoint(Guid measurementId);
        OCPScadaPoint GetOCPScadaPoint(String name);
        IEnumerable<OCPScadaPoint> GetOCPScadaPoints();
        bool ModifyOnHistoricalDB(string sql);
        public bool ModifyOnHistoricalDB(string StoredProcedure, IDbDataParameter[] dbDataParameter);
        public DataManager Get_historicalDataManager();
    }
}
