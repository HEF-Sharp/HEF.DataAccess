using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class StringStartsWithMySqlResolveExecutor : StringStartsWithSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE CONCAT(");
                visitFunc(expression.Arguments[0]);
                writeAction(", '%'))");
            }
        }
    }

    public class StringEndsWithMySqlResolveExecutor : StringEndsWithSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE CONCAT('%', ");
                visitFunc(expression.Arguments[0]);
                writeAction("))");
            }
        }
    }

    public class StringContainsMySqlResolveExecutor : StringContainsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE CONCAT('%', ");
                visitFunc(expression.Arguments[0]);
                writeAction(", '%'))");
            }
        }
    }

    public class StringConcatMySqlResolveExecutor : StringConcatSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                IList<Expression> args = expression.Arguments;
                if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                {
                    args = ((NewArrayExpression)args[0]).Expressions;
                }
                writeAction("CONCAT(");
                for (int i = 0, n = args.Count; i < n; i++)
                {
                    if (i > 0) writeAction(", ");
                    visitFunc(args[i]);
                }
                writeAction(")");
            }
        }
    }

    public class StringSubstringMySqlResolveExecutor : StringSubstringSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("SUBSTRING(");
                visitFunc(expression.Object);
                writeAction(", ");
                visitFunc(expression.Arguments[0]);
                writeAction(" + 1");
                if (expression.Arguments.Count == 2)
                {
                    writeAction(", ");
                    visitFunc(expression.Arguments[1]);
                }
                writeAction(")");
            }
        }
    }
}
