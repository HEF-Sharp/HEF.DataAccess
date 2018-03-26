using System;
using System.Threading.Tasks;

namespace HEF.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();

        Task SaveChangesAsync();
    }
}
