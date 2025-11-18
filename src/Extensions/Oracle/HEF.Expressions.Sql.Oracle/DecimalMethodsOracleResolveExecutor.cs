using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class DecimalRoundMethodsOracleResolveExecutor : DecimalRoundMethodsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                switch (expression.Method.Name)
                {
                    case "Ceiling":
                        writeAction("CEIL(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(")");
                        break;
                    case "Floor":
                        writeAction("FLOOR(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(")");
                        break;
                    case "Round":
                        writeAction("ROUND(");
                        visitFunc(expression.Arguments[0]);
                        if (expression.Arguments.Count == 2 && expression.Arguments[1].Type == typeof(int))
                        {
                            writeAction(", ");
                            visitFunc(expression.Arguments[1]);
                        }
                        writeAction(")");
                        break;
                    case "Truncate":
                        writeAction("TRUNC(");
                        visitFunc(expression.Arguments[0]);
                        if (expression.Arguments.Count == 2 && expression.Arguments[1].Type == typeof(int))
                        {
                            writeAction(", ");
                            visitFunc(expression.Arguments[1]);
                        }
                        writeAction(")");
                        break;
                }
            }
        }
    }
}
