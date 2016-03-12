using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Thalia.Services.Locations.Abstract;

namespace Thalia.Services.Locations
{
    public class LocationProvider : Provider<Location>, ILocationProvider
    {
        public LocationProvider(ILogger<LocationProvider> logger, ICacheRepository<Location> cacheRepository,
            IIpGeolocationService ipGeolocationService,
            IGeoLiteService geoLiteService,
            IFreegeoipService freegeoipService)
            : base(logger, cacheRepository)
        {
            
            _operations.Add(ipGeolocationService);
            _operations.Add(geoLiteService);
            _operations.Add(freegeoipService);
        }
    }
}
