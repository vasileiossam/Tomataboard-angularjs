using Tomataboard.Services;

namespace Tomataboard.Services.Weather.Yahoo
{
    public interface IYahooWeatherService : IOauthService, IServiceOperation<WeatherConditions>
    {
    }
}
