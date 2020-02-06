using DataAccess.TestCommon;
using Xunit;

namespace HEF.Repository.Dapper.Test
{
    public class DbRepositoryTest
    {
        [Fact]
        public void TestGetByKey()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = repository.GetByKey(5);

            Assert.NotNull(customer);
            Assert.Equal(5, customer.id);
        }
    }
}
