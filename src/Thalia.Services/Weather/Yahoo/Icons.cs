using System.Collections.Generic;
using System.Linq;

namespace Thalia.Services.Weather.Yahoo
{
    public class Icon
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CssClass { get; set; }
    }

    /// <summary>
    /// https://developer.yahoo.com/weather/documentation.html
    /// https://erikflowers.github.io/weather-icons/
    /// </summary>
    public static class Icons
    {
        public static string GetCssClass(string code)
        {
            var list = new List<Icon>()
            {
                new Icon() {Code = "0", Description = "tornado", CssClass = "wi-tornado"},
                new Icon() {Code = "1", Description = "tropical storm", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "2", Description = "hurricane", CssClass = "wi-hurricane"},
                new Icon() {Code = "3", Description = "severe thunderstorms", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "4", Description = "thunderstorms", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "5", Description = "mixed rain and snow", CssClass = "wi-rain-mix"},
                new Icon() {Code = "6", Description = "mixed rain and sleet", CssClass = "wi-sleet"},
                new Icon() {Code = "7", Description = "mixed snow and sleet", CssClass = "wi-sleet"},
                new Icon() {Code = "8", Description = "freezing drizzle", CssClass = "wi-sprinkle"},
                new Icon() {Code = "9", Description = "drizzle", CssClass = "wi-sprinkle"},
                new Icon() {Code = "10", Description = "freezing rain", CssClass = "wi-rain-mix"},
                new Icon() {Code = "11", Description = "showers", CssClass = "wi-showers"},
                new Icon() {Code = "12", Description = "showers", CssClass = "wi-showers"},
                new Icon() {Code = "13", Description = "snow flurries", CssClass = "wi-snowflake-cold"},
                new Icon() {Code = "14", Description = "light snow showers", CssClass = "wi-snow"},
                new Icon() {Code = "15", Description = "blowing snow", CssClass = "wi-snow-wind"},
                new Icon() {Code = "16", Description = "snow", CssClass = "wi-snow"},
                new Icon() {Code = "17", Description = "hail", CssClass = "wi-hail"},
                new Icon() {Code = "18", Description = "sleet", CssClass = "wi-sleet"},
                new Icon() {Code = "19", Description = "dust", CssClass = "wi-dust"},
                new Icon() {Code = "20", Description = "foggy", CssClass = "wi-fog"},
                new Icon() {Code = "21", Description = "haze", CssClass = "wi-fog"},
                new Icon() {Code = "22", Description = "smoky", CssClass = "wi-smoke"},
                new Icon() {Code = "23", Description = "blustery", CssClass = "wi-strong-wind"},
                new Icon() {Code = "24", Description = "windy", CssClass = "wi-windy"},
                new Icon() {Code = "25", Description = "cold", CssClass = "wi-cloudy"},
                new Icon() {Code = "26", Description = "cloudy", CssClass = "wi-cloudy"},
                new Icon() {Code = "27", Description = "mostly cloudy", CssClass = "wi-night-alt-cloudy"}, // (night)
                new Icon() {Code = "28", Description = "mostly cloudy", CssClass = "wi-day-cloudy"}, // (day)
                new Icon() {Code = "29", Description = "partly cloudy", CssClass = "wi-night-alt-cloudy"}, // (night)
                new Icon() {Code = "30", Description = "partly cloudy", CssClass = "wi-day-cloudy"}, // (day)
                new Icon() {Code = "31", Description = "clear", CssClass = "wi-night-clear"}, // (night)
                new Icon() {Code = "32", Description = "sunny", CssClass = "wi-day-sunny"},
                new Icon() {Code = "33", Description = "fair ", CssClass = "wi-night-clear"}, // (night)
                new Icon() {Code = "34", Description = "fair ", CssClass = "wi-day-sunny"}, // (day)
                new Icon() {Code = "35", Description = "mixed rain and hail", CssClass = "wi-rain-mix"},
                new Icon() {Code = "36", Description = "hot", CssClass = "wi-hot"},
                new Icon() {Code = "37", Description = "isolated thunderstorms", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "38", Description = "scattered thunderstorms", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "39", Description = "scattered thunderstorms", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "40", Description = "scattered showers", CssClass = "wi-showers"},
                new Icon() {Code = "41", Description = "heavy snow", CssClass = "wi-snow"},
                new Icon() {Code = "42", Description = "scattered snow showers", CssClass = "wi-snow-wind"},
                new Icon() {Code = "43", Description = "heavy snow", CssClass = "wi-snow"},
                new Icon() {Code = "44", Description = "partly cloudy", CssClass = "wi-cloudy"},
                new Icon() {Code = "45", Description = "thundershowers", CssClass = "wi-thunderstorm"},
                new Icon() {Code = "46", Description = "snow showers", CssClass = "wi-snow"},
                new Icon() {Code = "47", Description = "isolated thundershowers", CssClass = "wi-thunderstorm"},

                new Icon() {Code = "3200", Description = "not available", CssClass = ""},
            };

            var icon = list.FirstOrDefault(x => x.Code == code);
            return icon == null ? string.Empty : icon.CssClass;
        }
    }
}
