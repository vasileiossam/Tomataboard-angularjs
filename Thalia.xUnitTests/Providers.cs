using System.Collections.Generic;
using Thalia.Data;
using Thalia.Services.Cache;
using Thalia.Services.Photos;
using Thalia.Services.Photos.Api500px;
using Thalia.Services.Photos.Flickr;
using Thalia.Services.Weather;
using Thalia.Services.Weather.OpenWeatherMap;
using Thalia.Services.Weather.Yahoo;
using Thalia.xUnitTests.Stubs;
using Xunit;

namespace Thalia.xUnitTests
{
    /// <summary>
    /// https://xunit.github.io/docs/getting-started-dnx.html
    /// </summary>
    public class Providers
    {
        private readonly ThaliaContext _thaliaContext;
        private readonly StubOptions<Api500pxKeys> _api500pxKeys;
        private readonly StubOptions<FlickrKeys> _flickrKeys;
        private readonly StubOptions<OpenWeatherMapKeys> _openWeatherMapKeys;
        private readonly StubOptions<YahooWeatherKeys> _yahooWeatherKeys;

        public Providers()
        {
            var dataSettings = new DataSettings()
            {
                ConnectionString = @"Server=(localdb)\\mssqllocaldb;Database=aspnet5-Thalia-2758b425-7740-4589-825a-42e8d05436f5;Trusted_Connection=True;MultipleActiveResultSets=true",
                ThaliaContextConnection = @"Server=.;Database=Thalia;Trusted_Connection=True;MultipleActiveResultSets=true"
            };
            var settings = new StubOptions<DataSettings>(dataSettings);
            _thaliaContext = new ThaliaContext(settings);

            _api500pxKeys = new StubOptions<Api500pxKeys>(new Api500pxKeys()
            {
                ConsumerKey = "3Pyv3z7C11R0HGVDv4xdkql76Z0MpLGITwY8n5pK",
                ConsumerSecret = "kkfszxzVQcBxqLZfa605PvXf1ye7iO2PiTzaMudN",
                CallbackUrl = "http://localhost:4840/Home/Callback",
                AccessToken = "x05M0RlcIKgtfeErvaZJVqbWAHfSPwhkK3UELfnn",
                AccessSecret = "xJTPoSXuJu06fWIuewvp3wEPYDG0gJ0QyxaBMfzN"
            });

            _flickrKeys = new StubOptions<FlickrKeys>(new FlickrKeys()
            {
              ConsumerKey = "ae047ff46d722cdec62a140589ff56d5",
              ConsumerSecret = "48b96aeb42e8ac2e"
            });

            _openWeatherMapKeys = new StubOptions<OpenWeatherMapKeys>(new OpenWeatherMapKeys()
            {
                ConsumerKey = "c4cb39457a6fd5b7c5c17bd8027f4eea"
            });

            _yahooWeatherKeys = new StubOptions<YahooWeatherKeys>(new YahooWeatherKeys()
            {
                ConsumerKey = "dj0yJmk9MExOUEpDRzI0dFlsJmQ9WVdrOWRVdHphMHRWTkdzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD0zMA--",
                ConsumerSecret = "ef5e83e1aa41158f0fb486b073ed0a695806466b"
            });
        }

        /// <summary>
        /// http://dotnetliberty.com/index.php/2015/12/14/asp-net-5-web-api-unit-testing/
        /// </summary>
        [Fact]
        public async void GetPhotos()
        {
            var cacheRepository = new CacheRepository<List<Photo>>(_thaliaContext);
            var api500px = new Api500px(new StubLogger<Api500px>(), _api500pxKeys);
            var flickrService = new FlickrService(new StubLogger<FlickrService>(), _flickrKeys);
            var provider = new PhotoProvider(new StubLogger<PhotoProvider>(), cacheRepository, api500px, flickrService);

            var photos = await provider.Execute("landscape");
        }

        [Fact]
        public async void GetWeather()
        {
            var cacheRepository = new CacheRepository<WeatherConditions>(_thaliaContext);
            var openWeatherMapService = new OpenWeatherMapService(new StubLogger<OpenWeatherMapService>(), _openWeatherMapKeys);
            var yahooWeatherService = new YahooWeatherService(new StubLogger<YahooWeatherService>(), _yahooWeatherKeys);
            var provider = new WeatherProvider(new StubLogger<WeatherProvider>(), cacheRepository, openWeatherMapService, yahooWeatherService);

            var weather = await provider.Execute("Melbourne,AUS");
        }
    }
}
