using HEF.Expressions.Sql;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryExecutor : IDbEntityQueryExecutor
    {
        public DbEntityQueryExecutor(IExpressionSqlResolver exprSqlResolver)
        {
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        protected IExpressionSqlResolver ExprSqlResolver { get; }

        public TResult Execute<TResult>(Expression query)
        {
            var queryExpr = new QueryableExpressionVisitor().Visit(query);
            var selectExpr = GetQuerySelectExpression(queryExpr);

            var sqlSentence = ExprSqlResolver.Resolve(selectExpr.Predicate);

            throw new NotImplementedException();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual SelectExpression GetQuerySelectExpression(Expression expression)
        {
            if (expression is EntityQueryExpression entityQueryExpr)
            {
                return (SelectExpression)entityQueryExpr.QueryExpression;
            }

            return null;
        }
    }
}
