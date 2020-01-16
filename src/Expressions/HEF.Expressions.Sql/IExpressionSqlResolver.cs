using HEF.Sql;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public interface IExpressionSqlResolver
    {
        SqlSentence Resolve(Expression expression);
    }
}
