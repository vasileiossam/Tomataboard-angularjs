using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Thalia.Data;
using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Microsoft.Extensions.OptionsModel;
using Thalia.Services;
using Thalia.Services.Locations;
using Thalia.Services.Photos;
using Thalia.Services.Photos.Api500px;
using Thalia.Services.Photos.Flickr;
using Thalia.Services.Quotes;
using Thalia.Services.Weather.Yahoo;

namespace Thalia.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;
        private ThaliaContext _context;
        private IOptions<Api500pxKeys> _api500pxKeys;
        private IOptions<FlickrKeys> _flickrKeys;
        private IOptions<DataSettings> _dataSettings;
        private IOptions<YahooWeatherKeys> _yahooWeatherKeys;
        
        public HomeController(ILogger<HomeController> logger, ThaliaContext context, 
            IOptions<Api500pxKeys> api500pxKeys,
            IOptions<FlickrKeys> flickrKeys,
            IOptions<DataSettings> dataSettings,
            IOptions<YahooWeatherKeys> yahooWeatherKeys)
        {
            _logger = logger;
            _context = context;
            _api500pxKeys = api500pxKeys;
            _dataSettings = dataSettings;
            _flickrKeys = flickrKeys;
            _yahooWeatherKeys = yahooWeatherKeys;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Test()
        {
            IServiceOperation<List<Photo>> service;

            //var weatherService = new YahooWeatherService(_logger, _yahooWeatherKeys);
            //await weatherService.Execute("Melbourne");

            //service = new FlickrService(_logger, _flickrSettings);
            //ViewData.Model = await service.Execute("winter,snow,landscape");

           // service = new Api500px(_logger, _api500pxKeys);
           // ViewData.Model = await service.Execute("inspirational");

            var repo = new QuoteRepository(_context);
            var quote = repo.GetQuoteOfTheDay();

            var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.MapToIPv4().ToString();
#if DEBUG
            if ((ip == "0.0.0.1") || (ip == "127.0.0.1"))
            {
                //ip = "175342523";
                ip = "175.34.25.23";

            }
#endif      
            //var locationExecutor = new LocationProvider(_logger, new CacheRepository<Location>(_context), _context);
            //var o = await locationExecutor.Execute(ip);
            return View();
        }
    }
}
