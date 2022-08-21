using System;

namespace KYS.Library.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal input, int places)
        {
            return Math.Round(input, places);

            #region Alternate
            //double num = Math.Pow(10, places);
            //return Math.Ceiling(input * Convert.ToDecimal(num)) / Convert.ToDecimal(num);
            #endregion
        }
    }
}
