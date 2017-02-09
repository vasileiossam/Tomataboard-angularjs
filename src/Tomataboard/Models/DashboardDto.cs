using Tomataboard.Data.Entities;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Weather;

namespace Tomataboard.Models
{
    public class DashboardDto
    {
        public Photo[] Photos { get; set; }
        public Quote[] Quotes { get; set; }
        public WeatherConditions Weather { get; set; }
    }
}