using Microsoft.Extensions.Caching.Memory;
using System;

namespace KYS.Library.Extensions
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Add value to MemoryCache with expired after <c>expiredSeconds</c> seconds (absolute expiration).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <param name="expiredSeconds"></param>
        public static void Add<T>(this IMemoryCache cache, T o, string key, int expiredSeconds)
        {
            cache.Set<T>(key, o, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(expiredSeconds),
                SlidingExpiration = null
            });
        }

        /// <summary>
        /// Add value to MemoryCache with expired after <c>timeSpan</c> (absolute expiration).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        public static void Add<T>(this IMemoryCache cache, T o, string key, TimeSpan timeSpan)
        {
            cache.Set<T>(key, o, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.Add(timeSpan),
                SlidingExpiration = null
            });
        }

        public static bool Get<T>(this IMemoryCache cache, string key, out T value)
        {
            return cache.TryGetValue(key, out value);
        }
    }
}
