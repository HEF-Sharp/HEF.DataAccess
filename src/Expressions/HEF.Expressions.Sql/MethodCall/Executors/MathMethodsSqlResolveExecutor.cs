using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class MathRoundMethodsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        protected static string[] RoundMethodNames = { "Ceiling", "Floor", "Round", "Truncate" };

        public string ResolveMethod => string.Join('|', RoundMethodNames.Select(m => $"Math.{m}"));

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(Math)
                && expression.Method.IsStatic
                && RoundMethodNames.Contains(expression.Method.Name);
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                switch(expression.Method.Name)
                {
                    case "Ceiling":
                    case "Floor":
                        writeAction(expression.Method.Name.ToUpper());
                        writeAction("(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(")");
                        break;
                    case "Round":
                        writeAction("ROUND(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(", ");
                        if (expression.Arguments.Count == 1)
                        {
                            writeAction(0);
                        }
                        else if (expression.Arguments.Count == 2 && expression.Arguments[1].Type == typeof(int))
                        {
                            visitFunc(expression.Arguments[1]);
                        }
                        writeAction(")");
                        break;
                    case "Truncate":
                        writeAction("ROUND(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(", 0, 1)");
                        break;
                }
            }
        }
    }
}
