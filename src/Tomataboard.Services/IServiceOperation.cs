using System;
using System.Threading.Tasks;

namespace Tomataboard.Services
{
    public interface IServiceOperation<T>
    {
        Task<T> Execute(string parameters);
        TimeSpan? Expiration { get; }
        Quota Quota { get; }
    }
}
