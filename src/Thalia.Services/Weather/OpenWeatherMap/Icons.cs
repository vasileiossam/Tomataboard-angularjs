using System.Collections.Generic;
using System.Linq;

namespace Thalia.Services.Weather.OpenWeatherMap
{
    public class Icon
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CssClass { get; set; }
    }

    /// <summary>
    /// http://openweathermap.org/weather-conditions
    /// https://erikflowers.github.io/weather-icons/
    /// </summary>
    public static class Icons
    {
        public static string GetCssClass(string code)
        {
            var list = new List<Icon>()
            {
                new Icon() { Code = "01d", Description = "clear sky", CssClass = "wi-day-sunny" },
                new Icon() { Code = "01n", Description = "clear sky", CssClass = "wi-night-clear"},
                new Icon() { Code = "02d", Description = "few clouds", CssClass = "wi-day-cloudy" },
                new Icon() { Code = "02n", Description = "few clouds", CssClass = "wi-night-alt-cloudy"},
                new Icon() { Code = "03d", Description = "scattered clouds", CssClass = "wi-day-cloudy-high" },
                new Icon() { Code = "03n", Description = "scattered clouds", CssClass = "wi-night-alt-cloudy-high" },
                new Icon() { Code = "04d", Description = "broken clouds", CssClass = "wi-cloudy" },
                new Icon() { Code = "04n", Description = "broken clouds", CssClass = "wi-cloudy" },
                new Icon() { Code = "09d", Description = "shower rain", CssClass = "wi-showers"},
                new Icon() { Code = "09n", Description = "shower rain", CssClass = "wi-showers" },
                new Icon() { Code = "10d", Description = "rain", CssClass = "wi-day-showers" },
                new Icon() { Code = "10n", Description = "rain", CssClass = "wi-night-alt-showers" },
                new Icon() { Code = "11d", Description = "thunderstorm", CssClass = "wi-day-thunderstorm" },
                new Icon() { Code = "11n", Description = "thunderstorm", CssClass = "wi-night-alt-thunderstorm" },
                new Icon() { Code = "13d", Description = "snow", CssClass = "wi-snow" },
                new Icon() { Code = "13n", Description = "snow", CssClass = "wi-snow" },
                new Icon() { Code = "50d", Description = "mist", CssClass = "wi-fog" },
                new Icon() { Code = "50n", Description = "mist", CssClass = "wi-fog" },
            };

            var icon = list.FirstOrDefault(x => x.Code == code);
            return icon == null ? string.Empty : icon.CssClass;
        }
    }
}
