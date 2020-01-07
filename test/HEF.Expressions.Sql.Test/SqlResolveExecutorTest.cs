using System;
using System.Linq.Expressions;
using Xunit;

namespace HEF.Expressions.Sql.Test
{
    public class SqlResolveExecutorTest
    {
        [Fact]
        public void TestTranslateEqualWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.id == 1;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(id = 1)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestTranslateNullWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == null;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(companyName is null)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestTranslateWhereStrWithLocalVariable()
        {
            string companyName = "drore";
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == companyName;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(companyName = @p0)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestTranslateMultiParamWhereStr()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = @p0) and (city = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestTranslateEqualsMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => object.Equals(x.CompanyName, "drore") && x.City.Equals("Hangzhou");

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = @p0) and (city = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestTranslateByMySqlResolver()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => x.CompanyName == "drore" && x.City == "Hangzhou";

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);
            Assert.Equal("((companyName = @p0) and (city = @p1))", sqlSentence.SqlText, true);

            sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);
            Assert.Equal("((`companyName` = @p0) and (`city` = @p1))", sqlSentence.SqlText, true);
        }
    }
}
