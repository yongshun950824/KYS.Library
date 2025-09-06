using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KYS.Library.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes first occurrence of the given postfixes from end of the given string.
        /// Ordering is important. If one of the postFixes is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="postFixes">one or more postfix.</param>
        /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (postFixes.IsNullOrEmpty())
                return str;

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                    str = str.Substring(0, str.Length - postFix.Length);
            }

            return str;
        }

        /// <summary>
        /// Removes first occurrence of the given prefixes from end of the given string.
        /// Ordering is important. If one of the preFixes is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="preFixes">one or more prefix.</param>
        /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (preFixes.IsNullOrEmpty())
                return str;

            foreach (var preFix in preFixes)
            {
                if (str.StartsWith(preFix))
                    str = str.Substring(1, str.Length - preFix.Length);
            }

            return str;
        }

        public static bool IsValidBase64(this string str)
        {
            Span<byte> buffer = new Span<byte>(new byte[str.Length]);

            return Convert.TryFromBase64String(str, buffer, out _);
        }

        public static string Join(this IEnumerable<string?> values, string separator, bool isRemoveNullOrEmptyString = true)
        {
            if (isRemoveNullOrEmptyString)
                values = values.Where(x => !String.IsNullOrEmpty(x));

            return String.Join(separator, values);
        }

        public static string Join(string separator, bool isRemoveNullOrEmptyString = true, params string?[] values)
        {
            return values.Join(separator, isRemoveNullOrEmptyString);
        }

        public static string ToInvariantString(this object? value)
        {
            if (value is null)
                return null;

            return value is IFormattable f
                ? f.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();
        }
    }
}
