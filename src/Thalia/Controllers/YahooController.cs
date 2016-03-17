using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Thalia.Data.Entities;
using Thalia.Services;
using Thalia.Services.AccessTokens;
using Thalia.Services.Weather.Yahoo;

namespace Thalia.Controllers
{
    public class YahooController : Controller
    {
        private readonly ILogger<YahooWeatherService> _yahooLogger;
        private readonly IOptions<YahooWeatherKeys> _yahooWeatherKeys;
        private readonly ICookiesService<OauthToken> _cookiesService;
        private readonly IAccessTokensRepository _accessTokensRepository;

        public YahooController(
            ILogger<YahooWeatherService> yahooLogger, 
            IOptions<YahooWeatherKeys> yahooWeatherKeys,
            ICookiesService<OauthToken>  cookiesService,
            IAccessTokensRepository accessTokensRepository
)       {
            _yahooWeatherKeys = yahooWeatherKeys;
            _yahooLogger = yahooLogger;
            _cookiesService = cookiesService;
            _accessTokensRepository = accessTokensRepository;
}
        
        public async Task<ActionResult> Authenticate()
        {
            var service = new YahooWeatherService(_yahooLogger, _yahooWeatherKeys);

            var token = await service.GetRequestToken(_yahooWeatherKeys.Value.ConsumerKey, _yahooWeatherKeys.Value.ConsumerSecret, _yahooWeatherKeys.Value.CallbackUrl);
            _cookiesService.Save("YahooRequestToken", token);

            var uri = service.GetAuthorizationUrl(token);

            return new RedirectResult(uri);
        }

        public async Task<ActionResult> Callback(string oauth_token, string oauth_verifier)
        {
            var service = new YahooWeatherService(_yahooLogger, _yahooWeatherKeys);

            var requestToken = _cookiesService.Load("YahooRequestToken");
            var accessToken =
                await
                    service.GetAccessToken(
                        new OauthToken() { Token = oauth_token, Secret = requestToken.Secret, Verifier = oauth_verifier }, 
                        _yahooWeatherKeys.Value.ConsumerKey,
                        _yahooWeatherKeys.Value.ConsumerSecret);

            if (!string.IsNullOrEmpty(accessToken?.Token))
            {
                _accessTokensRepository.Add(service.GetType().Name, accessToken);
                RedirectToAction("Index", "Home");
            }

            return View("Error");
        }
    }
}
