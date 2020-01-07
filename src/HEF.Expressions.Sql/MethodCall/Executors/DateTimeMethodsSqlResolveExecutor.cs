using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class DateTimeAddMethodsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        protected static string[] AddMethodNames = { "AddYears", "AddMonths", "AddDays", "AddHours", "AddMinutes", "AddSeconds", "AddMilliseconds" };

        public string ResolveMethod => string.Join('|', AddMethodNames.Select(m => $"DateTime.{m}"));

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(DateTime)
                && AddMethodNames.Contains(expression.Method.Name);
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("DATEADD(");
                writeAction(GetAddDatePart(expression.Method.Name));
                writeAction(", ");
                visitFunc(expression.Arguments[0]);
                writeAction(", ");
                visitFunc(expression.Object);
                writeAction(")");
            }
        }

        protected virtual string GetAddDatePart(string addMethodName)
        {
            return addMethodName switch
            {
                "AddYears" => "yyyy",
                "AddMonths" => "mm",
                "AddDays" => "dd",
                "AddHours" => "hh",
                "AddMinutes" => "mi",
                "AddSeconds" => "ss",
                "AddMilliseconds" => "ms",
                _ => null
            };
        }
    }
}
