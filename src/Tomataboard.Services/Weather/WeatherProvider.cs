using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Tomataboard.Services.Cache;
using Tomataboard.Services.Locations;
using Tomataboard.Services.Weather.Forecast;
using Tomataboard.Services.Weather.OpenWeatherMap;
using Tomataboard.Services.Weather.Yahoo;

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
            try
            {
                var location = locationProvider.Execute().Result;
                if (location == null)
                {
                    _logger.LogError($"{GetType().Name}: Cannot get Location. Weather will disabled.");
                }
                else
                {
                    _serializedLocation = JsonConvert.SerializeObject(location);

                    _operations.Add(yahooWeatherService);
                    _operations.Add(openWeatherMapService);
                    _operations.Add(forecastService);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("WeatherProvider", e);
                throw e;
            }
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