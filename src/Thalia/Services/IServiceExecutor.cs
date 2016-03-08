using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IServiceExecutor<T>
    {
        Task<T> Execute(string parameters);
    }
}
