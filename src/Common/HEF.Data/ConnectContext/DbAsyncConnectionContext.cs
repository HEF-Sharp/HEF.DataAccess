using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Data
{
    public class DbAsyncConnectionContext : IDbAsyncConnectionContext
    {
        public DbAsyncConnectionContext(IDbConnectionContext connectionContext)
        {
            if (connectionContext == null)
                throw new ArgumentNullException(nameof(connectionContext));

            if (connectionContext.Connection is DbConnection dbConnection)
                Connection = dbConnection;
            else
                throw new ArgumentException("Async operations require use of a DbConnection");

            if (connectionContext.Transaction != null)
            {
                if (connectionContext.Transaction is DbTransaction dbTransaction)
                    Transaction = dbTransaction;
                else
                    throw new ArgumentException("Async operations require use of a DbTransaction");
            }
        }

        public DbConnection Connection { get; }

        public DbTransaction Transaction { get; private set; }

        public Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
        {
            if (Connection.State == ConnectionState.Closed)
            {
                return Connection.OpenAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        #region Transaction
        public async Task<IDbAsyncConnectionContext> UseTransactionAsync(
            IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            if (Transaction != null)
                throw new InvalidOperationException("connection has already begin transaction");

            await EnsureConnectionOpenAsync(cancellationToken);
            Transaction = await Connection.BeginTransactionAsync(isolationLevel, cancellationToken);

            return this;
        }

        public async Task<IDbAsyncConnectionContext> CommitAsync(CancellationToken cancellationToken)
        {
            await TransactionAction(() => Transaction.CommitAsync(cancellationToken));

            return this;
        }

        public async Task<IDbAsyncConnectionContext> RollbackAsync(CancellationToken cancellationToken)
        {
            await TransactionAction(() => Transaction.RollbackAsync(cancellationToken));

            return this;
        }

        private async Task TransactionAction(Func<Task> action)
        {
            if (Transaction == null)
                return;

            await action();

            Transaction = null;
        }
        #endregion

        private async ValueTask CloseConnectionAsync()
        {
            if (Transaction != null)
                await RollbackAsync(default);

            await Connection.CloseAsync();
        }

        public ValueTask DisposeAsync()
        {
            return CloseConnectionAsync();
        }
    }
}
