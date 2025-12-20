using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static KYS.Library.Helpers.JsonHelper;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IConfigurationBuilder" />.
    /// </summary>
    public static class IConfigurationBuilderExtensions
    {
        private static readonly JsonSerializer _DefaultSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateParseHandling = DateParseHandling.DateTime,
            DateFormatString = "o", // ISO 8601
            Culture = CultureInfo.InvariantCulture
        });

        /// <summary>
        /// Bind complex value (object/array) to configuration.
        /// <br /><br />
        /// Inspired from <a href="https://stackoverflow.com/a/79752367/8017690">IConfiguration Bind doesn't work with arrays</a>.
        /// </summary>
        /// <typeparam name="T">The type of <c>obj</c>.</typeparam>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder" /> instance this method extends.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="obj">The value to associate with the key.</param>
        public static void Bind<T>(this IConfigurationBuilder configurationBuilder, string key, T obj)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (obj is string s)
            {
                dict[key] = s;
            }
            else if (IsScalarLike(obj.GetType()))
            {
                dict[key] = obj.ToInvariantString();
            }
            else if (obj is IEnumerable seq)
            {
                JToken jToken = JArray.FromObject(seq, _DefaultSerializer);
                dict = Flatten(jToken, FlattenFormat.DotNet)
                    .Select(x => new KeyValuePair<string, string>(
                        $"{key}:{x.Key}",
                        x.Value?.ToString()))
                    .ToDictionary();
            }
            else
            {
                JToken jToken = JObject.FromObject(obj, _DefaultSerializer);
                dict = Flatten(jToken, FlattenFormat.DotNet)
                    .Select(x => new KeyValuePair<string, string>(
                        $"{key}:{x.Key}",
                        x.Value?.ToString()))
                    .ToDictionary();
            }

            configurationBuilder.AddInMemoryCollection(dict);
        }

        private static bool IsScalarLike(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return t.IsPrimitive
                || t.IsEnum
                || t == typeof(decimal)
                || t == typeof(Guid)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan)
                || t == typeof(Uri);
        }
    }
}
