using System;
using System.Data;

namespace HEF.Data
{
    public interface IDbConnectionContext : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        void EnsureConnectionOpen();

        IDbConnectionContext UseTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        IDbConnectionContext Commit();

        IDbConnectionContext Rollback();
    }
}
