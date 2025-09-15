using KYS.Library.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace KYS.Library.Helpers
{
    public static partial class JsonHelper
    {
        [GeneratedRegex(@"\[(\d+)\]", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
        private static partial Regex DigitJsonPathReplacementRegex();

        private static readonly JsonSerializer DefaultSerializer = JsonSerializer.Create(
            new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DateParseHandling = DateParseHandling.DateTime,
                DateFormatString = "o", // ISO 8601
                Culture = CultureInfo.InvariantCulture,
                Converters = { new IsoDateTimeConverter() }
            });

        /// <summary>
        /// Flatten object with dot notation property name.
        /// <br />
        /// Reference: <a href="https://stackoverflow.com/a/63487391">Generically Flatten Json using c#</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="flattenFormat"></param>
        /// <returns></returns>
        public static Dictionary<string, object> FlattenObject<T>(T source,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
            where T : class, new()
        {
            return JObject.FromObject(source, DefaultSerializer)
                .Descendants()
                .OfType<JValue>()
                .ToDictionary(k => ConstructFlattenKeyByFormat(k.Path, flattenFormat),
                    v => v.Value<object>());
        }

        public static Dictionary<string, object> FlattenArray<T>(T source,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
            where T : JToken
        {
            var dict = new Dictionary<string, object>();

            foreach (var jToken in JToken.FromObject(source, DefaultSerializer).Children())
            {
                if (jToken is JObject childObj)
                {
                    foreach (var kvp in FlattenObject(childObj, flattenFormat))
                        AddFlattenEntry(dict, $"{jToken.Path}.{kvp.Key}", kvp.Value, flattenFormat);
                }
                else if (jToken is JArray childArr)
                {
                    foreach (var kvp in FlattenArray(childArr, flattenFormat))
                        AddFlattenEntry(dict, $"{jToken.Path}{kvp.Key}", kvp.Value, flattenFormat);
                }
                else
                {
                    AddFlattenEntry(dict, jToken.Path, jToken.Value<JToken>(), flattenFormat);
                }
            }

            return dict;
        }

        public static Dictionary<string, object> Flatten(JToken token,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
        {
            if (token.Type == JTokenType.Object)
            {
                return FlattenObject(token.ToObject<JObject>(), flattenFormat);
            }
            else if (token.Type == JTokenType.Array)
            {
                return FlattenArray(token, flattenFormat);
            }

            throw new ArgumentException($"Provided {nameof(token)} is neither a valid JSON object nor array.");
        }

        private static void AddFlattenEntry(Dictionary<string, object> dict, string key, object value, FlattenFormat flattenFormat)
        {
            key = ConstructFlattenKeyByFormat(key, flattenFormat);

            dict.Add(key, value);
        }

        private static string ConstructFlattenKeyByFormat(string key, FlattenFormat flattenFormat)
        {
            return flattenFormat switch
            {
                FlattenFormat.DotNet =>
                    DigitJsonPathReplacementRegex().Replace(key, ":$1")
                        .Replace(".", ":")
                        .RemovePreFix(":"),
                FlattenFormat.JS =>
                    DigitJsonPathReplacementRegex().Replace(key, ".$1")
                        .RemovePreFix("."),
                _ => key
            };
        }

        public enum FlattenFormat
        {
            JsonPath,
            DotNet,
            JS
        }
    }
}
