using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Thalia.Services;
using Thalia.Services.Weather.Yahoo;

namespace Thalia.Controllers
{
    public class YahooController : Controller
    {
        private ILogger<YahooWeatherService> _yahooLogger;
        private IOptions<YahooWeatherKeys> _yahooWeatherKeys;

        #region private methods
        private void SaveToken(string key, OauthToken token)
        {
            HttpContext.Response.Cookies.Append(key + ".Token", token.Token ?? string.Empty);
            HttpContext.Response.Cookies.Append(key + ".Secret", token.Secret ?? string.Empty);
            HttpContext.Response.Cookies.Append(key + ".Verifier", token.Verifier ?? string.Empty);
        }

        private OauthToken LoadToken(string key)
        {
            return new OauthToken()
            {
                Token = HttpContext.Request.Cookies[key + ".Token"],
                Secret = HttpContext.Request.Cookies[key + ".Secret"],
                Verifier = HttpContext.Request.Cookies[key + ".Verifier"]
            };
        }
        #endregion

        public YahooController(ILogger<YahooWeatherService> yahooLogger, IOptions<YahooWeatherKeys> yahooWeatherKeys
)       {
            _yahooWeatherKeys = yahooWeatherKeys;
            _yahooLogger = yahooLogger;
        }
        
        public async Task<ActionResult> Authenticate()
        {
            var service = new YahooWeatherService(_yahooLogger, _yahooWeatherKeys);

            var token = await service.GetRequestToken(_yahooWeatherKeys.Value.ConsumerKey, _yahooWeatherKeys.Value.ConsumerSecret, _yahooWeatherKeys.Value.CallbackUrl);
            SaveToken("YahooRequestToken", token);

            var uri = service.GetAuthorizationUrl(token);

            return new RedirectResult(uri);
        }

        public async Task<ActionResult> Callback(string oauth_token, string oauth_verifier)
        {
            var service = new YahooWeatherService(_yahooLogger, _yahooWeatherKeys);

            var requestToken = LoadToken("YahooRequestToken");
            var accessToken =
                await
                    service.GetAccessToken(
                        new OauthToken() { Token = oauth_token, Secret = requestToken.Secret, Verifier = oauth_verifier }, 
                        _yahooWeatherKeys.Value.ConsumerKey,
                        _yahooWeatherKeys.Value.ConsumerSecret);

            if ((accessToken != null) && (!string.IsNullOrEmpty(accessToken.Token)))
            {
                SaveToken("AccessToken", accessToken);
                ViewBag.IsAuthenticated = 1;
            }
            else
            {
                ViewBag.IsAuthenticated = 0;
            }

            return View("Index");
        }
    }
}
