using KYS.Library.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for the JSON.
    /// </summary>
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
        /// Flatten the object into a key-value pair with the key based on the <c>flattenFormat</c> given. 
        /// <br />
        /// Default key format: <see cref="FlattenFormat.JsonPath"/>.
        /// <br />
        /// <br />
        /// Reference: <a href="https://stackoverflow.com/a/63487391">Generically Flatten Json using c#</a>
        /// </summary>
        /// <typeparam name="T">The type of <c>source</c>. Must be a reference type with a public parameterless constructor.</typeparam>
        /// <param name="source">The provided value to be flatten.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>The key value pair for the flatten JSON.</returns>
        public static Dictionary<string, object> FlattenObject<T>(T source,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
            where T : class, new()
        {
            return JObject.FromObject(source, DefaultSerializer)
                .Descendants()
                .OfType<JValue>()
                .ToDictionary(k => ConstructFlattenKey(k.Path, flattenFormat),
                    v => v.Value);
        }

        /// <summary>
        /// Flatten the array into a key-value pair with the key based on the <c>flattenFormat</c> given. 
        /// <br />
        /// Default key format: <see cref="FlattenFormat.JsonPath"/>.
        /// </summary>
        /// <typeparam name="T">The type of <c>source</c> which is a <see cref="JToken" /> type.</typeparam>
        /// <param name="source">The provided value to be flatten.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>The key value pair for the flatten JSON.</returns>
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
                    AddFlattenEntry(dict, jToken.Path, ((JValue)jToken).Value, flattenFormat);
                }
            }

            return dict;
        }

        /// <summary>
        /// Flatten the <see cref="JToken" /> instance into a key-value pair with the key based on the <c>flattenFormat</c> given. 
        /// <br />
        /// Default key format: <see cref="FlattenFormat.JsonPath"/>.
        /// </summary>
        /// <param name="token">The <see cref="JToken" /> instance to be flatten.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>The key value pair for the flatten JSON.</returns>
        /// <exception cref="ArgumentException"></exception>
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
            key = ConstructFlattenKey(key, flattenFormat);

            dict.Add(key, value);
        }

        /// <summary>
        /// Construct the key based on the <c>flattenFormat</c> given.
        /// </summary>
        /// <param name="key">The <see cref="string" /> for the key.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>The key after applying the <c>flattenFormat</c>.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static string ConstructFlattenKey(string key, FlattenFormat flattenFormat)
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
                FlattenFormat.JsonPath => key,
                _ => throw new InvalidEnumArgumentException(nameof(flattenFormat), (int)flattenFormat, typeof(FlattenFormat))
            };
        }

        /// <summary>
        /// Represents the available flatten format option for the key.
        /// </summary>
        public enum FlattenFormat
        {
            JsonPath,
            DotNet,
            JS
        }
    }
}
