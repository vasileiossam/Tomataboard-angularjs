using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IServiceProvider<T>
    {
        Task<T> Execute();
        string parameters { get; set; }
        bool CheckQuota();
    }
}
