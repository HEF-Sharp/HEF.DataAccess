using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class CollectionContainsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "ICollection.Contains";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType.Is(typeof(ICollection<>))
                && expression.Method.Name == "Contains"
                && !expression.Method.IsStatic
                && !expression.Method.IsGenericMethod
                && expression.Method.ReturnType == typeof(bool)
                && expression.Arguments.Count == 1;
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Arguments[0]);
                writeAction(" IN ");
                visitFunc(expression.Object);
                writeAction(")");
            }
        }
    }
}
