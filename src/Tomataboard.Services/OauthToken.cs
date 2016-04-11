using System;

namespace Tomataboard.Services
{
    public class OauthToken
    {
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Verifier { get; set; }
        public DateTime? Expires { get; set; }
        public string SessionHandle { get; set; }

        public bool IsExpired => (Expires != null) && (Expires < DateTime.Now);
    }
}
