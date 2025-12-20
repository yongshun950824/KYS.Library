using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for URI.
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        /// Construct the full URI with query parameter(s).
        /// </summary>
        /// <param name="queryParams">The <see cref="IDictionary{string, string}"/> instance this method extends which is key-value pair for the query parameter(s).</param>
        /// <param name="uri">The base URI.</param>
        /// <returns>The full URI with the query parameter(s).</returns>
        public static string BuildUriWithQueryParams(this IDictionary<string, string> queryParams, string uri)
        {
            StringBuilder sb = new StringBuilder(uri);

            return sb.Append('?')
                .Append(queryParams.ToQueryParams())
                .ToString();
        }

        /// <summary>
        /// Construct the query parameter(s) portion.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{string, string}"/> instance this method extends which is key-value pair for the query parameter(s).</param>
        /// <returns>The query parameter(s) portion for URI.</returns>
        public static string ToQueryParams(this IDictionary<string, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                if (index > 0)
                    sb.Append('&');

                sb.Append($"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}");

                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Construct the query parameter(s) portion. 
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/76874436">How to generate query string with flattened param name from a nested object</a>
        /// </summary>
        /// <typeparam name="T">The type of <c>source</c>. Must be a reference type with a public parameterless constructor.</typeparam>
        /// <param name="source">An object with query parameter(s).</param>
        /// <returns>The query parameter(s) portion for URI.</returns>
        public static string ToQueryParams<T>(T source)
            where T : class, new()
        {
            return JsonHelper.FlattenObject(source)
                .ToDictionary(k => k.Key, v => v.Value?.ToString())
                .ToQueryParams();
        }
    }
}
