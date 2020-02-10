using HEF.Repository;

namespace HEF.Service
{
    public interface IDbAsyncService<TEntity> where TEntity : class
    {
        IDbAsyncRepository<TEntity> Repository { get; }

        IAsyncUnitOfWork OpenWorkUnit();
    }
}
