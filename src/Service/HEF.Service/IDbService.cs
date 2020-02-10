using HEF.Repository;

namespace HEF.Service
{
    public interface IDbService<TEntity> where TEntity : class
    {
        IDbRepository<TEntity> Repository { get; }
        
        IUnitOfWork OpenWorkUnit();
    }
}
