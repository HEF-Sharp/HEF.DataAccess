using HEF.Repository;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Service
{
    public interface IDbAsyncService<TEntity> where TEntity : class
    {
        IDbAsyncRepository<TEntity> Repository { get; }

        Task<IAsyncUnitOfWork> OpenWorkUnitAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);
    }
}
