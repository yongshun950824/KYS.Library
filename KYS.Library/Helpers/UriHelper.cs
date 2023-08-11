using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KYS.Library.Helpers
{
    public static class UriHelper
    {
        public static string BuildUriWithQueryParams(this IDictionary<string, string> queryParams, string uri)
        {
            StringBuilder sb = new StringBuilder(uri);

            return sb.Append('?')
                .Append(queryParams.ToQueryParams())
                .ToString();
        }

        public static string ToQueryParams(this IDictionary<string, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                if (index > 0)
                    sb.Append('&');

                sb.Append($"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value?.ToString())}");

                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Reference: <a href="https://stackoverflow.com/a/76874436">How to generate query string with flattened param name from a nested object</a>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string ToQueryParams<T>(T source)
            where T : class, new()
        {
            return JsonHelper.FlattenObject(source)
                .ToDictionary(k => k.Key, v => v.Value?.ToString())
                .ToQueryParams();
        }
    }
}
