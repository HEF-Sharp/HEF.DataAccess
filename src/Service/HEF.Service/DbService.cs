using HEF.Repository;
using System;

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

        public IUnitOfWork OpenWorkUnit()
        {
            return new UnitOfWork(Repository.ConnectionContext);
        }
    }
}
