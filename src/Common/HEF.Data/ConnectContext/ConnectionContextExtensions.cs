namespace HEF.Data
{
    public static class ConnectionContextExtensions
    {
        public static IDbAsyncConnectionContext AsAsync(this IDbConnectionContext connectionContext)
        {
            return new DbAsyncConnectionContext(connectionContext);
        }
    }
}
