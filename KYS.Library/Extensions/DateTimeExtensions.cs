using System;
using System.Globalization;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="DateTime" />.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Get the provided date in ISO 8601 format.
        /// Reference: <a href="https://stackoverflow.com/q/114983/8017690">Given a DateTime object, how do I get an ISO 8601 date in string format?</a>
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <param name="cultureInfo">Culture info.</param>
        /// <returns>The <see cref="string" /> value displays the <see cref="DateTime" /> in ISO 8601 format.</returns>
        public static string ToIso8601String(this DateTime dt, CultureInfo cultureInfo = null)
        {
            return dt.ToString("O", cultureInfo ?? CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get first date of the month from the provided date.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <returns>First date of the month from the provided date</returns>
        public static DateTime GetFirstDateOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Get last date of the month from the provided date.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <returns>Last date of the month from the provided date</returns>
        public static DateTime GetLastDateOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, DateTimeKind.Unspecified)
                .AddMonths(1)
                .AddDays(-1);
        }

        /// <summary>
        /// Get the first date of the year from the provided date.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <returns>First date of the year from the provided date</returns>
        public static DateTime GetFirstDateOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Get the last date of the year from the provided date.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <returns>Last date of the year from the provided date.</returns>
        public static DateTime GetLastDateOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                .AddYears(1)
                .AddDays(-1);
        }

        /// <summary>
        /// Get the provided date in Thai Buddhist Calendar.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime" /> instance this method extends.</param>
        /// <returns>Date in Thai Buddhist Calendar</returns>
        public static DateTime ConvertToThaiBuddhistDateTime(this DateTime dt)
        {
            ThaiBuddhistCalendar cal = new ThaiBuddhistCalendar();

            return new DateTime(cal.GetYear(dt), cal.GetMonth(dt), cal.GetDayOfMonth(dt), dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Calculate the age between provided date of birth and calculated date.
        /// </summary>
        /// <param name="dob">The <see cref="DateTime" /> instance this method extends. Provided with date of birth.</param>
        /// <param name="calculatedDate">The end date to be deducted with the date of birth.</param>
        /// <returns>Age</returns>
        public static int GetAge(this DateTime dob, DateTime? calculatedDate = null)
        {
            calculatedDate ??= DateTime.Now;

            int a = (calculatedDate.Value.Year * 100 + calculatedDate.Value.Month) * 100 + calculatedDate.Value.Day;
            int b = (dob.Year * 100 + dob.Month) * 100 + dob.Day;

            return (a - b) / 10000;
        }
    }
}
