using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tomataboard.Services;
using Tomataboard.Services.AccessTokens;

namespace Tomataboard.Controllers
{
    [Authorize(Roles = "Administrator")]
    public abstract class OauthController : Controller
    {
        protected string _cookieKey;
        protected readonly ILogger<IOauthService> _logger;
        protected readonly ICookiesService<OauthToken> _cookiesService;
        protected readonly IAccessTokensRepository _accessTokensRepository;
        protected readonly IOauthService _service;

        public OauthController(
            ILogger<IOauthService> logger,
            IOauthService service,
            ICookiesService<OauthToken> cookiesService,
            IAccessTokensRepository accessTokensRepository)
        {
            _logger = logger;
            _service = service;
            _cookiesService = cookiesService;
            _accessTokensRepository = accessTokensRepository;
        }

        public IActionResult Index()
        {
            var accessToken = _accessTokensRepository.Find(_service.GetType().Name);
            ViewData["Token"] = accessToken;
            ViewData["Title"] = _service.GetType().Name;
            return View("~/Views/Oauth/Index.cshtml");
        }

        public async Task<ActionResult> Authenticate()
        {
            try
            {
                var token = await _service.GetRequestToken();
                _cookiesService.Save(_cookieKey, token);

                var uri = _service.GetAuthorizationUrl(token);
                return new RedirectResult(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        public async Task<ActionResult> Callback(string oauth_token, string oauth_verifier)
        {
            try
            {
                var requestToken = _cookiesService.Load(_cookieKey);
                var accessToken = await _service.GetAccessToken(
                        new OauthToken() { Token = oauth_token, Secret = requestToken.Secret, Verifier = oauth_verifier });

                _accessTokensRepository.Add(_service.GetType().Name, accessToken);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        public abstract Task<ActionResult> Execute();

        public abstract Task<ActionResult> Refresh();
    }
}