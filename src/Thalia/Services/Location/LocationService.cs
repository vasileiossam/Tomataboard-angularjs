using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalia.Data;

namespace Thalia.Services.Location
{
    public class LocationService : ILocationService
    {
        #region private members
        private ILogger _logger;
        private ThaliaContext _context;
        #endregion

        public LocationService(ILogger logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Location> GetLocationAsync(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return null;

            var location = GetLocationFromCache(ip);
            if (location != null) return location;

            location = await new GeoLiteService(_logger, _context).GetLocationAsync(ip);
            if (location != null) return location;

            location = await new FreegeoipService(_logger, _context).GetLocationAsync(ip);
            if (location != null) return location;

            location = await new IpGeolocationApi(_logger, _context).GetLocationAsync(ip);
            if (location != null) return location;

            return null;
        }

        private Location GetLocationFromCache(string ip)
        {
            var serviceRequest = _context.ServiceRequests.FirstOrDefault(x => x.Ip == ip);
            if (serviceRequest == null) return null;

            var service = CreateApi(serviceRequest.Api);
            if (service == null) return null;

            return service.GetLocation(serviceRequest.Response);
        }

        private ILocationApi CreateApi(string api)
        {
            if (api == "FreegeoipService")
            {
                return new FreegeoipService(_logger, _context);
            }

            if (api == "IpGeolocationApi")
            {
                return new IpGeolocationApi(_logger, _context);
            }

            if (api == "GeoLiteService")
            {
                return new GeoLiteService(_logger, _context);
            }

            _logger.LogDebug($"LocationService.CreateApi: Cannot create '{api}'");
            return null;
        }
    }
}
