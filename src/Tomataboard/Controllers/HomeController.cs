using Microsoft.AspNet.Mvc;
using Tomataboard.Data;
using Microsoft.Extensions.Logging;
using Tomataboard.Services.Locations;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Weather;
using Tomataboard.Services.Weather.Yahoo;

namespace Tomataboard.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;
        private TomataboardContext _context;
        private ILocationProvider _locationProvider;
        private IPhotoProvider _photoProvider;
        private IWeatherProvider _weatherProvider;
        private ILogger<YahooWeatherService> _yahooLogger;

        public HomeController(ILogger<HomeController> logger, TomataboardContext context, 
        //    ILocationProvider locationProvider,
            IPhotoProvider photoProvider,
        //    IWeatherProvider weatherProvider,
            ILogger<YahooWeatherService> yahooLogger)
        {
            _logger = logger;
            _context = context;
         //   _locationProvider = locationProvider;
            _photoProvider = photoProvider;
          //  _weatherProvider = weatherProvider;
            _yahooLogger = yahooLogger;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
