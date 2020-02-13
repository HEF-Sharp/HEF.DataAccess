using System;
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
                        case nameof(Queryable.Where)
                            when genericMethod == QueryableMethods.Where:
                            return QueryWhere(entityQueryExpr, GetLambdaExpressionFromArgument(1));

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

                        case nameof(Queryable.Skip)
                            when genericMethod == QueryableMethods.Skip:
                            return QuerySkip(entityQueryExpr, node.Arguments[1]);

                        case nameof(Queryable.Take)
                            when genericMethod == QueryableMethods.Take:
                            return QueryTake(entityQueryExpr, node.Arguments[1]);

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
    }
}
