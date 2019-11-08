using HEF.Entity.Mapper;

namespace HEF.Sql
{
    public interface IEntitySqlFormatter : ISqlFormatter
    {
        string TableName(IEntityMapper mapper);

        /// <summary>
        /// 字段名称
        /// </summary>
        /// <param name="propertyMap"></param>
        /// <param name="forSelect">是否作为select查询</param>
        /// <returns></returns>
        string ColumnName(IPropertyMap propertyMap, bool forSelect = false);
    }
}
