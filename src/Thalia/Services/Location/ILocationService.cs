using System.Threading.Tasks;

namespace Thalia.Services.Location
{
    public interface ILocationService
    {
        Task<Location> GetLocationAsync(string ip);
    }
}
