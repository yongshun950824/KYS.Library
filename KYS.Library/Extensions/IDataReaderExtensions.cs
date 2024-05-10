using System;
using System.Data;

namespace KYS.Library.Extensions
{
    public static class IDataReaderExtensions
    {
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
