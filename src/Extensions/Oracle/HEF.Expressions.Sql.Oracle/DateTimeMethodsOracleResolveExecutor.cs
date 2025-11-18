using System;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class DateTimeAddMethodsOracleResolveExecutor : DateTimeAddMethodsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                if (IsAddYearOrMonth(expression))
                {
                    writeAction("ADD_MONTHS(");
                    visitFunc(expression.Object);
                    writeAction(", ");
                    visitFunc(expression.Arguments[0]);
                    if (IsAddYear(expression))
                    {
                        writeAction(" * 12");
                    }
                    writeAction(")");
                }
                else
                {
                    writeAction("(");
                    visitFunc(expression.Object);
                    writeAction(" + ");
                    visitFunc(expression.Arguments[0]);
                    writeAction(GetDateDivideFormula(expression));
                    writeAction(")");
                }
            }
        }

        protected bool IsAddYearOrMonth(MethodCallExpression expression)
        {
            var addMethodName = expression.Method.Name;
            return addMethodName == "AddYears" || addMethodName == "AddMonths";
        }

        protected bool IsAddYear(MethodCallExpression expression)
        {           
            return expression.Method.Name == "AddYears";
        }

        protected string GetDateDivideFormula(MethodCallExpression expression)
        {
            var addMethodName = expression.Method.Name;

            return addMethodName switch
            {
                "AddDays" => "",
                "AddHours" => " / 24",
                "AddMinutes" => " / (24 * 60)",
                "AddSeconds" => " / (24 * 60 * 60)",
                "AddMilliseconds" => " / (24 * 60 * 60 * 1000)",
                _ => null
            };
        }
    }
}
