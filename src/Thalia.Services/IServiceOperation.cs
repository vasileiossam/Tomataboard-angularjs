using System;
using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IServiceOperation<T>
    {
        Task<T> Execute(string parameters);
        TimeSpan? Expiration { get; }
        Quota Quota { get; }
    }
}
