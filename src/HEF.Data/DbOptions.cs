using System;

namespace HEF.Data
{
    public class DbOptions
    {
        public string ConnectionString { get; private set; }

        public DbOptions WithConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(connectionString);

            ConnectionString = connectionString;

            return this;
        }
    }
}
