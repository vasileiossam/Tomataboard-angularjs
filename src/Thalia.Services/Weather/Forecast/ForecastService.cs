using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Thalia.Services.Extensions;

namespace Thalia.Services.Weather.Forecast
{
    /// <summary>
    /// https://developer.forecast.io/docs/v2
    /// </summary>
    public class ForecastService : IForecastService
    {
        #region Private Fields
        private readonly IOptions<ForecastKeys> _keys;
        private readonly ILogger<ForecastService> _logger;
        #endregion

        public string Parameters { get; set; }
        public string Result { get; set; }
        /// <summary>
        /// 1000 calls a day
        /// </summary>
        public int? RequestsPerMinute => 1000/24;
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public ForecastService(ILogger<ForecastService> logger, IOptions<ForecastKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">{LATITUDE},{LONGITUDE}</param>
        /// <returns></returns>
        public async Task<WeatherConditions> Execute(string parameters)
        {
            Parameters = parameters;
            
            try
            {
                var queryString = $"{_keys.Value.ConsumerKey}/{parameters}";
                var url = "https://api.forecast.io/forecast/";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url + queryString, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(content);
                        if (weatherDto == null) return null;

                        if (weatherDto.Code == null)
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
                Title = weatherDto.currently.summary,
                Description = weatherDto.currently.summary,
                TemperatureC = Convert.ToInt32(TemperatureConverter.FahrenheitToCelsius(weatherDto.currently.temperature)),
                TemperatureF = Convert.ToInt32(weatherDto.currently.temperature),
                Icon = Icons.GetCssClass(weatherDto.currently.icon)
            };
            return weatherConditions;
        }
    }
}
