using Thalia.Data.Entities;
using Thalia.Services.Photos;
using Thalia.Services.Weather;

namespace Thalia.Models
{
    public class DashboardDto
    {
        public string Name { get; set; }
        public string City { get; set; }
        public Photo[] Photos { get; set; }
        public int PhotoIndex { get; set; }
        public Quote[] Quotes { get; set; }
        public int QuoteIndex { get; set; }
        public string Greeting { get; set; }
        public WeatherConditions Weather { get; set; }
    }
}
