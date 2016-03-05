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
    public class IpGeolocationApi : ILocationApi
    {
        #region private members
        private ILogger _logger;
        private ThaliaContext _context;
        private int RequestsPerMinute = 150;
        #endregion

        public IpGeolocationApi(ILogger logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public bool CanRequest()
        {
            return _context.ServiceRequests.Where(x => x.Api == this.GetType().Name && x.Created >= DateTime.Now.AddHours(-1) && x.Created <= DateTime.Now).Count() <= RequestsPerMinute;
        }

        public Location GetLocation(string json)
        {
            throw new NotImplementedException();
        }
    }
}
