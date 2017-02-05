namespace Tomataboard.Services.Weather
{
    public class WeatherConditions
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public string Icon { get; set; }
        public string Location { get; set; }
        public string CountryCode { get; set; }
        public bool UsesFahrenheit { get; set; }
        public string Service { get; set; }
        public string ServiceUrl { get; set; }
    }
}