using System;
using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IServiceOperation<T>
    {
        Task<T> Execute(string parameters);
        string Parameters { get; set; }
        string Result { get; set; }
        T GetResult(string json);
        int? RequestsPerMinute { get; }
        TimeSpan? Expiration { get; }
    }
}
