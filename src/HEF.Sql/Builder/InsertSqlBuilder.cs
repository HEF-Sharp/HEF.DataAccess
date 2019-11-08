using System.Collections.Generic;
using System.Linq;

namespace HEF.Sql
{
    public class InsertSqlBuilder : ISqlBuilder
    {
        private InsertBuilderData InsertSqlData { get; } = new InsertBuilderData();

        public InsertSqlBuilder Table(string tableName)
        {
            InsertSqlData.TableName = tableName;

            return this;
        }

        public InsertSqlBuilder Column(string columnName, object value)
        {
            return Column(columnName, columnName, value);
        }

        public InsertSqlBuilder Column(string columnName, string parameterName, object value)
        {
            InsertSqlData.Columns.Add(new SqlBuilderColumn(columnName, parameterName));

            InsertSqlData.Parameters.Add(new SqlParameter(parameterName, value));

            return this;
        }

        public SqlSentence Build()
        {
            var sqlStr = BuildSql();

            return new SqlSentence(sqlStr, InsertSqlData.Parameters.ToArray());
        }

        protected string BuildSql()
        {
            var sortedColumns = new SortedList<string, SqlBuilderColumn>(InsertSqlData.Columns.ToDictionary(m => m.ColumnName));

            var columnsStr = string.Join(",", sortedColumns.Keys);
            var parametersStr = string.Join(",", sortedColumns.Values.Select(column => column.ParameterName));

            return $"insert into {InsertSqlData.TableName} ({columnsStr}) values ({parametersStr})";
        }
    }

    internal class InsertBuilderData : SqlBuilderData
    {
        internal string TableName { get; set; } = string.Empty;

        internal IList<SqlBuilderColumn> Columns { get; set; } = new List<SqlBuilderColumn>();
    }
}
