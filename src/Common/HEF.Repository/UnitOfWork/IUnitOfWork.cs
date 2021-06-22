using System;
using System.Data;

namespace HEF.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork UseTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void SaveChanges();
    }
}
