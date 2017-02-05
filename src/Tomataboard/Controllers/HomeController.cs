using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tomataboard.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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