using HEF.Data;
using System;
using System.Threading.Tasks;

namespace HEF.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected IDbContext DbContext { get; }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
