using HEF.Entity;
using HEF.Entity.Mapper;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public static class EntityMapperExtensions
    {
        public static IEnumerable<IPropertyMap> GetProperties<TEntity>(this IEntityMapper entityMapper,
            Func<IPropertyMap, bool> propertyPredicate,
            bool isExclude, params Expression<Func<TEntity, object>>[] propertyExpressions)
            where TEntity : class
        {
            if (isExclude)
                return GetPropertiesByExclude(entityMapper, propertyPredicate, propertyExpressions);

            return GetPropertiesByInclude(entityMapper, propertyPredicate, propertyExpressions);
        }

        internal static IEnumerable<IPropertyMap> GetPropertiesByInclude<TEntity>(this IEntityMapper entityMapper,
            Func<IPropertyMap, bool> propertyPredicate, params Expression<Func<TEntity, object>>[] propertyExpressions)
            where TEntity : class
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var propertyNames = propertyExpressions.Select(m => m.ParsePropertyName()) ?? Array.Empty<string>();

            return GetPropertiesInternal<TEntity>(entityMapper, propertyPredicate, (name) => propertyNames.Contains(name));
        }

        internal static IEnumerable<IPropertyMap> GetPropertiesByExclude<TEntity>(this IEntityMapper entityMapper,
            Func<IPropertyMap, bool> propertyPredicate, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
            where TEntity : class
        {
            var propertyNames = ignorePropertyExpressions.Select(m => m.ParsePropertyName()) ?? Array.Empty<string>();

            return GetPropertiesInternal<TEntity>(entityMapper, propertyPredicate, (name) => !propertyNames.Contains(name));
        }

        private static IEnumerable<IPropertyMap> GetPropertiesInternal<TEntity>(this IEntityMapper entityMapper,
            Func<IPropertyMap, bool> propertyPredicate, Func<string, bool> propertyNamePredicate)
            where TEntity : class
        {
            return entityMapper.Properties.Where(propertyPredicate).Where(p => propertyNamePredicate(p.Name));
        }
    }
}
