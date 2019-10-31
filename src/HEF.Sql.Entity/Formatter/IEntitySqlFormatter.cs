using HEF.Entity.Mapper;

namespace HEF.Sql
{
    public interface IEntitySqlFormatter
    {
        string TableName(IEntityMapper mapper);

        string ColumnName(IPropertyMap propertyMap);
    }
}
