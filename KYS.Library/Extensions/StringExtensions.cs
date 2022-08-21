using System;

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
    }
}
