using System.Collections.Generic;
using System.Linq;

namespace Tomataboard.Services.Weather.Forecast
{
    public class Icon
    {
        public string Code { get; set; }
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
            var list = new List<Icon>
            {
                new Icon { Code = "clear-day", CssClass = "wi-day-sunny" },
                new Icon { Code = "clear-night", CssClass = "wi-night-clear" },
                new Icon { Code = "rain", CssClass = "wi-rain" },
                new Icon { Code = "snow", CssClass = "wi-snow" },
                new Icon { Code = "sleet", CssClass = "wi-sleet" },
                new Icon { Code = "wind", CssClass = "wi-windy" },
                new Icon { Code = "fog", CssClass = "wi-fog" },
                new Icon { Code = "cloudy", CssClass = "wi-cloudy" },
                new Icon { Code = "partly-cloudy-day", CssClass = "wi-day-cloudy" },
                new Icon { Code = "partly-cloudy-night", CssClass = "wi-night-alt-cloudy" },
                new Icon { Code = "thunderstorm", CssClass = "wi-thunderstorm" },
                new Icon { Code = "tornado", CssClass = "wi-tornado" }
            };

            var icon = list.FirstOrDefault(x => x.Code == code);
            return icon == null ? string.Empty : icon.CssClass;
        }
    }
}
