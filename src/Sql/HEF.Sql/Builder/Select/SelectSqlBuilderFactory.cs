namespace HEF.Sql
{
    public class SelectSqlBuilderFactory : ISelectSqlBuilderFactory
    {
        public ISelectSqlBuilder Create()
        {
            return new SelectSqlBuilder();
        }
    }
}
