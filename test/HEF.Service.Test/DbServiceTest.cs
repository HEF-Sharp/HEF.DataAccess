using DataAccess.TestCommon;
using HEF.Repository;
using System;
using System.Linq;
using Xunit;

namespace HEF.Service.Test
{
    public class DbServiceTest
    {
        #region Helper Functions
        protected int InsertCustomer(IDbRepository<Customer> repository)
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

            return repository.Insert(customer, m => m.Country);
        }
        #endregion

        [Fact]
        public void TestOpenWorkUnitWithCommit()
        {
            var service = ServiceTestStatic.GetDbService<Customer>();

            var customers = service.Repository.Query();

            var beforeCustomerCount = customers.Count();

            using var workUnit = service.OpenWorkUnit();
            var result = InsertCustomer(service.Repository);
            workUnit.SaveChanges();

            var afterCustomerCount = customers.Count();

            Assert.Equal(1, afterCustomerCount - beforeCustomerCount);
        }

        [Fact]
        public void TestOpenWorkUnitNotCommit()
        {
            var service = ServiceTestStatic.GetDbService<Customer>();

            var customers = service.Repository.Query();

            var beforeCustomerCount = customers.Count();

            using (var workUnit = service.OpenWorkUnit())
            {
                var result = InsertCustomer(service.Repository);
            }

            var afterCustomerCount = customers.Count();

            Assert.Equal(beforeCustomerCount, afterCustomerCount);
        }
    }
}
