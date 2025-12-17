using System;
using System.Threading.Tasks;

namespace KYS.EFCore.Library
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
