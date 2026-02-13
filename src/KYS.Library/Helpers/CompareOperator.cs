using System;
using System.ComponentModel;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provides a set of comparison operations that can be applied to values implementing <see cref="IComparable"/>.
    /// </summary>
    /// <remarks>
    /// This class defines common comparison operators (e.g., equal, greater than, less than)
    /// and utility methods to perform comparisons in a type-safe and extensible way.
    /// </remarks>
    public static class CompareOperator
    {
        /// <summary>
        /// Represents the available comparison operators.
        /// </summary>
        public enum CompareOperatorConstants
        {
            /// <summary>
            /// Represents the "equal to" comparison operator.
            /// </summary>
            [Description("Equal to")]
            Equal,
            /// <summary>
            /// Represents the "not equal to" comparison operator.
            /// </summary>
            [Description("Not equal to")]
            NotEqual,
            /// <summary>
            /// Represents the "less than" comparison operator.
            /// </summary>
            [Description("Less than")]
            LessThan,
            /// <summary>
            /// Represents the "less than or equal to" comparison operator.
            /// </summary>
            [Description("Less than or equal to")]
            LessThanOrEqual,
            /// <summary>
            /// Represents the "greater than" comparison operator.
            /// </summary>
            [Description("Greater than")]
            GreaterThan,
            /// <summary>
            /// Represents the "greater than or equal to" comparison operator.
            /// </summary>
            [Description("Greater than or equal to")]
            GreaterThanOrEqual
        }

        /// <summary>
        /// Compares two <see cref="IComparable"/> values using the specified comparison operator.
        /// </summary>
        /// <param name="operator">The comparison operator to apply.</param>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns><see langword="true"/> if the comparison condition is satisfied; otherwise, <see langword="false"/>.</returns>
        public static bool Compare(CompareOperatorConstants @operator, IComparable a, IComparable b) =>
            GetCompareOperatorFunc(@operator)(a, b);

        /// <summary>
        /// Compares two values of type <typeparamref name="T"/> using the specified comparison operator.
        /// </summary>
        /// <typeparam name="T">The type of the objects being compared. Must implement <see cref="IComparable"/> and <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="operator">The comparison operator to apply.</param>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns><see langword="true"/> if the comparison condition is satisfied; otherwise, <see langword="false"/>.</returns>
        public static bool Compare<T>(CompareOperatorConstants @operator, T a, T b)
            where T : IComparable, IComparable<T> =>
            GetCompareOperatorFunc(@operator)(a, b);

        /// <summary>
        /// Returns a delegate function that performs the comparison corresponding to the specified operator.
        /// </summary>
        /// <param name="operator">The comparison operator to retrieve.</param>
        /// <returns>
        /// A <see cref="Func{T1, T2, TResult}"/> that accepts two <see cref="IComparable"/> values and returns
        /// <see langword="true"/> if the comparison condition is met; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
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
