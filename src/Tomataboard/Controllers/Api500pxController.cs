using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tomataboard.Services;
using Tomataboard.Services.AccessTokens;
using Tomataboard.Services.Photos.Api500px;

namespace Tomataboard.Controllers
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