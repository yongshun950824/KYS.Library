using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork, IDisposable
        where TDbContext : DbContext
    {
        private readonly bool _explicitTransaction;
        private readonly TDbContext _context;
        private DbTransaction transaction;
        private bool _disposed = false;

        public DbContextUnitOfWork(TDbContext context) : this(context, false) { }

        public DbContextUnitOfWork(TDbContext context, bool explicitTransaction)
        {
            _explicitTransaction = explicitTransaction;
            _context = context;
        }

        public async Task BeginAsync()
        {
            if (_explicitTransaction)
            {
                await _context.Database.OpenConnectionAsync(); // Open connection
                var _connection = _context.Database.GetDbConnection();
                transaction = await _connection.BeginTransactionAsync();
                await _context.Database.UseTransactionAsync(transaction);
            }
        }

        public async Task CommitAsync()
        {
            if (_explicitTransaction)
            {
                await transaction.CommitAsync();
                transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_explicitTransaction)
            {
                await transaction.RollbackAsync();
                transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);

            return ValueTask.CompletedTask;
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
            if (transaction == null)
                return;

            transaction.Rollback();
            transaction = null;
        }

        ~DbContextUnitOfWork()
        {
            Dispose(disposing: false);
        }
    }
}
