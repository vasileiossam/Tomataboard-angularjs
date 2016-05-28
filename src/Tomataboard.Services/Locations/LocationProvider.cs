using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tomataboard.Services.Locations.Freegeoip;
using Tomataboard.Services.Locations.GeoLite;
using Tomataboard.Services.Locations.IpGeolocation;
using Tomataboard.Services.Cache;
using Microsoft.AspNetCore.Http;

namespace Tomataboard.Services.Locations
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

        private string GetRemoteIP(HttpRequest request)
        {
            string[] headers =
            {
                "X-FORWARDED-FOR",
                "REMOTE_ADDR",
                "HTTP_X_FORWARDED_FOR",
                "HTTP_CLIENT_IP",
                "HTTP_X_FORWARDED",
                "HTTP_X_CLUSTER_CLIENT_IP",
                "HTTP_FORWARDED_FOR",
                "HTTP_FORWARDED",
                "X_FORWARDED_FOR",
                "CLIENT_IP",
                "X_FORWARDED",
                "X_CLUSTER_CLIENT_IP",
                "FORWARDED_FOR",
                "FORWARDED"
            };

            string value = string.Empty;
            foreach (var item in headers)
            {
                value = request.Headers[item];
                if (string.IsNullOrEmpty(value)) value = request.Headers[item.Replace('_', '-')];
                if (!string.IsNullOrEmpty(value))
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(value)) return value;

            value = value.Split(',')[0].Split(';')[0];
            if (value.Contains("="))
            {
                value = value.Split('=')[1];
            }
            value = value.Trim('"');

            if (value.Contains("."))
            {
                // remove port from IPv4
                if (value.Contains(":"))
                {
                    value = value.Substring(0, value.LastIndexOf(':'));
                }
            }
            else
            {
                // remove port from IPv6
                if (value.Contains("]"))
                {
                    value = value.Substring(0, value.LastIndexOf(']'));
                    value = value.TrimStart('[');
                }
            }

            IPAddress ip;
            if (IPAddress.TryParse(value, out ip))
            {
                value = ip.MapToIPv4().ToString();
            }

            return value;
        }

        /// <summary>
        ///  It passes the IPv4 to the services
        /// </summary>
        /// <returns>Returns the geo location</returns>
        public async Task<Location> Execute()
        {
            // TODO investigate: do I need to cater for Ipv6?
            var ip = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
            if (string.IsNullOrEmpty(ip))
            {
                // TODO remove this in RC2 where the RemoteIpAddress bug is fixed
                // https://github.com/aspnet/IISIntegration/issues/17
                ip = GetRemoteIP(_httpContextAccessor.HttpContext.Request);
            }
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
