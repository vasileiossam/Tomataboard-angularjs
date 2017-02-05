using System.Threading.Tasks;
using Tomataboard.Services.Mailer;

namespace Tomataboard.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}