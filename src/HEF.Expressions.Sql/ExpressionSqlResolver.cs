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
            IEntitySqlFormatter sqlFormatter,
            IMethodCallSqlResolver methodCallSqlResolver)
        {
            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            MethodCallSqlResolver = methodCallSqlResolver ?? throw new ArgumentNullException(nameof(methodCallSqlResolver));
        }

        public IEntityMapperProvider MapperProvider { get; }

        public IEntitySqlFormatter SqlFormatter { get; }

        public IMethodCallSqlResolver MethodCallSqlResolver { get; }

        public virtual SqlSentence Resolve(Expression expression)
        {
            var resolveExecutor = new ExpressionSqlResolveExecutor(MapperProvider, SqlFormatter, MethodCallSqlResolver, expression);

            return resolveExecutor.ResolveSqlSentence;
        }
    }
}
