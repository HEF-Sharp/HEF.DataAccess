using HEF.Repository;
using System;

namespace HEF.Service
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IRepository<TEntity> Repository => _repository;

        public IUnitOfWork OpenWorkUnit()
        {
            return new UnitOfWork(_repository.DbContext);
        }
    }
}
