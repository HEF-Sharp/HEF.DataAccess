using DataAccess.TestCommon;
using HEF.Data.Query;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HEF.Data.Query.Test
{
    public class DbEntityAsyncEnumerableTest
    {
        [Fact]
        public async Task TestDbEntityAsyncEnumerable()
        {
            IQueryable<Customer> customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerEnumerable = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Hangzhou")
                .OrderBy(m => m.createTime).ThenBy(m => m.id).Skip(1).Take(5).AsAsyncEnumerable();

            var customerList = await customerEnumerable.ToListAsync();

            Assert.True(customerList.Count > 0);
        }

        [Fact]
        public async Task TestDbEntityAsyncEnumerableSingle()
        {
            IQueryable<Customer> customers = new DbEntityQueryable<Customer>(QueryTestStatic.AsyncQueryProvider);
            var customerEnumerable = customers.Where(m => m.CompanyName.StartsWith("drore"))
                .Where(m => m.City == "Shanghai").AsAsyncEnumerable();
            var customer = await customerEnumerable.SingleAsync();

            Assert.NotNull(customer);
            Assert.True(customer.id > 0);
            
            customerEnumerable = customers.Where(m => m.CompanyName.StartsWith("drore"))
                .Where(m => m.City == "Wuhan").AsAsyncEnumerable();
            customer = await customerEnumerable.SingleOrDefaultAsync();

            Assert.Null(customer);
        }
    }
}
