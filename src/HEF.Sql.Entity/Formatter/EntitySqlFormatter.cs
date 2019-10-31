using HEF.Entity.Mapper;
using System;

namespace HEF.Sql
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

        public string ColumnName(IPropertyMap propertyMap)
        {
            if (string.IsNullOrWhiteSpace(propertyMap.ColumnName))
                throw new ArgumentNullException(nameof(propertyMap.ColumnName), "columnName cannot be null or empty.");

            if (string.Compare(propertyMap.ColumnName, propertyMap.Name) == 0)
                return Formatter.Name(propertyMap.ColumnName);

            return Formatter.Alias(Formatter.Name(propertyMap.ColumnName), Formatter.Name(propertyMap.Name));
        }
    }
}
