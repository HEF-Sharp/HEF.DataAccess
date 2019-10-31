namespace HEF.Sql
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
    }
}
