using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tomataboard.Services.Cache;

namespace Tomataboard.Services
{
    public class Provider<T> : IProvider<T> where T : class
    {
        protected List<IServiceOperation<T>> _operations = new List<IServiceOperation<T>>();
        protected readonly ICacheRepository<T> _cacheRepository;
        protected readonly ILogger _logger;

        public Provider(ILogger logger, ICacheRepository<T> cacheRepository)
        {
            _logger = logger;
            _cacheRepository = cacheRepository;
        }

        private T GetFromCache(string parameters, bool expired = false)
        {
            var item = _cacheRepository.Find(GetType().Name, parameters, expired);
            if (item == null) return null;

            foreach (var operation in _operations)
            {
                if (operation.GetType().Name == item.Operation)
                {
                    return JsonConvert.DeserializeObject<T>(item.Result);
                }
            }

            return default(T);
        }

        public async Task<T> Execute(string parameters, bool canCache)
        {
            if (canCache)
            {
                // get the most recent non expired item from cache
                var cached = GetFromCache(parameters);
                if (cached != null)
                {
                    return cached;
                }
            }

            // iterate and execute until we get a result from any operation
            foreach (var operation in _operations)
            {
                if (!CheckQuota(operation))
                {
                    _logger.LogCritical($"Over quota in {GetType().Name}.{operation.GetType()}");

                    // fall back to next operation
                    continue;
                }

                var resultObj = await operation.Execute(parameters);
                if (resultObj != null)
                {
                    var json = JsonConvert.SerializeObject(resultObj);
                    _cacheRepository.Add(GetType().Name, operation, parameters, json, false);
                    return resultObj;
                }

                // add in cache even if its errored because it has to count in the service's quota
                _cacheRepository.Add(GetType().Name, operation, parameters, string.Empty, true);
            }

            _logger.LogCritical($"{GetType().Name}: All operations failed to returned data. Attempting to use expired cache...");

            // fall back to any cached item even if its expired
            var cachedExpired = GetFromCache(parameters, true);

            if (cachedExpired == null)
            {
                _logger.LogCritical($"{GetType().Name}: No data returned");
            }

            return cachedExpired;
        }

        public bool CheckQuota(IServiceOperation<T> operation)
        {
            if (operation.Quota == null) return true;

            var count = _cacheRepository.CountItems(operation.GetType().Name, DateTime.Now.Subtract(operation.Quota.Time));
            return count <= operation.Quota.Requests;
        }
    }
}