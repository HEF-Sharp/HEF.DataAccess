using Dapper;
using HEF.Sql;
using HEF.Util;
using System;

namespace HEF.Repository.Dapper
{
    public static class SqlSentenceParametersExtensions
    {
        public static DynamicParameters FormatDynamicParameters(this SqlSentence sqlSentence)
        {
            if (sqlSentence == null)
                throw new ArgumentNullException(nameof(sqlSentence));

            var dynamicParams = new DynamicParameters();

            if (sqlSentence.Parameters.IsNotEmpty())
            {
                foreach (var sqlParam in sqlSentence.Parameters)
                {
                    dynamicParams.Add(sqlParam.ParameterName, sqlParam.Value);
                }
            }

            return dynamicParams;
        }
    }
}
