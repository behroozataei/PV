using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetPro.CsRedis;
using CSRedis;

//using ServiceStack.Redis;
//using Microsoft.Extensions.Configuration;

namespace COMMON
{

    public class RedisUtils
    {
        public static RedisUtils MainRTDBManager;
        private static string _password;
        private static string _redisKeySentinel1;
        private static string _redisKeySentinel2;
        private static string _redisKeySentinel3;
        private static string _redisKeySentinel4;
        private static string _redisKeySentinel5;
        private static string _servicename;
        private static string _redisConName1;
        private static string _redisConName2;
        private static string _redisConName3;
        private static string _redisConName4;
        private static string _redisConName5;
        private static int _database = 0;
        public RedisUtils(int database, string RedisKeySentinel1, string RedisKeySentinel2, string RedisKeySentinel3, string RedisKeySentinel4, string RedisKeySentinel5, string password, string servicename,
                                                                  string redisConName1, string redisConName2, string redisConName3, string redisConName4, string redisConName5, string isSentinel)
        {
            try
            {
                _database = database;
                _password = password;
                _redisKeySentinel1 = RedisKeySentinel1;
                _redisKeySentinel2 = RedisKeySentinel2;
                _redisKeySentinel3 = RedisKeySentinel3;
                _redisKeySentinel4 = RedisKeySentinel4;
                _redisKeySentinel5 = RedisKeySentinel5;
                _servicename = servicename;
                _redisConName1 = redisConName1;
                _redisConName2 = redisConName2;
                _redisConName3 = redisConName3;
                _redisConName4 = redisConName4;
                _redisConName5 = redisConName5;

                hasSentinel = Convert.ToBoolean(isSentinel);
                
            }
            catch (Exception ex)
            {

            }
        }


        //****************************************************

        //private static IConnectionMultiplexer RedisConnection;
        public static CSRedisClient RedisConn;

        //public static IServer Server { get; set; }
        //public IDatabase DataBase { get; set; }
        //public static bool IsConnected => RedisConnection != null ? RedisConnection.IsConnected : false;
       // public static bool IsConnected => RedisConnection1!= null ? RedisConnection1.Ping() : false;
        public static bool IsConnected => RedisConn != null ? true : false;
        // public static string RTDBStatus => RedisConnection != null ? RedisConnection.GetStatus() : "";

        public static void CheckConnection()
        {
            if (!IsConnected)
                throw new Exception("RTDB is not connected");
        }

        ~RedisUtils()
        {
            Stop();
        }

        void Stop()
        {
            _isRunning = false;
            RedisConn.Dispose();
        }

        string test(string redisKey)
        {

            return RedisConn.Get(redisKey);
        }

       



        private static List<string> GetEndPoints
        {
            get
            {
                if (hasSentinel)
                {
                    return new List<string>()
                {
                    {_redisKeySentinel1  },
                    {_redisKeySentinel2  },
                    {_redisKeySentinel3  },
                    {_redisKeySentinel4  },
                    {_redisKeySentinel5  },
                };
                }
                else
                {
                    return new List<string>()
                {
                    { _redisConName1},
                    { _redisConName2},
                    { _redisConName3},
                    { _redisConName4},
                    { _redisConName5},
                };
                }
            }
        }

        // [Obsolete]
        public static void RedisUtils_Connect()
        {
            try
            {
                connect();

                if (RedisConn == null)
                    return;
                
            }
            catch
            {
                waitForReconnecting();
            }
        }

       

        //private static Lazy<IConnectionMultiplexer> _Connection = null;
        //[Obsolete]
        //private static void connect()
        //{
        //    try
        //    {

        //        if (_isRunning)
        //        {
        //            return;
        //        }

        //        if (hasSentinel)
        //        {
        //            _Connection = new Lazy<IConnectionMultiplexer>(() =>
        //            {
        //                RTDBConfigSentinel.EndPoints.Clear();
        //                GetEndPoints.ForEach(n => RTDBConfigSentinel.EndPoints.Add(n));
        //                RedisConnection = ConnectionMultiplexer.Connect(RTDBConfigSentinel);
        //                return RedisConnection;
        //            });
        //            RedisConnection = _Connection.Value;


        //        }
        //        else
        //        {
        //            _Connection = new Lazy<IConnectionMultiplexer>(() =>
        //            {
        //                RTDBConfig.EndPoints.Clear();
        //                GetEndPoints.ForEach(n => RTDBConfig.EndPoints.Add(n));
        //                RedisConnection = ConnectionMultiplexer.Connect(RTDBConfig);
        //                return RedisConnection;
        //            });
        //            RedisConnection = _Connection.Value;
        //        }

        //        Ping();


        //    }
        //    catch
        //    {
        //        waitForReconnecting();
        //    }
        //}

       



        

        private static void connect()
        {

            try
            {
                string _connectionstringSentinel = $"{_servicename},password={_password},defaultDatabase={_database},poolsize=500,ssl=false";
                string _connectionstring = $"{_redisConName1},password={_password},defaultDatabase={_database},poolsize=500,ssl=false";

                if (_isRunning)
                {
                    return;
                }

                if (hasSentinel)
                {
                    RedisConn = new CSRedisClient(_connectionstringSentinel, GetEndPoints.ToArray());

                }
                else
                {
                    RedisConn = new CSRedisClient(_connectionstring);
                    
                }
            }
            catch(Exception ex)
            {
                waitForReconnecting();
            }
        }

        static bool hasSentinel;
        static bool _isRunning;

        //internal static void Ping()
        //{
        //    if (IsConnected)
        //        return;


        //    Server?.Ping();
        //    _isRunning = true;
        //}

        private static void waitForReconnecting()
        {
            while (!IsConnected)
            {
                Thread.Sleep(5000);
                if (IsConnected)
                    return;
                
            }
        }

       

        public static IEnumerable<T> StringGet<T>(string[] redisKeys)
        {
            var result = redisKeys.Select(n => RedisConn.Get(n));
            //var result = StringGet(redisKeys);
            return result.Select(n => JsonConvert.DeserializeObject<T>(n));
        }

        public static bool StringGet<T>(string redisKey, ref T value)
        {
            bool ret = false;
            int tryCount = 0;
            while (tryCount < 2)
            {
                try
                {
                    value = JsonConvert.DeserializeObject<T>(RedisConn.Get(redisKey));
                    ret = true;
                    break;

                }
                catch (Exception ex)
                {
                    tryCount++;
                    ret = false;
                    throw ex;                               
                   
                }
            }
            return ret;
        }

        public static String[] GetKeys(String pattern)
        {
            return  RedisConn.Keys(pattern + "*");
           // return RedisConn.KeysAsync(pattern + "*").Result;
                    
        }

        public static bool DelKeys(string pattern)

        {
            bool ret = false;
            try
            {
                var Keys = GetKeys(pattern);

                foreach (var key in Keys)
                    RedisConn.Del(key);
                ret = true;
            }
            catch(Exception ex)
            {
                ret = false;
                throw ex;
            }

            return ret;
            

        }
        
        

    }


}