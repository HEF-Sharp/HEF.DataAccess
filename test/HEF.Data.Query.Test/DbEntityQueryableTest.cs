using DataAccess.TestCommon;
using System.Linq;
using Xunit;

namespace HEF.Data.Query.Test
{
    public class DbEntityQueryableTest
    {
        [Fact]
        public void TestDbEntityQueryable()
        {
            var customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerList = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Hangzhou")
                .OrderBy(m => m.createTime).ThenBy(m => m.id).Skip(1).Take(5).ToList();

            Assert.True(customerList.Count > 0);
        }

        [Fact]
        public void TestDbEntityQueryableSingle()
        {
            var customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customer = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Shanghai")
                .ToList().Single();

            Assert.NotNull(customer);
            Assert.True(customer.id > 0);
        }

        [Fact]
        public void TestDbEntityQueryableCount()
        {
            var customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerQueryable = customers.Where(m => m.CompanyName.StartsWith("drore"));

            var customerList = customerQueryable.Take(5).ToList();
            var customerCount = customerQueryable.Count();

            Assert.True(customerList.Count > 0);
            Assert.True(customerCount > 0);
        }
    }
}
