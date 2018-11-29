using System;
using System.Data.Common;

namespace HEF.Data.Storage
{
    public interface IDbConnectionProvider : IDisposable
    {
        string ConnectionString { get; }

        DbConnection Connection { get; }
    }
}
