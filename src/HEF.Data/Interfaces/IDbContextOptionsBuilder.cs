namespace HEF.Data
{
    public interface IDbContextOptionsBuilder
    {
        void AddOrUpdateModule<TModule>(TModule module)
            where TModule : class, IDbContextOptionsModule;
    }
}
