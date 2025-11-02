using System;
using System.ComponentModel;

namespace KYS.Library.Helpers
{
    public static class CompareOperator
    {
        public enum CompareOperatorConstants
        {
            [Description("Equal to")]
            Equal,
            [Description("Not equal to")]
            NotEqual,
            [Description("Less than")]
            LessThan,
            [Description("Less than or equal to")]
            LessThanOrEqual,
            [Description("Greater than")]
            GreaterThan,
            [Description("Greater than or equal to")]
            GreaterThanOrEqual
        }

        public static bool Compare(CompareOperatorConstants @operator, IComparable a, IComparable b) =>
            GetCompareOperatorFunc(@operator)(a, b);

        public static bool Compare<T>(CompareOperatorConstants @operator, T a, T b)
            where T : IComparable, IComparable<T> =>
            GetCompareOperatorFunc(@operator)(a, b);

        public static Func<IComparable, IComparable, bool> GetCompareOperatorFunc(CompareOperatorConstants @operator)
        {
            return @operator switch
            {
                CompareOperatorConstants.Equal => (a, b) => a.Equals(b),
                CompareOperatorConstants.NotEqual => (a, b) => !a.Equals(b),
                CompareOperatorConstants.LessThan => (a, b) => a.CompareTo(b) == -1,
                CompareOperatorConstants.LessThanOrEqual => (a, b) => a.CompareTo(b) == -1
                    || a.CompareTo(b) == 0,
                CompareOperatorConstants.GreaterThan => (a, b) => a.CompareTo(b) == 1,
                CompareOperatorConstants.GreaterThanOrEqual => (a, b) => a.CompareTo(b) == 1
                    || a.CompareTo(b) == 0,
                _ => throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(CompareOperatorConstants))
            };
        }
    }
}
