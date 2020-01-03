using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class ExpressionSqlResolver : IExpressionSqlResolver
    {
        public ExpressionSqlResolver(IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            if (sqlFormatter == null)
                throw new ArgumentNullException(nameof(sqlFormatter));

            MapperProvider = mapperProvider;
            SqlFormatter = sqlFormatter;
        }

        protected IEntityMapperProvider MapperProvider { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        public virtual SqlSentence Resolve(Expression expression)
        {
            var resolveExecutor = new ExpressionSqlResolveExecutor(MapperProvider, SqlFormatter, expression);

            return resolveExecutor.ResolveSqlSentence;
        }
    }
}
