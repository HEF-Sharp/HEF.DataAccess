using System;
using System.Data;

namespace HEF.Data
{
    public interface IDbConnectionContext : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        IDbConnectionContext UseTransaction(IsolationLevel isolationLevel);

        IDbConnectionContext Commit();

        IDbConnectionContext Rollback();
    }
}
