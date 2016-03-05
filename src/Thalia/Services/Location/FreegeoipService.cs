using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Thalia.Services.Location
{
    public class FreegeoipService
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
            public string RegionNname { get; set; }
            [DataMember(Name = "city")]
            public string City { get; set; }
            [DataMember(Name = "zip_code")]
            public string ZipCode { get; set; }
            [DataMember(Name = "time_zone")]
            public string TimeZone { get; set; }
            [DataMember(Name = "latitude")]
            public string Latitude { get; set; }
            [DataMember(Name = "longitude")]
            public string Longitude { get; set; }
            [DataMember(Name = "metro_code")]
            public string MetroCode { get; set; }
        }

        private ILogger _logger;
        #endregion

        public FreegeoipService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<Location> GetLocationAsync(string ip)
        {
            try
            {
                var url = $"http://www.freegeoip.net/json/{ip}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var locationDto = JsonConvert.DeserializeObject<LocationDto>(content);
                        if (locationDto != null)
                        {
                            return new Location()
                            {
                                Country = locationDto.CountryName,
                                Region = locationDto.RegionNname,
                                City = locationDto.City
                            };
                        }
                    }

                    _logger.LogCritical($"FreegeoipService: Cannot get location for '{ip}'. Status code: {response.StatusCode} Content: {content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"FreegeoipService: Cannot get location for '{ip}'. Exception: " + ex.Message);
            }

            return null;
        }
    }
}
