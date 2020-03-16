using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class MethodCallSqlResolver : IMethodCallSqlResolver
    {
        public MethodCallSqlResolver(IEnumerable<IMethodCallSqlResolveExecutor> methodSqlResolveExecutors)
        {
            if (methodSqlResolveExecutors == null)
                throw new ArgumentNullException(nameof(methodSqlResolveExecutors));

            MethodSqlResolveExecutors = new Dictionary<string, IMethodCallSqlResolveExecutor>();
            InitMethodResolveExecutors(methodSqlResolveExecutors);
        }

        protected IDictionary<string, IMethodCallSqlResolveExecutor> MethodSqlResolveExecutors { get; }

        private void InitMethodResolveExecutors(IEnumerable<IMethodCallSqlResolveExecutor> methodSqlResolveExecutors)
        {
            foreach(var methodSqlResolveExecutor in methodSqlResolveExecutors)
            {
                AddOrUpdateResolveExecutor(methodSqlResolveExecutor);
            }
        }

        protected void AddOrUpdateResolveExecutor(IMethodCallSqlResolveExecutor methodSqlResolveExecutor)
        {
            if (MethodSqlResolveExecutors.ContainsKey(methodSqlResolveExecutor.ResolveMethod))
            {
                MethodSqlResolveExecutors[methodSqlResolveExecutor.ResolveMethod] = methodSqlResolveExecutor;
                return;
            }

            MethodSqlResolveExecutors.Add(methodSqlResolveExecutor.ResolveMethod, methodSqlResolveExecutor);
        }

        public bool VisitMethodCall(MethodCallExpression expression, Action<object> writeAction, Func<Expression, Expression> visitFunc)
        {
            foreach(var methodResolveExecutorItem in MethodSqlResolveExecutors)
            {
                var methodResolveExecutor = methodResolveExecutorItem.Value;
                if (methodResolveExecutor.IsResolveMethod(expression))
                {
                    methodResolveExecutor.Execute(expression, writeAction, visitFunc);
                    return true;
                }
            }

            return false;
        }
    }
}
