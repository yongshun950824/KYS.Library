using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KYS.Library.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Flatten object with dot notation property name.
        /// <br />
        /// Reference: <a href="https://stackoverflow.com/a/63487391">Generically Flatten Json using c#</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<string, object> FlattenObject<T>(T source)
            where T : class, new()
        {
            return JObject.FromObject(source)
                .Descendants()
                .OfType<JValue>()
                .ToDictionary(k => k.Path, v => v.Value<object>());
        }

        public static Dictionary<string, object> FlattenArray<T>(T source)
            where T : JToken
        {
            JObject jObj = new JObject();
            var jTokens = JToken.FromObject(source)
                .Children()
                .OfType<JToken>()
                .ToDictionary(k => k.Path, v => Flatten(v.Value<JToken>()));

            var flattenedTokens = jTokens
                .SelectMany(jToken => jToken.Value.Select(kvp => new { Key = $"{jToken.Key}.{kvp.Key}", Value = kvp.Value }));

            foreach (var token in flattenedTokens)
            {
                jObj.Add(token.Key, JToken.FromObject(token.Value));
            }

            return jObj.ToObject<Dictionary<string, object>>();
        }

        public static Dictionary<string, object> Flatten<T>(T source)
            where T : JToken
        {
            JToken token = JToken.FromObject(source);

            if (token.Type == JTokenType.Object)
            {
                return FlattenObject(JObject.FromObject(source));
            }
            else if (token.Type == JTokenType.Array)
            {
                return FlattenArray(token);
            }

            throw new ApplicationException($"Provided {nameof(source)} is neither a valid JSON object nor array.");
        }
    }
}
