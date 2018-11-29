using HEF.Data.Storage;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace HEF.Data.MySql.Storage
{
    public class MySqlConnectionProvider : DbConnectionProvider
    {
        public MySqlConnectionProvider(IDbContextOptions contextOptions,
            INamedConnectionStringResolver connectionStringResolver)
            : base(contextOptions, connectionStringResolver)
        { }

        protected override DbConnection CreateDbConnection() => new MySqlConnection(ConnectionString);
    }
}
