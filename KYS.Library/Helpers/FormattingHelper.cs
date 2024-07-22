using System;
using System.Text.RegularExpressions;

namespace KYS.Library.Helpers
{
    public static class FormattingHelper
    {
        public enum FormattingEnum
        {
            SnakeCase
        }

        public static string Convert(this string value, FormattingEnum format)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            switch (format)
            {
                case FormattingEnum.SnakeCase:
                    return ConvertToSnakeCase(value);

                default:
                    return value;
            }
        }

        public static string ConvertToSnakeCase(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            // Add an underscore before each uppercase letter (excluding the first one).
            string snakeCase = Regex.Replace(value, @"([A-Z])", "_$1").ToLower();

            // If the string starts with an underscore (because the first letter was uppercase), remove it.
            if (snakeCase.StartsWith("_"))
            {
                snakeCase = snakeCase.Substring(1);
            }

            return snakeCase;
        }
    }
}
