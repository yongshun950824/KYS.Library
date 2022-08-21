using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace KYS.Library.Helpers
{
    public static class StringHelper
    {
        public static string BuildUriWithQueryParams(string uri, Dictionary<string, string> queryParams)
        {
            StringBuilder sb = new StringBuilder(uri);
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in queryParams)
            {
                var query = String.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value));

                if (index == 0)
                    sb.Append(String.Format("?{0}", query));
                else
                    sb.Append(String.Format("&{0}", query));
            }

            return sb.ToString();
        }
    }
}
