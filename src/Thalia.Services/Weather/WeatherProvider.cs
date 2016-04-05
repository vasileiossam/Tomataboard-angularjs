using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Thalia.Services.Cache;
using Thalia.Services.Extensions;
using Thalia.Services.Locations;
using Thalia.Services.Weather.Forecast;
using Thalia.Services.Weather.OpenWeatherMap;
using Thalia.Services.Weather.Yahoo;

namespace Thalia.Services.Weather
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

            _operations.Add(openWeatherMapService);
            _operations.Add(forecastService);
            _operations.Add(yahooWeatherService);
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
