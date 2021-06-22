using HEF.Repository;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Service
{
    public class DbAsyncService<TEntity> : IDbAsyncService<TEntity>
        where TEntity : class
    {
        public DbAsyncService(IDbAsyncRepository<TEntity> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IDbAsyncRepository<TEntity> Repository { get; }

        public Task<IAsyncUnitOfWork> OpenWorkUnitAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            return new AsyncUnitOfWork(Repository.AsyncConnectionContext)
                .UseTransactionAsync(isolationLevel, cancellationToken);
        }
    }
}
