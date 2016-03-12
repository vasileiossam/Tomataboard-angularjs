using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Thalia.Services.Locations.Abstract;
using Thalia.Services.Extensions;

namespace Thalia.Services.Locations.Providers
{
    /// <summary>
    /// http://ip-api.com
    /// </summary>
    public class IpGeolocationService : IIpGeolocationService
    {
        #region private members
        [DataContract]
        private class LocationDto
        {
            [DataMember(Name = "as")]
            public string As { get; set; }
            [DataMember(Name = "city")]
            public string City { get; set; }
            [DataMember(Name = "country")]
            public string Country { get; set; }
            [DataMember(Name = "countryCode")]
            public string CountryCode { get; set; }
            [DataMember(Name = "isp")]
            public string Isp { get; set; }
            [DataMember(Name = "lat")]
            public string Latitude { get; set; }
            [DataMember(Name = "lon")]
            public string Longitude { get; set; }
            [DataMember(Name = "org")]
            public string Org { get; set; }
            [DataMember(Name = "region")]
            public string Region { get; set; }
            [DataMember(Name = "regionName")]
            public string RegionName { get; set; }
            [DataMember(Name = "timezone")]
            public string TimeZone { get; set; }
            [DataMember(Name = "zip")]
            public string PostCode { get; set; }
            [DataMember(Name = "status")]
            public string Status { get; set; }
            [DataMember(Name = "query")]
            public string Ip { get; set; }
            [DataMember(Name = "message")]
            public string Message { get; set; }
        }

        private readonly ILogger<IpGeolocationService> _logger;
        #endregion

        public string Parameters { get; set; }
        public string Result { get; set; }
        public int? RequestsPerMinute { get; }
        public TimeSpan? Expiration { get; }

        public IpGeolocationService(ILogger<IpGeolocationService> logger)
        {
            _logger = logger;
            RequestsPerMinute = 140;
        }

        public async Task<Location> Execute(string parameters)
        {
            Parameters = parameters;

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

        public Location GetResult(string json)
        {
            // todo Code smell, it shouldn't even be here. Result is used in cache because it is in the interface but what will make sure that it has a value?
            Result = json;

            var locationDto = JsonConvert.DeserializeObject<LocationDto>(json);
            if (string.IsNullOrEmpty(locationDto?.City) || string.IsNullOrEmpty(locationDto.Country)) return null;

            return new Location()
            {
                Country = locationDto.Country,
                Region = locationDto.RegionName,
                StateCode = locationDto.Region,
                City = locationDto.City
            };
        }
    }
}
