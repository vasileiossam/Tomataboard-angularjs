using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Thalia.Data;
using Thalia.Extensions;
using System.Linq;
using Thalia.Data.Entities;

namespace Thalia.Services.Location
{
    /// <summary>
    /// http://freegeoip.net
    /// Limit: 10000 queries per hour
    /// </summary>
    public class FreegeoipService : ILocationApi
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

        private int RequestsPerMinute = 150;
        private ILogger _logger;
        private ThaliaContext _context;
        #endregion

        public FreegeoipService(ILogger logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Location> GetLocationAsync(string ip)
        {
            try
            {
                if (!CanRequest())
                {
                    _logger.LogCritical($"FreegeoipService: Cannot get location for '{ip}' because the request limit {RequestsPerMinute} was reached ");
                    return null;
                }

                var url = $"http://www.freegeoip.net/json/{ip}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        _context.ServiceRequests.Add(new ServiceRequest()
                        {
                            Ip = ip,
                            Api = this.GetType().Name,
                            Created = DateTime.Now,
                            Operation = "GetLocationAsync",
                            Response = content
                        });
                        _context.SaveChanges();

                        return GetLocation(content);
                    }

                    _logger.LogCritical($"FreegeoipService: Cannot get location for '{ip}'. Status code: {response.StatusCode} Content: {content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"FreegeoipService: Cannot get location for '{ip}'. Exception: " + ex.GetError());
            }

            return null;
        }

        public bool CanRequest()
        {
            return _context.ServiceRequests.Where(x => x.Api == this.GetType().Name && x.Created >= DateTime.Now.AddHours(-1) && x.Created <= DateTime.Now).Count() <= RequestsPerMinute;
        }

        public Location GetLocation(string json)
        {
            var locationDto = JsonConvert.DeserializeObject<LocationDto>(json);
            if (locationDto == null) return null;

            return new Location()
            {
                Country = locationDto.CountryName,
                Region = locationDto.RegionNname,
                City = locationDto.City
            };
        }
    }
}
