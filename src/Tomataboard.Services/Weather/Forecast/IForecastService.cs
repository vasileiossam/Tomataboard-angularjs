using Tomataboard.Services;

namespace Tomataboard.Services.Weather.Forecast
{
    public interface IForecastService : IServiceOperation<WeatherConditions>
    {
    }
}
