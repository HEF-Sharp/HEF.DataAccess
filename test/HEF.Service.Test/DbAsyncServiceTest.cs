using DataAccess.TestCommon;
using HEF.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HEF.Service.Test
{
    public class DbAsyncServiceTest
    {
        #region Helper Functions
        protected Task<int> InsertCustomerAsync(IDbAsyncRepository<Customer> repository)
        {
            var customer = new Customer
            {
                ContactName = "李文亮",
                CompanyName = "Wuhan Center Hospital",
                Phone = "15926238764",
                City = "Wuhan",
                Balance = 19726m,
                createTime = DateTime.Now
            };

            return repository.InsertAsync(customer, m => m.Country);
        }
        #endregion

        [Fact]
        public async Task TestOpenWorkUnitAsyncWithCommit()
        {
            var service = ServiceTestStatic.GetDbAsyncService<Customer>();

            var customers = await service.Repository.QueryAsync();

            var beforeCustomerCount = customers.Count();

            await using var workUnit = await service.OpenWorkUnitAsync();
            var result = await InsertCustomerAsync(service.Repository);
            await workUnit.SaveChangesAsync();

            var afterCustomerCount = customers.Count();

            Assert.Equal(1, afterCustomerCount - beforeCustomerCount);
        }

        [Fact]
        public async Task TestOpenWorkUnitAsyncNotCommit()
        {
            var service = ServiceTestStatic.GetDbAsyncService<Customer>();

            var customers = await service.Repository.QueryAsync();

            var beforeCustomerCount = customers.Count();

            await using (var workUnit = await service.OpenWorkUnitAsync())
            {
                var result = await InsertCustomerAsync(service.Repository);
            }

            var afterCustomerCount = customers.Count();

            Assert.Equal(beforeCustomerCount, afterCustomerCount);
        }
    }
}
