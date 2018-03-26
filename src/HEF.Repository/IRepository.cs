using HEF.Data;

namespace HEF.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IDbContext DbContext { get; }
    }
}
