using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            session.SetString(key, jsonString);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var jsonString = session.GetString(key);
            return jsonString == null ? default : JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
