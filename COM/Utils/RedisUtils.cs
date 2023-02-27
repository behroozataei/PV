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

namespace COM
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
                //RTDBConfigSentinel.Password = password;
                //RTDBConfigSentinel.ServiceName = servicename;
                //RTDBConfig.Password = password;

            }
            catch (Exception ex)
            {

            }
        }


        //****************************************************

        //private static IConnectionMultiplexer RedisConnection;
        public static CSRedisClient RedisConnection1;

        //public static IServer Server { get; set; }
        //public IDatabase DataBase { get; set; }
        //public static bool IsConnected => RedisConnection != null ? RedisConnection.IsConnected : false;
       // public static bool IsConnected => RedisConnection1!= null ? RedisConnection1.Ping() : false;
        public static bool IsConnected => RedisConnection1 != null ? true : false;
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
            RedisConnection1.Dispose();
        }

        string test(string redisKey)
        {

            return RedisConnection1.Get(redisKey);
        }

        [Obsolete]
        //static ConfigurationOptions RTDBConfigCommon = new ConfigurationOptions
        //{
        //    AbortOnConnectFail = false,
        //    AllowAdmin = true,
        //    //CheckCertificateRevocation = false,
        //    //ConfigCheckSeconds = 5000,
        //    //ConnectRetry = 5,
        //    ConnectTimeout = 100000,
        //    //KeepAlive = 60,
        //    SyncTimeout = 60000,
        //    AsyncTimeout = 60000,
        //    ResponseTimeout = 60000,
        //    //ReconnectRetryPolicy = new LinearRetry(5000),
        //    //DefaultVersion = new Version(2, 6, 90)
        //    SocketManager = SocketManager.ThreadPool,
        //    // Ssl = true 
        //    HighPrioritySocketThreads = true


        //};





        //[Obsolete]
        //private static ConfigurationOptions RTDBConfig { get; } = new ConfigurationOptions
        //{
        //    AbortOnConnectFail = RTDBConfigCommon.AbortOnConnectFail,
        //    AllowAdmin = RTDBConfigCommon.AllowAdmin,
        //    ConfigCheckSeconds = RTDBConfigCommon.ConfigCheckSeconds,
        //    ConnectRetry = RTDBConfigCommon.ConnectRetry,
        //    ConnectTimeout = RTDBConfigCommon.ConnectTimeout,
        //    SyncTimeout = RTDBConfigCommon.SyncTimeout,
        //    ResponseTimeout = RTDBConfigCommon.ResponseTimeout,
        //    SocketManager = RTDBConfigCommon.SocketManager,
        //    HighPrioritySocketThreads = RTDBConfigCommon.HighPrioritySocketThreads
        //};

        //[Obsolete]
        //private static ConfigurationOptions RTDBConfigSentinel { get; } = new ConfigurationOptions
        //{
        //    CommandMap = CommandMap.Default,
        //    AbortOnConnectFail = RTDBConfigCommon.AbortOnConnectFail,
        //    AllowAdmin = RTDBConfigCommon.AllowAdmin,
        //    ConfigCheckSeconds = RTDBConfigCommon.ConfigCheckSeconds,
        //    ConnectRetry = RTDBConfigCommon.ConnectRetry,
        //    ConnectTimeout = RTDBConfigCommon.ConnectTimeout,
        //    KeepAlive = RTDBConfigCommon.KeepAlive,
        //    SyncTimeout = RTDBConfigCommon.SyncTimeout,
        //    AsyncTimeout = RTDBConfigCommon.AsyncTimeout,
        //    ResponseTimeout = RTDBConfigCommon.ResponseTimeout,
        //    ReconnectRetryPolicy = RTDBConfigCommon.ReconnectRetryPolicy,
        //    DefaultVersion = RTDBConfig.DefaultVersion,
        //    SocketManager = RTDBConfigCommon.SocketManager,
        //    HighPrioritySocketThreads = RTDBConfigCommon.HighPrioritySocketThreads

        //};



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

                if (RedisConnection1 == null)
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
                string _connectionstringSentinel = $"{_servicename},password={_password},defaultDatabase=0,poolsize=500,ssl=false";
                string _connectionstring = $"{_redisConName1},password={_password},defaultDatabase=0,poolsize=500,ssl=false";

                if (_isRunning)
                {
                    return;
                }

                if (hasSentinel)
                {
                    RedisConnection1 = new CSRedisClient(_connectionstringSentinel, GetEndPoints.ToArray());

                }
                else
                {
                    RedisConnection1 = new CSRedisClient(_connectionstring);
                    
                }
            }
            catch
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
            var result = redisKeys.Select(n => RedisConnection1.Get(n));
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
                    value = JsonConvert.DeserializeObject<T>(RedisConnection1.Get(redisKey));
                    ret = true;
                    break;

                }
                catch (Exception ex)
                {
                    tryCount++;
                    ret = false;
                    Console.WriteLine(ex.Message);
                }
            }
            return ret;
        }

        public static String[] GetKeys(String pattern)
        {
            return RedisConnection1.KeysAsync(pattern + "*").Result;

          // return RedisConnection1.HKeys(pattern);
           // return Server.Keys(_database, pattern: pattern + "*").ToArray();
        }

        
        

    }


}