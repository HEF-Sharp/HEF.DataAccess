using System;
using System.Data;
using System.Data.Common;

namespace HEF.Data
{
    public static class ConnectionContextExtensions
    {
        public static IDbAsyncConnectionContext AsAsync(this IDbConnectionContext connectionContext)
        {
            return new DbAsyncConnectionContext(connectionContext);
        }

        public static DbConnection AsDbConnection(this IDbConnection connection)
        {
            if (connection == null)
                return null;

            if (connection is DbConnection dbConnection)
                return dbConnection;

            throw new InvalidCastException("cast to DbConnection failed");
        }

        public static DbTransaction AsDbTransaction(this IDbTransaction transaction)
        {
            if (transaction == null)
                return null;

            if (transaction is DbTransaction dbTransaction)
                return dbTransaction;

            throw new InvalidCastException("cast to DbTransaction failed");
        }
    }
}
