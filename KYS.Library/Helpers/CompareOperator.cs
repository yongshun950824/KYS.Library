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

        public static bool Compare(CompareOperatorConstants @operator, IComparable a, IComparable b)
        {
            switch (@operator)
            {
                case CompareOperatorConstants.Equal:
                    return a.Equals(b);

                case CompareOperatorConstants.NotEqual:
                    return !a.Equals(b);

                case CompareOperatorConstants.LessThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LessThanOrEqual:
                    return a.CompareTo(b) == -1
                        || a.CompareTo(b) == 0;

                case CompareOperatorConstants.GreaterThan:
                    return a.CompareTo(b) == 1;

                case CompareOperatorConstants.GreaterThanOrEqual:
                    return a.CompareTo(b) == 1
                        || a.CompareTo(b) == 0;

                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        public static bool Compare<T>(CompareOperatorConstants @operator, T a, T b) where T : IComparable, IComparable<T>
        {
            switch (@operator)
            {
                case CompareOperatorConstants.Equal:
                    return a.Equals(b);

                case CompareOperatorConstants.NotEqual:
                    return !a.Equals(b);

                case CompareOperatorConstants.LessThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LessThanOrEqual:
                    return a.CompareTo(b) == -1
                        || a.CompareTo(b) == 0;

                case CompareOperatorConstants.GreaterThan:
                    return a.CompareTo(b) == 1;

                case CompareOperatorConstants.GreaterThanOrEqual:
                    return a.CompareTo(b) == 1
                        || a.CompareTo(b) == 0;

                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                _ => throw new Exception("Invalid operator"),
            };
        }
    }
}
