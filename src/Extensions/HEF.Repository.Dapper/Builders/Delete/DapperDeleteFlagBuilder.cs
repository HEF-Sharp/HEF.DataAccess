using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    internal class DapperDeleteFlagBuilder<TEntity> : DapperDataBuilder, IDapperDeleteBuilder<TEntity>
        where TEntity : class
    {
        internal DapperDeleteFlagBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory)
            : base(connectionContext, entitySqlBuilderFactory)
        {
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));

            EntityUpdateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>().Table().ColumnDeleteFlag();
        }

        #region Properties
        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected UpdateSqlBuilder<TEntity> EntityUpdateSqlBuilder { get; }

        protected override ISqlBuilder EntitySqlBuilder => EntityUpdateSqlBuilder;
        #endregion

        public IDapperDeleteBuilder<TEntity> Key(object id)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate<TEntity>(id);

            EntityUpdateSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperDeleteBuilder<TEntity> Key(TEntity entity)
        {
            var keyPredicate = EntityPredicateFactory.GetKeyPredicate(entity);

            EntityUpdateSqlBuilder.Where(keyPredicate);

            return this;
        }

        public IDapperDeleteBuilder<TEntity> Wheres(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
        {
            var propertyPredicate = EntityPredicateFactory.GetPropertyPredicate(entity, wherePropertyExpressions);

            EntityUpdateSqlBuilder.Where(propertyPredicate);

            return this;
        }
    }
}
