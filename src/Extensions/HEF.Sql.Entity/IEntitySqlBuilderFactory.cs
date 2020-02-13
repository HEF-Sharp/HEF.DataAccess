namespace HEF.Sql.Entity
{
    public interface IEntitySqlBuilderFactory
    {
        SelectSqlBuilder<TEntity> Select<TEntity>() where TEntity : class;

        InsertSqlBuilder<TEntity> Insert<TEntity>() where TEntity : class;

        UpdateSqlBuilder<TEntity> Update<TEntity>() where TEntity : class;

        DeleteSqlBuilder<TEntity> Delete<TEntity>() where TEntity : class;
    }
}
