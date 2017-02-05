namespace Tomataboard.Services.AccessTokens
{
    public interface IAccessTokensRepository
    {
        void Add(string service, OauthToken token);

        void Delete(string service);

        OauthToken Find(string service);
    }
}