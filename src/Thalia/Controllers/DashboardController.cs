using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Thalia.Models;
using Thalia.Services.Greetings;
using Thalia.Services.Photos;
using Thalia.Services.Quotes;
using Thalia.Services.Weather;

namespace Thalia.Controllers
{
    public class DashboardController : Controller
    {
        private ILogger _logger;
        private IPhotoProvider _photoProvider;
        private IWeatherProvider _weatherProvider;
        private IQuoteRepository _quoteRepository;
        private IGreetingsService _greetingsService;

        public DashboardController(
            ILogger<HomeController> logger, 
            IPhotoProvider photoProvider,
            IWeatherProvider weatherProvider,
            IQuoteRepository quoteRepository,
            IGreetingsService greetingsService)
        {
            _logger = logger;
            _photoProvider = photoProvider;
            _weatherProvider = weatherProvider;
            _quoteRepository = quoteRepository;
            _greetingsService = greetingsService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("api/dashboard/{milliseconds}")]
        public async Task<JsonResult> Get(long milliseconds)
        {
            var weather = await _weatherProvider.Execute();
            var photos = await _photoProvider.Execute("landscape", true);
           
            var dashboardDto = new DashboardDto
            {
                Name = "Young Grasshopper",
                DefaultName = "Young Grasshopper",
                Photos = photos.Take(4).ToArray(),
                Quotes = _quoteRepository.GetRandomQuotes("inspirational,motivational").Take(4).ToArray(),
                Greeting = _greetingsService.GetGreeting(milliseconds),
                Weather = weather,
                Question = "What is your goal for today?",
                DefaultQuestion = "What is your goal for today?"
            };
            return Json(dashboardDto);
        }
    }
}
