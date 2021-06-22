using Dapper;
using HEF.Data;
using HEF.Data.Query;
using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class
    {
        private readonly Lazy<bool> _hasDeleteFlag;

        public DapperRepository(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory,
            IAsyncQueryProvider queryProvider,
            IEntityMapperProvider mapperProvider)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
            AsyncConnectionContext = ConnectionContext.AsAsync();

            EntitySqlBuilderFactory = entitySqlBuilderFactory ?? throw new ArgumentNullException(nameof(entitySqlBuilderFactory));
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));

            QueryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));

            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            _hasDeleteFlag = new Lazy<bool>(() => MapperProvider.GetEntityMapper<TEntity>().GetDeleteFlagProperty() != null);
        }

        #region Injected Properties
        public IDbConnectionContext ConnectionContext { get; }

        public IDbAsyncConnectionContext AsyncConnectionContext { get; }

        protected IEntitySqlBuilderFactory EntitySqlBuilderFactory { get; }

        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected IAsyncQueryProvider QueryProvider { get; }

        protected IEntityMapperProvider MapperProvider { get; }
        #endregion

        #region DeleteFlag Properties
        protected bool HasDeleteFlag => _hasDeleteFlag.Value;
        #endregion

        #region Helper Functions
        protected int ExecuteSqlSentence(SqlSentence sqlSentence)
        {
            if (sqlSentence == null)
                throw new ArgumentNullException(nameof(sqlSentence));

            return ConnectionContext.Connection.Execute(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }

        protected Task<int> ExecuteSqlSentenceAsync(SqlSentence sqlSentence)
        {
            if (sqlSentence == null)
                throw new ArgumentNullException(nameof(sqlSentence));

            return AsyncConnectionContext.Connection.ExecuteAsync(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                AsyncConnectionContext.Transaction);
        }
        #endregion

        #region Build SqlSentence
        protected SqlSentence BuildSelectByKeySql(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);
            var selectSqlBuilder = EntitySqlBuilderFactory.Select<TEntity>()
                .Table().ColumnIgnore().Where(keyPredicate);

            return selectSqlBuilder.Build();
        }

        protected SqlSentence BuildInsertSql(TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var insertSqlBuilder = EntitySqlBuilderFactory.Insert<TEntity>()
                .Table()
                .ColumnIgnore(entity, ignorePropertyExpressions);

            return insertSqlBuilder.Build();
        }

        #region Delete
        protected SqlSentence BuildDeleteSql(TEntity entity)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate(entity);
            var deleteSqlBuilder = EntitySqlBuilderFactory.Delete<TEntity>()
                .Table()
                .Where(keyPredicate);

            return deleteSqlBuilder.Build();
        }

        protected SqlSentence BuildDeleteByKeySql(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);
            var deleteSqlBuilder = EntitySqlBuilderFactory.Delete<TEntity>()
                .Table()
                .Where(keyPredicate);

            return deleteSqlBuilder.Build();
        }

        protected SqlSentence BuildDeleteByWhereSql(TEntity entity,
            params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var propertyPredicate = EntityPredicateFactory.GetPropertyPredicate(
                entity, wherePropertyExpressions);
            var deleteSqlBuilder = EntitySqlBuilderFactory.Delete<TEntity>()
                .Table()
                .Where(propertyPredicate);

            return deleteSqlBuilder.Build();
        }
        #endregion

        #region DeleteFlag
        protected SqlSentence BuildUpdateDeleteFlagSql(TEntity entity)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate(entity);
            var updateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>()
                .Table()
                .ColumnDeleteFlag()
                .Where(keyPredicate);

            return updateSqlBuilder.Build();
        }

        protected SqlSentence BuildUpdateDeleteFlagByKeySql(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);
            var updateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>()
                .Table()
                .ColumnDeleteFlag()
                .Where(keyPredicate);

            return updateSqlBuilder.Build();
        }

        protected SqlSentence BuildUpdateDeleteFlagByWhereSql(TEntity entity,
            params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var propertyPredicate = EntityPredicateFactory.GetPropertyPredicate(
                entity, wherePropertyExpressions);
            var updateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>()
                .Table()
                .ColumnDeleteFlag()
                .Where(propertyPredicate);

            return updateSqlBuilder.Build();
        }
        #endregion

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
            var sqlSentence = BuildSelectByKeySql(id);

            return ConnectionContext.Connection.QuerySingleOrDefault<TEntity>(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Query()
        {
            return new DbEntityQueryable<TEntity>(QueryProvider);
        }
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int Insert(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var sqlSentence = BuildInsertSql(entity, ignorePropertyExpressions);

            return ExecuteSqlSentence(sqlSentence);
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
            return Update().Key(entity).Columns(entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            return Update().Key(entity).ColumnsIgnore(entity, ignorePropertyExpressions).Execute();
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
            return Update().Key(id).Columns(entity, includePropertyExpressions).Execute();
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
            return Update().Key(id).ColumnsIgnore(entity, ignorePropertyExpressions).Execute();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public IDapperUpdateBuilder<TEntity> Update()
        {
            return new DapperUpdateBuilder<TEntity>(ConnectionContext, EntitySqlBuilderFactory, EntityPredicateFactory);
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
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagByKeySql(id) : BuildDeleteByKeySql(id);

            return ExecuteSqlSentence(sqlSentence);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(TEntity entity)
        {
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagSql(entity) : BuildDeleteSql(entity);

            return ExecuteSqlSentence(sqlSentence);
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public int DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagByWhereSql(entity, wherePropertyExpressions)
                : BuildDeleteByWhereSql(entity, wherePropertyExpressions);

            return ExecuteSqlSentence(sqlSentence);
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
            var sqlSentence = BuildSelectByKeySql(id);

            return AsyncConnectionContext.Connection.QuerySingleOrDefaultAsync<TEntity>(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                AsyncConnectionContext.Transaction);
        }

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> QueryAsync()
        {
            await Task.Yield();

            return new DbEntityQueryable<TEntity>(QueryProvider);
        }
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> InsertAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var sqlSentence = BuildInsertSql(entity, ignorePropertyExpressions);

            return ExecuteSqlSentenceAsync(sqlSentence);
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
            return Update().Key(entity).Columns(entity, includePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            return Update().Key(entity).ColumnsIgnore(entity, ignorePropertyExpressions).ExecuteAsync();
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
            return Update().Key(id).Columns(entity, includePropertyExpressions).ExecuteAsync();
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
            return Update().Key(id).ColumnsIgnore(entity, ignorePropertyExpressions).ExecuteAsync();
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
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagByKeySql(id) : BuildDeleteByKeySql(id);

            return ExecuteSqlSentenceAsync(sqlSentence);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(TEntity entity)
        {
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagSql(entity) : BuildDeleteSql(entity);

            return ExecuteSqlSentenceAsync(sqlSentence);
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public Task<int> DeleteByWhereAsync(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var sqlSentence = HasDeleteFlag ? BuildUpdateDeleteFlagByWhereSql(entity, wherePropertyExpressions)
                : BuildDeleteByWhereSql(entity, wherePropertyExpressions);

            return ExecuteSqlSentenceAsync(sqlSentence);
        }
        #endregion

        #endregion
    }
}
