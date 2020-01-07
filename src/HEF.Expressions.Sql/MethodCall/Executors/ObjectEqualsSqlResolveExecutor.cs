using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class ObjectEqualsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "Object.Equals";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(object)
                && expression.Method.IsStatic
                && expression.Method.Name == "Equals";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Arguments[0]);
                writeAction(" = ");
                visitFunc(expression.Arguments[1]);
                writeAction(")");
            }
        }
    }
}
