using DataAccess.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace HEF.Expressions.Sql.Test
{
    public class OracleMethodCallResolveExecutorTest
    {
        [Fact]
        public void TestResolveEqualsMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => Equals(x.CompanyName, "drore") && x.City.Equals("Hangzhou");

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = :p0) AND (city = :p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveCompareMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate = x => string.Compare(x.CompanyName, "drore") == 0 && x.City.CompareTo("Hangzhou") == 0;
            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((companyName = :p0) AND (city = :p1))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);

            customerPredicate = x => string.Compare(x.CompanyName, "drore") + x.City.CompareTo("Hangzhou") == 0;
            sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((" + "(CASE WHEN CompanyName = :p0 THEN 0 WHEN CompanyName < :p0 THEN -1 ELSE 1 END)" + " + "
                + "(CASE WHEN City = :p1 THEN 0 WHEN City < :p1 THEN -1 ELSE 1 END)" + ") = 0)", sqlSentence.SqlText, true);
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

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((((((" + "(ADD_MONTHS(createTime, 1 * 12) > :p0)"
                + " AND " + "(ADD_MONTHS(createTime, 2) < :p0))"
                + " AND " + "((createTime + 3.0) >= :p0))"
                + " AND " + "((createTime + 4.0 / 24) <= :p0))"
                + " AND " + "((createTime + 5.0 / (24 * 60)) = :p0))"
                + " AND " + "((createTime + 10.0 / (24 * 60 * 60)) <> :p0))"
                + " AND " + "((createTime + 100.0 / (24 * 60 * 60 * 1000)) < :p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);
        }

        [Fact]
        public void TestResolveDecimalMathOperations()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Remainder(decimal.Divide(decimal.Multiply(decimal.Subtract(decimal.Add(x.Balance, 1), 5), 4), 2), 3) == 1;

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((((((Balance + 1) - 5) * 4) / 2) % 3) = 1)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveDecimalRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => decimal.Truncate(decimal.Floor(decimal.Ceiling(decimal.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("(TRUNC(FLOOR(CEIL(ROUND(Balance, 2)))) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveMathRoundMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => Math.Truncate(Math.Floor(Math.Ceiling(Math.Round(x.Balance, 2)))) == 5;

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("(TRUNC(FLOOR(CEIL(ROUND(Balance, 2)))) = 5)", sqlSentence.SqlText, true);
        }

        [Fact]
        public void TestResolveStringMethods()
        {
            Expression<Func<Customer, bool>> customerPredicate =
                x => x.CompanyName.StartsWith("drore") && x.CompanyName.EndsWith("Inc") && x.City.Contains("Hangzhou");
            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("(((CompanyName LIKE (:p0 || '%')) AND (CompanyName LIKE ('%' || :p1))) AND (City LIKE ('%' || :p2 || '%')))",
                sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);

            customerPredicate = x => string.IsNullOrEmpty(x.ContactName) && string.Concat(x.City, x.CompanyName) == "Hangzhou drore";
            sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((ContactName IS NULL OR ContactName = '') AND (City || CompanyName = :p0))", sqlSentence.SqlText, true);
            Assert.Single(sqlSentence.Parameters);

            customerPredicate = x => x.CompanyName.Trim().Replace("drore", "dvance").Substring(3).ToLower().ToUpper() == "DVANCE";
            sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("(UPPER(LOWER(SUBSTR(REPLACE(TRIM(CompanyName), :p0, :p1), 3 + 1))) = :p2)", sqlSentence.SqlText, true);
            Assert.Equal(3, sqlSentence.Parameters.Length);
        }

        [Fact]
        public void TestResolveIEnumerableContainMethod()
        {
            var idList = new List<long> { 1, 3, 5, 6 };
            var cityList = new[] { "Shanghai", "Wuhan" };
            Expression<Func<Customer, bool>> customerPredicate = x => idList.Contains(x.id) && !cityList.Contains(x.City);

            var sqlSentence = TestStatic.ExprOracleResolver.Resolve(customerPredicate);

            Assert.Equal("((id IN (1, 3, 5, 6)) AND NOT (City IN (:p0, :p1)))", sqlSentence.SqlText, true);
            Assert.Equal(2, sqlSentence.Parameters.Length);
        }
    }
}
