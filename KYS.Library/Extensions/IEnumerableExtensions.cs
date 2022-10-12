using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KYS.Library.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) 
            => enumerable.IsNull()
                || enumerable.IsEmpty();

        public static bool IsNullOrEmpty(this IEnumerable enumerable)
            => enumerable.IsNull()
                || !enumerable.GetEnumerator().MoveNext();

        public static string AsString(this IEnumerable<string> enumerable, string separator = ",")
            => String.Join(separator, enumerable);

        public static IEnumerable<string> Trim(this IEnumerable<string> enumerable)
            => enumerable.Where(x => !String.IsNullOrEmpty(x)).AsEnumerable();

        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
            => enumerable.Distinct().Count() != enumerable.Count();

        /// <summary>
        /// An extension method to concat <c>IEnumerable</c> into <c>string</c> value for displaying.
        /// <br />
        /// Use case(s):
        /// <para />
        /// 1. <c>enumerable.ToString(',', true, true)</c>
        /// <para />
        /// 2. <c>enumerable.ToString&lt;T&gt;()</c>
        /// <para />
        /// 3. <c>IEnumerable.ToString(enumerable, ',', true, true)</c>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="separator">Separator. (Optional)</param>
        /// <param name="hasWhiteSpaceAfterSeparator">To add whitespace after separator. (Optional) </param>
        /// <param name="removeEmptyItem">Remove item which is null. For `string` value, remove null or empty string.(Optional) </param>
        /// <returns></returns>
        public static string ToString<T>(this IEnumerable<T> enumerable,
            char separator = ',',
            bool hasWhiteSpaceAfterSeparator = true,
            bool removeEmptyItem = false)
        {
            if (enumerable.IsNullOrEmpty())
                return String.Empty;

            string separatorString = separator.ToString();
            if (hasWhiteSpaceAfterSeparator)
                separatorString += " ";

            if (removeEmptyItem)
            {
                Type type = typeof(T);
                if (type == typeof(string))
                    enumerable = enumerable.Where(x => !String.IsNullOrEmpty(x.ToString()));
                else
                    enumerable = enumerable.Where(x => x != null);
            }

            return String.Join(separator, enumerable);
        }

        #region Paging Methods
        public static IEnumerable<T> Paging<T>(this IEnumerable<T> enumerable, 
            int pageNumber, int pageSize, bool isZeroBasedPageNumber = false)
            => enumerable.Paging(pageNumber, pageSize, isZeroBasedPageNumber, out _);

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> enumerable,
            int pageNumber, int pageSize, out int totalCount)
            => enumerable.Paging(pageNumber, pageSize, false, out totalCount);

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> enumerable, 
            int pageNumber, int pageSize, bool isZeroBasedPageNumber, out int totalCount)
        {
            totalCount = 0;

            if (enumerable.IsNullOrEmpty())
                return enumerable;

            totalCount = enumerable.Count();

            if (!isZeroBasedPageNumber)
                pageNumber -= 1;

            enumerable = enumerable
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();

            // Set totalCount = 0 when provide pageNumber out of range
            if (enumerable.IsNullOrEmpty())
                totalCount = 0;

            return enumerable;
        }
        #endregion

        #region Private Methods
        private static bool IsNull<T>(this IEnumerable<T> enumerable)
            => enumerable == null;

        private static bool IsEmpty<T>(this IEnumerable<T> enumerable) 
            => !enumerable.Any();

        private static bool IsNull(this IEnumerable enumerable)
            => enumerable == null;
        #endregion
    }
}
