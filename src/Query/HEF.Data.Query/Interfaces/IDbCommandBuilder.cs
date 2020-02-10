using HEF.Sql;
using System.Collections.Generic;
using System.Data;

namespace HEF.Data.Query
{
    public interface IDbCommandBuilder
    {
        IDbConnectionContext ConnectionContext { get; }

        IReadOnlyList<SqlParameter> Parameters { get; }

        IDbCommandBuilder Append(string sqlText);

        IDbCommandBuilder AddParameters(params SqlParameter[] parameters);

        IDbCommand Build();
    }
}
