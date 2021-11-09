using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCP
{
    public static class RedisHelperList
    {
        //private static ConnectionMultiplexer _cnn;
        //private string key;
        //public RedisList(string key, ConnectionMultiplexer cnn)
        //{
        //    this.key = key;
        //    _cnn = cnn;
        //}
        //private static IDatabase GetRedisDb(this ConnectionMultiplexer cnn)
        //{
        //    return cnn.GetDatabase();
        //}
        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        private static T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        public static void Insert<T>(int index, T item, IDatabase db, string key)
        {
            //var db = GetRedisDb(cnn);
            var before = db.ListGetByIndex(key, index);
            db.ListInsertBefore(key, before, Serialize(item));
        }
        public static void RemoveAt(int index, IDatabase db, string key)
        {
            //var db = GetRedisDb(cnn);
            var value = db.ListGetByIndex(key, index);
            if (!value.IsNull)
            {
                db.ListRemove(key, value);
            }
        }
        public static T GetItem<T>(int index, IDatabase db, string key)
        {

            var value = db.ListGetByIndex(key, index);
            return Deserialize<T>(value.ToString());
        }

        public static void SetItem<T>(int index, T value, IDatabase db, string key)
        {
            Insert(index, value, db, key);
        }

        public static void Add<T>(T item, IDatabase db, string key)
        {
            db.ListRightPush(key, Serialize(item));
        }
        public static void Clear(IDatabase db, string key)
        {
            db.KeyDelete(key);
        }
        public static bool Contains<T>(T item, IDatabase db, string key)
        {
            for (int i = 0; i < Count(db, key); i++)
            {
                if (db.ListGetByIndex(key, i).ToString().Equals(Serialize(item)))
                {
                    return true;
                }
            }
            return false;
        }
        public static void CopyTo<T>(T[] array, int arrayIndex, IDatabase db, string key)
        {
            db.ListRange(key).CopyTo(array, arrayIndex);
        }
        public static int IndexOf<T>(T item, IDatabase db, string key)
        {
            for (int i = 0; i < Count(db, key); i++)
            {
                if (db.ListGetByIndex(key, i).ToString().Equals(Serialize(item)))
                {
                    return i;
                }
            }
            return -1;
        }
        public static int Count(IDatabase db, string key)
        {
            return (int)db.ListLength(key);
        }
        public static bool IsReadOnly
        {
            get { return false; }
        }
        public static bool Remove<T>(T item, IDatabase db, string key)
        {
            return db.ListRemove(key, Serialize(item)) > 0;
        }
        public static IEnumerator<T> GetEnumerator<T>(IDatabase db, string key)
        {
            for (int i = 0; i < Count(db, key); i++)
            {
                yield return Deserialize<T>(db.ListGetByIndex(key, i).ToString());
            }
        }

        public static IEnumerable<T> GetEnumerable<T>(IDatabase db, string key)
        {
            var count = Count(db, key);
            for (int i = 0; i < count; i++)
            {
                yield return Deserialize<T>(db.ListGetByIndex(key, i).ToString());
            }
        }
    }
}
