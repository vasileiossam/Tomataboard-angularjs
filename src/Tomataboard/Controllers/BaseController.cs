using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tomataboard.Models.EmailViewModels;
using Tomataboard.Services;

namespace Tomataboard.Controllers
{
    public class BaseController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly IEmailSender _emailSender;

        public BaseController(IViewRenderService viewRenderService, IEmailSender emailSender)
        {
            _viewRenderService = viewRenderService;
            _emailSender = emailSender;
        }

        public async Task SendEmailAsync(string email, string viewName, MessageViewModel messageViewModel)
        {
            var emailMessage = await _viewRenderService.RenderAsync(viewName, messageViewModel);
            emailMessage.Email = email;
            await _emailSender.SendEmailAsync(emailMessage);
        }
    }
}