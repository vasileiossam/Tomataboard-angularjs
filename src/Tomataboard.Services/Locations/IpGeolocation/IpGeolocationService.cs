using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tomataboard.Services.Extensions;

namespace Tomataboard.Services.Locations.IpGeolocation
{
    /// <summary>
    /// http://ip-api.com
    /// </summary>
    public class IpGeolocationService : IIpGeolocationService
    {
        #region private members

        private readonly ILogger<IpGeolocationService> _logger;

        #endregion private members

        public Quota Quota => new Quota() { Requests = 140, Time = TimeSpan.FromMinutes(1) };
        public TimeSpan? Expiration { get; }

        public IpGeolocationService(ILogger<IpGeolocationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters">IPv4</param>
        /// <returns></returns>
        public async Task<Location> Execute(string parameters)
        {
            try
            {
                var url = $"http://ip-api.com/json/{parameters}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();
                    var locationDto = JsonConvert.DeserializeObject<LocationDto>(content);

                    if ((response.IsSuccessStatusCode) && (locationDto.Status == "success"))
                    {
                        return GetResult(content);
                    }

                    _logger.LogCritical($"{GetType().Name}: Cannot get location for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{GetType().Name}: Cannot get location for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }

        private Location GetResult(string json)
        {
            var locationDto = JsonConvert.DeserializeObject<LocationDto>(json);
            if (string.IsNullOrEmpty(locationDto?.City) || string.IsNullOrEmpty(locationDto.Country)) return null;

            return new Location()
            {
                Country = locationDto.Country,
                CountryCode = locationDto.CountryCode,
                Region = locationDto.RegionName,
                StateCode = locationDto.Region,
                City = locationDto.City,
                Latitude = locationDto.Latitude,
                Longitude = locationDto.Longitude
            };
        }
    }
}