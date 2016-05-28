using Newtonsoft.Json;
using Tomataboard.Services.Encryption;
using Microsoft.AspNetCore.Http;

namespace Tomataboard.Services
{
    public class CookiesService<T> : ICookiesService<T>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IEncryptionService _encryptor;

        public CookiesService(IHttpContextAccessor httpContextAccessor, IEncryptionService encryptor)
        {
            _accessor = httpContextAccessor;
            _encryptor = encryptor;
        }
        public void Save(string key, T entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var cookieKey = _encryptor.Encrypt(key);
            _accessor.HttpContext.Response.Cookies.Append(cookieKey, _encryptor.Encrypt(json));
        }

        public T Load(string key)
        {
            var cookieKey = _encryptor.Encrypt(key);
            var json = _accessor.HttpContext.Request.Cookies[cookieKey];
            if (string.IsNullOrEmpty(json)) return default(T);

            json = _encryptor.Decrypt(json);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
