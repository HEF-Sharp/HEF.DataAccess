﻿using HEF.Expressions;
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

        public ColumnSqlExpression ColumnSql { get; private set; }

        public LambdaExpression Predicate { get; private set; }

        public IReadOnlyList<OrderingExpression> Orderings => _orderings;

        public ConstantExpression Limit { get; private set; }

        public ConstantExpression Offset { get; private set; }

        public override Type Type => typeof(object);

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public virtual void ApplyColumn(ColumnSqlExpression columnSqlExpression)
        {
            if (columnSqlExpression == null)
                throw new ArgumentNullException(nameof(columnSqlExpression));

            ColumnSql = columnSqlExpression;
        }

        public virtual void ApplyPredicate(LambdaExpression whereExpression)
        {
            if (whereExpression == null)
                throw new ArgumentNullException(nameof(whereExpression));

            if (whereExpression.Body is ConstantExpression whereConstant
                && (bool)whereConstant.Value)
            {
                return;
            }

            Predicate = CombinePredicates(Predicate, whereExpression);
        }

        public virtual void ApplyOrdering(OrderingExpression orderingExpression)
        {
            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));

            _orderings.Clear();
            _orderings.Add(orderingExpression);
        }

        public virtual void AppendOrdering(OrderingExpression orderingExpression)
        {
            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));            

            if (_orderings.FirstOrDefault(o => o.Expression.Equals(orderingExpression.Expression)) == null)
            {
                _orderings.Add(orderingExpression);
            }
        }

        public virtual void ClearOrdering()
        {
            _orderings.Clear();
        }

        public virtual void ReverseOrderings()
        {
            var existingOrdering = _orderings.ToArray();

            _orderings.Clear();

            for (var i = 0; i < existingOrdering.Length; i++)
            {
                _orderings.Add(
                    new OrderingExpression(existingOrdering[i].Expression,
                    !existingOrdering[i].IsAscending));
            }
        }

        public virtual void ApplyLimit(Expression countExpression)
        {
            if (countExpression == null)
                throw new ArgumentNullException(nameof(countExpression));

            Limit = countExpression as ConstantExpression;
        }

        public virtual void ApplyOffset(Expression countExpression)
        {
            if (countExpression == null)
                throw new ArgumentNullException(nameof(countExpression));

            Offset = countExpression as ConstantExpression;
        }

        protected static LambdaExpression CombinePredicates(LambdaExpression leftPredicate, LambdaExpression rightPredicate)
        {
            if (leftPredicate == null)
                return rightPredicate;

            var replaceVisitor = new ParameterReplaceVisitor(rightPredicate.Parameters.Single(), leftPredicate.Parameters.Single());
            var replaceRightExpr = replaceVisitor.Visit(rightPredicate.Body);

            var combineExpr = AndAlso(leftPredicate.Body, replaceRightExpr);

            return Lambda(leftPredicate.Type, combineExpr, leftPredicate.Parameters);
        }
    }
}
