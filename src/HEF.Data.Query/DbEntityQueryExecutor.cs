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
            var sqlSentence = ExprSqlResolver.Resolve(query);

            throw new NotImplementedException();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
