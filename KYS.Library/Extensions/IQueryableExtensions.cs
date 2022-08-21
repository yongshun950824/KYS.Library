using System;
using System.Linq;

namespace KYS.Library.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Predicate<T> predicate)
            => condition
                ? query.Where(x => predicate(x))
                : query;
    }
}
