using HEF.Data;
using System;

namespace HEF.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
