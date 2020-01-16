namespace HEF.Sql
{
    public interface IDeleteSqlBuilder : ISqlBuilder
    {
        IDeleteSqlBuilder Table(string tableName);

        IDeleteSqlBuilder Where(string sql);

        IDeleteSqlBuilder Parameter(string name, object value);
    }
}
