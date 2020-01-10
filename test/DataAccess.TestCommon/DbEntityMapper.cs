using HEF.Entity.Mapper;

namespace DataAccess.TestCommon
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
