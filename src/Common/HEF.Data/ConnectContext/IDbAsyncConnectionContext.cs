using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Data
{
    public interface IDbAsyncConnectionContext : IAsyncDisposable
    {
        DbConnection Connection { get; }

        DbTransaction Transaction { get; }

        Task EnsureConnectionOpenAsync(CancellationToken cancellationToken = default);

        Task<IDbAsyncConnectionContext> UseTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        Task<IDbAsyncConnectionContext> CommitAsync(CancellationToken cancellationToken = default);

        Task<IDbAsyncConnectionContext> RollbackAsync(CancellationToken cancellationToken = default);
    }
}
