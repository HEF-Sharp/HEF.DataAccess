using System;
using System.Data;

namespace HEF.Data
{
    public class DbConnectionContext : IDbConnectionContext
    {
        public DbConnectionContext(IDbConnectionProvider connectionProvider)
        {
            if (connectionProvider == null)
                throw new ArgumentNullException(nameof(connectionProvider));

            Connection = connectionProvider.GetConnection();
        }

        public IDbConnection Connection { get; }
        
        public IDbTransaction Transaction { get; internal set; }

        public void EnsureConnectionOpen()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        #region Transaction
        public IDbConnectionContext UseTransaction(IsolationLevel isolationLevel)
        {
            if (Transaction != null)
                throw new InvalidOperationException("connection has already begin transaction");

            EnsureConnectionOpen();
            Transaction = Connection.BeginTransaction(isolationLevel);

            return this;
        }

        public IDbConnectionContext Commit()
        {
            TransactionAction(() => Transaction.Commit());

            return this;
        }

        public IDbConnectionContext Rollback()
        {
            TransactionAction(() => Transaction.Rollback());

            return this;
        }

        private void TransactionAction(Action action)
        {
            if (Transaction == null)
                return;
            
            action();

            Transaction = null;
        }
        #endregion

        private void CloseConnection()
        {
            if (Transaction != null)
                Rollback();

            Connection.Close();
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
