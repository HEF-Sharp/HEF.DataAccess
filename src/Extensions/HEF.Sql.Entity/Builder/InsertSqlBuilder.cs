using HEF.Entity.Mapper;
using HEF.Sql.Formatter;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class InsertSqlBuilder<TEntity> : ISqlBuilder
        where TEntity : class
    {
        public InsertSqlBuilder(IInsertSqlBuilder insertSqlBuilder,
            IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));            
            
            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlBuilder = insertSqlBuilder ?? throw new ArgumentNullException(nameof(insertSqlBuilder));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
        }

        public IInsertSqlBuilder SqlBuilder { get; }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        public InsertSqlBuilder<TEntity> Table()
        {
            SqlBuilder.Table(SqlFormatter.TableName(Mapper));

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

        public SqlSentence Build() => SqlBuilder.Build();

        /// <summary>
        /// 获取Insert属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetInsertProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            Func<IPropertyMap, bool> insertPredicate =
                p => !p.Ignored && !p.IsReadOnly && p.KeyType != KeyType.Identity;  //排除自增主键和只读忽略的属性

            return Mapper.GetProperties(insertPredicate, isExclude, propertyExpressions);
        }

        private void MappingEntityProperty(TEntity entity, IPropertyMap propertyMap)
        {
            var propertyValue = propertyMap.PropertyInfo.GetValue(entity);

            SqlBuilder.Column(SqlFormatter.ColumnName(propertyMap), SqlFormatter.Parameter(propertyMap.Name), propertyValue);
        }
    }
}
