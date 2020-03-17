using DataAccess.TestCommon;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace HEF.Expressions.Sql.Test
{
    public class MethodCallResolveExecutorTest
    {
        [Fact]
        public void TestResolveEqualsMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => Equals(x.CompanyName, "drore") && x.City.Equals("Hangzhou");

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = @p0) AND (city = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveCompareMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => string.Compare(x.CompanyName, "drore") == 0 && x.City.CompareTo("Hangzhou") == 0;
            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = @p0) AND (city = @p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
            
            customerPredicate = x => string.Compare(x.CompanyName, "drore") + x.City.CompareTo("Hangzhou") == 0;
            sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((" + "(CASE WHEN CompanyName = @p0 THEN 0 WHEN CompanyName < @p0 THEN -1 ELSE 1 END)" + " + "
                + "(CASE WHEN City = @p1 THEN 0 WHEN City < @p1 THEN -1 ELSE 1 END)" + ") = 0)", sqlSentence.SqlText, true);
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

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((((((" + "(DATEADD(yyyy, 1, createTime) > @p0)"
                + " AND " + "(DATEADD(mm, 2, createTime) < @p0))"
                + " AND " + "(DATEADD(dd, 3.0, createTime) >= @p0))"
                + " AND " + "(DATEADD(hh, 4.0, createTime) <= @p0))"
                + " AND " + "(DATEADD(mi, 5.0, createTime) = @p0))"
                + " AND " + "(DATEADD(ss, 10.0, createTime) <> @p0))"
                + " AND " + "(DATEADD(ms, 100.0, createTime) < @p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);
        }

        [Fact]
        public void TestResolveDecimalMathOperations()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Remainder(decimal.Divide(decimal.Multiply(decimal.Subtract(decimal.Add(x.Balance, 1), 5), 4), 2), 3) == 1;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((((((Balance + 1) - 5) * 4) / 2) % 3) = 1)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveDecimalRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Truncate(decimal.Floor(decimal.Ceiling(decimal.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(ROUND(FLOOR(CEILING(ROUND(Balance, 2))), 0, 1) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveMathRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => Math.Truncate(Math.Floor(Math.Ceiling(Math.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(ROUND(FLOOR(CEILING(ROUND(Balance, 2))), 0, 1) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveStringMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => x.CompanyName.StartsWith("drore") && x.CompanyName.EndsWith("Inc") && x.City.Contains("Hangzhou");
            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(((CompanyName LIKE @p0 + '%') AND (CompanyName LIKE '%' + @p1)) AND (City LIKE '%' + @p2 + '%'))",
                sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);

            customerPredicate = x => string.IsNullOrEmpty(x.ContactName) && string.Concat(x.City, x.CompanyName) == "Hangzhou drore";
            sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((ContactName IS NULL OR ContactName = '') AND (City + CompanyName = @p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);

            customerPredicate = x => x.CompanyName.Trim().Replace("drore", "dvance").Substring(3, 6).ToLower().ToUpper() == "DVANCE";
            sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("(UPPER(LOWER(SUBSTRING(REPLACE(TRIM(CompanyName), @p0, @p1), 3 + 1, 6))) = @p2)", sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveIEnumerableContainMethod()
        {
            var idList = new long[] { 1, 3, 5, 6 };
            var cityList = new[] { "Shanghai", "Wuhan" };
            Expression<Func<Customer, bool>> customerPredicate = x => idList.Contains(x.id) && !cityList.Contains(x.City);

            var sqlSentence = TestStatic.ExprSqlResolver.Resolve(customerPredicate);

            Assert.Equal("((id IN (1, 3, 5, 6)) AND NOT (City IN (@p0, @p1)))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }
    }
}
