using System;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    /// <summary>
    /// Defines an explicit unit of work: a transaction that is started, then either committed or rolled back.
    /// <br /><br />
    /// Disposing the unit of work rolls back any transaction that has not been committed.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BeginAsync();
        /// <summary>
        /// Saves the pending changes and commits the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitAsync();
        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RollbackAsync();
    }
}
