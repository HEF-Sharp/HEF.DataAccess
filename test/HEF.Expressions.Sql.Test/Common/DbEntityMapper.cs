using HEF.Entity.Mapper;

namespace HEF.Expressions.Sql.Test
{
    public class DbEntityMapper<TEntity> : AutoEntityMapper<TEntity>
        where TEntity : class
    {
        public DbEntityMapper()
        {
            DeleteFlag("IsDel");
            AutoMap();
        }
    }
}
