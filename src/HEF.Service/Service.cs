using HEF.Repository;
using System;

namespace HEF.Service
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        public Service(IRepository<TEntity> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IRepository<TEntity> Repository { get; }

        public IUnitOfWork OpenWorkUnit()
        {
            return new UnitOfWork(Repository.DbContext);
        }
    }
}
