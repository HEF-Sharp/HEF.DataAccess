using HEF.Entity.Mapper;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class UpdateSqlBuilder<TEntity> : UpdateSqlBuilder where TEntity : class
    {
        public UpdateSqlBuilder(IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
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

        public UpdateSqlBuilder<TEntity> Table()
        {
            Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public UpdateSqlBuilder<TEntity> Column(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var updateProperties = GetUpdateProperties(false, propertyExpressions).ToList();

            updateProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        public UpdateSqlBuilder<TEntity> ColumnIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var updateProperties = GetUpdateProperties(true, ignorePropertyExpressions).ToList();

            updateProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        public UpdateSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取Update属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetUpdateProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            Func<IPropertyMap, bool> updatePredicate =
                p => !p.Ignored && !p.IsReadOnly && p.KeyType == KeyType.NotAKey;  //排除主键和只读忽略的属性

            return Mapper.GetProperties(updatePredicate, isExclude, propertyExpressions);
        }

        private void MappingEntityProperty(TEntity entity, IPropertyMap propertyMap)
        {
            var propertyValue = propertyMap.PropertyInfo.GetValue(entity);

            Column(SqlFormatter.ColumnName(propertyMap), SqlFormatter.Parameter(propertyMap.Name), propertyValue);
        }
    }
}
