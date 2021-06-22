using Dapper;
using HEF.Data;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    public class DapperUpdateBuilder<TEntity> : IDapperUpdateBuilder<TEntity>
        where TEntity : class
    {
        public DapperUpdateBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));            

            EntitySqlBuilderFactory = entitySqlBuilderFactory ?? throw new ArgumentNullException(nameof(entitySqlBuilderFactory));
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));

            EntityUpdateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>().Table();
        }

        #region Properties
        protected IDbConnectionContext ConnectionContext { get; }

        protected IEntitySqlBuilderFactory EntitySqlBuilderFactory { get; }

        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected UpdateSqlBuilder<TEntity> EntityUpdateSqlBuilder { get; }
        #endregion

        public IDapperUpdateBuilder<TEntity> Key(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);

            EntityUpdateSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperUpdateBuilder<TEntity> Key(TEntity entity)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate(entity);

            EntityUpdateSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperUpdateBuilder<TEntity> Columns(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            EntityUpdateSqlBuilder.Column(entity, includePropertyExpressions);

            return this;
        }

        public IDapperUpdateBuilder<TEntity> ColumnsIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            EntityUpdateSqlBuilder.ColumnIgnore(entity, ignorePropertyExpressions);

            return this;
        }

        public IDapperUpdateBuilder<TEntity> Wheres(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var propertyPredicate = EntityPredicateFactory.GetPropertyPredicate(entity, wherePropertyExpressions);

            EntityUpdateSqlBuilder.Where(propertyPredicate);

            return this;
        }

        public int Execute()
        {
            var sqlSentence = EntityUpdateSqlBuilder.Build();

            return ConnectionContext.Connection.Execute(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }

        public Task<int> ExecuteAsync()
        {
            var sqlSentence = EntityUpdateSqlBuilder.Build();

            return ConnectionContext.Connection.ExecuteAsync(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }
    }
}
