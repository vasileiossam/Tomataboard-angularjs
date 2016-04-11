using System.Threading.Tasks;
using Tomataboard.Services;

namespace Tomataboard.Services
{
    public interface IOauthService
    {
        Task<OauthToken> GetRequestToken();
        string GetAuthorizationUrl(OauthToken token);
        Task<OauthToken> GetAccessToken(OauthToken token);
    }
}
