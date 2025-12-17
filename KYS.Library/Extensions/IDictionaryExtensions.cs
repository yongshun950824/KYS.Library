using System;
using System.Collections.Generic;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IDictionary{TKey, TValue}" />.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Upsert entry into the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of <c>key</c>.</typeparam>
        /// <typeparam name="TValue">The type of <c>value</c>.</typeparam>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}" /> instance this method extends.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <returns>The <c>value</c> from the key-value pair to be inserted.</returns>
        public static TValue Upsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
            => dictionary.Upsert(key, () => value);

        /// <summary>
        /// Upsert entry into the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of <c>key</c>.</typeparam>
        /// <typeparam name="TValue">The type of <c>value</c>.</typeparam>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}" /> instance this method extends.</param>
        /// <param name="key">The key of the entry to set.</param>
        /// <param name="valueProvider">The value provider that returns the value to associate with the key.</param>
        /// <returns>The <c>value</c> from the key-value pair to be inserted.</returns>
        public static TValue Upsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> valueProvider)
        {
            TValue value;

            if (dictionary.TryGetValue(key, out value))
            {
                value = valueProvider();
                dictionary[key] = value;
            }
            else
            {
                value = valueProvider();
                dictionary.Add(key, value);
            }

            return value;
        }
    }
}
