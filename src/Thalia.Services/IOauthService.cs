using System.Threading.Tasks;

namespace Thalia.Services
{
    public interface IOauthService
    {
        Task<OauthToken> GetRequestToken();
        string GetAuthorizationUrl(OauthToken token);
        Task<OauthToken> GetAccessToken(OauthToken token);
    }
}
