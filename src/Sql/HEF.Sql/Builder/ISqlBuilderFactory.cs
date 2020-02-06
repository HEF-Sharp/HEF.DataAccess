namespace HEF.Sql
{
    public interface ISqlBuilderFactory
    {
        ISelectSqlBuilder Select();

        IInsertSqlBuilder Insert();

        IUpdateSqlBuilder Update();

        IDeleteSqlBuilder Delete();
    }
}
