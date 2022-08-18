using HEF.Data;
using HEF.Sql.Entity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    public interface IDapperRepository
    {
        IDbConnectionContext ConnectionContext { get; }

        IDbAsyncConnectionContext AsyncConnectionContext { get; }

        IEntitySqlBuilderFactory EntitySqlBuilderFactory { get; }

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByKey<TEntity>(object id) where TEntity : class;

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetByKeyAsync<TEntity>(object id) where TEntity : class;

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync<TEntity>() where TEntity : class;
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int Insert<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        Task<int> InsertAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        IDapperInsertBuilder<TEntity> Insert<TEntity>() where TEntity : class;
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int UpdateIgnore<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int UpdateByKey<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int UpdateIgnoreByKey<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        Task<int> UpdateIgnoreAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        Task<int> UpdateByKeyAsync<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        Task<int> UpdateIgnoreByKeyAsync<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        IDapperUpdateBuilder<TEntity> Update<TEntity>() where TEntity : class;
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int DeleteByKey<TEntity>(object id) where TEntity : class;

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        int DeleteByWhere<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteByKeyAsync<TEntity>(object id) where TEntity : class;

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        Task<int> DeleteByWhereAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        IDapperDeleteBuilder<TEntity> Delete<TEntity>() where TEntity : class;
        #endregion
    }

    public interface IDapperRepository<TEntity> : IDapperRepository,
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
