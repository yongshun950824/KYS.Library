using System;

namespace KYS.Library.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal input, int places)
        {
            return Math.Round(input, places, MidpointRounding.AwayFromZero);
        }
    }
}
