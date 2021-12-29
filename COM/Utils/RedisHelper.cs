using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace COM
{
    public static class RedisHelper
    {
        //Serialize in Redis format:
        public static HashEntry[] ToHashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties.Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString())).ToArray();
        }
        //Deserialize from Redis format
        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            List<object> objs = new List<object>();
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                objs.Add(Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
                //property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            var obj = Activator.CreateInstance(typeof(T), objs.ToArray());

            return (T)obj;
        }
    }
}
