using System;
using System.Collections.Generic;

namespace KYS.Library.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue Upsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
            => dictionary.Upsert(key, () => value);

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
