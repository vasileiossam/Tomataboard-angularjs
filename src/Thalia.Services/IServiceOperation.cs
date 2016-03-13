using System;
using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IServiceOperation<T>
    {
        Task<T> Execute(string parameters);
        int? RequestsPerMinute { get; }
        TimeSpan? Expiration { get; }
    }
}
