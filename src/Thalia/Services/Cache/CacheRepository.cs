using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalia.Data;
using Thalia.Services;
using Entities = Thalia.Data.Entities;

namespace Thalia.Services.Cache
{
    public class CacheRepository<T> : ICacheRepository<T>
    {
        private ThaliaContext _context;

        public CacheRepository(ThaliaContext context)
        {
            _context = context;
        }

        public void Add(string service, IServiceOperation<T> operation)
        {

            DateTime? expired = null;
            if (operation.Expiration != null)
            {
                expired = DateTime.Now.Add((TimeSpan)operation.Expiration);
            }

            var item = new Entities.Cache()
            {
                Service = service,
                Operation = operation.GetType().Name,
                Params = operation.Parameters,
                Result = operation.Result,
                Created = DateTime.Now,
                Expired = expired
            };

            _context.Add(item);
            _context.SaveChanges();
        }

        public Entities.Cache Find(string service, string parameters) 
        {
            // todo error handling, loging

            var cacheItem = _context.Cache.OrderByDescending(x=>x.Created).FirstOrDefault(x =>
                    ((x.Expired == null) || (x.Expired > DateTime.Now)) &&
                    (x.Service == service) &&
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
            return _context.Cache.Where(x => 
                ((x.Expired == null) || (x.Expired > DateTime.Now)) && 
                x.Service == service && x.Created >= created && x.Created <= DateTime.Now).Count();
        }

    }
}
