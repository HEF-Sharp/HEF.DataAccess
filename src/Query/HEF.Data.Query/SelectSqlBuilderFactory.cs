using HEF.Sql;

namespace HEF.Data.Query
{
    public class SelectSqlBuilderFactory : ISelectSqlBuilderFactory
    {
        public ISelectSqlBuilder Create()
        {
            return new SelectSqlBuilder();
        }
    }
}
