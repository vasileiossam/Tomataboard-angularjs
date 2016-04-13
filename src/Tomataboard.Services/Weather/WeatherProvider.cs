using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tomataboard.Services.Locations;
using Tomataboard.Services.Weather;
using Tomataboard.Services.Weather.Forecast;
using Tomataboard.Services.Weather.OpenWeatherMap;
using Tomataboard.Services.Weather.Yahoo;
using Tomataboard.Services.Cache;

namespace Tomataboard.Services.Weather
{
    public class WeatherProvider : Provider<WeatherConditions>, IWeatherProvider
    {
        private readonly string _serializedLocation;

        public WeatherProvider(
            ILogger<WeatherProvider> logger, 
            ILocationProvider locationProvider,
            ICacheRepository<WeatherConditions> cacheRepository,
            IForecastService forecastService,
            IOpenWeatherMapService openWeatherMapService,
            IYahooWeatherService yahooWeatherService)
            : base(logger, cacheRepository)
        {
            var location = locationProvider.Execute().Result;
            if (location == null) throw new Exception("Cannot determine location");

            _serializedLocation = JsonConvert.SerializeObject(location);

            _operations.Add(yahooWeatherService);
            _operations.Add(openWeatherMapService);
            _operations.Add(forecastService);
        }

        /// <summary>
        ///  It passes the current geo location to the weather services
        /// </summary>
        /// <returns>Returns the current weather conditions</returns>
        public async Task<WeatherConditions> Execute()
        {
            return await Execute(_serializedLocation, true);
        }
    }
}
