using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public interface IMethodCallSqlResolveExecutor
    {
        string ResolveMethod { get; }

        bool IsResolveMethod(MethodCallExpression expression);

        void Execute(MethodCallExpression expression,
            Action<object> writeAction, Func<Expression, Expression> visitFunc);
    }
}
