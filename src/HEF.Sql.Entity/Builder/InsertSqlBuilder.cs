using HEF.Entity;
using HEF.Entity.Mapper;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql
{
    public class InsertSqlBuilder<TEntity> : InsertSqlBuilder where TEntity : class
    {
        public InsertSqlBuilder(IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            if (sqlFormatter == null)
                throw new ArgumentNullException(nameof(sqlFormatter));

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlFormatter = sqlFormatter;
        }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        public InsertSqlBuilder<TEntity> Table()
        {
            Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public InsertSqlBuilder<TEntity> Column(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var insertProperties = GetInsertProperties(false, propertyExpressions).ToList();

            insertProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        public InsertSqlBuilder<TEntity> ColumnIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var insertProperties = GetInsertProperties(true, ignorePropertyExpressions).ToList();

            insertProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        /// <summary>
        /// 获取Insert属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetInsertProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            var insertProperties = Mapper.Properties.Where(
                p => !p.Ignored && !p.IsReadOnly && p.KeyType != KeyType.Identity);  //排除自增主键和只读忽略的属性

            var propertyNames = propertyExpressions.Select(m => m.ParsePropertyName()) ?? Array.Empty<string>();

            Func<string, bool> propertyPredicate = (name) => propertyNames.Contains(name);
            if (isExclude)
            {
                propertyPredicate = (name) => !propertyNames.Contains(name);
            }

            insertProperties = insertProperties.Where(p => propertyPredicate(p.Name));

            return insertProperties;
        }

        private void MappingEntityProperty(TEntity entity, IPropertyMap propertyMap)
        {
            var propertyValue = propertyMap.PropertyInfo.GetValue(entity);            

            Column(SqlFormatter.ColumnName(propertyMap), SqlFormatter.Parameter(propertyMap.Name), propertyValue);
        }
    }
}
