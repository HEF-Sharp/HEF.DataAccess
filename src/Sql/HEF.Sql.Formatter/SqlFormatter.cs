namespace HEF.Sql.Formatter
{
    public class SqlFormatter : ISqlFormatter
    {
        public virtual string Name(string name)
        {
            return name;
        }

        public virtual string Alias(string name, string alias)
        {
            return $"{name} as {alias}";
        }

        public virtual string Parameter(string name)
        {
            return $"@{name}";
        }
    }

    public class MySqlFormatter : SqlFormatter
    {
        public override string Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            if (name.Contains("`"))
                return name;

            return "`" + name + "`";
        }
    }

    public class SqlServerFormatter : SqlFormatter
    {
        public override string Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            if (name.Contains("["))
                return name;

            return "[" + name + "]";
        }
    }

    public class OracleFormatter : SqlFormatter
    {
        public override string Alias(string name, string alias)
        {
            return $"{name} {alias}";
        }

        public override string Parameter(string name)
        {
            return $":{name}";
        }
    }

    public class PostgreSqlFormatter : SqlFormatter
    {
        public override string Parameter(string name)
        {
            return $":{name}";
        }
    }
}
