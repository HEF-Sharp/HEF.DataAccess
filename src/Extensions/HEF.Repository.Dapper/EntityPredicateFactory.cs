using HEF.Entity.Mapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using HEF.Util;
using HEF.Sql.Entity;

namespace HEF.Repository.Dapper
{
    public class EntityPredicateFactory : IEntityPredicateFactory
    {
        public EntityPredicateFactory(IEntityMapperProvider mapperProvider)
        {
            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
        }

        protected IEntityMapperProvider MapperProvider { get; }

        #region Helper Functions
        protected IEnumerable<IPropertyMap> GetKeyProperties<TEntity>()
            where TEntity : class
        {
            var mapper = MapperProvider.GetEntityMapper<TEntity>();
            return mapper.Properties.Where(p => p.KeyType != KeyType.NotAKey);
        }

        protected static object EntityPropertyValueGetter<TEntity>(TEntity entity, IPropertyMap property)
            where TEntity : class
        {
            return property.PropertyInfo.GetValue(entity);
        }

        protected static Expression<Func<TEntity, bool>> BuildPropertyPredicate<TEntity>(
            Func<IPropertyMap, object> propertyValueGetter,
            params IPropertyMap[] predicateProperties)
            where TEntity : class
        {
            Expression bodyExpr = null;
            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");

            foreach (var predicateProperty in predicateProperties)
            {
                var keyValue = propertyValueGetter(predicateProperty);

                //code: entity.ID == keyValue
                var propertyExpr = Expression.Property(parameterExpr, predicateProperty.PropertyInfo);

                var propertyType = predicateProperty.PropertyInfo.PropertyType;
                Expression propertyValueExpr = Expression.Constant(keyValue);
                if (keyValue.GetType() != propertyType)
                    propertyValueExpr = Expression.Convert(propertyValueExpr, propertyType);
                
                var propertyEqualExpr = Expression.Equal(propertyExpr, propertyValueExpr);

                bodyExpr = (bodyExpr == null) ? propertyEqualExpr
                    : Expression.AndAlso(bodyExpr, propertyEqualExpr); //多个主键字段 进行AND运算
            }

            return Expression.Lambda<Func<TEntity, bool>>(bodyExpr, parameterExpr);            
        }
        #endregion

        public Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(object id)
            where TEntity : class
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var keyProperties = GetKeyProperties<TEntity>();

            IDictionary<string, object> keyValues = null;
            bool isSimpleType = id.GetType().IsSimpleType();
            if (!isSimpleType)
            {
                keyValues = id.ToDictionaryByExpression(); //复杂类型时转换成字典
            }

            object keyValue = id;
            object keyPropertyValueGetter(IPropertyMap property)
            {
                if (!isSimpleType)
                {
                    keyValue = keyValues[property.Name];
                }
                return keyValue;
            }

            return BuildPropertyPredicate<TEntity>(keyPropertyValueGetter, keyProperties.ToArray());
        }

        public Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyProperties = GetKeyProperties<TEntity>();

            object keyPropertyValueGetter(IPropertyMap property)
            {
                return EntityPropertyValueGetter(entity, property);
            }

            return BuildPropertyPredicate<TEntity>(keyPropertyValueGetter, keyProperties.ToArray());
        }

        public Expression<Func<TEntity, bool>> GetPropertyPredicate<TEntity>(TEntity entity,
            params Expression<Func<TEntity, object>>[] wherePropertyExpressions)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (wherePropertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(wherePropertyExpressions));

            var mapper = MapperProvider.GetEntityMapper<TEntity>();
            Func<IPropertyMap, bool> predicate = p => !p.Ignored && !p.IsReadOnly;  //排除只读忽略的属性
            var whereProperties = mapper.GetProperties(predicate, false, wherePropertyExpressions);

            object propertyValueGetter(IPropertyMap property)
            {
                return EntityPropertyValueGetter(entity, property);
            }

            return BuildPropertyPredicate<TEntity>(propertyValueGetter, whereProperties.ToArray());
        }
    }
}
