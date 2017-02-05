using System.Threading.Tasks;

namespace Tomataboard.Services.Weather
{
    public interface IWeatherProvider : IProvider<WeatherConditions>
    {
        /// <summary>
        ///  It passes the current geo location to the weather services
        /// </summary>
        /// <returns>Returns the current weather conditions</returns>
        Task<WeatherConditions> Execute();
    }
}