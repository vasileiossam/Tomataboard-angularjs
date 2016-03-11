using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Thalia.Data;
using Thalia.Data.Entities;
using Thalia.Services.Locations.Abstract;

namespace Thalia.Services.Locations.Providers
{
    public class GeoLiteService : IGeoLiteService
    {
        #region private members
        private ILogger<GeoLiteService> _logger;
        private ThaliaContext _context;
        #endregion

        public string Parameters { get; set; }
        public string Result { get; set; }
        public int? RequestsPerMinute { get; }
        public TimeSpan? Expiration { get; }
        
        public GeoLiteService(ILogger<GeoLiteService> logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Task<Location> Execute(string parameters)
        {
            Parameters = parameters;

            var ipNum = ConvertIPToLong(parameters);

            var geoBlock = _context.GeoLite2IPv4.FirstOrDefault(x => ipNum >= x.StartIpNum && ipNum <= x.EndIpNum);
            if (geoBlock != null)
            {
                var location = _context.GeoLite2Locations.FirstOrDefault(x => x.GeonameId == geoBlock.GeonameId);
                if (location != null)
                {
                    var json = JsonConvert.SerializeObject(location);
                    return Task.FromResult(GetResult(json));
                }
            }

            return null;
        }

        public static long ConvertIPToLong(string ipAddress)
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

        public Location GetResult(string json)
        {
            // todo Code smell, it shouldn't even be here. Result is used in cache because it is in the interface but what will make sure that it has a value?
            Result = json;

            var geoLocation = JsonConvert.DeserializeObject<GeoLite2Location>(json);
            if (geoLocation == null) return null;

            if (string.IsNullOrEmpty(geoLocation.CityName) || string.IsNullOrEmpty(geoLocation.CountryName)) return null;

            return new Location()
            {
                Country = geoLocation.CountryName,
                Region = geoLocation.Subdivision1Name,
                StateCode = geoLocation.Subdivision1IsoCode,
                City = geoLocation.CityName
            };
        }
    }
}