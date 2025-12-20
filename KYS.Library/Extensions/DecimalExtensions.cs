using System;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="decimal" />.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Round off <c>input</c> to nearest provided decimal place(s).
        /// </summary>
        /// <param name="input">The <see cref="decimal" /> instance this method extends.</param>
        /// <param name="places">The decimal places used for the rounding off.</param>
        /// <returns>The <see cref="decimal" /> value after rounding off.</returns>
        public static decimal Round(this decimal input, int places)
        {
            return Math.Round(input, places, MidpointRounding.AwayFromZero);
        }
    }
}
