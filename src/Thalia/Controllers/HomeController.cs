using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Thalia.Services.Location;
using Thalia.Data;
using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;

namespace Thalia.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;
        private ThaliaContext _context;

        public HomeController(ILogger<HomeController> logger, ThaliaContext context)
        {
            _logger = logger;
            _context = context;
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
            var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.MapToIPv4().ToString();
#if DEBUG
            if ((ip == "0.0.0.1") || (ip == "127.0.0.1"))
            {
                //ip = "175342523";
                ip = "175.34.25.23";

            }
#endif      
            var locationExecutor = new LocationExecutor(_logger, new CacheRepository<Location>(_context), _context);
            var o = await locationExecutor.Execute(ip);
            return View();
        }
    }
}
