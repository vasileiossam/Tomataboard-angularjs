using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Thalia.Data.Entities;
using Thalia.Services;
using Thalia.Services.AccessTokens;
using Thalia.Services.Photos.Api500px;

namespace Thalia.Controllers
{
    public class Api500pxController : Controller
    {
        private readonly ILogger<Api500px> _api500pxLogger;
        private readonly IOptions<Api500pxKeys> _api500pxKeys;
        private readonly ICookiesService<OauthToken> _cookiesService;
        private readonly IAccessTokensRepository _accessTokensRepository;

        public Api500pxController(
            ILogger<Api500px> api500pxLogger, 
            IOptions<Api500pxKeys> api500pxKeys,
            ICookiesService<OauthToken>  cookiesService,
            IAccessTokensRepository accessTokensRepository
)       {
            _api500pxKeys = api500pxKeys;
            _api500pxLogger = api500pxLogger;
            _cookiesService = cookiesService;
            _accessTokensRepository = accessTokensRepository;
}
        
        public async Task<ActionResult> Authenticate()
        {
            var service = new Api500px(_api500pxLogger, _api500pxKeys);

            var token = await service.GetRequestToken(_api500pxKeys.Value.ConsumerKey, _api500pxKeys.Value.ConsumerSecret, _api500pxKeys.Value.CallbackUrl);
            _cookiesService.Save("Api500pxRequestToken", token);

            var uri = service.GetAuthorizationUrl(token);

            return new RedirectResult(uri);
        }

        public async Task<ActionResult> Callback(string oauth_token, string oauth_verifier)
        {
            var service = new Api500px(_api500pxLogger, _api500pxKeys);

            var requestToken = _cookiesService.Load("Api500pxRequestToken");
            var accessToken =
                await
                    service.GetAccessToken(
                        new OauthToken() { Token = oauth_token, Secret = requestToken.Secret, Verifier = oauth_verifier },
                        _api500pxKeys.Value.ConsumerKey,
                        _api500pxKeys.Value.ConsumerSecret);

            if (!string.IsNullOrEmpty(accessToken?.Token))
            {
                _accessTokensRepository.Add(service.GetType().Name, accessToken);
                RedirectToAction("Index", "Home");
            }

            return View("Error");
        }
    }
}
