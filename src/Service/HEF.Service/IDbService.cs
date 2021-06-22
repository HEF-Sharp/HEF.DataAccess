using HEF.Repository;
using System.Data;

namespace HEF.Service
{
    public interface IDbService<TEntity> where TEntity : class
    {
        IDbRepository<TEntity> Repository { get; }
        
        IUnitOfWork OpenWorkUnit(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
