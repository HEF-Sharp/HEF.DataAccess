using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class SelectExpression : Expression
    {
        private readonly List<OrderingExpression> _orderings = new List<OrderingExpression>();

        internal SelectExpression(Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }

        public Type EntityType { get; }

        public LambdaExpression Predicate { get; private set; }

        public IReadOnlyList<OrderingExpression> Orderings => _orderings;

        public override Type Type => typeof(object);

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public void ApplyPredicate(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression.Body is ConstantExpression sqlConstant
                && (bool)sqlConstant.Value)
            {
                return;
            }

            Predicate = CombinePredicates(Predicate, expression);
        }

        public void ApplyOrdering(OrderingExpression orderingExpression)
        {
            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));

            _orderings.Clear();
            _orderings.Add(orderingExpression);
        }

        public void AppendOrdering(OrderingExpression orderingExpression)
        {
            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));            

            if (_orderings.FirstOrDefault(o => o.Expression.Equals(orderingExpression.Expression)) == null)
            {
                _orderings.Add(orderingExpression);
            }
        }

        protected virtual LambdaExpression CombinePredicates(LambdaExpression leftPredicate, LambdaExpression rightPredicate)
        {
            if (leftPredicate == null)
                return rightPredicate;

            var replaceVisitor = new ParameterReplaceVisitor(rightPredicate.Parameters.Single(), leftPredicate.Parameters.Single());
            var replaceRightExpr = replaceVisitor.Visit(rightPredicate.Body);

            var combineExpr = AndAlso(leftPredicate.Body, replaceRightExpr);

            return Lambda(leftPredicate.Type, combineExpr, leftPredicate.Parameters);
        }
    }

    /// <summary>
    /// 参数替换 ExpressionVisitor
    /// </summary>
    internal class ParameterReplaceVisitor : ExpressionVisitor
    {
        internal ParameterReplaceVisitor(ParameterExpression originalParam, ParameterExpression replacementParam)
        {
            OriginalParameter = originalParam ?? throw new ArgumentNullException(nameof(originalParam));
            ReplacementParameter = replacementParam ?? throw new ArgumentNullException(nameof(replacementParam));
        }

        protected ParameterExpression OriginalParameter { get; }

        protected ParameterExpression ReplacementParameter { get; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == OriginalParameter ? ReplacementParameter : base.VisitParameter(node);
        }
    }
}
