using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class MathRoundMethodsMySqlResolveExecutor : MathRoundMethodsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                switch (expression.Method.Name)
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
                        if (expression.Arguments.Count == 2 && expression.Arguments[1].Type == typeof(int))
                        {
                            writeAction(", ");
                            visitFunc(expression.Arguments[1]);
                        }
                        writeAction(")");
                        break;
                    case "Truncate":
                        writeAction("TRUNCATE(");
                        visitFunc(expression.Arguments[0]);
                        writeAction(", ");
                        if (expression.Arguments.Count == 2 && expression.Arguments[1].Type == typeof(int))
                        {
                            visitFunc(expression.Arguments[1]);
                        }
                        else
                        {
                            writeAction(0);
                        }
                        writeAction(")");
                        break;
                }
            }
        }
    }
}
