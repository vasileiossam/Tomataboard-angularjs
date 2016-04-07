using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Thalia.Services.Locations.Freegeoip;
using Thalia.Services.Locations.GeoLite;
using Thalia.Services.Locations.IpGeolocation;

namespace Thalia.Services.Locations
{
    public class LocationProvider : Provider<Location>, ILocationProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocationProvider(ILogger<LocationProvider> logger, 
            ICacheRepository<Location> cacheRepository,
            IHttpContextAccessor httpContextAccessor,
            IIpGeolocationService ipGeolocationService,
            IGeoLiteService geoLiteService,
            IFreegeoipService freegeoipService)
            : base(logger, cacheRepository)
        {
            _httpContextAccessor = httpContextAccessor;

            _operations.Add(geoLiteService);
            _operations.Add(ipGeolocationService);
            _operations.Add(freegeoipService);
        }

        /// <summary>
        ///  It passes the IPv4 to the services
        /// </summary>
        /// <returns>Returns the geo location</returns>
        public async Task<Location> Execute()
        {
            var ip = _httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.MapToIPv4().ToString();
#if DEBUG
            if ((ip == null) || (ip == "0.0.0.1") || (ip == "127.0.0.1"))
            {
                ip = "175.34.25.23";
            }
#else
            if (ip == null) 
            {
                _logger.LogError($"{GetType().Name}: Cannot get RemoteIpAddress");
            }
#endif
            return await Execute(ip, true);
        }
    }
}
