using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Thalia.Services;
using Thalia.Services.AccessTokens;
using Thalia.Services.Weather.Yahoo;
using System;
using Newtonsoft.Json;
using Thalia.Services.Locations;

namespace Thalia.Controllers
{
    public class YahooController : OauthController
    {
        public YahooController(
            ILogger<YahooWeatherService> logger,
            IYahooWeatherService service,
            ICookiesService<OauthToken> cookiesService,
            IAccessTokensRepository accessTokensRepository) : base(logger, service, cookiesService, accessTokensRepository)
        {
            _cookieKey = "YahooRequestToken";
        }

        public override async Task<ActionResult> Execute()
        {
            try
            {
                var melbourneLocation = new Location()
                {
                    City = "Melbournze",
                    CountryCode = "AUS"
                };
                var serializedMelbourne = JsonConvert.SerializeObject(melbourneLocation);

                var service = _service as YahooWeatherService;
                if (service != null)
                {
                    service.AccessToken = _accessTokensRepository.Find(_service.GetType().Name);
                    return View(await service.Execute(serializedMelbourne));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
            return View("Error");
        }
    }
}
