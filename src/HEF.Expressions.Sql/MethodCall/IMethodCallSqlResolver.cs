using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public interface IMethodCallSqlResolver
    {
        bool VisitMethodCall(MethodCallExpression expression,
            Action<object> writeAction, Func<Expression, Expression> visitFunc);
    }
}
