using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tomataboard.Services.Extensions;

namespace Tomataboard.Services.Locations.Freegeoip
{
    /// <summary>
    /// http://freegeoip.net
    /// </summary>
    public class FreegeoipService : IFreegeoipService
    {
        #region private members
        private readonly ILogger<FreegeoipService> _logger;
        #endregion

        public Quota Quota => new Quota() { Requests = 150, Time = TimeSpan.FromMinutes(1) };
        public TimeSpan? Expiration { get; }

        public FreegeoipService(ILogger<FreegeoipService> logger)
        {
            _logger = logger;
            Expiration = null;
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
                var url = $"http://www.freegeoip.net/json/{parameters}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
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
            if (string.IsNullOrEmpty(locationDto?.City) || string.IsNullOrEmpty(locationDto.CountryName)) return null;

            return new Location()
            {
                Country = locationDto.CountryName,
                CountryCode = locationDto.CountryCode,
                Region = locationDto.RegionName,
                StateCode = locationDto.RegionCode,
                City = locationDto.City,
                Latitude = locationDto.Latitude,
                Longitude = locationDto.Longitude
            };
        }
    }
}
