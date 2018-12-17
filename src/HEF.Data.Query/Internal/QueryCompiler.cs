using System;
using System.Linq.Expressions;

namespace HEF.Data.Query.Internal
{
    public class QueryCompiler : IQueryCompiler
    {
        private readonly IQueryContextFactory _queryContextFactory;

        public QueryCompiler(IQueryContextFactory queryContextFactory)
        {
            _queryContextFactory = queryContextFactory ?? throw new ArgumentNullException(nameof(queryContextFactory));
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

            query = ExtractParameters(query, _queryContextFactory.Create());

            throw new NotImplementedException();
        }

        protected virtual Expression ExtractParameters(
            Expression query,
            IParameterValues parameterValues)
        {
            throw new NotImplementedException();
        }
    }
}
