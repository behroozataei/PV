﻿using COM;
using StackExchange.Redis;
using System;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CLR
{
    class Program
    {
        private RedisUtils _RedisConnectorHelper;
        static void Main(string[] args)
        {
            int aSec = DateTime.Now.Second;
            int mSec = DateTime.Now.Millisecond;
                       
            System.Threading.Thread.Sleep((3000 - ((aSec % 3) * 1000 + mSec)));


            Program p1 = new Program();

            p1.menu();
            while (!p1.run())
            {
                Console.WriteLine("Error");
                p1.menu();
            }

            Console.ReadKey();

        }

       
        void menu()
        {
            Console.Clear();
            Console.WriteLine("Clear Application Tables from Redis");
            Console.WriteLine("Please Select an Aplication number to clear these tables");
            Console.WriteLine(" 1- OCP");
            Console.WriteLine(" 2- LSP");
            Console.WriteLine(" 3- MAB");
            Console.WriteLine(" 4- DCP");
            Console.WriteLine(" 5- EEC");
            Console.WriteLine(" 6- OPC");
            Console.WriteLine(" 7- ALL");
            Console.WriteLine(" 8- Exit");
        }

        

        bool run()
        {
            JObject _config = JObject.Parse(File.ReadAllText(@"appsettings.json"));
            _RedisConnectorHelper = new RedisUtils();
            _RedisConnectorHelper.RedisUtils_Connect(0, _config.GetValue("RedisKeySentinel1").ToString(),
                                                        _config.GetValue("RedisKeySentinel2").ToString(),
                                                        _config.GetValue("RedisKeySentinel3").ToString(),
                                                        _config.GetValue("RedisKeySentinel4").ToString(),
                                                        _config.GetValue("RedisKeySentinel5").ToString(),
                                                        _config.GetValue("RedisPassword").ToString(),
                                                        _config.GetValue("RedisServiceName").ToString());

            char sel;
            RedisKey[] appkeys = null;
            bool ret = false;
            bool exit = false;
            bool notselected = false;
            sel = Console.ReadKey(true).KeyChar;
            switch (sel)
            {
                case '1':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:OCP_PARAMS");
                    break;

                case '2':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:LSP_PARAMS");
                    break;

                case '3':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:MAB_PARAMS");
                    break;

                case '4':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:DCP_PARAMS");
                    break;

                case '5':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:EEC_PARAMS");
                    break;

                case '6':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:OPC_PARAMS");
                    break;

                case '7':
                    appkeys = _RedisConnectorHelper.GetKeys("APP:*");
                    break;

                case '8':
                    exit = true;
                    ret = true;
                    break;
                default:
                    notselected = true;
                    ret = false;
                    break;
            }
            if (!(exit == true || notselected == true))
                foreach (RedisKey key in appkeys)
                    ret = _RedisConnectorHelper.DataBase.KeyDelete(key);
            return ret;


        }
    }
}
