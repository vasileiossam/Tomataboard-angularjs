using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalia.Data;
using Thalia.Services.Cache;
using Thalia.Services.Photos;
using Thalia.xUnitTests.Stubs;
using Xunit;

namespace Thalia.xUnitTests
{
    /// <summary>
    /// https://xunit.github.io/docs/getting-started-dnx.html
    /// </summary>
    public class LocationProvider
    {
        public LocationProvider()
        {
        }

        /// <summary>
        /// http://dotnetliberty.com/index.php/2015/12/14/asp-net-5-web-api-unit-testing/
        /// </summary>
        [Fact]
        public void GetPhotos()
        {
            //var cache = new CacheRepository<List<Photo>(new ThaliaContext(null));
            //var provider = new PhotoProvider(new StubLogger<PhotoProvider>(), );
        }
    }
}
