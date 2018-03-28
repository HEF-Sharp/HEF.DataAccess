namespace HEF.Data.Storage
{
    public interface INamedConnectionStringResolver
    {
        string ResolveConnectionString(string connectionString);
    }
}
