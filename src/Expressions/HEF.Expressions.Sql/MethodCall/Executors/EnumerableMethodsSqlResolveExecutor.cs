using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class EnumerableContainsMethodsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "IEnumerable.Contains";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(Enumerable)
                && expression.Method.Name == "Contains"
                && expression.Method.IsStatic
                && expression.Method.IsGenericMethod
                && expression.Method.ReturnType == typeof(bool)
                && expression.Arguments.Count == 2;
        }

        public void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Arguments[1]);
                writeAction(" IN ");
                visitFunc(expression.Arguments[0]);
                writeAction(")");
            }
        }
    }
}
