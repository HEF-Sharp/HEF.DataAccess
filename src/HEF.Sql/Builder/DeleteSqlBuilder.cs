using System.Linq;
using System.Text;

namespace HEF.Sql
{
    public class DeleteSqlBuilder : ISqlBuilder
    {
        private DeleteBuilderData DeleteSqlData { get; } = new DeleteBuilderData();

        public DeleteSqlBuilder Table(string tableName)
        {
            DeleteSqlData.TableName = tableName;

            return this;
        }

        public DeleteSqlBuilder Where(string sql)
        {
            if (DeleteSqlData.WhereSql.Length > 0)
                DeleteSqlData.WhereSql += " and ";
            DeleteSqlData.WhereSql += sql;

            return this;
        }

        public DeleteSqlBuilder Parameter(string name, object value)
        {
            DeleteSqlData.Parameters.Add(new SqlParameter(name, value));

            return this;
        }

        public SqlSentence Build()
        {
            var sqlStr = BuildSql();

            return new SqlSentence(sqlStr, DeleteSqlData.Parameters.ToArray());
        }

        protected string BuildSql()
        {
            var builder = new StringBuilder();
            
            builder.Append($"delete from {DeleteSqlData.TableName}");

            if (DeleteSqlData.WhereSql.Length > 0)
                builder.Append($" where {DeleteSqlData.WhereSql}");

            return builder.ToString();
        }
    }

    internal class DeleteBuilderData : SqlBuilderData
    {
        internal string TableName { get; set; } = string.Empty;

        internal string WhereSql { get; set; } = string.Empty;
    }
}
