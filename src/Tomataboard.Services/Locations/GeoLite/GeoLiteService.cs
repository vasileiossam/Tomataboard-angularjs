using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tomataboard.Data;

namespace Tomataboard.Services.Locations.GeoLite
{
    public class GeoLiteService : IGeoLiteService
    {
        #region private members

        private ILogger<GeoLiteService> _logger;
        private TomataboardContext _context;

        #endregion private members

        // no quota since we always read from our database
        public Quota Quota => null;

        public TimeSpan? Expiration { get; }

        public GeoLiteService(ILogger<GeoLiteService> logger, TomataboardContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters">IPv4</param>
        /// <returns></returns>
        public Task<Location> Execute(string parameters)
        {
            var ipNum = ConvertIpToLong(parameters);

            var geoBlock = _context.GeoLite2IPv4.FirstOrDefault(x => ipNum >= x.StartIpNum && ipNum <= x.EndIpNum);
            if (geoBlock != null)
            {
                var geoLocation = _context.GeoLite2Locations.FirstOrDefault(x => x.GeonameId == geoBlock.GeonameId);
                if (geoLocation != null)
                {
                    var location = new Location()
                    {
                        Country = geoLocation.CountryName,
                        CountryCode = geoLocation.CountryIsoCode,
                        Region = geoLocation.Subdivision1Name,
                        StateCode = geoLocation.Subdivision1IsoCode,
                        City = geoLocation.CityName,
                        Latitude = geoBlock.Latitude == null ? string.Empty : geoBlock.Latitude.ToString(),
                        Longitude = geoBlock.Longitude == null ? string.Empty : geoBlock.Longitude.ToString(),
                    };

                    return Task.FromResult(location);
                }
            }

            return Task.FromResult(default(Location));
        }

        public static long ConvertIpToLong(string ipAddress)
        {
            System.Net.IPAddress ip;

            if (System.Net.IPAddress.TryParse(ipAddress, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();

                return (long)
                    (
                    16777216 * (long)bytes[0] +
                    65536 * (long)bytes[1] +
                    256 * (long)bytes[2] +
                    (long)bytes[3]
                    )
                    ;
            }
            else
                return 0;
        }
    }
}