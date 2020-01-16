using System.Linq;
using System.Text;

namespace HEF.Sql
{
    public class SelectSqlBuilder : ISelectSqlBuilder
    {
        private SelectBuilderData SelectSqlData { get; } = new SelectBuilderData();

        public ISelectSqlBuilder Select(string sql)
        {
            SelectSqlData.Select += sql;

            return this;
        }

        public ISelectSqlBuilder From(string sql)
        {
            SelectSqlData.From += sql;

            return this;
        }

        public ISelectSqlBuilder Where(string sql)
        {
            if (SelectSqlData.WhereSql.Length > 0)
                SelectSqlData.WhereSql += " and ";
            SelectSqlData.WhereSql += sql;

            return this;
        }

        public ISelectSqlBuilder GroupBy(string sql)
        {
            SelectSqlData.GroupBy += sql;

            return this;
        }

        public ISelectSqlBuilder OrderBy(string sql)
        {
            SelectSqlData.OrderBy += sql;

            return this;
        }

        public ISelectSqlBuilder Having(string sql)
        {
            SelectSqlData.Having += sql;

            return this;
        }

        public ISelectSqlBuilder Limit(int count)
        {
            SelectSqlData.Limit = count;

            return this;
        }

        public ISelectSqlBuilder Offset(int count)
        {
            SelectSqlData.Offset = count;

            return this;
        }

        public ISelectSqlBuilder Paging(int currentPage, int itemsPerPage)
        {
            SelectSqlData.Limit = itemsPerPage;
            SelectSqlData.Offset = (currentPage - 1) * itemsPerPage;            

            return this;
        }

        public ISelectSqlBuilder Parameter(string name, object value)
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
            if (SelectSqlData.IsTruncate)
                return BuildTruncateSql();

            return BuildNoTruncateSql();
        }

        protected string BuildNoTruncateSql()
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

        protected virtual string BuildTruncateSql()
        {
            var builder = new StringBuilder(BuildNoTruncateSql());

            if (SelectSqlData.Limit > 0)
                builder.Append($" limit {SelectSqlData.Limit}");

            if (SelectSqlData.Offset > 0)
                builder.Append($" offset {SelectSqlData.Offset}");

            return builder.ToString();
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

        internal int Limit { get; set; }

        internal int Offset { get; set; }

        internal bool IsTruncate => Limit > 0 || Offset > 0;
    }
}
