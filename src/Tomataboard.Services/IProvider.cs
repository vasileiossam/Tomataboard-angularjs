using System.Threading.Tasks;

namespace Tomataboard.Services
{
    public interface IProvider<T>
    {
        Task<T> Execute(string parameters, bool readCache);
    }
}