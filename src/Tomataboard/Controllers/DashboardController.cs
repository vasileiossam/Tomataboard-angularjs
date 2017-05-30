using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tomataboard.Models;
using Tomataboard.Services.Extensions;
using Tomataboard.Services.Greetings;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Quotes;
using Tomataboard.Services.Weather;

namespace Tomataboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPhotoProvider _photoProvider;
        private readonly IWeatherProvider _weatherProvider;
        private readonly IQuoteRepository _quoteRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ILogger<HomeController> logger,
            IPhotoProvider photoProvider,
            IWeatherProvider weatherProvider,
            IQuoteRepository quoteRepository,
            IGreetingsService greetingsService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _photoProvider = photoProvider;
            _weatherProvider = weatherProvider;
            _quoteRepository = quoteRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("api/ping")]
        public JsonResult Ping()
        {
            return Json(true);
        }

        /// <summary>
        /// http://damienbod.com/2014/08/22/web-api-2-exploring-parameter-binding/
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="readCache"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("api/dashboard")]
        public async Task<DashboardDto> Get(string tags, bool? readCache)
        {
            return await GetDashboardDto(tags, readCache);
        }

        [HttpGet("api/dashboard-public")]
        public async Task<DashboardDto> GetPublic(string tags, bool? readCache)
        {
            // TODO limit this to return same data per day per IP?
            return await GetDashboardDto(tags, readCache);
        }

        private async Task<DashboardDto> GetDashboardDto(string tags, bool? readCache)
        {
            try
            {
                var weather = await _weatherProvider.Execute();
                var photos = await _photoProvider.Execute(
                    string.IsNullOrEmpty(tags) ? "landscape" : tags,
                    readCache ?? true);

                var dashboardDto = new DashboardDto
                {
                    Photos = photos.Shuffle().Take(20).ToArray(),
                    Quotes = _quoteRepository.GetQuotes("inspirational,motivational").Shuffle().Take(20).ToArray(),
                    Weather = weather
                };

                return dashboardDto;
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting DashboardDto", e);
                throw;
            }
        }

        [Authorize]
        [HttpPost("api/settings")]
        public async Task Post([FromBody] SettingsDto settings)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    user.Settings = JsonConvert.SerializeObject(settings);
                    await _userManager.UpdateAsync(user);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Error posting settings", e);
                throw;
            }
        }

        [Authorize]
        [HttpGet("api/settings")]
        public async Task<SettingsDto> Get()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if ( user != null && !string.IsNullOrEmpty(user.Settings))
                {
                    return JsonConvert.DeserializeObject<SettingsDto>(user.Settings);
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting settings", e);
                throw;
            }
        }
    }
}