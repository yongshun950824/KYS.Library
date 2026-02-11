using Microsoft.Extensions.Caching.Memory;
using System;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IMemoryCache" />.
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Add value to <see cref="IMemoryCache" /> instance with expired after <c>expiredSeconds</c> seconds (absolute expiration).
        /// </summary>
        /// <typeparam name="T">The type of <c>o</c>.</typeparam>
        /// <param name="cache">The <see cref="IMemoryCache" /> instance this method extends.</param>
        /// <param name="o">The value to associate with the key.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="expiredSeconds">The duration in second(s) that the entry to be expired (absolute expiration).</param>
        public static void Add<T>(this IMemoryCache cache, T o, string key, int expiredSeconds)
        {
            cache.Set<T>(key, o, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(expiredSeconds),
                SlidingExpiration = null
            });
        }

        /// <summary>
        /// Add value to <see cref="IMemoryCache" /> instance with expired after <c>timeSpan</c> (absolute expiration).
        /// </summary>
        /// <typeparam name="T">The type of <c>o</c>.</typeparam>
        /// <param name="cache">The <see cref="IMemoryCache" /> instance this method extends.</param>
        /// <param name="o">The value to associate with the key.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="timeSpan">The duration in <see cref="TimeSpan" /> that the entry to be expired (absolute expiration).</param>
        public static void Add<T>(this IMemoryCache cache, T o, string key, TimeSpan timeSpan)
        {
            cache.Set<T>(key, o, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.Add(timeSpan),
                SlidingExpiration = null
            });
        }

        /// <summary>
        /// Safely retrieve cache value from <see cref="IMemoryCache" /> by key.
        /// </summary>
        /// <typeparam name="T">The type of <c>value</c>.</typeparam>
        /// <param name="cache">The <see cref="IMemoryCache" /> instance this method extends.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="value">The value associated with this key.</param>
        /// <returns>The <see cref="bool" /> value indicates the key existed in the cache.</returns>
        public static bool Get<T>(this IMemoryCache cache, string key, out T value)
        {
            return cache.TryGetValue(key, out value);
        }
    }
}
