using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace Library.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var jsonString = JsonSerializer.Serialize(value);
            session.SetString(key, jsonString);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var jsonString = session.GetString(key);
            return jsonString == null ? default : JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
