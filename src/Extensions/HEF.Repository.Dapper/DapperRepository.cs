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
    public class DapperRepository : IDapperRepository
    {
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
        }

        #region Injected Properties
        public IDbConnectionContext ConnectionContext { get; }

        public IDbAsyncConnectionContext AsyncConnectionContext { get; }

        protected IEntitySqlBuilderFactory EntitySqlBuilderFactory { get; }

        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected IAsyncQueryProvider QueryProvider { get; }

        protected IEntityMapperProvider MapperProvider { get; }
        #endregion

        #region DeleteFlag Functions
        protected bool CheckHasDeleteFlag<TEntity>() where TEntity : class
        {
            var entityMapper = MapperProvider.GetEntityMapper<TEntity>();

            return entityMapper?.GetDeleteFlagProperty() != null;
        }
        #endregion

        #region Build SqlSentence
        protected SqlSentence BuildSelectByKeySql<TEntity>(object id) where TEntity : class
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);
            var selectSqlBuilder = EntitySqlBuilderFactory.Select<TEntity>()
                .Table().ColumnIgnore().Where(keyPredicate);

            return selectSqlBuilder.Build();
        }
        #endregion

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetByKey<TEntity>(object id) where TEntity : class
        {
            var sqlSentence = BuildSelectByKeySql<TEntity>(id);

            return ConnectionContext.Connection.QuerySingleOrDefault<TEntity>(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return new DbEntityQueryable<TEntity>(QueryProvider);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TEntity> GetByKeyAsync<TEntity>(object id) where TEntity : class
        {
            var sqlSentence = BuildSelectByKeySql<TEntity>(id);

            return AsyncConnectionContext.Connection.QuerySingleOrDefaultAsync<TEntity>(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                AsyncConnectionContext.Transaction);
        }

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> QueryAsync<TEntity>() where TEntity : class
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
        public int Insert<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Insert<TEntity>().ColumnsIgnore(entity, ignorePropertyExpressions).Execute();
        }

        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> InsertAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Insert<TEntity>().ColumnsIgnore(entity, ignorePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public IDapperInsertBuilder<TEntity> Insert<TEntity>() where TEntity : class
        {
            return new DapperInsertBuilder<TEntity>(ConnectionContext, EntitySqlBuilderFactory);
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(entity).Columns(entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnore<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(entity).ColumnsIgnore(entity, ignorePropertyExpressions).Execute();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int UpdateByKey<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(id).Columns(entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnoreByKey<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(id).ColumnsIgnore(entity, ignorePropertyExpressions).Execute();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(entity).Columns(entity, includePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(entity).ColumnsIgnore(entity, ignorePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateByKeyAsync<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(id).Columns(entity, includePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreByKeyAsync<TEntity>(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            return Update<TEntity>().Key(id).ColumnsIgnore(entity, ignorePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public IDapperUpdateBuilder<TEntity> Update<TEntity>() where TEntity : class
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
        public int DeleteByKey<TEntity>(object id) where TEntity : class
        {
            return Delete<TEntity>().Key(id).Execute();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<TEntity>(TEntity entity) where TEntity : class
        {
            return Delete<TEntity>().Key(entity).Execute();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public int DeleteByWhere<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
            where TEntity : class
        {
            return Delete<TEntity>().Wheres(entity, wherePropertyExpressions).Execute();
        }

        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteByKeyAsync<TEntity>(object id) where TEntity : class
        {
            return Delete<TEntity>().Key(id).ExecuteAsync();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return Delete<TEntity>().Key(entity).ExecuteAsync();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public Task<int> DeleteByWhereAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
            where TEntity : class
        {
            return Delete<TEntity>().Wheres(entity, wherePropertyExpressions).ExecuteAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public IDapperDeleteBuilder<TEntity> Delete<TEntity>() where TEntity : class
        {
            if (CheckHasDeleteFlag<TEntity>())
                return new DapperDeleteFlagBuilder<TEntity>(ConnectionContext, EntitySqlBuilderFactory, EntityPredicateFactory);

            return new DapperDeleteBuilder<TEntity>(ConnectionContext, EntitySqlBuilderFactory, EntityPredicateFactory);
        }
        #endregion
    }

    public class DapperRepository<TEntity> : DapperRepository, IDapperRepository<TEntity>
        where TEntity : class
    {
        public DapperRepository(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory,
            IAsyncQueryProvider queryProvider,
            IEntityMapperProvider mapperProvider)
            : base(connectionContext, entitySqlBuilderFactory, entityPredicateFactory, queryProvider, mapperProvider)
        { }

        #region Sync

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetByKey(object id) => GetByKey<TEntity>(id);

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Query() => Query<TEntity>();
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int Insert(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => Insert<TEntity>(entity, ignorePropertyExpressions);

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public IDapperInsertBuilder<TEntity> Insert() => Insert<TEntity>();
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            => Update<TEntity>(entity, includePropertyExpressions);

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => UpdateIgnore<TEntity>(entity, ignorePropertyExpressions);

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public int UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            => UpdateByKey<TEntity>(id, entity, includePropertyExpressions);

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public int UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => UpdateIgnoreByKey<TEntity>(id, entity, ignorePropertyExpressions);

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public IDapperUpdateBuilder<TEntity> Update() => Update<TEntity>();
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteByKey(object id) => DeleteByKey<TEntity>(id);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(TEntity entity) => Delete<TEntity>(entity);

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public int DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
            => DeleteByWhere<TEntity>(entity, wherePropertyExpressions);

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public IDapperDeleteBuilder<TEntity> Delete() => Delete<TEntity>();
        #endregion

        #endregion

        #region Async

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TEntity> GetByKeyAsync(object id) => GetByKeyAsync<TEntity>(id);

        /// <summary>
        /// Queryable查询
        /// </summary>
        /// <returns></returns>
        public Task<IQueryable<TEntity>> QueryAsync() => QueryAsync<TEntity>();
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>        
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> InsertAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => InsertAsync<TEntity>(entity, ignorePropertyExpressions);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            => UpdateAsync<TEntity>(entity, includePropertyExpressions);

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => UpdateIgnoreAsync<TEntity>(entity, ignorePropertyExpressions);

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
            => UpdateByKeyAsync<TEntity>(id, entity, includePropertyExpressions);

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public Task<int> UpdateIgnoreByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            => UpdateIgnoreByKeyAsync<TEntity>(id, entity, ignorePropertyExpressions);
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteByKeyAsync(object id) => DeleteByKeyAsync<TEntity>(id);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(TEntity entity) => DeleteAsync<TEntity>(entity);

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wherePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public Task<int> DeleteByWhereAsync(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
            => DeleteByWhereAsync<TEntity>(entity, wherePropertyExpressions);
        #endregion

        #endregion
    }
}
