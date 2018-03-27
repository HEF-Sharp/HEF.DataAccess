using System.Data.Common;

namespace HEF.Data.Storage
{
    public interface IDbConnectionFactory
    {
        string ConnectionString { get; }

        DbConnection CreateDbConnection();
    }
}
