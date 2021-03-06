﻿using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    internal class DapperUpdateBuilder<TEntity> : DapperDataBuilder, IDapperUpdateBuilder<TEntity>
        where TEntity : class
    {
        internal DapperUpdateBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory,
            IEntityPredicateFactory entityPredicateFactory)
            : base(connectionContext, entitySqlBuilderFactory)
        {
            EntityPredicateFactory = entityPredicateFactory ?? throw new ArgumentNullException(nameof(entityPredicateFactory));

            EntityUpdateSqlBuilder = EntitySqlBuilderFactory.Update<TEntity>().Table();
        }

        #region Properties
        protected IEntityPredicateFactory EntityPredicateFactory { get; }

        protected UpdateSqlBuilder<TEntity> EntityUpdateSqlBuilder { get; }

        protected override ISqlBuilder EntitySqlBuilder => EntityUpdateSqlBuilder;
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
    }
}
