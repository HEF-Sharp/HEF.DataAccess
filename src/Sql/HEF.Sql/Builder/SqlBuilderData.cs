using System;
using System.Collections.Generic;

namespace HEF.Sql
{
    internal class SqlBuilderData
    {
        internal IList<SqlParameter> Parameters { get; set; } = new List<SqlParameter>();
    }

    internal class SqlBuilderColumn
    {
        internal SqlBuilderColumn(string columnName, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException(nameof(columnName));

            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            ColumnName = columnName;
            ParameterName = parameterName;
        }

        internal string ColumnName { get; set; }

        internal string ParameterName { get; set; }
    }
}
