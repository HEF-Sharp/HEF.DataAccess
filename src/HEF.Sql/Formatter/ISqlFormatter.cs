namespace HEF.Sql
{
    public interface ISqlFormatter
    {
        string Name(string name);

        string Alias(string name, string alias);

        string Parameter(string name);
    }
}
