using HEF.Entity.Mapper;
using HEF.Expressions.Sql;
using HEF.Sql.Formatter;
using System;

namespace HEF.Sql.Entity
{
    public class EntitySqlBuilderFactory : IEntitySqlBuilderFactory
    {
        public EntitySqlBuilderFactory(ISqlBuilderFactory sqlBuilderFactory,
            IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            SqlBuilderFactory = sqlBuilderFactory ?? throw new ArgumentNullException(nameof(sqlBuilderFactory));

            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        #region Injected Properties
        protected ISqlBuilderFactory SqlBuilderFactory { get; }

        protected IEntityMapperProvider MapperProvider { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }
        #endregion

        public SelectSqlBuilder<TEntity> Select<TEntity>() where TEntity : class
        {
            return new SelectSqlBuilder<TEntity>(SqlBuilderFactory.Select(),
                MapperProvider, SqlFormatter, ExprSqlResolver);
        }

        public InsertSqlBuilder<TEntity> Insert<TEntity>() where TEntity : class
        {
            return new InsertSqlBuilder<TEntity>(SqlBuilderFactory.Insert(),
                MapperProvider, SqlFormatter);
        }

        public UpdateSqlBuilder<TEntity> Update<TEntity>() where TEntity : class
        {
            return new UpdateSqlBuilder<TEntity>(SqlBuilderFactory.Update(),
                MapperProvider, SqlFormatter, ExprSqlResolver);
        }

        public DeleteSqlBuilder<TEntity> Delete<TEntity>() where TEntity : class
        {
            return new DeleteSqlBuilder<TEntity>(SqlBuilderFactory.Delete(),
                MapperProvider, SqlFormatter, ExprSqlResolver);
        }
    }
}
