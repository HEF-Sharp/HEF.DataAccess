using HEF.Data;
using System;
using System.Data;

namespace HEF.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDbConnectionContext connectionContext)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
        }

        internal IDbConnectionContext ConnectionContext { get; private set; }

        public IUnitOfWork UseTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            ConnectionContext = ConnectionContext.UseTransaction(isolationLevel);

            return this;
        }

        public void SaveChanges()
        {
            ConnectionContext.Commit();
        }

        public void Dispose()
        {
            ConnectionContext.Rollback();
        }
    }
}
