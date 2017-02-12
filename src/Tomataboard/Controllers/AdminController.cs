using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tomataboard.Models;
using Tomataboard.Models.AccountViewModels;
using Tomataboard.Models.EmailViewModels;
using Tomataboard.Services;

namespace Tomataboard.Controllers
{
    /// <summary>
    /// http://www.binaryintellect.net/articles/b957238b-e2dd-4401-bfd7-f0b8d984786d.aspx
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class AdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IViewRenderService viewRenderService,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory) : base(viewRenderService, emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PurgedAllCache()
        {
            return RedirectToAction("Index");
        }

        public IActionResult PurgedExpiredCache()
        {
            return RedirectToAction("Index");
        }
    }
}