using MySqlConnector;
using System.Data;

namespace HEF.Data.MySql
{
    public class MySqlConnectionProvider : DbConnectionProvider
    {
        public MySqlConnectionProvider(DbOptions options)
            : base(options)
        { }

        public override IDbConnection GetConnection() => new MySqlConnection(ConnectionString);
    }
}
