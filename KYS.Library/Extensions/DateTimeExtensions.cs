using System;
using System.Globalization;

namespace KYS.Library.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Reference: <a href="https://stackoverflow.com/q/114983/8017690">Given a DateTime object, how do I get an ISO 8601 date in string format?</a>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string ToIso8601String(this DateTime dt, CultureInfo cultureInfo = null)
        {
            return dt.ToString("O", cultureInfo ?? CultureInfo.InvariantCulture);
        }

        public static DateTime GetFirstDateOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime GetLastDateOfMonth(this DateTime dt)
        {
            return dt.AddMonths(1).AddDays(-1);
        }

        public static DateTime GetFirstDateOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        public static DateTime GetLastDateOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1)
                .AddYears(1)
                .AddDays(-1);
        }
    }
}
