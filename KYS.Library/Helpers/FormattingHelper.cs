using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for formatting string.
    /// </summary>
    public static partial class FormattingHelper
    {
        [GeneratedRegex(@"([A-Z])", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
        private static partial Regex UppercaseReplacementRegex();

        /// <summary>
        /// Represents the available formatting options.
        /// </summary>
        public enum Formatting
        {
            SnakeCase
        }

        /// <summary>
        /// Format the <c>value</c> based on the provided <see cref="Formatting" /> option.
        /// </summary>
        /// <param name="value">The <see cref="string" /> value provided for the formatting.</param>
        /// <param name="format">The selected <see cref="Formatting" /> option.</param>
        /// <returns>The <see cref="string" /> value after formatted.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static string Convert(this string value, Formatting format)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            return format switch
            {
                Formatting.SnakeCase => ConvertToSnakeCase(value),
                _ => throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(Formatting)),
            };
        }

        private static string ConvertToSnakeCase(string value)
        {
            // Add an underscore before each uppercase letter (excluding the first one).
            string snakeCase = UppercaseReplacementRegex()
                .Replace(value, "_$1")
                .ToLower();

            // If the string starts with an underscore (because the first letter was uppercase), remove it.
            if (snakeCase.StartsWith('_'))
                snakeCase = snakeCase.Substring(1);

            return snakeCase;
        }
    }
}
