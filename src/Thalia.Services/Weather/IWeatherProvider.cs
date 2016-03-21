using System.Threading.Tasks;
using Thalia.Services.Locations;

namespace Thalia.Services.Weather
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
