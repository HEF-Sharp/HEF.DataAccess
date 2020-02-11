using HEF.Core;
using HEF.Entity;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Service.CRUD
{
    internal static class EntityValidateExtensions
    {
        internal static HEFDoResult<TResult> Validate<TEntity, TResult>(this TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var validateResults = entity.Validate(ignorePropertyExpressions);

            return HEFDoResultHelper.DoValidate<TResult>(validateResults.ToArray());
        }

        internal static HEFDoResult<TResult> ValidateInclude<TEntity, TResult>(this TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var validateResults = entity.ValidateInclude(includePropertyExpressions);

            return HEFDoResultHelper.DoValidate<TResult>(validateResults.ToArray());
        }
    }
}
