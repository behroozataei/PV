using COM;
using System;
using System.Collections.Generic;

namespace RPC
{
    internal interface IRepository
    {
        
        RPCScadaPoint GetRPCScadaPoint(Guid measurementId);
        RPCScadaPoint GetRPCScadaPoint(String name);
        public RedisUtils GetRedisUtiles();

    }
}
