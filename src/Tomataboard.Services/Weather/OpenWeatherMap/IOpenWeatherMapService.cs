using Tomataboard.Services;

namespace Tomataboard.Services.Weather.OpenWeatherMap
{
    public interface IOpenWeatherMapService : IServiceOperation<WeatherConditions>
    {
    }
}
