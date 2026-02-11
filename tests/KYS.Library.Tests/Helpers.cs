using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.IO;
using System.Resources;

namespace KYS.Library.Tests
{
    public static class Helpers
    {
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

        public static string GetTranslatedText(string input, Type resourceType, CultureInfo cultureInfo)
        {
            ResourceManager resourceManager = new ResourceManager(resourceType);

            ResourceSet rs = resourceManager.GetResourceSet(cultureInfo, true, false);

            return rs.GetObject(input)?.ToString();
        }

        public static string SerializeJson(object obj)
        {
            StringWriter stringWriter = new StringWriter();
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                DefaultSerializer.Serialize(jsonWriter, obj);
            }

            return stringWriter.ToString();
        }

        public static DateTime ParseDateFromIso8601(string dateString)
        {
            return DateTime.Parse(dateString,  CultureInfo.InvariantCulture,  DateTimeStyles.RoundtripKind);
        }
    }
}
