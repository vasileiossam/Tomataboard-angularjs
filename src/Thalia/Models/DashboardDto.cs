using Thalia.Data.Entities;
using Thalia.Services.Photos;
using Thalia.Services.Weather;

namespace Thalia.Models
{
    public class DashboardDto
    {
        public string Name { get; set; }
        public string DefaultName { get; set; }
        public Photo[] Photos { get; set; }
        public Quote[] Quotes { get; set; }
        public string Greeting { get; set; }
        public WeatherConditions Weather { get; set; }
        public string Question { get; set; }
        public string DefaultQuestion { get; set; }
    }
}
