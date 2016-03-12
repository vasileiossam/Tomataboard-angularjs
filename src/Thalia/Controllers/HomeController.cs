using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Thalia.Data;
using Microsoft.Extensions.Logging;
using Thalia.Services.Quotes;
using Thalia.Services.Locations;
using Thalia.Services.Photos;
using Thalia.Services.Weather;

namespace Thalia.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;
        private ThaliaContext _context;
        private ILocationProvider _locationProvider;
        private IPhotoProvider _photoProvider;
        private IWeatherProvider _weatherProvider;

        public HomeController(ILogger<HomeController> logger, ThaliaContext context, 
            ILocationProvider locationProvider,
            IPhotoProvider photoProvider,
            IWeatherProvider weatherProvider)
        {
            _logger = logger;
            _context = context;
            _locationProvider = locationProvider;
            _photoProvider = photoProvider;
            _weatherProvider = weatherProvider;
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
           var o = await _locationProvider.Execute(ip);
            return View();
        }
    }
}
