using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension and helper methods for working with <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes first occurrence of the given <c>postFixes</c> from end of the given string.
        /// Ordering is important. If one of the <c>postFixes</c> is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The <see cref="string" /> instance this method extends.</param>
        /// <param name="postFixes">One or more postfixes.</param>
        /// <returns>Modified string or the same string if it has not any of given <c>postFixes</c></returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (postFixes.IsNullOrEmpty())
                return str;

            foreach (var postFix in postFixes.Where(x => str.EndsWith(x)))
            {
                str = str.Substring(0, str.Length - postFix.Length);
            }

            return str;
        }

        /// <summary>
        /// Removes first occurrence of the given <c>preFixes</c> from end of the given string.
        /// Ordering is important. If one of the <c>preFixes</c> is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The <see cref="string" /> instance this method extends.</param>
        /// <param name="preFixes">One or more prefixes.</param>
        /// <returns>Modified string or the same string if it has not any of given <c>preFixes</c></returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (preFixes.IsNullOrEmpty())
                return str;

            foreach (var preFix in preFixes.Where(x => str.StartsWith(x)))
            {
                str = str.Substring(1, str.Length - preFix.Length);
            }

            return str;
        }

        /// <summary>
        /// Validate the value is a valid Base64 string.
        /// </summary>
        /// <param name="str">The <see cref="string" /> instance this method extends.</param>
        /// <returns>The <see cref="bool"/> value indicates is a valid Base64 string.</returns>
        public static bool IsValidBase64(this string str)
        {
            Span<byte> buffer = new Span<byte>(new byte[str.Length]);

            return Convert.TryFromBase64String(str, buffer, out _);
        }

        /// <summary>
        /// Concatenate <see cref="IEnumerable{T}"/> into a <see cref="string" /> value.
        /// </summary>
        /// <param name="values">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="separator">The <see cref="char"/> value in between the elements.</param>
        /// <param name="isRemoveNullOrEmptyString">Remove element which is <see langword="null"/>. For <c>T</c> as <see cref="string"/> type, remove <see langword="null"/> or empty string.</param>
        /// <returns>The <see cref="string"/> value that joins all the elements with the <c>separator</c>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Join(this IEnumerable<string> values, string separator, bool isRemoveNullOrEmptyString = true)
        {
            ArgumentNullException.ThrowIfNull(values);

            ArgumentNullException.ThrowIfNull(separator);

            if (isRemoveNullOrEmptyString)
                values = values.Where(x => !String.IsNullOrEmpty(x));

            return String.Join(separator, values);
        }

        /// <summary>
        /// Concatenate <c>values</c> into a <see cref="string" /> value.
        /// </summary>
        /// <param name="separator">The <see cref="string" /> in between the elements.</param>
        /// <param name="isRemoveNullOrEmptyString">Remove element which is <see langword="null"/>.</param>
        /// <param name="values">One or more <see cref="string"/>(s) element provided for the string concatenation.</param>
        /// <returns>The <see cref="string"/> value that joins all the elements with the <c>separator</c>.</returns>
        public static string Join(string separator, bool isRemoveNullOrEmptyString = true, params string[] values)
        {
            return values.Join(separator, isRemoveNullOrEmptyString);
        }

        /// <exclude />
        /// <summary>
        /// Convert to invariant string.
        /// </summary>
        /// <param name="value">The <see cref="object"/> instance this method extends.</param>
        /// <returns>Converted invariant string.</returns>
        public static string ToInvariantString(this object value)
        {
            if (value is null)
                return null;

            return value is IFormattable f
                ? f.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();
        }
    }
}
