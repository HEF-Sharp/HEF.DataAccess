using System.Linq;
using System.Text;

namespace HEF.Sql
{
    public class UpdateSqlBuilder : IUpdateSqlBuilder
    {
        private UpdateBuilderData UpdateSqlData { get; } = new UpdateBuilderData();

        public IUpdateSqlBuilder Table(string tableName)
        {
            UpdateSqlData.TableName = tableName;

            return this;
        }

        public IUpdateSqlBuilder Column(string columnName, object value)
        {
            return Column(columnName, columnName, value);
        }

        public IUpdateSqlBuilder Column(string columnName, string parameterName, object value)
        {
            UpdateSqlData.Columns.Add(new SqlBuilderColumn(columnName, parameterName));

            UpdateSqlData.Parameters.Add(new SqlParameter(parameterName, value));

            return this;
        }

        public IUpdateSqlBuilder Where(string sql)
        {
            if (UpdateSqlData.WhereSql.Length > 0)
                UpdateSqlData.WhereSql += " and ";
            UpdateSqlData.WhereSql += sql;

            return this;
        }

        public IUpdateSqlBuilder Parameter(string name, object value)
        {
            UpdateSqlData.Parameters.Add(new SqlParameter(name, value));

            return this;
        }

        public SqlSentence Build()
        {
            var sqlStr = BuildSql();

            return new SqlSentence(sqlStr, UpdateSqlData.Parameters.ToArray());
        }

        protected string BuildSql()
        {
            var builder = new StringBuilder();

            var columnsSetStr = string.Join(",", UpdateSqlData.Columns.Select(column => $"{column.ColumnName} = {column.ParameterName}"));
            builder.Append($"update {UpdateSqlData.TableName} set {columnsSetStr}");

            if (UpdateSqlData.WhereSql.Length > 0)
                builder.Append($" where {UpdateSqlData.WhereSql}");

            return builder.ToString();
        }
    }

    internal class UpdateBuilderData : InsertBuilderData
    {
        internal string WhereSql { get; set; } = string.Empty;
    }
}
