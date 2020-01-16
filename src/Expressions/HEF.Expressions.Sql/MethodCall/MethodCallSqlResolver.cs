using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class MethodCallSqlResolver : IMethodCallSqlResolver
    {
        public MethodCallSqlResolver()
        {
            MethodSqlResolveExecutors = new Dictionary<string, IMethodCallSqlResolveExecutor>();

            InitMethodResolveExecutors();
        }

        protected IDictionary<string, IMethodCallSqlResolveExecutor> MethodSqlResolveExecutors { get; }

        private void InitMethodResolveExecutors()
        {
            AddOrUpdateResolveExecutor(new ObjectEqualsSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new EquatableSqlResolveExecutor());

            AddOrUpdateResolveExecutor(new CompareSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new ComparableSqlResolveExecutor());

            AddOrUpdateResolveExecutor(new DateTimeAddMethodsSqlResolveExecutor());
            
            AddOrUpdateResolveExecutor(new DecimalMathOperationsSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new DecimalRoundMethodsSqlResolveExecutor());

            AddOrUpdateResolveExecutor(new MathRoundMethodsSqlResolveExecutor());

            //String Methods
            AddOrUpdateResolveExecutor(new StringStartsWithSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringEndsWithSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringContainsSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringConcatSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringIsNullOrEmptySqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringToUpperSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringToLowerSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringReplaceSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringSubstringSqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringTrimSqlResolveExecutor());
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
