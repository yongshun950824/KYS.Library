using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Check <see cref="IEnumerable{T}"/> instance is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <returns>The <see cref="bool" /> value indicates the <c>enumerable</c> is null or empty.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable.IsNull()
                || enumerable.IsEmpty();

        /// <summary>
        /// Check <see cref="IEnumerable"/> instance is null or empty.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <returns>The <see cref="bool" /> value indicates the <c>enumerable</c> is null or empty.</returns>
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
            => enumerable.IsNull()
                || !enumerable.GetEnumerator().MoveNext();

        /// <summary>
        /// Combine elements into a <see cref="string" />.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="separator">The <see cref="string" /> in between the elements.</param>
        /// <returns>The <see cref="String" /> value that joins all the elements with the <c>separator</c>.</returns>
        public static string AsString(this IEnumerable<string> enumerable, string separator = ",")
            => String.Join(separator, enumerable);

        /// <summary>
        /// Remove <see langword="null" /> or empty string from the <c>enumerable</c>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> instance after removing <see langword="null" /> or empty string.</returns>
        public static IEnumerable<string> Trim(this IEnumerable<string> enumerable)
            => enumerable.Where(x => !String.IsNullOrEmpty(x)).AsEnumerable();

        /// <summary>
        /// Check the <see cref="IEnumerable{T}"/> contains duplicate element(s).
        /// </summary>
        /// <typeparam name="T">The type of the element in the <c>enumerable</c>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <returns>The <see cref="bool" /> value indicates the <c>enumerable</c> contains duplicate element(s).</returns>
        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
            => enumerable.Distinct().Count() != enumerable.Count();

        /// <summary>
        /// Concatenate <see cref="IEnumerable{T}"/> into a <see cref="string" /> value.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c> inherits <see cref="class" />.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> this method extends.</param>
        /// <param name="separator">The <see cref="char" /> value in between the elements.</param>
        /// <param name="hasWhiteSpaceAfterSeparator">Add whitespace after separator.</param>
        /// <param name="removeEmptyItem">Remove element which is <see langword="null" />. For <c>T</c> as <see cref="string" /> type, remove <see langword="null" /> or empty string.</param>
        /// <returns>The <see cref="string" /> value that joins all the elements with the <c>separator</c>.</returns>
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
                if (enumerable is IEnumerable<string> stringEnumerable)
                    enumerable = (IEnumerable<T>)stringEnumerable.Where(x => !String.IsNullOrEmpty(x));
                else
                    enumerable = enumerable.Where(x => x != null);
            }

            return String.Join(separatorString, enumerable);
        }

        /// <summary>
        /// Concatenate <see cref="IEnumerable{T}"/> into a <see cref="string" /> value.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c> inherits <see cref="struct" />.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> this method extends.</param>
        /// <param name="separator">The <see cref="char" /> value in between the elements.</param>
        /// <param name="hasWhiteSpaceAfterSeparator">Add whitespace after separator.</param>
        /// <param name="removeEmptyItem">Remove element which is <see langword="null" />. For <c>T</c> as <see cref="string" /> type, remove <see langword="null" /> or empty string.</param>
        /// <returns>The <see cref="string" /> value that joins all the elements with the <c>separator</c>.</returns>
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
        /// <summary>
        /// Paginate the <see cref="IEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="isZeroBasedPageNumber">The <see cref="bool" /> value indicates the page number starts from zero.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> instance result after pagination.</returns>
        public static IEnumerable<T> Paging<T>(this IEnumerable<T> enumerable,
            int pageNumber, int pageSize, bool isZeroBasedPageNumber = false)
            => enumerable.Paging(pageNumber, pageSize, isZeroBasedPageNumber, out _);

        /// <summary>
        /// Paginate the <see cref="IEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="totalCount">Total count of the element(s) in the <c>enumerable</c>.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> instance result after pagination.</returns>
        public static IEnumerable<T> Paging<T>(this IEnumerable<T> enumerable,
            int pageNumber, int pageSize, out int totalCount)
            => enumerable.Paging(pageNumber, pageSize, false, out totalCount);

        /// <summary>
        /// Paginate the <see cref="IEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>enumerable</c>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="isZeroBasedPageNumber"><c>Boolean</c> indicates the page number starts from zero.</param>
        /// <param name="totalCount">Total count of the element(s) in the <c>enumerable</c>.</param>
        /// <returns><see cref="IEnumerable{T}"/> instance after pagination.</returns>
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
        /// Order the <see cref="IEnumerable{T}"/> based on the <c>order</c>. 
        /// <br />
        /// <br />
        /// Note: If the element's value from the <c>source</c> doesn't existed in the <c>order</c>, the element will not be shown after ordered.
        /// <br />
        /// <br />
        /// <a href="https://stackoverflow.com/a/15275682/8017690">Sort a list from another list IDs</a>
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>source</c>.</typeparam>
        /// <typeparam name="TValue">The type of element for the value to be compared from the <c>source</c> and in the <c>order</c>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="valueSelector">The value provider for extracting value from the <c>source</c>.</param>
        /// <param name="order">The element(s) to be ordered in the sequence.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> instance after ordering.</returns>
        public static IEnumerable<T> OrderBySequence<T, TValue>(this IEnumerable<T> source,
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
        }

        /// <summary>
        /// Order the <see cref="IEnumerable{T}"/> based on the <c>order</c>. 
        /// <br />
        /// <br />
        /// Idea from <c>OrderBySequence&lt;T, TValue&gt;</c> method and include returning element(s) which is not existed in the <c>order</c>.
        /// <br />
        /// The non-existent element(s) will be sorted at last.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>source</c>.</typeparam>
        /// <typeparam name="TValue">The type of element for the value to be compared from the <c>source</c> and in the <c>order</c>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> instance this method extends.</param>
        /// <param name="valueSelector">The value provider for extracting value from the <c>source</c>.</param>
        /// <param name="order">The element(s) to be ordered in the sequence.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> instance after ordering.</returns>
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

            foreach (var value in elementsNotInOrder.OrderBy(x => x))
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
