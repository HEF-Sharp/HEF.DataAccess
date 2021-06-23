using Dapper;
using HEF.Data;
using HEF.Sql;
using HEF.Sql.Entity;
using System;
using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    internal abstract class DapperDataBuilder : IDapperExecute
    {
        protected DapperDataBuilder(IDbConnectionContext connectionContext,
            IEntitySqlBuilderFactory entitySqlBuilderFactory)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));

            EntitySqlBuilderFactory = entitySqlBuilderFactory ?? throw new ArgumentNullException(nameof(entitySqlBuilderFactory));
        }

        #region Properties
        protected IDbConnectionContext ConnectionContext { get; }

        protected IEntitySqlBuilderFactory EntitySqlBuilderFactory { get; }

        protected abstract ISqlBuilder EntitySqlBuilder { get; }
        #endregion

        public int Execute()
        {
            var sqlSentence = EntitySqlBuilder.Build();

            return ConnectionContext.Connection.Execute(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }

        public Task<int> ExecuteAsync()
        {
            var sqlSentence = EntitySqlBuilder.Build();

            return ConnectionContext.Connection.ExecuteAsync(
                sqlSentence.SqlText, sqlSentence.FormatDynamicParameters(),
                ConnectionContext.Transaction);
        }
    }
}
