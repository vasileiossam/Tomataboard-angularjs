using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Thalia.Services;
using Thalia.Services.AccessTokens;
using Thalia.Services.Photos.Api500px;

namespace Thalia.Controllers
{
    public class Api500pxController : OauthController
    {
        public Api500pxController(
            ILogger<Api500px> logger,
            IApi500px service,
            ICookiesService<OauthToken> cookiesService,
            IAccessTokensRepository accessTokensRepository) : base(logger, service, cookiesService, accessTokensRepository)
        {
            _cookieKey = "Api500pxRequestToken";
        }

        public override async Task<ActionResult> Execute()
        {
            try
            {
                var service = _service as Api500px;
                if (service != null)
                {
                    var photos = await service.Execute("Greece");
                    return View(photos);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
            return View("Error");
        }

        public override Task<ActionResult> Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
