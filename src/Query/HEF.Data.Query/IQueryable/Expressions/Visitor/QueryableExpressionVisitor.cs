﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class QueryableExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.IsDbEntityQueryable())
            {
                var elementType = ((IQueryable)node.Value).ElementType;
                return new EntityQueryExpression(elementType);
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (method.DeclaringType == typeof(Queryable))
            {
                var sourceExpr = Visit(node.Arguments[0]);
                if (sourceExpr is EntityQueryExpression entityQueryExpr)
                {
                    var genericMethod = method.IsGenericMethod ? method.GetGenericMethodDefinition() : null;
                    switch (method.Name)
                    {
                        #region Ordering
                        case nameof(Queryable.OrderBy)
                            when genericMethod == QueryableMethods.OrderBy:
                            return QueryOrderBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), true);

                        case nameof(Queryable.OrderByDescending)
                            when genericMethod == QueryableMethods.OrderByDescending:
                            return QueryOrderBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), false);

                        case nameof(Queryable.ThenBy)
                            when genericMethod == QueryableMethods.ThenBy:
                            return QueryThenBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), true);

                        case nameof(Queryable.ThenByDescending)
                            when genericMethod == QueryableMethods.ThenByDescending:
                            return QueryThenBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), false);
                        #endregion

                        #region Paging
                        case nameof(Queryable.Skip)
                            when genericMethod == QueryableMethods.Skip:
                            return QuerySkip(entityQueryExpr, node.Arguments[1]);

                        case nameof(Queryable.Take)
                            when genericMethod == QueryableMethods.Take:
                            return QueryTake(entityQueryExpr, node.Arguments[1]);
                        #endregion

                        #region Count
                        case nameof(Queryable.Count)
                            when genericMethod == QueryableMethods.CountWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryCount(entityQueryExpr, null);

                        case nameof(Queryable.Count)
                            when genericMethod == QueryableMethods.CountWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryCount(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                        case nameof(Queryable.LongCount)
                            when genericMethod == QueryableMethods.LongCountWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryLongCount(entityQueryExpr, null);

                        case nameof(Queryable.LongCount)
                            when genericMethod == QueryableMethods.LongCountWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryLongCount(entityQueryExpr, GetLambdaExpressionFromArgument(1));
                        #endregion

                        #region FirstOrDefault
                        case nameof(Queryable.First)
                            when genericMethod == QueryableMethods.FirstWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryFirstOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.First)
                            when genericMethod == QueryableMethods.FirstWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryFirstOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                        case nameof(Queryable.FirstOrDefault)
                            when genericMethod == QueryableMethods.FirstOrDefaultWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QueryFirstOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.FirstOrDefault)
                            when genericMethod == QueryableMethods.FirstOrDefaultWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QueryFirstOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));
                        #endregion

                        #region LastOrDefault
                        case nameof(Queryable.Last)
                            when genericMethod == QueryableMethods.LastWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryLastOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.Last)
                            when genericMethod == QueryableMethods.LastWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QueryLastOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                        case nameof(Queryable.LastOrDefault)
                            when genericMethod == QueryableMethods.LastOrDefaultWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QueryLastOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.LastOrDefault)
                            when genericMethod == QueryableMethods.LastOrDefaultWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QueryLastOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));
                        #endregion

                        #region SingleOrDefault
                        case nameof(Queryable.Single)
                            when genericMethod == QueryableMethods.SingleWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QuerySingleOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.Single)
                            when genericMethod == QueryableMethods.SingleWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(false);
                            return QuerySingleOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                        case nameof(Queryable.SingleOrDefault)
                            when genericMethod == QueryableMethods.SingleOrDefaultWithoutPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QuerySingleOrDefault(entityQueryExpr, null);

                        case nameof(Queryable.SingleOrDefault)
                            when genericMethod == QueryableMethods.SingleOrDefaultWithPredicate:
                            entityQueryExpr = entityQueryExpr.QuerySingle(true);
                            return QuerySingleOrDefault(entityQueryExpr, GetLambdaExpressionFromArgument(1));
                        #endregion

                        case nameof(Queryable.Where)
                            when genericMethod == QueryableMethods.Where:
                            return QueryWhere(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                            LambdaExpression GetLambdaExpressionFromArgument(int argumentIndex) =>
                                node.Arguments[argumentIndex].UnwrapLambdaFromQuote();
                    }
                }
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
        }

        protected virtual EntityQueryExpression QueryWhere(EntityQueryExpression sourceExpr, LambdaExpression predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            sourceExpr.QueryExpression.ApplyPredicate(predicate);

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QueryOrderBy(EntityQueryExpression sourceExpr, LambdaExpression keySelector, bool ascending)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            sourceExpr.QueryExpression.ApplyOrdering(new OrderingExpression(keySelector, ascending));

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QueryThenBy(EntityQueryExpression sourceExpr, LambdaExpression keySelector, bool ascending)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            sourceExpr.QueryExpression.AppendOrdering(new OrderingExpression(keySelector, ascending));

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QuerySkip(EntityQueryExpression sourceExpr, Expression count)
        {
            if (count == null)
                throw new ArgumentNullException(nameof(count));

            sourceExpr.QueryExpression.ApplyOffset(count);

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QueryTake(EntityQueryExpression sourceExpr, Expression count)
        {
            if (count == null)
                throw new ArgumentNullException(nameof(count));

            sourceExpr.QueryExpression.ApplyLimit(count);

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QueryCount(EntityQueryExpression sourceExpr,
            LambdaExpression predicate)
        {
            if (predicate != null)
                sourceExpr.QueryExpression.ApplyPredicate(predicate);

            sourceExpr.QueryExpression.ClearOrdering();
            sourceExpr.QueryExpression.ApplyColumn(
                new ColumnSqlExpression(Expression.Constant("COUNT(*)"), string.Empty));

            return sourceExpr.QueryReturn(typeof(int));
        }

        protected virtual EntityQueryExpression QueryLongCount(EntityQueryExpression sourceExpr,
            LambdaExpression predicate)
        {
            if (predicate != null)
                sourceExpr.QueryExpression.ApplyPredicate(predicate);

            sourceExpr.QueryExpression.ClearOrdering();
            sourceExpr.QueryExpression.ApplyColumn(
                new ColumnSqlExpression(Expression.Constant("COUNT(*)"), string.Empty));

            return sourceExpr.QueryReturn(typeof(long));
        }

        protected virtual EntityQueryExpression QueryFirstOrDefault(EntityQueryExpression sourceExpr,
            LambdaExpression predicate)
        {
            if (predicate != null)
                sourceExpr.QueryExpression.ApplyPredicate(predicate);

            sourceExpr.QueryExpression.ApplyLimit(Expression.Constant(1));

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QueryLastOrDefault(EntityQueryExpression sourceExpr,
            LambdaExpression predicate)
        {
            if (sourceExpr.QueryExpression.Orderings.Count == 0)
                throw new InvalidOperationException("orderings required for query last");

            if (predicate != null)
                sourceExpr.QueryExpression.ApplyPredicate(predicate);

            sourceExpr.QueryExpression.ReverseOrderings();
            sourceExpr.QueryExpression.ApplyLimit(Expression.Constant(1));

            return sourceExpr;
        }

        protected virtual EntityQueryExpression QuerySingleOrDefault(EntityQueryExpression sourceExpr,
            LambdaExpression predicate)
        {
            if (predicate != null)
                sourceExpr.QueryExpression.ApplyPredicate(predicate);

            sourceExpr.QueryExpression.ApplyLimit(Expression.Constant(2));

            return sourceExpr;
        }
    }
}
