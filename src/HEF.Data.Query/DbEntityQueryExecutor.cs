using HEF.Sql;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryExecutor : IDbEntityQueryExecutor
    {
        public DbEntityQueryExecutor(IQueryableExpressionVisitorFactory expressionVisitorFactory,
            IDbConnectionContext dbConnectionContext)
        {
            if (expressionVisitorFactory == null)
                throw new ArgumentNullException(nameof(expressionVisitorFactory));

            ExpressionVisitor = expressionVisitorFactory.Create();

            ConnectionContext = dbConnectionContext;
        }

        protected QueryableExpressionVisitor ExpressionVisitor { get; }

        protected IDbConnectionContext ConnectionContext { get; }

        public TResult Execute<TResult>(Expression query)
        {
            var selectExpr = GetQuerySelectExpression(query);

            throw new NotImplementedException();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            var selectExpr = GetQuerySelectExpression(query);

            throw new NotImplementedException();
        }

        protected virtual SelectExpression GetQuerySelectExpression(Expression query)
        {
            var queryExpr = ExpressionVisitor.Visit(query);

            if (queryExpr is EntityQueryExpression entityQueryExpr)
            {
                return entityQueryExpr.QueryExpression;
            }

            return null;
        }

        protected virtual ISelectSqlBuilder ConvertToSelectSqlBuilder(SelectExpression selectExpr)
        {
            throw new NotImplementedException();
        }
    }
}
