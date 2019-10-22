namespace HEF.Repository.Dapper
{
    public interface IDapperRepository<TEntity> :
        IDbRepository<TEntity>, IDbAsyncRepository<TEntity> where TEntity : class
    {
        
    }
}
