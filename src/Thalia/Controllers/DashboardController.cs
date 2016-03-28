using System;
using System.Collections.Generic;
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
            var rnd = new Random();
            var weather = await _weatherProvider.Execute();
            var photos = await _photoProvider.Execute("Lego");
           
            var dashboardDto = new DashboardDto()
            {
                Weather = weather,
                Photos = photos.Take(10).ToArray(),
                Quotes = _quoteRepository.GetRandomQuotes("inspirational,motivational").Take(10).ToArray(),
                Greeting = _greetingsService.GetGreeting(milliseconds)
            };
            dashboardDto.PhotoIndex = rnd.Next(dashboardDto.Photos.Length - 1);
            dashboardDto.QuoteIndex = rnd.Next(dashboardDto.Quotes.Length - 1);

            return Json(dashboardDto);
        }
    }
}
