namespace HEF.Sql.Entity
{
    public interface ISelectEntitySqlBuilderFactory
    {
        SelectSqlBuilder<TEntity> Create<TEntity>() where TEntity : class;
    }
}
