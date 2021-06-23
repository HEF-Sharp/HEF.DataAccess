using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    internal class DapperInsertBuilder<TEntity> : DapperDataBuilder, IDapperInsertBuilder<TEntity>
        where TEntity : class
    {
        internal DapperInsertBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory)
            : base(connectionContext, entitySqlBuilderFactory)
        {
            EntityInsertSqlBuilder = EntitySqlBuilderFactory.Insert<TEntity>().Table();
        }

        #region Properties
        protected InsertSqlBuilder<TEntity> EntityInsertSqlBuilder { get; }

        protected override ISqlBuilder EntitySqlBuilder => EntityInsertSqlBuilder;
        #endregion

        public IDapperInsertBuilder<TEntity> Columns(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            EntityInsertSqlBuilder.Column(entity, includePropertyExpressions);

            return this;
        }

        public IDapperInsertBuilder<TEntity> ColumnsIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            EntityInsertSqlBuilder.ColumnIgnore(entity, ignorePropertyExpressions);

            return this;
        }
    }
}
