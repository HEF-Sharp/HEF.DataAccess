using System;

namespace HEF.Sql
{
    public class SqlSentence
    {
        public SqlSentence(string sqlStr, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sqlStr))
                throw new ArgumentNullException(nameof(sqlStr));

            SqlText = sqlStr;
            Parameters = parameters;
        }

        public string SqlText { get; set; }

        public SqlParameter[] Parameters { get; }
    }

    public class SqlParameter
    {
        public SqlParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));            

            ParameterName = name;
            Value = value;
        }

        public string ParameterName { get; }

        public object Value { get; }
    }
}
