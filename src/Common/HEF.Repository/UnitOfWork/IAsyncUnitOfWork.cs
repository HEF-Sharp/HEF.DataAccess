using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Repository
{
    public interface IAsyncUnitOfWork : IAsyncDisposable
    {
        Task<IAsyncUnitOfWork> UseTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
