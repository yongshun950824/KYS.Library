using System.Collections.Generic;
using System.Linq;

namespace KYS.Library.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.IsNull()
                || enumerable.IsEmpty();
        }

        private static bool IsNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null;
        }

        private static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Count() == 0;
        }
    }
}
