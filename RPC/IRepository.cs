using COMMON;
using System;
using System.Collections.Generic;

namespace RPC
{
    internal interface IRepository
    {
        
        RPCScadaPoint GetRPCScadaPoint(Guid measurementId);
        RPCScadaPoint GetRPCScadaPoint(String name);
        public RedisUtils GetRedisUtiles();
        public bool TryGetHISAverageinIntervalTime(RPCScadaPoint scadaPoint, IntervalTime duration, out float value);
        public void PrintRepository();
        Dictionary<string,ACCScadaPoint> accScadaPoint { get; }
        public ACCScadaPoint GetAccScadaPoint(string name);


    }
}
