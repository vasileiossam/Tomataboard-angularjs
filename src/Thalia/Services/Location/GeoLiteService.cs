using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Thalia.Data;
using Thalia.Extensions;
using System.Linq;
using Thalia.Data.Entities;

namespace Thalia.Services.Location
{
    public class GeoLiteService : ILocationApi
    {
        #region private members
        private ILogger _logger;
        private ThaliaContext _context;
        #endregion

        public GeoLiteService(ILogger logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public bool CanRequest()
        {
            return true;
        }

        public Location GetLocation(string json)
        {
            throw new NotImplementedException();
        }
    }
}
