namespace KYS.EFCore.Library;

/// <summary>
/// Marker interface for aggregate roots: the entities through which an aggregate is loaded and persisted.
/// Only aggregate roots can have an <see cref="IRepository{T}"/>.
/// </summary>
public interface IAggregateRoot
{

}
