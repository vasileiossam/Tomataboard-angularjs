using System.Threading.Tasks;

namespace Tomataboard.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
