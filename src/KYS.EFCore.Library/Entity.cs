using System;
using System.Collections.Generic;

namespace KYS.EFCore.Library;

/// <summary>
/// Base class for domain entities, which are identified by their <see cref="Id"/> rather than by their values,
/// and which can raise domain events.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    private List<INotification> _domainEvents;

    /// <summary>
    /// Gets or sets (in derived classes) the identifier of the entity.
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// Gets the domain events raised by this entity, or <see langword="null"/> if none have been added yet.
    /// </summary>
    public List<INotification> DomainEvents => _domainEvents;

    /// <summary>
    /// Adds a domain event to <see cref="DomainEvents"/>.
    /// </summary>
    /// <param name="eventItem">The domain event to add.</param>
    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    /// <summary>
    /// Removes a domain event from <see cref="DomainEvents"/>.
    /// </summary>
    /// <param name="eventItem">The domain event to remove.</param>
    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    /// <summary>
    /// Removes all domain events from <see cref="DomainEvents"/>, typically after they have been dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    /// <summary>
    /// Determines whether the entity has not been assigned an identifier yet (e.g. it has not been persisted).
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="Id"/> equals the default value of <typeparamref name="TId"/>; otherwise <see langword="false"/>.</returns>
    public bool IsTransient()
    {
        return EqualityComparer<TId>.Default.Equals(Id, default);
    }

    /// <summary>
    /// Determines whether the specified object is an <see cref="Entity{TId}"/> equal to this entity.
    /// </summary>
    /// <param name="obj">The object to compare with this entity.</param>
    /// <returns><see langword="true"/> if the objects are equal; otherwise <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not Entity<TId>)
            return false;

        return Equals(obj as Entity<TId>);
    }

    /// <summary>
    /// Determines whether the specified entity is equal to this entity.
    /// <br /><br />
    /// Two entities are equal when they are the same instance, or when they are of the same type,
    /// neither is transient, and they have the same <see cref="Id"/>.
    /// </summary>
    /// <param name="other">The entity to compare with this entity.</param>
    /// <returns><see langword="true"/> if the entities are equal; otherwise <see langword="false"/>.</returns>
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

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns><see langword="true"/> if both are <see langword="null"/> or equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (Equals(left, null))
            return Equals(right, null);
        else
            return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns><see langword="true"/> if the entities are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
}
