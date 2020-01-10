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
            var customers = new DbEntityQueryable<Customer>(new DbEntityQueryProvider(new DbEntityQueryExecutor(TestStatic.ExprSqlResolver)));
            var customerList = customers.Where(m => m.CompanyName.StartsWith("drore")).Where(m => m.City == "Hangzhou").ToList();

            Assert.True(1 == 1);
        }
    }
}
