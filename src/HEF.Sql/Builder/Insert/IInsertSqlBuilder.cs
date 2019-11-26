namespace HEF.Sql
{
    public interface IInsertSqlBuilder : ISqlBuilder
    {
        IInsertSqlBuilder Table(string tableName);

        IInsertSqlBuilder Column(string columnName, object value);

        IInsertSqlBuilder Column(string columnName, string parameterName, object value);
    }
}
