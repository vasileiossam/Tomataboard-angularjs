using System;
using System.Linq;
using Tomataboard.Data;

namespace Tomataboard.Services.Cache
{
    public class CacheRepository<T> : ICacheRepository<T>
    {
        private readonly TomataboardContext _context;

        public CacheRepository(TomataboardContext context)
        {
            _context = context;
        }

        public void Add(string service, IServiceOperation<T> operation, string parameters, string result, bool hasErrored)
        {

            DateTime? expires = null;
            if (operation.Expiration != null)
            {
                expires = DateTime.Now.Add((TimeSpan)operation.Expiration);
            }

            var item = new Data.Entities.Cache()
            {
                Service = service,
                Operation = operation.GetType().Name,
                Params = parameters,
                Result = result,
                Created = DateTime.Now,
                Expires = expires,
                HasErrored = hasErrored
            };

            _context.Add(item);
            _context.SaveChanges();
        }

        public Data.Entities.Cache Find(string service, string parameters, bool expired)
        {
            Data.Entities.Cache item;

            // todo error handling, loging
            
            // the most recent non errored item in cache will do regardless if its actually expired or not
            if (expired)
            {
                item = _context.Cache.OrderByDescending(x => x.Created).FirstOrDefault(x =>
                          (x.Service == service) &&
                          (x.HasErrored == false) &&
                          (x.Params == parameters)
                    );
                return item;
            }
            
            // return the most recent non errored, non expired item
            item = _context.Cache.OrderByDescending(x=>x.Created).FirstOrDefault(x =>
                    ((x.Expires == null) || (x.Expires > DateTime.Now)) &&
                    (x.Service == service) && 
                    (x.HasErrored == false) &&
                    (x.Params == parameters)
                );
            return item;
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

        public int CountItems(string operation, DateTime created)
        {
            return _context.Cache.Count(x => 
                x.Operation == operation && 
                x.Created >= created && x.Created <= DateTime.Now);
        }

    }
}
