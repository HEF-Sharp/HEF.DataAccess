using System;
using System.Data;

namespace HEF.Data
{
    public interface IDbContext : IDisposable
    {
        IDbConnection Connection { get; }
    }
}
