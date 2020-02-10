using HEF.Data;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Repository
{
    public class AsyncUnitOfWork : IAsyncUnitOfWork
    {
        public AsyncUnitOfWork(IDbAsyncConnectionContext connectionContext)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
        }

        internal IDbAsyncConnectionContext ConnectionContext { get; private set; }

        public async Task<IAsyncUnitOfWork> UseTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            ConnectionContext = await ConnectionContext.UseTransactionAsync(isolationLevel, cancellationToken);

            return this;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return ConnectionContext.CommitAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await ConnectionContext.RollbackAsync();
        }
    }
}
