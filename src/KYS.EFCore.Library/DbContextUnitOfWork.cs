using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    /// <summary>
    /// <see cref="IUnitOfWork"/> implementation that wraps a database transaction of a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> whose transaction is managed.</typeparam>
    /// <remarks>
    /// Disposing the unit of work rolls back any transaction that has not been committed.
    /// The <typeparamref name="TDbContext"/> instance itself is not disposed, as its lifetime is owned by the caller (e.g. the DI container).
    /// </remarks>
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextUnitOfWork{TDbContext}"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> used to start and commit transactions.</param>
        public DbContextUnitOfWork(TDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Starts a new database transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task BeginAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Saves all pending changes with <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/>
        /// and commits the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method calls the plain <c>SaveChangesAsync</c>, so no audit records are written. To audit the changes of an
        /// <see cref="DBContext.ApplicationDbContext"/>, call <see cref="DBContext.ApplicationDbContext.SaveChangesWithAuditAsync"/>
        /// before committing.
        /// </remarks>
        /// <exception cref="InvalidOperationException"><see cref="BeginAsync"/> has not been called.</exception>
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException($"{nameof(BeginAsync)}() must be called first.");
            }

            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            _transaction = null; // Clear the transaction reference after committing
        }

        /// <summary>
        /// Rolls back and disposes the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException"><see cref="BeginAsync"/> has not been called.</exception>
        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException($"{nameof(BeginAsync)}() must be called first.");
            }

            await DisposeTransactionAsync();
        }

        /// <summary>
        /// Rolls back any uncommitted transaction and releases it.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronously rolls back any uncommitted transaction and releases it.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await DisposeTransactionAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the unit of work.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release managed resources (the current transaction);
        /// <see langword="false"/> when called from a finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeTransaction();
            }

            _disposed = true;
        }

        /// <summary>
        /// Rolls back and disposes the current transaction, if any.
        /// </summary>
        private void DisposeTransaction()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback(); // Ensure any uncommitted transaction is rolled back before disposing
            _transaction.Dispose();
            _transaction = null;
        }

        /// <summary>
        /// Asynchronously rolls back and disposes the current transaction, if any.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task DisposeTransactionAsync()
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync(); // Ensure any uncommitted transaction is rolled back before disposing
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
