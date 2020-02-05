using HEF.Entity.Mapper;
using HEF.Expressions.Sql;
using HEF.Sql.Formatter;
using System;

namespace HEF.Sql.Entity
{
    public class SelectEntitySqlBuilderFactory : ISelectEntitySqlBuilderFactory
    {
        public SelectEntitySqlBuilderFactory(ISelectSqlBuilderFactory selectSqlBuilderFactory,
            IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            SelectSqlBuilderFactory = selectSqlBuilderFactory ?? throw new ArgumentNullException(nameof(selectSqlBuilderFactory));

            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        #region Injected Properties
        protected ISelectSqlBuilderFactory SelectSqlBuilderFactory { get; }

        protected IEntityMapperProvider MapperProvider { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }
        #endregion

        public SelectSqlBuilder<TEntity> Create<TEntity>() where TEntity : class
        {
            return new SelectSqlBuilder<TEntity>(SelectSqlBuilderFactory.Create(),
                MapperProvider, SqlFormatter, ExprSqlResolver);
        }
    }
}
