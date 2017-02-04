using System.Threading.Tasks;
using Tomataboard.Services.Mailer;

namespace Tomataboard.Services
{
    public interface IViewRenderService
    {
        Task<EmailMessage> RenderAsync(string viewName, object model);
    }
}
