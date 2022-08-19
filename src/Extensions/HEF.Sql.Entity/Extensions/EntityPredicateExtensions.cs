using HEF.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    internal static class EntityPredicateExtensions
    {
        internal static Expression<Func<TEntity, bool>> CombinePredicate<TEntity>(
            this Expression<Func<TEntity, bool>> sourcePredicate, Expression<Func<TEntity, bool>> andPredicate)
            where TEntity : class
        {
            if (andPredicate == null)
                throw new ArgumentNullException(nameof(andPredicate));

            if (sourcePredicate == null)
                return andPredicate;

            var replaceVisitor = new ParameterReplaceVisitor(andPredicate.Parameters.Single(), sourcePredicate.Parameters.Single());
            var replaceAndPredicateExpr = replaceVisitor.Visit(andPredicate.Body);

            var combineExpr = Expression.AndAlso(sourcePredicate.Body, replaceAndPredicateExpr);

            return Expression.Lambda<Func<TEntity, bool>>(combineExpr, sourcePredicate.Parameters);
        }
    }
}
