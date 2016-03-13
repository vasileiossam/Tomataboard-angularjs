using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Thalia.Services.Locations.Freegeoip;
using Thalia.Services.Locations.GeoLite;
using Thalia.Services.Locations.IpGeolocation;

namespace Thalia.Services.Locations
{
    public class LocationProvider : Provider<Location>, ILocationProvider
    {
        public LocationProvider(ILogger<LocationProvider> logger, 
            ICacheRepository<Location> cacheRepository,
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
