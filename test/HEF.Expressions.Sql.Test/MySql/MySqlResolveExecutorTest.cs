using DataAccess.TestCommon;
using System;
using System.Linq.Expressions;
using Xunit;

namespace HEF.Expressions.Sql.Test
{
    public class MySqlResolveExecutorTest
    {
        [Fact]
        public void TestResolveEqualWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.id == 1;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(`id` = 1)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveNullWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == null;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(`companyName` IS NULL)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveWhereStrWithLocalVariable()
        {
            string companyName = "drore";
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == companyName;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(`companyName` = @p0)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveMultiParamWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((`companyName` = @p0) AND (`city` = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }
    }
}
