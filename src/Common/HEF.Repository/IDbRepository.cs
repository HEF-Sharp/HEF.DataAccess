using HEF.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Repository
{
    public interface IDbRepository<TEntity> where TEntity : class
    {
        IDbConnectionContext ConnectionContext { get; }

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByKey(object id);

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Query();
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int Insert(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int DeleteByKey(object id);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Delete(TEntity entity);

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        int DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions);
        #endregion
    }
}
