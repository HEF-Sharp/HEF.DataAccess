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
            var customers = new DbEntityQueryable<Customer>(new DbEntityQueryProvider(new DbEntityQueryExecutor(new QueryableExpressionVisitorFactory(), null)));
            var customerList = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Hangzhou")
                .OrderBy(m => m.CompanyName).ThenByDescending(m => m.City).Skip(10).Take(5).ToList();

            Assert.True(1 == 1);
        }
    }
}
