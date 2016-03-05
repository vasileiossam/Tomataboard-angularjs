using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalia.Data.Entities;

namespace Thalia.Services
{
    interface ILocationApi
    {
        Task<Location.Location> GetLocationAsync(string ip);
        bool CanRequest();
        Location.Location GetLocation(string json);
    }
}
