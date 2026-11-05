using System;
using System.Collections.Generic;
using System.Linq;

namespace KYS.EFCore.Library;

/// <summary>
/// Base class for value objects: objects without identity that are equal when all their
/// equality components (see <see cref="GetEqualityComponents"/>) are equal.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Helper for implementing <c>operator ==</c> in derived classes.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns><see langword="true"/> if both are <see langword="null"/> or equal; otherwise <see langword="false"/>.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, right) || left.Equals(right);
    }

    /// <summary>
    /// Helper for implementing <c>operator !=</c> in derived classes.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns><see langword="true"/> if the value objects are not equal; otherwise <see langword="false"/>.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Returns the values that define the equality of the value object, always in the same order.
    /// </summary>
    /// <returns>The equality components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is a value object of the same type with equal components.
    /// </summary>
    /// <param name="obj">The object to compare with this value object.</param>
    /// <returns><see langword="true"/> if the objects are equal; otherwise <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code combined from the hash codes of the equality components.
    /// </summary>
    /// <returns>A hash code for the current value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}
