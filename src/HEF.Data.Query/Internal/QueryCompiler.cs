using HEF.Data.Query.ExpressionVisitors;
using System;
using System.Linq.Expressions;

namespace HEF.Data.Query.Internal
{
    public class QueryCompiler : IQueryCompiler
    {
        private readonly Type _contextType;

        private readonly IQueryContextFactory _queryContextFactory;

        private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;

        public QueryCompiler(DbContext dbContext,
            IQueryContextFactory queryContextFactory,
            IEvaluatableExpressionFilter evaluatableExpressionFilter)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            _contextType = dbContext.GetType();

            _queryContextFactory = queryContextFactory ?? throw new ArgumentNullException(nameof(queryContextFactory));

            _evaluatableExpressionFilter = evaluatableExpressionFilter
                ?? throw new ArgumentNullException(nameof(evaluatableExpressionFilter));
        }

        public virtual TResult Execute<TResult>(Expression query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var queryContext = _queryContextFactory.Create();

            query = ExtractParameters(query, queryContext);

            throw new NotImplementedException();
        }

        public virtual Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            query = ExtractParameters(query, _queryContextFactory.Create(), parameterize: false);

            throw new NotImplementedException();
        }

        protected virtual Expression ExtractParameters(
            Expression query,
            IParameterValues parameterValues,
            bool parameterize = true,
            bool generateContextAccessors = false)
        {
            var visitor = new ParameterExtractingExpressionVisitor(
                _evaluatableExpressionFilter,
                parameterValues,
                _contextType,
                parameterize,
                generateContextAccessors);

            return visitor.ExtractParameters(query);
        }
    }
}
