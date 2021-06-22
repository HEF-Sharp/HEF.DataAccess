using HEF.Repository;
using System;
using System.Data;

namespace HEF.Service
{
    public class DbService<TEntity> : IDbService<TEntity>
        where TEntity : class
    {
        public DbService(IDbRepository<TEntity> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IDbRepository<TEntity> Repository { get; }

        public IUnitOfWork OpenWorkUnit(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new UnitOfWork(Repository.ConnectionContext)
                .UseTransaction(isolationLevel);
        }
    }
}
