using System.Threading.Tasks;

namespace GraphLinqQL
{
    public interface IApplicationCommandExecutor
    {
        Task<int> ExecuteAsync(string[] args);
    }
}
