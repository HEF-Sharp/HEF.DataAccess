using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class StringStartsWithOracleResolveExecutor : StringStartsWithSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE (");
                visitFunc(expression.Arguments[0]);
                writeAction(" || '%'))");
            }
        }
    }

    public class StringEndsWithOracleResolveExecutor : StringEndsWithSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE ('%' || ");
                visitFunc(expression.Arguments[0]);
                writeAction("))");
            }
        }
    }

    public class StringContainsOracleResolveExecutor : StringContainsSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE ('%' || ");
                visitFunc(expression.Arguments[0]);
                writeAction(" || '%'))");
            }
        }
    }

    public class StringConcatOracleResolveExecutor : StringConcatSqlResolveExecutor
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
                for (int i = 0, n = args.Count; i < n; i++)
                {
                    if (i > 0) writeAction(" || ");
                    visitFunc(args[i]);
                }
            }
        }
    }

    public class StringSubstringOracleResolveExecutor : StringSubstringSqlResolveExecutor
    {
        public override void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("SUBSTR(");
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
