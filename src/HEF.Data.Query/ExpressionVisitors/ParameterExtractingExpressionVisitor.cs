using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HEF.Data.Query.ExpressionVisitors
{
    public class ParameterExtractingExpressionVisitor : ExpressionVisitor
    {
        private readonly IParameterValues _parameterValues;
        private readonly bool _parameterize;
        private readonly bool _generateContextAccessors;
        private readonly EvaluatableExpressionFindingExpressionVisitor _evaluatableExpressionFindingExpressionVisitor;
        private readonly ContextParameterReplacingExpressionVisitor _contextParameterReplacingExpressionVisitor;

        public ParameterExtractingExpressionVisitor(
            IEvaluatableExpressionFilter evaluatableExpressionFilter,
            IParameterValues parameterValues,
            Type contextType,
            bool parameterize,
            bool generateContextAccessors)
        {
            _evaluatableExpressionFindingExpressionVisitor
                = new EvaluatableExpressionFindingExpressionVisitor(evaluatableExpressionFilter);
            _parameterValues = parameterValues;
            _parameterize = parameterize;
            _generateContextAccessors = generateContextAccessors;
            if (_generateContextAccessors)
            {
                _contextParameterReplacingExpressionVisitor
                    = new ContextParameterReplacingExpressionVisitor(contextType);
            }
        }

        public virtual Expression ExtractParameters(Expression expression)
        {
            throw new NotImplementedException();
        }

        private class ContextParameterReplacingExpressionVisitor : ExpressionVisitor
        {
            private readonly Type _contextType;

            public ContextParameterReplacingExpressionVisitor(Type contextType)
            {
                ContextParameterExpression = Expression.Parameter(contextType, "context");
                _contextType = contextType;
            }

            public ParameterExpression ContextParameterExpression { get; }

            public override Expression Visit(Expression expression)
            {
                return expression?.Type.GetTypeInfo().IsAssignableFrom(_contextType) == true
                    ? ContextParameterExpression
                    : base.Visit(expression);
            }
        }

        private class EvaluatableExpressionFindingExpressionVisitor : ExpressionVisitor
        {
            private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;

            public EvaluatableExpressionFindingExpressionVisitor(IEvaluatableExpressionFilter evaluatableExpressionFilter)
            {
                _evaluatableExpressionFilter = evaluatableExpressionFilter;
            }
        }
    }
}
