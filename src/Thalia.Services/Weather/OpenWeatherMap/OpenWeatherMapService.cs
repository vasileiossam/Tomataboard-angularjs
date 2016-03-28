using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Thalia.Services.Extensions;
using Thalia.Services.Locations;

namespace Thalia.Services.Weather.OpenWeatherMap
{
    /// <summary>
    /// http://openweathermap.org/current
    /// </summary>
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        #region Private Fields
        private readonly IOptions<OpenWeatherMapKeys> _keys;
        private readonly ILogger<OpenWeatherMapService> _logger;
        #endregion
        
        // 60 calls a min
        public Quota Quota => new Quota() { Requests = 60, Time = TimeSpan.FromMinutes(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public OpenWeatherMapService(ILogger<OpenWeatherMapService> logger, IOptions<OpenWeatherMapKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        private string GetQueryString(Location location)
        {
            // use geographic coordinates 
            if (!string.IsNullOrEmpty(location.Latitude) && !string.IsNullOrEmpty(location.Longitude))
            {
                return $"lat={location.Latitude}&&lon={location.Longitude}&appid={_keys.Value.ConsumerKey}";
            }

            // use city and country names
            var country = string.IsNullOrEmpty(location.CountryCode) ? location.Country : location.CountryCode;
            var cityName = $"{location.City},{location.StateCode},{country}".Replace(",,", "");
            return  $"q={cityName}&appid={_keys.Value.ConsumerKey}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">serialized Location</param>
        /// <returns></returns>
        public async Task<WeatherConditions> Execute(string parameters)
        {
            try
            {
                var location = JsonConvert.DeserializeObject<Location>(parameters);
                if (location == null)
                {
                    _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Cannot deserialize parameters");
                    return null;
                }

                using (var client = new HttpClient())
                {
                    var url = "http://api.openweathermap.org/data/2.5/weather";
                    var response = await client.GetAsync(new Uri(url + "?" + GetQueryString(location), UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(content);
                        if (weatherDto == null) return null;

                        if (weatherDto.Code == 200)
                        {
                            return GetResult(content, location);
                        }

                        _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Code: {weatherDto.Code}, Message: {weatherDto.Message}");
                    }

                    _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }

        private WeatherConditions GetResult(string json, Location location)
        {
            var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(json);
            if (weatherDto == null) return null;

            var weatherConditions = new WeatherConditions()
            {
                Title = weatherDto.Title,
                Description = weatherDto.Description,
                TemperatureC = (int)Math.Ceiling(TemperatureConverter.KelvinToCelsius(weatherDto.Main.Temperature)),
                TemperatureF = (int)Math.Ceiling(TemperatureConverter.KelvinToFahrenheit(weatherDto.Main.Temperature)),
                Icon = Icons.GetCssClass(weatherDto.IconCode),
                Location = location.City,
                Service = "OpenWeatherMap",
                ServiceUrl = "http://openweathermap.org/"
            };
            return weatherConditions;
        }
    }
}
