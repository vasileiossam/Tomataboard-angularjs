using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Tomataboard.Services.Cache;
using Tomataboard.Services.Extensions;
using Tomataboard.Services.Locations.Freegeoip;
using Tomataboard.Services.Locations.GeoLite;
using Tomataboard.Services.Locations.IpGeolocation;

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

            // commented out because it is too slow in Azure
            //_operations.Add(geoLiteService);

            _operations.Add(ipGeolocationService);
            _operations.Add(freegeoipService);
        }

        private string GetIPFromHeaders(HttpRequest request)
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
                value = ConvertIPToString(ip);
            }

            return value;
        }

        private string ConvertIPToString(IPAddress ip)
        {
            if (ip.IsIPv4MappedToIPv6)
            {
                return ip.MapToIPv4().ToString();
            }
            return ip.ToString();
        }

        private bool IsLocal(IPAddress ip)
        {
            if (ip == null) return false;
            return (ip.ToString() == "127.0.0.1") || (ip.ToString() == "::1");
        }

        private string GetIP()
        {
            try
            {
                var ip = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress;
                if (ip == null)
                {
                    // fall back to this (although it was fixed in RC2)
                    // https://github.com/aspnet/IISIntegration/issues/17
                    _logger.LogError($"{GetType().Name}: Context.Connection.RemoteIpAddress is not working");
                    return GetIPFromHeaders(_httpContextAccessor.HttpContext.Request);
                }

                if (IsLocal(ip))
                {
                    _logger.LogError("Cannot determine the IP of the request");
                    return string.Empty;
                }

                return ConvertIPToString(ip);
            }
            catch(Exception ex)
            {
                _logger.LogError("Cannot determine the IP of the request", ex);
                return string.Empty;
            }
        }

        /// <summary>
        ///  It passes the IP to the services
        /// </summary>
        /// <returns>Returns the geo location</returns>
        public async Task<Location> Execute()
        {
            return await Execute(GetIP(), true);
        }
    }
}