using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using System;
using System.Linq.Expressions;

namespace HEF.Data.Query.Internal
{
    public class QueryModelGenerator : IQueryModelGenerator
    {
        private readonly INodeTypeProvider _nodeTypeProvider;
        private readonly IEvaluatableExpressionFilter _reLinqEvaluatableExpressionFilter;

        public QueryModelGenerator(
            INodeTypeProviderFactory nodeTypeProviderFactory,
            IEvaluatableExpressionFilter reLinqEvaluatableExpressionFilter)
        {
            if (nodeTypeProviderFactory == null)
                throw new ArgumentNullException(nameof(nodeTypeProviderFactory));

            _nodeTypeProvider = nodeTypeProviderFactory.Create();

            _reLinqEvaluatableExpressionFilter = reLinqEvaluatableExpressionFilter
                ?? throw new ArgumentNullException(nameof(reLinqEvaluatableExpressionFilter));
        }

        public virtual QueryModel ParseQuery(Expression query)
            => CreateQueryParser(_nodeTypeProvider).GetParsedQuery(query);

        private QueryParser CreateQueryParser(INodeTypeProvider nodeTypeProvider)
            => new QueryParser(
                new ExpressionTreeParser(
                    nodeTypeProvider,
                    new CompoundExpressionTreeProcessor(
                        new IExpressionTreeProcessor[]
                        {
                            new PartialEvaluatingExpressionTreeProcessor(_reLinqEvaluatableExpressionFilter),
                            new TransformingExpressionTreeProcessor(ExpressionTransformerRegistry.CreateDefault())
                        })));
    }
}
