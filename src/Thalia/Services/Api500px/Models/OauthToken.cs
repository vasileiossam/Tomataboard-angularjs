namespace Thalia.Services.Api500.Models
{
    public class OauthToken
    {
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Verifier { get; set; }
    }
}
