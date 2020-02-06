namespace HEF.Sql
{
    public class SqlBuilderFactory : ISqlBuilderFactory
    {
        public ISelectSqlBuilder Select()
        {
            return new SelectSqlBuilder();
        }

        public IInsertSqlBuilder Insert()
        {
            return new InsertSqlBuilder();
        }

        public IUpdateSqlBuilder Update()
        {
            return new UpdateSqlBuilder();
        }

        public IDeleteSqlBuilder Delete()
        {
            return new DeleteSqlBuilder();
        }
    }
}
