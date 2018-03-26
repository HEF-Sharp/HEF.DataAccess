using HEF.Repository;

namespace HEF.Service
{
    public interface IService<TEntity> where TEntity : class
    {
        IRepository<TEntity> Repository { get; }

        IUnitOfWork OpenWorkUnit();
    }
}
