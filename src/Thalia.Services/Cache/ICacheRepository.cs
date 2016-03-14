using System;
using Entities=Thalia.Data.Entities;

namespace Thalia.Services.Cache
{
    public interface ICacheRepository<T>
    {
        Entities.Cache Find(string service, string parameters);
        void Add(string service, IServiceOperation<T> operation, string parameters, string result, bool hasErrored);
        void PurgedAll();
        void PurgedExpired();
        int CountItems(string service, DateTime created);
    }
}
