using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class DecimalMathOperationsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        protected static string[] OperationMethodNames = { "Add", "Subtract", "Multiply", "Divide", "Remainder" };

        public string ResolveMethod => string.Join('|', OperationMethodNames.Select(m => $"Decimal.{m}"));

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(decimal)
                && expression.Method.IsStatic
                && OperationMethodNames.Contains(expression.Method.Name);
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Arguments[0]);
                writeAction(" ");
                writeAction(GetMathOperator(expression.Method.Name));
                writeAction(" ");
                visitFunc(expression.Arguments[1]);
                writeAction(")");
            }
        }

        protected virtual string GetMathOperator(string methodName)
        {
            return methodName switch
            {
                "Add" => "+",
                "Subtract" => "-",
                "Multiply" => "*",
                "Divide" => "/",
                "Remainder" => "%",
                _ => null
            };
        }
    }

    public class DecimalRoundMethodsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        protected static string[] RoundMethodNames = { "Ceiling", "Floor", "Round", "Truncate" };

        public string ResolveMethod => string.Join('|', RoundMethodNames.Select(m => $"Decimal.{m}"));

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(decimal)
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
