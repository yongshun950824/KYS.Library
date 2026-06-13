using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        public DbContextUnitOfWork(TDbContext context)
        {
            _context = context;
        }

        public async Task BeginAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

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

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException($"{nameof(BeginAsync)}() must be called first.");
            }

            await DisposeTransactionAsync();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeTransactionAsync();
            GC.SuppressFinalize(this);
        }

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

        private void DisposeTransaction()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback(); // Ensure any uncommitted transaction is rolled back before disposing
            _transaction.Dispose();
            _transaction = null;
        }

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
