using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IProvider<T>
    {
        Task<T> Execute(string parameters, bool canCache);
    }
}
