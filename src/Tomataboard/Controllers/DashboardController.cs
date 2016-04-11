using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Tomataboard.Models;
using Tomataboard.Services.Greetings;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Quotes;
using Tomataboard.Services.Weather;
using Tomataboard.Services.Extensions;

namespace Tomataboard.Controllers
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

        /// <summary>
        /// http://damienbod.com/2014/08/22/web-api-2-exploring-parameter-binding/
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="tags"></param>
        /// <param name="readCache"></param>
        /// <returns></returns>
        [HttpGet("api/dashboard")]
        [HttpGet("api/dashboard/{milliseconds}")]
        public async Task<JsonResult> Get(long milliseconds, string tags, bool? readCache)
        {
            var weather = await _weatherProvider.Execute();
            var photos = await _photoProvider.Execute(
                string.IsNullOrEmpty(tags) ? "landscape" : tags,
                readCache ?? true);
           
            var dashboardDto = new DashboardDto
            {
                Photos = photos.Shuffle().Take(4).ToArray(),
                Quotes = _quoteRepository.GetQuotes("inspirational,motivational").Shuffle().Take(4).ToArray(),
                Greeting = _greetingsService.GetGreeting(milliseconds),
                Weather = weather,
            };

            return Json(dashboardDto);
        }
    }

    public class FromUriAttribute : Attribute
    {
    }
}
