using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Thalia.Services.Extensions;

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

        public string Parameters { get; set; }
        public string Result { get; set; }
        public int? RequestsPerMinute => 60;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public OpenWeatherMapService(ILogger<OpenWeatherMapService> logger, IOptions<OpenWeatherMapKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">{city name},{country code}</param>
        /// <returns></returns>
        public async Task<WeatherConditions> Execute(string parameters)
        {
            Parameters = parameters;
            
            try
            {
                var queryString = $"q={parameters}&appid={_keys.Value.ConsumerKey}";
                var url = "http://api.openweathermap.org/data/2.5/weather";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url + "?" + queryString, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(content);
                        if (weatherDto == null) return null;

                        if (weatherDto.Code == 200)
                        {
                            return GetResult(content);
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

        public WeatherConditions GetResult(string json)
        {
            // todo Code smell, it shouldn't even be here. Result is used in cache because it is in the interface but what will make sure that it has a value?
            Result = json;

            var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(json);
            if (weatherDto == null) return null;

            var weatherConditions = new WeatherConditions()
            {
                Title = weatherDto.Title,
                Description = weatherDto.Description,
                TemperatureC = Convert.ToInt32(TemperatureConverter.KelvinToCelsius(weatherDto.Main.Temperature)),
                TemperatureF = Convert.ToInt32(TemperatureConverter.KelvinToFahrenheit(weatherDto.Main.Temperature)),
                Icon = Icons.GetCssClass(weatherDto.IconCode)
            };
            return weatherConditions;
        }
    }
}
