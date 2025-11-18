using Dm;
using System.Data;

namespace HEF.Data.DaMeng
{
    public class DaMengConnectionProvider : DbConnectionProvider
    {
        public DaMengConnectionProvider(DbOptions options)
            : base(options)
        { }

        public override IDbConnection GetConnection() => new DmConnection(ConnectionString);
    }
}
