using System.Threading.Tasks;

namespace Thalia.Services.Location
{
    public class Location
    {
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public interface ILocationService
    {
        Task<Location> GetLocationAsync(string ip);
    }
}
