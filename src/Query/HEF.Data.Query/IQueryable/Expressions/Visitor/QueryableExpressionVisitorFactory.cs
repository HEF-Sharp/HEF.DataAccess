namespace HEF.Data.Query
{
    public class QueryableExpressionVisitorFactory : IQueryableExpressionVisitorFactory
    {
        public QueryableExpressionVisitor Create()
        {
            return new QueryableExpressionVisitor();
        }
    }
}
