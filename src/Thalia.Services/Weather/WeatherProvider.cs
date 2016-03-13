using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Thalia.Services.Weather.Forecast;
using Thalia.Services.Weather.OpenWeatherMap;
using Thalia.Services.Weather.Yahoo;

namespace Thalia.Services.Weather
{
    public class WeatherProvider : Provider<WeatherConditions>, IWeatherProvider
    {
        public WeatherProvider(
            ILogger<WeatherProvider> logger, 
            ICacheRepository<WeatherConditions> cacheRepository,
            IForecastService forecastService,
            IOpenWeatherMapService openWeatherMapService,
            IYahooWeatherService yahooWeatherService)
            : base(logger, cacheRepository)
        {
            _operations.Add(forecastService);
            _operations.Add(openWeatherMapService);
            _operations.Add(yahooWeatherService);
        }
    }
}
