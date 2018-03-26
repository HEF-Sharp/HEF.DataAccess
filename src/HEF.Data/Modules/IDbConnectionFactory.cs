using System.Data.Common;

namespace HEF.Data.Modules
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateDbConnection();
    }
}
