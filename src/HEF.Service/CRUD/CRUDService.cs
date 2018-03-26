using HEF.Repository;

namespace HEF.Service.CRUD
{
    public class CRUDService<TEntity> : Service<TEntity>, ICRUDService<TEntity> where TEntity : class
    {
        public CRUDService(IRepository<TEntity> repository)
            : base(repository)
        { }
    }
}
