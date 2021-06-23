namespace HEF.Repository.Dapper
{
    public interface IDapperRepository<TEntity> :
        IDbRepository<TEntity>, IDbAsyncRepository<TEntity>
        where TEntity : class
    {
        #region 插入
        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        IDapperInsertBuilder<TEntity> Insert();
        #endregion

        #region 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        IDapperUpdateBuilder<TEntity> Update();
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        IDapperDeleteBuilder<TEntity> Delete();
        #endregion
    }
}
