using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class StringStartsWithSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.StartsWith";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "StartsWith";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE ");
                visitFunc(expression.Arguments[0]);
                writeAction(" + '%')");
            }
        }
    }

    public class StringEndsWithSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.EndsWith";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "EndsWith";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE '%' + ");
                visitFunc(expression.Arguments[0]);
                writeAction(")");
            }
        }
    }

    public class StringContainsSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.Contains";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "Contains";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Object);
                writeAction(" LIKE '%' + ");
                visitFunc(expression.Arguments[0]);
                writeAction(" + '%')");
            }
        }
    }

    public class StringConcatSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.Concat";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.IsStatic
                && expression.Method.Name == "Concat";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
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
                    if (i > 0) writeAction(" + ");
                    visitFunc(args[i]);
                }
            }
        }
    }

    public class StringIsNullOrEmptySqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.IsNullOrEmpty";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.IsStatic
                && expression.Method.Name == "IsNullOrEmpty";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("(");
                visitFunc(expression.Arguments[0]);
                writeAction(" IS NULL OR ");
                visitFunc(expression.Arguments[0]);
                writeAction(" = '')");
            }
        }
    }

    public class StringToUpperSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.ToUpper";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)                
                && expression.Method.Name == "ToUpper";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("UPPER(");
                visitFunc(expression.Object);
                writeAction(")");
            }
        }
    }

    public class StringToLowerSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.ToLower";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "ToLower";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("LOWER(");
                visitFunc(expression.Object);
                writeAction(")");
            }
        }
    }

    public class StringReplaceSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.Replace";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "Replace";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("REPLACE(");
                visitFunc(expression.Object);
                writeAction(", ");
                visitFunc(expression.Arguments[0]);
                writeAction(", ");
                visitFunc(expression.Arguments[1]);
                writeAction(")");
            }
        }
    }

    public class StringSubstringSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.Substring";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "Substring";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("SUBSTRING(");
                visitFunc(expression.Object);
                writeAction(", ");
                visitFunc(expression.Arguments[0]);
                writeAction(" + 1, ");
                if (expression.Arguments.Count == 2)
                {
                    visitFunc(expression.Arguments[1]);
                }
                else
                {
                    writeAction(8000);
                }
                writeAction(")");
            }
        }
    }

    public class StringTrimSqlResolveExecutor : IMethodCallSqlResolveExecutor
    {
        public string ResolveMethod => "String.Trim";

        public bool IsResolveMethod(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                && expression.Method.Name == "Trim";
        }

        public virtual void Execute(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            if (IsResolveMethod(expression))
            {
                writeAction("TRIM(");
                visitFunc(expression.Object);
                writeAction(")");
            }
        }
    }
}
