using HEF.Sql;

namespace HEF.Data.Query
{
    public interface ISelectSqlBuilderFactory
    {
        ISelectSqlBuilder Create();
    }
}
