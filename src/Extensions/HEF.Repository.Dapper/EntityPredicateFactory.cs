using HEF.Entity.Mapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using HEF.Util;

namespace HEF.Repository.Dapper
{
    public class EntityPredicateFactory : IEntityPredicateFactory
    {
        public EntityPredicateFactory(IEntityMapperProvider mapperProvider)
        {
            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
        }

        protected IEntityMapperProvider MapperProvider { get; }

        public Expression<Func<TEntity, bool>> GetKeyPredicate<TEntity>(object id) where TEntity : class
        {
            var mapper = MapperProvider.GetEntityMapper<TEntity>();
            var keyPropertyMaps = mapper.Properties.Where(p => p.KeyType != KeyType.NotAKey);

            IDictionary<string, object> keyValues = null;
            bool isSimpleType = id.GetType().IsSimpleType();
            if (!isSimpleType)
            {
                keyValues = id.ToDictionaryByExpression(); //复杂类型时转换成字典
            }

            object keyValue = id;
            Expression bodyExpr = null;
            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");
            foreach (var keyPropertyMap in keyPropertyMaps)
            {
                if (!isSimpleType)
                {
                    keyValue = keyValues[keyPropertyMap.Name];
                }
                //code: entity.ID == keyValue
                var propertyExpr = Expression.Property(parameterExpr, keyPropertyMap.PropertyInfo);
                var propertyValueExpr = Expression.Constant(keyValue, keyPropertyMap.PropertyInfo.PropertyType);
                var propertyEqualExpr = Expression.Equal(propertyExpr, propertyValueExpr);

                bodyExpr = (bodyExpr == null) ? propertyEqualExpr
                    : Expression.AndAlso(bodyExpr, propertyEqualExpr); //多个主键字段 进行AND运算
            }
            var lambdaExpr = Expression.Lambda<Func<TEntity, bool>>(bodyExpr, parameterExpr);
            return lambdaExpr;
        }
    }
}
