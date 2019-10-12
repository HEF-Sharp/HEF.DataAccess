using System.Data;

namespace HEF.Data
{
    public interface IDbConnectionProvider
    {
        string ConnectionString { get; }

        IDbConnection GetConnection();
    }
}
