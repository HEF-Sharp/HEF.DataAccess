using HEF.Sql;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HEF.Data.Query
{
    public class DbCommandBuilder : IDbCommandBuilder
    {
        private readonly IDbConnectionContext _connectionContext;

        private readonly StringBuilder _commandTextBuilder = new StringBuilder();
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        public DbCommandBuilder(IDbConnectionContext connectionContext)
        {
            _connectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
        }

        public IReadOnlyList<SqlParameter> Parameters => _parameters;

        public IDbCommandBuilder AddParameters(params SqlParameter[] parameters)
        {
            if (parameters.IsNotEmpty())
            {
                foreach(var parameter in parameters)
                {
                    if (parameter == null)
                        continue;

                    _parameters.Add(parameter);
                }
            }

            return this;
        }

        public IDbCommandBuilder Append(string sqlText)
        {
            if (sqlText.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(sqlText));

            _commandTextBuilder.Append(sqlText);

            return this;
        }

        public IDbCommand Build()
        {
            var command = _connectionContext.Connection.CreateCommand();

            command.CommandText = _commandTextBuilder.ToString();

            if (_connectionContext.Transaction != null)
                command.Transaction = _connectionContext.Transaction;

            foreach (var param in Parameters)
            {
                var commandParam = command.CreateParameter();
                commandParam.ParameterName = param.ParameterName;
                commandParam.Value = param.Value;

                command.Parameters.Add(commandParam);
            }

            return command;
        }
    }
}
