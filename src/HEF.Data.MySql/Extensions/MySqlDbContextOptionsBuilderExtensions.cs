using HEF.Data.MySql.Storage;
using System;
using System.Data.Common;

namespace HEF.Data
{
    public static class MySqlDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMySql(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString)
        {
            if (optionsBuilder == null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            var module = (MySqlOptionsModule)GetOrCreateModule(optionsBuilder).WithConnectionString(connectionString);
            optionsBuilder.AddOrUpdateModule(module);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseMySql(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnection connection)
        {
            if (optionsBuilder == null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var module = (MySqlOptionsModule)GetOrCreateModule(optionsBuilder).WithConnection(connection);
            optionsBuilder.AddOrUpdateModule(module);

            return optionsBuilder;
        }

        private static MySqlOptionsModule GetOrCreateModule(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindModule<MySqlOptionsModule>()
               ?? new MySqlOptionsModule();
    }
}
