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
using System.NetPro;

//using ServiceStack.Redis;
//using Microsoft.Extensions.Configuration;

namespace COMMON
{

    public class RedisUtils
    {
        private static RedisUtils MainRTDBManager = null;
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
        bool Connectionstat = true;
        private RedisUtils()
        {
           
        }
        private static readonly object Instancelock = new object();
        public static RedisUtils GetRedisUtils()
        {
            lock (Instancelock)
            {
                if (MainRTDBManager == null)
                {
                    MainRTDBManager = new RedisUtils();
                    MainRTDBManager.connect();
                }
                return MainRTDBManager;
            }
        }

        public static void SetRedisUtilsParams(int database, string RedisKeySentinel1, string RedisKeySentinel2, string RedisKeySentinel3, string RedisKeySentinel4, string RedisKeySentinel5, string password, string servicename,
                                                                 string redisConName1, string redisConName2, string redisConName3, string redisConName4, string redisConName5, string isSentinel)
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




        //****************************************************

        //private static IConnectionMultiplexer RedisConnection;
        public  CSRedisClient RedisConn;

        //public static IServer Server { get; set; }
        //public IDatabase DataBase { get; set; }
        //public static bool IsConnected => RedisConnection != null ? RedisConnection.IsConnected : false;
       // public static bool IsConnected => RedisConnection1!= null ? RedisConnection1.Ping() : false;
        public  bool IsConnected => RedisConn!= null ? true : false;
        // public static string RTDBStatus => RedisConnection != null ? RedisConnection.GetStatus() : "";

        public  bool CheckConnection(string APP_Connection)
        {
            bool result = false;
            Connectionstat = !Connectionstat;
            if (!RedisConn.Set(APP_Connection, Connectionstat))
            {
                return false;
                //throw new Exception("RTDB is not connected");
            }
            result = true;
            return result;
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

       



        private  List<string> GetEndPoints
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
        public  void RedisUtils_Connect()
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

       



        

        private  void connect()
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

        private  void waitForReconnecting()
        {
            while (!IsConnected)
            {
                Thread.Sleep(5000);
                if (IsConnected)
                    return;
                else
                {
                    Stop();
                    RedisUtils_Connect();
                }
                
            }
        }

       

        public  IEnumerable<T> StringGet<T>(string[] redisKeys)
        {
            var result = redisKeys.Select(n => RedisConn.Get(n));
            //var result = StringGet(redisKeys);
            return result.Select(n => JsonConvert.DeserializeObject<T>(n));
        }

        public  bool StringGet<T>(string redisKey, ref T value)
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

        public  String[] GetKeys(String pattern)
        {
            return  RedisConn.Keys(pattern + "*");
           // return RedisConn.KeysAsync(pattern + "*").Result;
                    
        }

        public  bool DelKeys(string pattern)

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