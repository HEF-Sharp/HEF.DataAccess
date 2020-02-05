using Dapper;
using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using HEF.Util;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class
    {
        public DapperRepository(IDbConnectionContext connectionContext,
            ISelectEntitySqlBuilderFactory selectSqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));

            SelectSqlBuilderFactory = selectSqlBuilderFactory ?? throw new ArgumentNullException(nameof(selectSqlBuilderFactory));
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));
        }

        #region Injected Properties
        public IDbConnectionContext ConnectionContext { get; }

        protected ISelectEntitySqlBuilderFactory SelectSqlBuilderFactory { get; }

        protected IEntityPredicateFactory EntityPredicateFactory { get; }
        #endregion

        #region Helper Functions
        protected static DynamicParameters ConvertToDynamicParameters(params SqlParameter[] sqlParameters)
        {
            var dynamicParams = new DynamicParameters();

            if (sqlParameters.IsNotEmpty())
            {
                foreach(var sqlParam in sqlParameters)
                {
                    dynamicParams.Add(sqlParam.ParameterName, sqlParam.Value);
                }
            }

            return dynamicParams;
        }
        #endregion

        #region Sync

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetByKey(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);
            var selectSqlBuilder = SelectSqlBuilderFactory.Create<TEntity>()
                .ColumnIgnore().Table().Where(keyPredicate);
            var sqlSentence = selectSqlBuilder.Build();

            return ConnectionContext.Connection.QuerySingle<TEntity>(
                sqlSentence.SqlText, ConvertToDynamicParameters(sqlSentence.Parameters),
                ConnectionContext.Transaction);
        }
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public TKey Insert<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteByKey(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public int DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        #region Async

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TEntity> GetByKeyAsync(object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<TKey> InsertAsync<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteByKeyAsync(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public Task<int> DeleteByWhereAsync(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
