using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    internal class DapperDeleteBuilder<TEntity> : DapperDataBuilder, IDapperDeleteBuilder<TEntity>
        where TEntity : class
    {
        internal DapperDeleteBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory)
            : base(connectionContext, entitySqlBuilderFactory)
        {
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));

            EntityDeleteSqlBuilder = EntitySqlBuilderFactory.Delete<TEntity>().Table();
        }

        #region Properties
        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected DeleteSqlBuilder<TEntity> EntityDeleteSqlBuilder { get; }

        protected override ISqlBuilder EntitySqlBuilder => EntityDeleteSqlBuilder;
        #endregion

        public IDapperDeleteBuilder<TEntity> Key(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);

            EntityDeleteSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperDeleteBuilder<TEntity> Key(TEntity entity)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate(entity);

            EntityDeleteSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperDeleteBuilder<TEntity> Wheres(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var propertyPredicate = EntityPredicateFactory.GetPropertyPredicate(entity, wherePropertyExpressions);

            EntityDeleteSqlBuilder.Where(propertyPredicate);

            return this;
        }
    }
}
