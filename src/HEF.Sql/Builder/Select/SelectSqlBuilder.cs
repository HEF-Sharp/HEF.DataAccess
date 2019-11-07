using System.Linq;
using System.Text;

namespace HEF.Sql
{
    public class SelectSqlBuilder : ISqlBuilder
    {
        internal SelectBuilderData SelectSqlData { get; } = new SelectBuilderData();

        public SelectSqlBuilder Select(string sql)
        {
            SelectSqlData.Select += sql;

            return this;
        }

        public SelectSqlBuilder From(string sql)
        {
            SelectSqlData.From += sql;

            return this;
        }

        public SelectSqlBuilder Where(string sql)
        {
            if (SelectSqlData.WhereSql.Length > 0)
                SelectSqlData.WhereSql += " and ";
            SelectSqlData.WhereSql += sql;

            return this;
        }

        public SelectSqlBuilder GroupBy(string sql)
        {
            SelectSqlData.GroupBy += sql;

            return this;
        }

        public SelectSqlBuilder OrderBy(string sql)
        {
            SelectSqlData.OrderBy += sql;

            return this;
        }

        public SelectSqlBuilder Having(string sql)
        {
            SelectSqlData.Having += sql;

            return this;
        }

        public SelectSqlBuilder Paging(int currentPage, int itemsPerPage)
        {
            SelectSqlData.PagingCurrentPage = currentPage;
            SelectSqlData.PagingItemsPerPage = itemsPerPage;

            return this;
        }

        public SelectSqlBuilder Parameter(string name, object value)
        {
            SelectSqlData.Parameters.Add(new SqlParameter(name, value));

            return this;
        }

        public SqlSentence Build()
        {
            var sqlStr = BuildSql();

            return new SqlSentence(sqlStr, SelectSqlData.Parameters.ToArray());
        }

        protected string BuildSql()
        {
            if (SelectSqlData.IsPaging)
                return BuildPagingSql();

            return BuildNoPagingSql();
        }

        protected string BuildNoPagingSql()
        {
            var builder = new StringBuilder();

            builder.Append($"select {SelectSqlData.Select} from {SelectSqlData.From}");

            if (SelectSqlData.WhereSql.Length > 0)
                builder.Append($" where {SelectSqlData.WhereSql}");

            if (SelectSqlData.GroupBy.Length > 0)
                builder.Append($" group by {SelectSqlData.GroupBy}");

            if (SelectSqlData.Having.Length > 0)
                builder.Append($" having {SelectSqlData.Having}");

            if (SelectSqlData.OrderBy.Length > 0)
                builder.Append($" order by {SelectSqlData.OrderBy}");

            return builder.ToString();
        }

        protected virtual string BuildPagingSql()
        {
            var sqlStr = BuildNoPagingSql();

            sqlStr += $" limit {SelectSqlData.GetFromItemNumber() - 1}, {SelectSqlData.PagingItemsPerPage}";

            return sqlStr;
        }
    }

    internal class SelectBuilderData : SqlBuilderData
    {
        internal string Select { get; set; } = string.Empty;

        internal string From { get; set; } = string.Empty;

        internal string WhereSql { get; set; } = string.Empty;

        internal string GroupBy { get; set; } = string.Empty;

        internal string OrderBy { get; set; } = string.Empty;

        internal string Having { get; set; } = string.Empty;

        internal int PagingCurrentPage { get; set; } = 1;

        internal int PagingItemsPerPage { get; set; }

        internal bool IsPaging => PagingCurrentPage > 0 && PagingItemsPerPage > 0;

        internal int GetFromItemNumber()
        {
            return GetToItemNumber() - PagingItemsPerPage + 1;
        }

        internal int GetToItemNumber()
        {
            return PagingCurrentPage * PagingItemsPerPage;
        }
    }
}
