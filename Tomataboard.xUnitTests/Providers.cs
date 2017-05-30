using Newtonsoft.Json;
using System.Collections.Generic;
using Tomataboard.Data;
using Tomataboard.Services.Cache;
using Tomataboard.Services.Locations;
using Tomataboard.Services.Locations.Freegeoip;
using Tomataboard.Services.Locations.GeoLite;
using Tomataboard.Services.Locations.IpGeolocation;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Weather;
using Tomataboard.Services.Weather.Forecast;
using Tomataboard.Services.Weather.OpenWeatherMap;
using Tomataboard.xUnitTests.Stubs;
using Xunit;

namespace Tomataboard.xUnitTests
{
    /// <summary>
    /// https://xunit.github.io/docs/getting-started-dnx.html
    /// </summary>
    public class Providers
    {
        private readonly TomataboardContext _TomataboardContext = null;

        //private readonly StubOptions<Api500pxKeys> _api500pxKeys;
        //private readonly StubOptions<FlickrKeys> _flickrKeys;
        private readonly StubOptions<OpenWeatherMapKeys> _openWeatherMapKeys;

        //private readonly StubOptions<YahooWeatherKeys> _yahooWeatherKeys;
        private readonly StubOptions<ForecastKeys> _forecastKeys;

        public Providers()
        {
            var dataSettings = new DataSettings()
            {
                ConnectionString =
                    @"Server=(localdb)\\mssqllocaldb;Database=aspnet5-Tomataboard-2758b425-7740-4589-825a-42e8d05436f5;Trusted_Connection=True;MultipleActiveResultSets=true",
                TomataboardContextConnection =
                    @"Server=.;Database=Tomataboard;Trusted_Connection=True;MultipleActiveResultSets=true"
            };
             //var settings = new StubOptions<DataSettings>(dataSettings);
             //_TomataboardContext = new TomataboardContext(settings);

            //_api500pxKeys = new StubOptions<Api500pxKeys>(new Api500pxKeys()
            //{
            //    ConsumerKey = "3Pyv3z7C11R0HGVDv4xdkql76Z0MpLGITwY8n5pK",
            //    ConsumerSecret = "kkfszxzVQcBxqLZfa605PvXf1ye7iO2PiTzaMudN",
            //    CallbackUrl = "http://localhost:4840/Home/Callback"
            //});

            //_flickrKeys = new StubOptions<FlickrKeys>(new FlickrKeys()
            //{
            //    ConsumerKey = "ae047ff46d722cdec62a140589ff56d5",
            //    ConsumerSecret = "48b96aeb42e8ac2e"
            //});

            _openWeatherMapKeys = new StubOptions<OpenWeatherMapKeys>(new OpenWeatherMapKeys()
            {
                ConsumerKey = "c4cb39457a6fd5b7c5c17bd8027f4eea"
            });

            //_yahooWeatherKeys = new StubOptions<YahooWeatherKeys>(new YahooWeatherKeys()
            //{
            //    ConsumerKey =
            //        "dj0yJmk9MExOUEpDRzI0dFlsJmQ9WVdrOWRVdHphMHRWTkdzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD0zMA--",
            //    ConsumerSecret = "ef5e83e1aa41158f0fb486b073ed0a695806466b"
            //});

            _forecastKeys = new StubOptions<ForecastKeys>(new ForecastKeys()
            {
                ConsumerKey = "62888b611b3ffd9d6fc0601dfde59850",
            });
        }

        /// <summary>
        /// http://dotnetliberty.com/index.php/2015/12/14/asp-net-5-web-api-unit-testing/
        /// </summary>
        //[Fact]
        //public async void GetPhotos()
        //{
            //List<Photo> photos;

            //var cacheRepository = new CacheRepository<List<Photo>>(_TomataboardContext);

            //var api500px = new Api500px(new StubLogger<Api500px>(), _api500pxKeys);
            //photos = await api500px.Execute("Greece");
            //Assert.NotNull(photos);

            //var flickrService = new FlickrService(new StubLogger<FlickrService>(), _flickrKeys);
            //photos = await api500px.Execute("Greece");
            //Assert.NotNull(photos);

            //var provider = new PhotoProvider(new StubLogger<PhotoProvider>(), cacheRepository, api500px, flickrService);
            //photos = await provider.Execute("landscape");
            //Assert.NotNull(photos);
       // }

        [Fact]
        public async void GetWeather()
        {
            WeatherConditions weather;

            var melbourneLocation = new Location()
            {
                Latitude = "-37.8136",
                Longitude = "144.9631"
            };
            var serializedMelbourne = JsonConvert.SerializeObject(melbourneLocation);

            var cacheRepository = new CacheRepository<WeatherConditions>(_TomataboardContext);

            var openWeatherMapService = new OpenWeatherMapService(new StubLogger<OpenWeatherMapService>(), _openWeatherMapKeys);
            weather = await openWeatherMapService.Execute(serializedMelbourne);
            Assert.NotNull(weather);

            var forecastService = new ForecastService(new StubLogger<ForecastService>(), _forecastKeys);
            weather = await forecastService.Execute(serializedMelbourne);
            Assert.NotNull(weather);

            //var yahooWeatherService = new YahooWeatherService(new StubLogger<YahooWeatherService>(), _yahooWeatherKeys);

            //var provider = new WeatherProvider(new StubLogger<WeatherProvider>(), cacheRepository, forecastService,openWeatherMapService, yahooWeatherService);
            //weather = await provider.Execute(serializedMelbourne);
            Assert.NotNull(weather);
        }

        [Fact]
        public async void GetLocation()
        {
            Location location;
            const string ip = "175.34.25.23";
            var cacheRepository = new CacheRepository<Location>(_TomataboardContext);

            var ipGeolocationService = new IpGeolocationService(new StubLogger<IpGeolocationService>());
            location = await ipGeolocationService.Execute(ip);
            Assert.NotNull(location);

            var geoLiteService = new GeoLiteService(new StubLogger<GeoLiteService>(), _TomataboardContext);
            location = await geoLiteService.Execute(ip);
            Assert.NotNull(location);

            var freegeoipService = new FreegeoipService(new StubLogger<FreegeoipService>());
            location = await freegeoipService.Execute(ip);
            Assert.NotNull(location);

            //var provider = new LocationProvider(new StubLogger<LocationProvider>(), cacheRepository, ipGeolocationService, geoLiteService, freegeoipService);
            //location = await provider.Execute(ip);
            //Assert.NotNull(location);

            //// invalid ip
            //location = await provider.Execute("asdfasdF");
            //Assert.Null(location);

            //location = await provider.Execute(ip);
            //Assert.NotNull(location);
        }
    }
}