using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class CompareSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "Static.Compare";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.Name == "Compare"
                && expression.Method.IsStatic
                && expression.Method.ReturnType == typeof(int)
                && expression.Arguments.Count == 2;
        }

        public void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(CASE WHEN ");
                visitFunc(expression.Arguments[0]);
                writeAction(" = ");
                visitFunc(expression.Arguments[1]);
                writeAction(" THEN 0 WHEN ");
                visitFunc(expression.Arguments[0]);
                writeAction(" < ");
                visitFunc(expression.Arguments[1]);
                writeAction(" THEN -1 ELSE 1 END)");
            }
        }
    }
}
