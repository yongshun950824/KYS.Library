using Newtonsoft.Json.Linq;
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
    }
}
