using HEF.Repository;
using System;

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

        public IAsyncUnitOfWork OpenWorkUnit()
        {
            return new AsyncUnitOfWork(Repository.AsyncConnectionContext);
        }
    }
}
