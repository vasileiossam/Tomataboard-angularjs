namespace Thalia.Services.Weather.Yahoo
{
    public interface IYahooWeatherService : IOauthService, IServiceOperation<WeatherConditions>
    {
    }
}
