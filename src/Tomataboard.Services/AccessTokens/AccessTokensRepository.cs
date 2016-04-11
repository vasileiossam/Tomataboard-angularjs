using System;
using Tomataboard.Services.Encryption;

namespace Tomataboard.Services.AccessTokens
{
    public class AccessTokensRepository : IAccessTokensRepository
    {
        private readonly TomataboardContext _context;
        private readonly IEncryptionService _encryptor;

        public AccessTokensRepository(TomataboardContext context, IEncryptionService encryptor)
        {
            _context = context;
            _encryptor = encryptor;
        }

        public void Add(string service, OauthToken token)
        {
            Delete(service);

            _context.Add(new AccessToken()
            {
                Service = service,
                Token = _encryptor.Encrypt(token.Token),
                Secret = _encryptor.Encrypt(token.Secret),
                Created = DateTime.Now,
                Expires = token.Expires,
                SessionHandle = _encryptor.Encrypt(token.SessionHandle)
            });
            _context.SaveChanges();
        }

        public void Delete(string service)
        {
            var token = _context.AccessTokens.FirstOrDefault(x => x.Service == service);
            if (token == null) return;
            _context.Remove(token);
            _context.SaveChanges();
        }

        public OauthToken Find(string service)
        {
            var token = _context.AccessTokens.FirstOrDefault(x => x.Service == service);
            if (token == null) return null;

            return new OauthToken()
            {
                Token = _encryptor.Decrypt(token.Token),
                Secret = _encryptor.Decrypt(token.Secret),
                Expires = token.Expires,
                SessionHandle = _encryptor.Decrypt(token.SessionHandle)
            };
        }
    }
}
