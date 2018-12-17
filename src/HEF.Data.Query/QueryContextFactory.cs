using System;

namespace HEF.Data.Query
{
    public class QueryContextFactory : IQueryContextFactory
    {
        public QueryContextFactory(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected virtual DbContext DbContext { get; }

        public virtual QueryContext Create()
            => new QueryContext(DbContext);
    }
}
