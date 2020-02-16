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
            IQueryable<Customer> customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerList = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Hangzhou")
                .OrderBy(m => m.createTime).ThenBy(m => m.id).Skip(1).Take(5).ToList();

            Assert.True(customerList.Count > 0);
        }

        [Fact]
        public void TestDbEntityQueryableSingle()
        {
            IQueryable<Customer> customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customer = customers.Where(m => m.CompanyName.StartsWith("drore"))
                .Where(m => m.City == "Shanghai").Single();

            Assert.NotNull(customer);
            Assert.True(customer.id > 0);

            customer = customers.Where(m => m.CompanyName.StartsWith("drore"))
                .Where(m => m.City == "Wuhan").SingleOrDefault();

            Assert.Null(customer);
        }

        [Fact]
        public void TestDbEntityQueryableCount()
        {
            IQueryable<Customer> customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerQueryable = customers.Where(m => m.CompanyName.StartsWith("drore"));

            var customerList = customerQueryable.Take(5).ToList();
            var customerCount = customerQueryable.Count();
            var customerLongCount = customerQueryable.LongCount(m => m.City == "Hangzhou");

            Assert.True(customerList.Count > 0);
            Assert.True(customerCount > 0);
            Assert.True(customerLongCount > 0);
        }
    }
}
