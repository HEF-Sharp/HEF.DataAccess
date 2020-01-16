using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class DateTimeAddMethodsMySqlResolveExecutor : DateTimeAddMethodsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("DATE_ADD(");
                visitFunc(expression.Object);
                writeAction(", INTERVAL ");
                if (expression.Method.Name == "AddMilliseconds")
                {
                    writeAction("(");
                    visitFunc(expression.Arguments[0]);
                    writeAction("* 1000)");
                }
                else
                {
                    visitFunc(expression.Arguments[0]);
                }
                writeAction(" ");
                writeAction(GetAddDatePart(expression.Method.Name));
                writeAction(")");
            }
        }

        protected override string GetAddDatePart(string addMethodName)
        {
            return addMethodName switch
            {
                "AddYears" => "YEAR",
                "AddMonths" => "MONTH",
                "AddDays" => "DAY",
                "AddHours" => "HOUR",
                "AddMinutes" => "MINUTE",
                "AddSeconds" => "SECOND",
                "AddMilliseconds" => "MICROSECOND",
                _ => null
            };
        }
    }
}
