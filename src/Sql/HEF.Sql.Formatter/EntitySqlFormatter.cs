﻿using HEF.Entity.Mapper;
using System;

namespace HEF.Sql.Formatter
{
    public class EntitySqlFormatter : IEntitySqlFormatter
    {
        public EntitySqlFormatter(ISqlFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        protected ISqlFormatter Formatter { get; }

        public string TableName(IEntityMapper mapper)
        {
            if (string.IsNullOrWhiteSpace(mapper.TableName))
                throw new ArgumentNullException(nameof(mapper.TableName), "tableName cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(mapper.SchemaName))
                return Formatter.Name(mapper.TableName);

            return $"{Formatter.Name(mapper.SchemaName)}.{Formatter.Name(mapper.TableName)}";
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        /// <param name="propertyMap"></param>
        /// <param name="forSelect">是否作为select查询</param>
        /// <returns></returns>
        public string ColumnName(IPropertyMap propertyMap, bool forSelect)
        {
            if (string.IsNullOrWhiteSpace(propertyMap.ColumnName))
                throw new ArgumentNullException(nameof(propertyMap.ColumnName), "columnName cannot be null or empty.");

            if (string.Compare(propertyMap.ColumnName, propertyMap.Name) != 0 && forSelect)
                return Formatter.Alias(Formatter.Name(propertyMap.ColumnName), Formatter.Name(propertyMap.Name));

            return Formatter.Name(propertyMap.ColumnName);
        }

        string ISqlFormatter.Name(string name)
        {
            return Formatter.Name(name);
        }

        string ISqlFormatter.Alias(string name, string alias)
        {
            return Formatter.Alias(name, alias);
        }

        string ISqlFormatter.Parameter(string name)
        {
            return Formatter.Parameter(name);
        }
    }
}
