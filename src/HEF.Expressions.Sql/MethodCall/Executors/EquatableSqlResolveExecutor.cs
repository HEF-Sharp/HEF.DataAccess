using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class EquatableSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "IEquatable.Equals";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.Name == "Equals"
                && !expression.Method.IsStatic
                && expression.Arguments.Count == 1
                && expression.Object.Type == expression.Arguments[0].Type;
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" = ");
                visitFunc(expression.Arguments[0]);
                writeAction(")");
            }
        }
    }
}
