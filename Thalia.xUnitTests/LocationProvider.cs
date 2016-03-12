using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalia.Services.Photos;
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

        
        [Fact]
        public void PassingTest()
        {
            var provider = new PhotoProvider();
        }
    }
}
