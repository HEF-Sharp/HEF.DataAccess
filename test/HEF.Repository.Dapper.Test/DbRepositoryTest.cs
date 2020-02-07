using DataAccess.TestCommon;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HEF.Repository.Dapper.Test
{
    public class DbRepositoryTest
    {
        #region Helper Functions
        protected int InsertCustomer(IDapperRepository<Customer> repository)
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

        protected Task<int> InsertCustomerAsync(IDapperRepository<Customer> repository)
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

        #region Sync

        #region 查询
        [Fact]
        public void TestGetByKey()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = repository.GetByKey(5);

            Assert.NotNull(customer);
            Assert.Equal(5, customer.id);
        }
        #endregion

        #region 插入
        [Fact]
        public void TestInsert()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var result = InsertCustomer(repository);

            Assert.Equal(1, result);
        }
        #endregion

        #region 更新
        [Fact]
        public void TestUpdate()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                id = 4,
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                Balance = 1582m
            };

            var result = repository.Update(customer,
                m => m.CompanyName, m => m.Phone, m => m.City, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestUpdateIgnore()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                id = 5,
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                createTime = DateTime.Now
            };

            var result = repository.UpdateIgnore(customer,
                m => m.ContactName, m => m.Country, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestUpdateByKey()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                Balance = 1582m
            };

            var result = repository.UpdateByKey(4, customer,
                m => m.CompanyName, m => m.Phone, m => m.City, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestUpdateIgnoreByKey()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                createTime = DateTime.Now
            };

            var result = repository.UpdateIgnoreByKey(5, customer,
                m => m.ContactName, m => m.Country, m => m.Balance);

            Assert.Equal(1, result);
        }
        #endregion

        #region 删除
        [Fact]
        public void TestDelete()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer { id = 7 };

            var result = repository.Delete(customer);

            Assert.True(result >= 0);
        }

        [Fact]
        public void TestDeleteByKey()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var result = repository.DeleteByKey(8);

            Assert.True(result >= 0);
        }

        [Fact]
        public void TestDeleteByWhere()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            InsertCustomer(repository);

            var customer = new Customer
            {
                ContactName = "李文亮",
                City = "Wuhan"
            };

            var result = repository.DeleteByWhere(customer, m => m.ContactName, m => m.City);

            Assert.True(result > 0);
        }
        #endregion

        #endregion

        #region Async

        #region 查询
        [Fact]
        public async Task TestGetByKeyAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = await repository.GetByKeyAsync(5);

            Assert.NotNull(customer);
            Assert.Equal(5, customer.id);
        }
        #endregion

        #region 插入
        [Fact]
        public async Task TestInsertAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var result = await InsertCustomerAsync(repository);

            Assert.Equal(1, result);
        }
        #endregion

        #region 更新
        [Fact]
        public async Task TestUpdateAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                id = 4,
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                Balance = 1582m
            };

            var result = await repository.UpdateAsync(customer,
                m => m.CompanyName, m => m.Phone, m => m.City, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task TestUpdateIgnoreAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                id = 5,
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                createTime = DateTime.Now
            };

            var result = await repository.UpdateIgnoreAsync(customer,
                m => m.ContactName, m => m.Country, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task TestUpdateByKeyAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                Balance = 1582m
            };

            var result = await repository.UpdateByKeyAsync(4, customer,
                m => m.CompanyName, m => m.Phone, m => m.City, m => m.Balance);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task TestUpdateIgnoreByKeyAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer
            {
                CompanyName = "Wuhan Center Hospital",
                Phone = "17968523876",
                City = "Wuhan",
                createTime = DateTime.Now
            };

            var result = await repository.UpdateIgnoreByKeyAsync(5, customer,
                m => m.ContactName, m => m.Country, m => m.Balance);

            Assert.Equal(1, result);
        }
        #endregion

        #region 删除
        [Fact]
        public async Task TestDeleteAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var customer = new Customer { id = 7 };

            var result = await repository.DeleteAsync(customer);

            Assert.True(result >= 0);
        }

        [Fact]
        public async Task TestDeleteByKeyAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            var result = await repository.DeleteByKeyAsync(8);

            Assert.True(result >= 0);
        }

        [Fact]
        public async Task TestDeleteByWhereAsync()
        {
            var repository = RepositoryTestStatic.GetDapperRepository<Customer>();

            await InsertCustomerAsync(repository);

            var customer = new Customer
            {
                ContactName = "李文亮",
                City = "Wuhan"
            };

            var result = await repository.DeleteByWhereAsync(customer, m => m.ContactName, m => m.City);

            Assert.True(result > 0);
        }
        #endregion

        #endregion
    }
}
