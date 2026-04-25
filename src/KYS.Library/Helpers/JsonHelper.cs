using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using KYS.Library.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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
        /// <returns>A <see cref="Result{Dictionary{string, object}}" /> containing the key value pair for the flatten JSON.</returns>
        public static Result<Dictionary<string, object>> FlattenObject<T>(T source,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
            where T : class, new()
        {
            return ValidateFlattenFormat(flattenFormat)
                .Bind(() => Result.Try(
                    () => Result.Success(FlattenObjectCore(source, flattenFormat)),
                    ex => ex.Message
                ))
                .Bind(result => result);
        }

        /// <summary>
        /// Flatten the array into a key-value pair with the key based on the <c>flattenFormat</c> given. 
        /// <br />
        /// Default key format: <see cref="FlattenFormat.JsonPath"/>.
        /// </summary>
        /// <typeparam name="T">The type of <c>source</c> which is a <see cref="JToken" /> type.</typeparam>
        /// <param name="source">The provided value to be flatten.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>A <see cref="Result{Dictionary{string, object}}" /> containing the key value pair for the flatten JSON.</returns>
        public static Result<Dictionary<string, object>> FlattenArray<T>(T source,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
            where T : JToken
        {
            return ValidateFlattenFormat(flattenFormat)
                .Bind(() => Result.Try(
                    () => Result.Success(FlattenArrayCore(source, flattenFormat)),
                    ex => ex.Message
                ))
                .Bind(result => result);
        }

        /// <summary>
        /// Flatten the <see cref="JToken" /> instance into a key-value pair with the key based on the <c>flattenFormat</c> given. 
        /// <br />
        /// Default key format: <see cref="FlattenFormat.JsonPath"/>.
        /// </summary>
        /// <param name="token">The <see cref="JToken" /> instance to be flatten.</param>
        /// <param name="flattenFormat">The <see cref="FlattenFormat" /> format to construct the key.</param>
        /// <returns>A <see cref="Result{T}" /> containing the flattened key-value pairs.</returns>
        public static Result<Dictionary<string, object>> Flatten(JToken token,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
        {
            return ValidateFlattenFormat(flattenFormat)
                .Bind(() => Result.Try(
                    () => Result.Success(FlattenCore(token, flattenFormat)),
                    ex => ex.Message
                ))
                .Bind(result => result);
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
        internal static string ConstructFlattenKey(string key, FlattenFormat flattenFormat)
        {
            return flattenFormat switch
            {
                FlattenFormat.DotNet =>
                    DigitJsonPathReplacementRegex()
                        .Replace(key, ":$1")
                        .Replace(".", ":")
                        .RemovePreFix(":"),
                FlattenFormat.JS =>
                    DigitJsonPathReplacementRegex()
                        .Replace(key, ".$1")
                        .RemovePreFix("."),
                FlattenFormat.JsonPath => key,
                _ => throw new InvalidEnumArgumentException(nameof(flattenFormat), (int)flattenFormat, typeof(FlattenFormat))
            };
        }

        private static Dictionary<string, object> FlattenCore(JToken token,
            FlattenFormat flattenFormat = FlattenFormat.JsonPath)
        {
            if (token.Type == JTokenType.Object)
            {
                return FlattenObjectCore(token.ToObject<JObject>(), flattenFormat);
            }
            else if (token.Type == JTokenType.Array)
            {
                return FlattenArrayCore(token, flattenFormat);
            }

            throw new ArgumentException($"Provided {nameof(token)} is neither a valid JSON object nor array.");
        }

        private static Dictionary<string, object> FlattenObjectCore<T>(T src, FlattenFormat format)
            where T : class, new()
        {
            var flattenObject = JObject.FromObject(src, DefaultSerializer)
                .Descendants()
                .OfType<JValue>()
                .ToDictionary(k => ConstructFlattenKey(k.Path, format),
                    v => v.Value);

            return flattenObject;
        }

        private static Dictionary<string, object> FlattenArrayCore<T>(T src, FlattenFormat format)
            where T : JToken
        {
            var dict = new Dictionary<string, object>();

            foreach (var jToken in JToken.FromObject(src, DefaultSerializer).Children())
            {
                if (jToken is JObject childObj)
                {
                    foreach (var kvp in FlattenObjectCore(childObj, format))
                    {
                        AddFlattenEntry(dict, $"{jToken.Path}.{kvp.Key}", kvp.Value, format);
                    }
                }
                else if (jToken is JArray childArr)
                {
                    foreach (var kvp in FlattenArrayCore(childArr, format))
                    {
                        AddFlattenEntry(dict, $"{jToken.Path}{kvp.Key}", kvp.Value, format);
                    }
                }
                else
                {
                    AddFlattenEntry(dict, jToken.Path, ((JValue)jToken).Value, format);
                }
            }

            return dict;
        }

        private static Result ValidateFlattenFormat(FlattenFormat flattenFormat)
        {
            const string INVALID_FLATTEN_FORMAT_ERROR_MSG = "Invalid formatting option (flattenFormat): {0}.";

            if (!Enum.IsDefined(typeof(FlattenFormat), flattenFormat))
                return Result.Failure(String.Format(INVALID_FLATTEN_FORMAT_ERROR_MSG, flattenFormat));

            return Result.Success();
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
