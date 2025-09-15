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
            where T : class
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
                    enumerable = enumerable.Where(x => !String.IsNullOrEmpty(x?.ToString()));
                else
                    enumerable = enumerable.Where(x => x != null);
            }

            return String.Join(separatorString, enumerable);
        }

        public static string ToString<T>(this IEnumerable<T?> enumerable,
            char separator = ',',
            bool hasWhiteSpaceAfterSeparator = true,
            bool removeEmptyItem = false)
            where T : struct
        {
            if (enumerable.IsNullOrEmpty())
                return String.Empty;

            string separatorString = separator.ToString();
            if (hasWhiteSpaceAfterSeparator)
                separatorString += " ";

            if (removeEmptyItem)
            {
                enumerable = enumerable.Where(x => x.HasValue);
            }

            return String.Join(separatorString, enumerable);
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

        /// <summary>
        /// Order the <c>IEnumerable</c> based on another <c>IEnumerable</c>. 
        /// <br />
        /// <br />
        /// Note: If the element's value from <c>source</c> doesn't existed in the <c>order</c>, the element will not be shown after ordered.
        /// <br />
        /// <br />
        /// <a href="https://stackoverflow.com/a/15275682/8017690">Sort a list from another list IDs</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="order"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderBySequence<T, TValue>(this IEnumerable<T> source,
           IEnumerable<TValue> order,
           Func<T, TValue> valueSelector)
        {
            var lookup = source.ToLookup(valueSelector, t => t);
            foreach (var value in order)
            {
                foreach (var t in lookup[value])
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Order the <c>IEnumerable</c> based on another <c>IEnumerable</c>. 
        /// <br />
        /// <br />
        /// Idea from <c>OrderBySequence&lt;T, TValue&gt;</c> method and include returning element(s) which is not existed in the <c>order</c>.
        /// <br />
        /// The non-existent element(s) will be sorted at last.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="valueSelector"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderBy<T, TValue>(this IEnumerable<T> source,
            Func<T, TValue> valueSelector,
            IEnumerable<TValue> order)
        {
            var lookup = source.ToLookup(valueSelector, t => t);
            foreach (var value in order)
            {
                foreach (var t in lookup[value])
                {
                    yield return t;
                }
            }

            #region Return the element(s) which is not in order at last (result in sorted by ASCII)
            var elementsNotInOrder = lookup
                .Where(x => !order.Contains(x.Key))
                .Select(x => x.Key)
                .ToList();

            foreach (var value in elementsNotInOrder)
            {
                foreach (var t in lookup[value])
                {
                    yield return t;
                }
            }
            #endregion
        }

        #region Private Methods
        private static bool IsNull<T>(this IEnumerable<T> enumerable)
            => enumerable == null;

        private static bool IsNull(this IEnumerable enumerable)
            => enumerable == null;

        private static bool IsEmpty<T>(this IEnumerable<T> enumerable)
            => !enumerable.Any();
        #endregion
    }
}
