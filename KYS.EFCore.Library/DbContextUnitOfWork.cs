using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        private readonly bool _explicitTransaction;
        private readonly TDbContext _context;
        private DbTransaction transaction;

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
            if (transaction != null)
            {
                transaction.RollbackAsync().GetAwaiter().GetResult();
                transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                transaction = null;
            }
        }
    }
}
