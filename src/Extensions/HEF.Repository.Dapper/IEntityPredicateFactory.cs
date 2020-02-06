﻿using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    public interface IEntityPredicateFactory
    {
        Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(object id) where TEntity : class;

        Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(TEntity entity) where TEntity : class;

        Expression<Func<TEntity, bool>> GetPropertyPredicate<TEntity>(TEntity entity,
            params Expression<Func<TEntity, object>>[] wherePropertyExpressions) where TEntity : class;
    }
}
