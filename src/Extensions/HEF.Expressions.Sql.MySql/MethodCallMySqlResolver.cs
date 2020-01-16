namespace HEF.Expressions.Sql
{
    public class MethodCallMySqlResolver : MethodCallSqlResolver
    {
        public MethodCallMySqlResolver()
        {
            InitMethodResolveExecutors();
        }

        private void InitMethodResolveExecutors()
        {
            AddOrUpdateResolveExecutor(new DateTimeAddMethodsMySqlResolveExecutor());
            
            AddOrUpdateResolveExecutor(new DecimalRoundMethodsMySqlResolveExecutor());

            AddOrUpdateResolveExecutor(new MathRoundMethodsMySqlResolveExecutor());

            //String Methods
            AddOrUpdateResolveExecutor(new StringStartsWithMySqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringEndsWithMySqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringContainsMySqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringConcatMySqlResolveExecutor());
            AddOrUpdateResolveExecutor(new StringSubstringMySqlResolveExecutor());
        }
    }
}
