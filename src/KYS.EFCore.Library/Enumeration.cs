using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace KYS.EFCore.Library;

/// <summary>
/// Base class for "smart enums": strongly typed enumerations with an <see cref="Id"/> and a <see cref="Name"/>,
/// which can carry behaviour, unlike a C# <see langword="enum"/>.
/// </summary>
/// <remarks>
/// Derive from this class and declare each value as a <c>public static readonly</c> field of the derived type.
/// Instances are compared and ordered by <see cref="Id"/>.
/// </remarks>
[DebuggerDisplay("Id: {Id} | Name: {Name,nq}")]
public class Enumeration : IComparable<Enumeration>, IEquatable<Enumeration>
{
    /// <summary>
    /// Gets the display name of the value.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the identifier of the value, used for equality and ordering.
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class.
    /// </summary>
    /// <param name="id">The identifier of the value.</param>
    /// <param name="name">The display name of the value.</param>
    protected Enumeration(long id, string name)
        => (Id, Name) = (id, name);

    /// <summary>
    /// Returns the <see cref="Name"/> of the value.
    /// </summary>
    /// <returns>The <see cref="Name"/> of the value.</returns>
    public override string ToString()
        => Name;

    /// <summary>
    /// Returns all values declared as public static fields on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The enumeration type. Every public static field declared on it must be of type <typeparamref name="T"/>.</typeparam>
    /// <returns>The declared values of <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidCastException">A public static field declared on <typeparamref name="T"/> is not of type <typeparamref name="T"/> (thrown on enumeration).</exception>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
        => typeof(T)
            .GetFields(BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Select(x => x.GetValue(null))
            .Cast<T>();

    /// <summary>
    /// Determines whether the specified object is an <see cref="Enumeration"/> with the same <see cref="Id"/>.
    /// </summary>
    /// <param name="obj">The object to compare with this value.</param>
    /// <returns><see langword="true"/> if the objects are equal; otherwise <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not Enumeration)
        {
            return false;
        }

        return Equals(obj as Enumeration);
    }

    /// <summary>
    /// Determines whether the specified value has the same <see cref="Id"/> as this value.
    /// </summary>
    /// <param name="other">The value to compare with this value.</param>
    /// <returns><see langword="true"/> if the values are equal; otherwise <see langword="false"/>.</returns>
    public virtual bool Equals(Enumeration other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns a hash code based on <see cref="Id"/>.
    /// </summary>
    /// <returns>A hash code for the current value.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Compares this value with another value by <see cref="Id"/>.
    /// </summary>
    /// <param name="other">The value to compare with this value.</param>
    /// <returns>
    /// A negative number if this value precedes <paramref name="other"/>, zero if they are equal,
    /// or a positive number if this value follows <paramref name="other"/> or <paramref name="other"/> is <see langword="null"/>.
    /// </returns>
    public int CompareTo(Enumeration other)
    {
        if (other is null)
            return 1;

        return Id.CompareTo(other.Id);
    }

    #region Relational operators
    /// <summary>
    /// Determines whether two values are equal.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if both are <see langword="null"/> or have the same <see cref="Id"/>; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Enumeration left, Enumeration right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Determines whether two values are not equal.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if the values are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Enumeration left, Enumeration right)
        => !(left == right);

    /// <summary>
    /// Determines whether <paramref name="left"/> precedes <paramref name="right"/>. <see langword="null"/> precedes any value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> precedes <paramref name="right"/>; otherwise <see langword="false"/>.</returns>
    public static bool operator <(Enumeration left, Enumeration right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether <paramref name="left"/> precedes or equals <paramref name="right"/>. <see langword="null"/> precedes any value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> precedes or equals <paramref name="right"/>; otherwise <see langword="false"/>.</returns>
    public static bool operator <=(Enumeration left, Enumeration right)
        => left is null || left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether <paramref name="left"/> follows <paramref name="right"/>. <see langword="null"/> precedes any value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> follows <paramref name="right"/>; otherwise <see langword="false"/>.</returns>
    public static bool operator >(Enumeration left, Enumeration right)
        => left is not null && left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether <paramref name="left"/> follows or equals <paramref name="right"/>. <see langword="null"/> precedes any value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> follows or equals <paramref name="right"/>; otherwise <see langword="false"/>.</returns>
    public static bool operator >=(Enumeration left, Enumeration right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
    #endregion
}

/// <summary>
/// Equality comparer that compares <see cref="Enumeration"/> values by <see cref="Enumeration.Id"/>.
/// </summary>
public sealed class EnumerationComparer : EqualityComparer<Enumeration>
{
    /// <summary>
    /// Determines whether two values have the same <see cref="Enumeration.Id"/>.
    /// </summary>
    /// <param name="x">The first value to compare.</param>
    /// <param name="y">The second value to compare.</param>
    /// <returns><see langword="true"/> if the values are equal; otherwise <see langword="false"/>.</returns>
    public override bool Equals(Enumeration x, Enumeration y)
    {
        if (y is null)
        {
            return false;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        return x.Id.Equals(y.Id);
    }

    /// <summary>
    /// Returns a hash code based on <see cref="Enumeration.Id"/>.
    /// </summary>
    /// <param name="obj">The value for which to get a hash code.</param>
    /// <returns>A hash code for <paramref name="obj"/>.</returns>
    public override int GetHashCode([DisallowNull] Enumeration obj)
    {
        return obj.Id.GetHashCode();
    }
}