using System;

namespace HEF.Data.Query
{
    public class DbCommandBuilderFactory : IDbCommandBuilderFactory
    {
        public DbCommandBuilderFactory(IDbConnectionContext connectionContext)
        {
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
        }

        protected IDbConnectionContext ConnectionContext { get; }

        public virtual IDbCommandBuilder Create()
        {
            return new DbCommandBuilder(ConnectionContext);
        }
    }
}
