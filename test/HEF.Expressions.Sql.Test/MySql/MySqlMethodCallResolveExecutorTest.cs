using DataAccess.TestCommon;
using System;
using System.Linq.Expressions;
using Xunit;

namespace HEF.Expressions.Sql.Test
{
    public class MySqlMethodCallResolveExecutorTest
    {
        [Fact]
        public void TestResolveEqualsMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => Equals(x.CompanyName, "drore") && x.City.Equals("Hangzhou");

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((`companyName` = @p0) and (`city` = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveCompareMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => string.Compare(x.CompanyName, "drore") == 0 && x.City.CompareTo("Hangzhou") == 0;
            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((`companyName` = @p0) and (`city` = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);

            customerPredicate = x => string.Compare(x.CompanyName, "drore") + x.City.CompareTo("Hangzhou") == 0;
            sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((" + "(CASE WHEN `CompanyName` = @p0 THEN 0 WHEN `CompanyName` < @p0 THEN -1 ELSE 1 END)" + " + "
                + "(CASE WHEN `City` = @p1 THEN 0 WHEN `City` < @p1 THEN -1 ELSE 1 END)" + ") = 0)", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveDateTimeAddMethods()
        {
            var dtUtcNow = DateTime.UtcNow;

            Expression<Func<Customer, bool>> customerPredicate = x =>
                x.createTime.AddYears(1) > dtUtcNow
                && x.createTime.AddMonths(2) < dtUtcNow
                && x.createTime.AddDays(3) >= dtUtcNow
                && x.createTime.AddHours(4) <= dtUtcNow
                && x.createTime.AddMinutes(5) == dtUtcNow
                && x.createTime.AddSeconds(10) != dtUtcNow
                && x.createTime.AddMilliseconds(100) < dtUtcNow;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((((((" + "(DATE_ADD(`createTime`, INTERVAL 1 YEAR) > @p0)"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL 2 MONTH) < @p0))"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL 3.0 DAY) >= @p0))"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL 4.0 HOUR) <= @p0))"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL 5.0 MINUTE) = @p0))"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL 10.0 SECOND) <> @p0))"
                + " and " + "(DATE_ADD(`createTime`, INTERVAL (100.0* 1000) MICROSECOND) < @p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);
        }

        [Fact]
        public void TestResolveDecimalMathOperations()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Remainder(decimal.Divide(decimal.Multiply(decimal.Subtract(decimal.Add(x.Balance, 1), 5), 4), 2), 3) == 1;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((((((`Balance` + 1) - 5) * 4) / 2) % 3) = 1)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveDecimalRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Truncate(decimal.Floor(decimal.Ceiling(decimal.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(TRUNCATE(FLOOR(CEILING(ROUND(`Balance`, 2))), 0) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveMathRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => Math.Truncate(Math.Floor(Math.Ceiling(Math.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(TRUNCATE(FLOOR(CEILING(ROUND(`Balance`, 2))), 0) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveStringMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => x.CompanyName.StartsWith("drore") && x.CompanyName.EndsWith("Inc") && x.City.Contains("Hangzhou");
            var sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(((`CompanyName` LIKE CONCAT(@p0, '%')) and (`CompanyName` LIKE CONCAT('%', @p1))) and (`City` LIKE CONCAT('%', @p2, '%')))",
                sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);

            customerPredicate = x => string.IsNullOrEmpty(x.ContactName) && string.Concat(x.City, x.CompanyName) == "Hangzhou drore";
            sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("((`ContactName` is null or `ContactName` = '') and (CONCAT(`City`, `CompanyName`) = @p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);

            customerPredicate = x => x.CompanyName.Trim().Replace("drore", "dvance").Substring(3).ToLower().ToUpper() == "DVANCE";
            sqlSentence = TestStatic.ExprMySqlResolver.Resolve(customerPredicate);

            Assert.Equal("(UPPER(LOWER(SUBSTRING(REPLACE(TRIM(`CompanyName`), @p0, @p1), 3 + 1))) = @p2)", sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);
        }
    }
}
