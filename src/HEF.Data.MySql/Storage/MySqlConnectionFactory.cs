using HEF.Data.Storage;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace HEF.Data.MySql.Storage
{
    public class MySqlConnectionFactory : DbConnectionFactory
    {
        public MySqlConnectionFactory(IDbContextOptions contextOptions,
            INamedConnectionStringResolver connectionStringResolver)
            : base(contextOptions, connectionStringResolver)
        { }

        public override DbConnection CreateDbConnection() => new MySqlConnection(ConnectionString);
    }
}
