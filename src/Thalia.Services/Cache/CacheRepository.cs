using System;
using System.Linq;
using Thalia.Data;

namespace Thalia.Services.Cache
{
    public class CacheRepository<T> : ICacheRepository<T>
    {
        private readonly ThaliaContext _context;

        public CacheRepository(ThaliaContext context)
        {
            _context = context;
        }

        public void Add(string service, IServiceOperation<T> operation, string parameters, string result, bool hasErrored)
        {

            DateTime? expired = null;
            if (operation.Expiration != null)
            {
                expired = DateTime.Now.Add((TimeSpan)operation.Expiration);
            }

            var item = new Data.Entities.Cache()
            {
                Service = service,
                Operation = operation.GetType().Name,
                Params = parameters,
                Result = result,
                Created = DateTime.Now,
                Expired = expired,
                HasErrored = hasErrored
            };

            _context.Add(item);
            _context.SaveChanges();
        }

        public Data.Entities.Cache Find(string service, string parameters) 
        {
            // todo error handling, loging

            var cacheItem = _context.Cache.OrderByDescending(x=>x.Created).FirstOrDefault(x =>
                    ((x.Expired == null) || (x.Expired > DateTime.Now)) &&
                    (x.Service == service) && 
                    (x.HasErrored == false) &&
                    (x.Params == parameters)
                );

            return cacheItem;
        }

        // todo 
        public void PurgedAll()
        {
            throw new NotImplementedException();
        }

        // todo
        public void PurgedExpired()
        {
            throw new NotImplementedException();
        }

        public int CountItems(string service, DateTime created)
        {
            return _context.Cache.Count(x => 
                ((x.Expired == null) || (x.Expired > DateTime.Now)) && 
                x.Service == service && 
                x.Created >= created && x.Created <= DateTime.Now);
        }

    }
}
