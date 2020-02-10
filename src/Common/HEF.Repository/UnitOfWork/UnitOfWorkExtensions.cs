using HEF.Data;

namespace HEF.Repository
{
    public static class UnitOfWorkExtensions
    {
        public static IAsyncUnitOfWork AsAsync(this UnitOfWork unitOfWork)
        {
            return new AsyncUnitOfWork(unitOfWork.ConnectionContext.AsAsync());
        }
    }
}
