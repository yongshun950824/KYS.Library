using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace KYS.EFCore.Library;

[DebuggerDisplay("Id: {Id} | Name: {Name},nq}}")]
public class Enumeration : IComparable<Enumeration>, IEquatable<Enumeration>, IEqualityComparer<Enumeration>
{
    public string Name { get; private set; }

    public long Id { get; private set; }

    protected Enumeration(long id, string name)
        => (Id, Name) = (id, name);

    public override string ToString()
        => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
        => typeof(T)
            .GetFields(BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Select(x => x.GetValue(null))
            .Cast<T>();

    public bool Equals(Enumeration other)
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

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration)
        {
            return false;
        }

        return Equals(obj as Enumeration);
    }

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }

    public int CompareTo(Enumeration other)
    {
        if (other is null)
            return 1;

        return Id.CompareTo(other.Id);
    }

    public bool Equals(Enumeration x, Enumeration y)
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

    public int GetHashCode([DisallowNull] Enumeration obj)
    {
        return obj.Id.GetHashCode();
    }

    #region Relational operators
    public static bool operator ==(Enumeration left, Enumeration right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Enumeration left, Enumeration right)
        => !(left == right);

    public static bool operator <(Enumeration left, Enumeration right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Enumeration left, Enumeration right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Enumeration left, Enumeration right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Enumeration left, Enumeration right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
    #endregion
}
