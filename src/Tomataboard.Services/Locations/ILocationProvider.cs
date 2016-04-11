using System.Threading.Tasks;
using Tomataboard.Services;

namespace Tomataboard.Services.Locations
{
    public interface ILocationProvider : IProvider<Location>
    {
        /// <summary>
        ///  It passes the IPv4 to the services
        /// </summary>
        /// <returns>Returns the geo location</returns>
        Task<Location> Execute();
    }
}
