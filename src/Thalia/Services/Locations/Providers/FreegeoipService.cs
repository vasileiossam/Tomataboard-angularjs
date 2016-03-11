using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Thalia.Extensions;
using Thalia.Services.Locations.Abstract;

namespace Thalia.Services.Locations.Providers
{
    /// <summary>
    /// http://freegeoip.net
    /// </summary>
    public class FreegeoipService : IFreegeoipService
    {
        #region private members
        [DataContract]
        private class LocationDto
        {
            [DataMember(Name = "ip")]
            public string Ip { get; set; }
            [DataMember(Name = "country_code")]
            public string CountryCode { get; set; }
            [DataMember(Name = "country_name")]
            public string CountryName { get; set; }
            [DataMember(Name = "region_code")]
            public string RegionCode { get; set; }
            [DataMember(Name = "region_name")]
            public string RegionName { get; set; }
            [DataMember(Name = "city")]
            public string City { get; set; }
            [DataMember(Name = "zip_code")]
            public string PostCode { get; set; }
            [DataMember(Name = "time_zone")]
            public string TimeZone { get; set; }
            [DataMember(Name = "latitude")]
            public string Latitude { get; set; }
            [DataMember(Name = "longitude")]
            public string Longitude { get; set; }
            [DataMember(Name = "metro_code")]
            public string MetroCode { get; set; }
        }

        private readonly ILogger<FreegeoipService> _logger;
        #endregion

        public string Parameters { get; set; }
        public string Result { get; set; }
        public int? RequestsPerMinute { get; }
        public TimeSpan? Expiration { get; }

        public FreegeoipService(ILogger<FreegeoipService> logger)
        {
            _logger = logger;
            RequestsPerMinute = 150;
            Expiration = null;
        }

        public async Task<Location> Execute(string parameters)
        {
            Parameters = parameters;

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

        public Location GetResult(string json)
        {
            // todo Code smell, it shouldn't even be here. Result is used in cache because it is in the interface but what will make sure that it has a value?
            Result = json;

            var locationDto = JsonConvert.DeserializeObject<LocationDto>(json);
            if (locationDto == null) return null;
            if (string.IsNullOrEmpty(locationDto.City) || string.IsNullOrEmpty(locationDto.CountryName)) return null;

            return new Location()
            {
                Country = locationDto.CountryName,
                Region = locationDto.RegionName,
                StateCode = locationDto.RegionCode,
                City = locationDto.City
            };
        }
    }
}
