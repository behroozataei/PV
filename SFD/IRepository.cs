using System;
using System.Data;

namespace SRC_FEED_DETECTION
{
    public interface IRepository
    {
        ScadaPoint GetScadaPoint(Guid measurementId);
        DigitalStatus GetDigitalStatusByScadaName(string name);
        DigitalSingleStatusOnOff DigitalSingleStatusOnOffByScadaName(string name);
        ScadaPoint GetScadaPoint(String name);
        bool ModifyOnHistoricalDB(string sql);
        public bool ModifyOnHistoricalDB(string StoredProcedure, IDbDataParameter[] dbDataParameter);
        DataTable GetFromHistoricalDB(string sql);
    }
}
