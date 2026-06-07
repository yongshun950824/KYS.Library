using System;
using System.Collections.Generic;

namespace KYS.EFCore.Library;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    private List<INotification> _domainEvents;

    public TId Id { get; protected set; }

    public List<INotification> DomainEvents => _domainEvents;

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public bool IsTransient()
    {
        return EqualityComparer<TId>.Default.Equals(Id, default);
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity<TId>)
            return false;

        return Equals(obj as Entity<TId>);
    }

    public virtual bool Equals(Entity<TId> other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (other.IsTransient() || IsTransient())
            return false;

        return EqualityComparer<TId>.Default.Equals(other.Id, Id);
    }

    /// <summary>
    /// Returns a hash code for this entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
     =>
        // XOR with a prime number (31) to help distribute hash codes more evenly.
        // See: https://learn.microsoft.com/en-us/dotnet/api/system.object.gethashcode#remarks
        Id.GetHashCode() ^ 31;

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (Equals(left, null))
            return Equals(right, null);
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
}
