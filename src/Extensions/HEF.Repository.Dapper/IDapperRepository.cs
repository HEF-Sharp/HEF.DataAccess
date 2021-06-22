namespace HEF.Repository.Dapper
{
    public interface IDapperRepository<TEntity> :
        IDbRepository<TEntity>, IDbAsyncRepository<TEntity> where TEntity : class
    {
        #region 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        IDapperUpdateBuilder<TEntity> Update();
        #endregion
    }
}
