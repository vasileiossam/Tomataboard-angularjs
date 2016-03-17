using Thalia.Data.Entities;

namespace Thalia.Services.AccessTokens
{
    public interface IAccessTokensRepository
    {
        void Add(string service, OauthToken token);
        void Delete(string service);
        AccessToken Find(string service);
    }
}
