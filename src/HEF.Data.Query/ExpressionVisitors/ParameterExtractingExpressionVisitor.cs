using System;
using System.Collections.Generic;
using System.Linq;
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

        private IDictionary<Expression, bool> _evaluatableExpressions;

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
            private readonly ISet<ParameterExpression> _allowedParameters = new HashSet<ParameterExpression>();

            private bool _evaluatable;
            private bool _containsClosure;
            private bool _inLambda;
            private IDictionary<Expression, bool> _evaluatableExpressions;

            public EvaluatableExpressionFindingExpressionVisitor(IEvaluatableExpressionFilter evaluatableExpressionFilter)
            {
                _evaluatableExpressionFilter = evaluatableExpressionFilter;
            }

            public IDictionary<Expression, bool> Find(Expression expression)
            {
                _evaluatable = true;
                _containsClosure = false;
                _inLambda = false;
                _evaluatableExpressions = new Dictionary<Expression, bool>();
                _allowedParameters.Clear();

                Visit(expression);

                return _evaluatableExpressions;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression == null)
                {
                    return base.Visit(expression);
                }

                var parentEvaluatable = _evaluatable;
                var parentContainsClosure = _containsClosure;

                _evaluatable = IsEvalutableNodeType(expression)
                    // Extension point to disable funcletization
                    && _evaluatableExpressionFilter.IsEvaluatableExpression(expression);
                _containsClosure = false;

                base.Visit(expression);

                if (_evaluatable)
                {
                    _evaluatableExpressions[expression] = _containsClosure;
                }

                _evaluatable = parentEvaluatable && _evaluatable;
                _containsClosure = parentContainsClosure || _containsClosure;

                return expression;
            }

            protected override Expression VisitLambda<T>(Expression<T> lambdaExpression)
            {
                var oldInLambda = _inLambda;
                _inLambda = true;

                // Note: Don't skip visiting parameter here.
                // SelectMany does not use parameter in lambda but we should still block it from evaluating
                base.VisitLambda(lambdaExpression);

                _inLambda = oldInLambda;
                return lambdaExpression;
            }

            protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
            {
                Visit(memberInitExpression.Bindings, VisitMemberBinding);

                // Cannot make parameter for NewExpression if Bindings cannot be evaluated
                if (_evaluatable)
                {
                    Visit(memberInitExpression.NewExpression);
                }

                return memberInitExpression;
            }

            protected override Expression VisitListInit(ListInitExpression listInitExpression)
            {
                Visit(listInitExpression.Initializers, VisitElementInit);

                // Cannot make parameter for NewExpression if Initializers cannot be evaluated
                if (_evaluatable)
                {
                    Visit(listInitExpression.NewExpression);
                }

                return listInitExpression;
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                Visit(methodCallExpression.Object);
                var parameterInfos = methodCallExpression.Method.GetParameters();
                for (var i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    if (i == 1
                        && _evaluatableExpressions.ContainsKey(methodCallExpression.Arguments[0])
                        && methodCallExpression.Method.DeclaringType == typeof(Enumerable)
                        && methodCallExpression.Method.Name == nameof(Enumerable.Select)
                        && methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression)
                    {
                        // Allow evaluation Enumerable.Select operation
                        foreach (var parameter in lambdaExpression.Parameters)
                        {
                            _allowedParameters.Add(parameter);
                        }
                    }

                    Visit(methodCallExpression.Arguments[i]);

                    if (_evaluatableExpressions.ContainsKey(methodCallExpression.Arguments[i]))
                    {
                        if (!_inLambda)
                        {
                            // Force parameterization when not in lambada
                            _evaluatableExpressions[methodCallExpression.Arguments[i]] = true;
                        }
                    }
                }

                return methodCallExpression;
            }

            protected override Expression VisitMember(MemberExpression memberExpression)
            {
                if (memberExpression.Expression == null)
                {
                    // Static members which can change value
                    _containsClosure
                        = !(memberExpression.Member is FieldInfo fieldInfo && fieldInfo.IsInitOnly);
                }

                return base.VisitMember(memberExpression);
            }

            protected override Expression VisitParameter(ParameterExpression parameterExpression)
            {
                _evaluatable = _allowedParameters.Contains(parameterExpression);

                return base.VisitParameter(parameterExpression);
            }

            protected override Expression VisitConstant(ConstantExpression constantExpression)
            {
                _evaluatable = !(constantExpression.Value is IQueryable);
#pragma warning disable RCS1096 // Use bitwise operation instead of calling 'HasFlag'.
                _containsClosure = constantExpression.Type.Attributes.HasFlag(TypeAttributes.NestedPrivate); // Closure
                   
#pragma warning restore RCS1096 // Use bitwise operation instead of calling 'HasFlag'.

                return base.VisitConstant(constantExpression);
            }

            private static bool IsEvalutableNodeType(Expression expression)
            {
                if (expression.NodeType == ExpressionType.Extension)
                {
                    if (!expression.CanReduce)
                    {
                        return false;
                    }

                    return IsEvalutableNodeType(expression.ReduceAndCheck());
                }

                return true;
            }
        }
    }
}
