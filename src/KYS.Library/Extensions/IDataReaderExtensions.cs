using System;
using System.Data;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IDataReader" />.
    /// </summary>
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Safely retrieves the value from the <see cref="IDataReader" /> instance for the specified column.
        /// <br />
        /// Handles DBNull, type conversion, and provides specialized validation for <see cref="DateTime"/> boundaries.
        /// </summary>
        /// <typeparam name="T">The type of returned value and <c>defaultValue</c>.</typeparam>
        /// <param name="reader">The <see cref="IDataReader" /> instance this method extends.</param>
        /// <param name="columnName">The name for the data column.</param>
        /// <param name="defaultValue">The returned value when the column is <c>DBNull</c> or contains an out-of-range DateTime.</param>
        /// <returns>The value from the column cast to <typeparamref name="T"/>. Or <paramref name="defaultValue"/> if the column is null or an invalid date.</returns>
        public static T SafeGetValue<T>(this IDataReader reader, string columnName, T defaultValue = default)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
            {
                return defaultValue;
            }

            object value = reader.GetValue(ordinal);

            if (value is DateTime dt)
            {
                DateTime fallback = defaultValue switch
                {
                    DateTime d => d,
                    _ => DateTime.Now
                };

                // Prevent getting the out-of-range date, assigned with default value
                return (T)(object)HandleDate(dt, fallback);
            }

            if (value is T variable)
            {
                return variable;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static DateTime HandleDate(DateTime value, DateTime? defaultValue = null)
        {
            if (value == DateTime.MinValue || value == DateTime.MaxValue || value.Year == 1)
            {
                return defaultValue ?? value;
            }

            return value;
        }
    }
}
