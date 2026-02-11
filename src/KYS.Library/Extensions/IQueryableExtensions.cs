using System;
using System.Linq;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Filter the element with provided <c>predicate</c> if the <c>condition</c> is fulfilled.
        /// </summary>
        /// <typeparam name="T">The type of element in <see cref="IQueryable"/>.</typeparam>
        /// <param name="query">The <see cref="IQueryable{T}"/> instance this method extends.</param>
        /// <param name="condition">The <see cref="bool" /> value indicates <c>predicate</c> to be included.</param>
        /// <param name="predicate">Filter criteria.</param>
        /// <returns>The <see cref="IQueryable{T}"/> instance includes <c>predicate</c> if the <c>condition</c> is fulfilled.</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Predicate<T> predicate)
            => condition
                ? query.Where(x => predicate(x))
                : query;
    }
}
