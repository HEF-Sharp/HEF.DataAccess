namespace HEF.Sql
{
    public interface ISelectSqlBuilder : ISqlBuilder
    {
        ISelectSqlBuilder Select(string sql);

        ISelectSqlBuilder From(string sql);

        ISelectSqlBuilder Where(string sql);

        ISelectSqlBuilder GroupBy(string sql);

        ISelectSqlBuilder OrderBy(string sql);

        ISelectSqlBuilder Having(string sql);

        ISelectSqlBuilder Paging(int currentPage, int itemsPerPage);

        ISelectSqlBuilder Parameter(string name, object value);
    }
}
