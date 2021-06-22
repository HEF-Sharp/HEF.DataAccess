using System.Threading.Tasks;

namespace HEF.Repository.Dapper
{
    public interface IDapperExecute
    {
        int Execute();

        Task<int> ExecuteAsync();
    }
}
