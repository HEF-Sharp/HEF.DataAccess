using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    public interface IEntityPredicateFactory
    {
        Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(object id) where TEntity : class;
    }
}
