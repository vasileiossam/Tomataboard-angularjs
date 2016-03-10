using Microsoft.Extensions.Logging;
using Thalia.Data;
using Thalia.Services.Cache;

namespace Thalia.Services.Locations
{
    public class LocationExecutor : ServiceExecutor<Locations.Location>, IServiceExecutor<Locations.Location>
    {
        public LocationExecutor(ILogger logger, ICacheRepository<Locations.Location> cacheRepository, ThaliaContext context)
            : base(logger, cacheRepository)
        {
            // todo change this with a container that will register the types instead
            _operations.Add(new IpGeolocationApi(logger));
            _operations.Add(new GeoLiteService(logger, context));
            _operations.Add(new FreegeoipService(logger));
        }
    }
}
