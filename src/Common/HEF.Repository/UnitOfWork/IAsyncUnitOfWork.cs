using System;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Repository
{
    public interface IAsyncUnitOfWork : IAsyncDisposable
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
