namespace HEF.Sql
{
    public interface IUpdateSqlBuilder : ISqlBuilder
    {
        IUpdateSqlBuilder Table(string tableName);

        IUpdateSqlBuilder Column(string columnName, object value);

        IUpdateSqlBuilder Column(string columnName, string parameterName, object value);

        IUpdateSqlBuilder Where(string sql);

        IUpdateSqlBuilder Parameter(string name, object value);
    }
}
