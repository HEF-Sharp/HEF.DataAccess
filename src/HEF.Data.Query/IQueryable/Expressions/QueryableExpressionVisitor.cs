using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    internal class QueryableExpressionVisitor : ExpressionVisitor
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
                            return ApplyWhere(entityQueryExpr, GetLambdaExpressionFromArgument(1));

                        case nameof(Queryable.OrderBy)
                            when genericMethod == QueryableMethods.OrderBy:
                            return ApplyOrderBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), true);

                        case nameof(Queryable.OrderByDescending)
                            when genericMethod == QueryableMethods.OrderByDescending:
                            return ApplyOrderBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), false);

                        case nameof(Queryable.ThenBy)
                            when genericMethod == QueryableMethods.ThenBy:
                            return ApplyThenBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), true);

                        case nameof(Queryable.ThenByDescending)
                            when genericMethod == QueryableMethods.ThenByDescending:
                            return ApplyThenBy(entityQueryExpr, GetLambdaExpressionFromArgument(1), false);

                            LambdaExpression GetLambdaExpressionFromArgument(int argumentIndex) =>
                                node.Arguments[argumentIndex].UnwrapLambdaFromQuote();
                    }                    
                }
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
        }

        protected virtual EntityQueryExpression ApplyWhere(EntityQueryExpression sourceExpr, LambdaExpression predicate)
        {
            if (sourceExpr == null)
                throw new ArgumentNullException(nameof(sourceExpr));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            ((SelectExpression)sourceExpr.QueryExpression).ApplyPredicate(predicate);

            return sourceExpr;
        }

        protected virtual EntityQueryExpression ApplyOrderBy(EntityQueryExpression sourceExpr, LambdaExpression keySelector, bool ascending)
        {
            if (sourceExpr == null)
                throw new ArgumentNullException(nameof(sourceExpr));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            ((SelectExpression)sourceExpr.QueryExpression).ApplyOrdering(new OrderingExpression(keySelector, ascending));

            return sourceExpr;
        }

        protected virtual EntityQueryExpression ApplyThenBy(EntityQueryExpression sourceExpr, LambdaExpression keySelector, bool ascending)
        {
            if (sourceExpr == null)
                throw new ArgumentNullException(nameof(sourceExpr));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            ((SelectExpression)sourceExpr.QueryExpression).AppendOrdering(new OrderingExpression(keySelector, ascending));

            return sourceExpr;
        }
    }
}
