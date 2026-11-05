namespace KYS.EFCore.Library;

/// <summary>
/// Base interface for repositories of an aggregate root.
/// </summary>
/// <typeparam name="T">The type of the aggregate root.</typeparam>
public interface IRepository<T> where T : IAggregateRoot
{
    /// <summary>
    /// Gets the unit of work used to save the changes made through this repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
