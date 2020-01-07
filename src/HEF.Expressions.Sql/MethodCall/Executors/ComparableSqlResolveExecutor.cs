using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class ComparableSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "IComparable.CompareTo";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.Name == "CompareTo"
                && !expression.Method.IsStatic
                && expression.Method.ReturnType == typeof(int)
                && expression.Arguments.Count == 1;
        }

        public void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(CASE WHEN ");
                visitFunc(expression.Object);
                writeAction(" = ");
                visitFunc(expression.Arguments[0]);
                writeAction(" THEN 0 WHEN ");
                visitFunc(expression.Object);
                writeAction(" < ");
                visitFunc(expression.Arguments[0]);
                writeAction(" THEN -1 ELSE 1 END)");
            }
        }
    }
}
