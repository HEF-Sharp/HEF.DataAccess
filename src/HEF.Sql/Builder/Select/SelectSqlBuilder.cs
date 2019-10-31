using System.Text;

namespace HEF.Sql
{
    public class SelectSqlBuilder : ISqlBuilder
    {
        internal SelectBuilderData SqlData { get; } = new SelectBuilderData();

        public SelectSqlBuilder Select(string sql)
        {
            SqlData.Select += sql;

            return this;
        }

        public SelectSqlBuilder From(string sql)
        {
            SqlData.From += sql;

            return this;
        }

        public SelectSqlBuilder Where(string sql)
        {
            if (SqlData.WhereSql.Length > 0)
                SqlData.WhereSql += " and ";
            SqlData.WhereSql += sql;

            return this;
        }

        public SelectSqlBuilder GroupBy(string sql)
        {
            SqlData.GroupBy += sql;

            return this;
        }

        public SelectSqlBuilder OrderBy(string sql)
        {
            SqlData.OrderBy += sql;

            return this;
        }

        public SelectSqlBuilder Having(string sql)
        {
            SqlData.Having += sql;

            return this;
        }

        public SelectSqlBuilder Paging(int currentPage, int itemsPerPage)
        {
            SqlData.PagingCurrentPage = currentPage;
            SqlData.PagingItemsPerPage = itemsPerPage;

            return this;
        }

        public string Build()
        {
            if (SqlData.IsPaging)
                return PagingBuild();

            return NoPagingBuild();
        }

        protected string NoPagingBuild()
        {
            var sql = new StringBuilder();

            sql.Append($"select {SqlData.Select} from {SqlData.From}");

            if (SqlData.WhereSql.Length > 0)
                sql.Append($" where {SqlData.WhereSql}");

            if (SqlData.GroupBy.Length > 0)
                sql.Append($" group by {SqlData.GroupBy}");

            if (SqlData.Having.Length > 0)
                sql.Append($" having {SqlData.Having}");

            if (SqlData.OrderBy.Length > 0)
                sql.Append($"  order by {SqlData.OrderBy}");

            return sql.ToString();
        }

        protected virtual string PagingBuild()
        {
            var sql = NoPagingBuild();
            
            sql += $" limit {SqlData.GetFromItemNumber() - 1}, {SqlData.PagingItemsPerPage}";

            return sql;
        }
    }

    internal class SelectBuilderData
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
