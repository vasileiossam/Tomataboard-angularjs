using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Tomataboard.Services;
using Tomataboard.Services.AccessTokens;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Tomataboard.Services
{
    /// <summary>
    /// OAuth 1.0
    /// http://oauth.googlecode.com/svn/code/javascript/example/signature.html
    /// </summary>
    public class OauthService : IOauthService
    {
        #region Protected Fields
        protected Dictionary<string, string> AuthorizationParameters;
        protected ILogger<OauthService> _logger;
        protected IOptions<OauthKeys> _keys;
        protected IAccessTokensRepository _accessTokensRepository;
        protected const string OAuthSignatureMethod = "HMAC-SHA1";
        protected const string OAuthVersion = "1.0";
        protected string AccessTokenUrl;
        protected string AuthorizeUrl;
        protected string RequestTokenUrl;
        protected bool AlwaysEscapeSignature;
        #endregion

        #region Constructors
        public OauthService(ILogger<OauthService> logger, IAccessTokensRepository accessTokensRepository)
        {
            _logger = logger;
            _accessTokensRepository = accessTokensRepository;
        }
        #endregion

        #region Protected Methods        
        protected static string GetNonce()
        {
            var rand = new Random();
            var nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        protected static string GetTimeStamp()
        {
            var sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(sinceEpoch.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        protected static Dictionary<string, string> ParseQueryString(string query)
        {
            var queryDict = new Dictionary<string, string>();
            foreach (var token in query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = token.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    queryDict[parts[0].Trim()] = parts[1].Trim();
                }
                // yahoo queries: q="select item.condition from weather.forecast where woeid in (select woeid from geo.places(1) where text='{parameters}'")
                if (parts.Length == 3)
                {
                    queryDict[parts[0].Trim()] = parts[1].Trim() + "=" + parts[2].Trim();
                }
            }
            return queryDict;
        }

        protected void Sign(string url, string tokenSecret1, string tokenSecret2, string requestType, Dictionary<string, string> query)
        {
            // join the Oauth and query parameters in one dictionary

            // get Oauth params first
            var parameters = new Dictionary<string, string>(AuthorizationParameters);
            
            //  add the query params
            foreach (var param in query)
            {
                if (!parameters.ContainsKey(param.Key)) parameters.Add(param.Key, param.Value);
            }

            // sorting all params is important
            var signatureParams = string.Join("&", parameters.OrderBy(x => x.Key).Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value ?? string.Empty)));

            // build signature base
            var signatureBase = requestType + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(signatureParams);

            var hash = GetHash(tokenSecret1, tokenSecret2);
            var dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            var hashBytes = hash.ComputeHash(dataBuffer);

            AuthorizationParameters.Add(OauthParameter.OauthSignature, Convert.ToBase64String(hashBytes));
        }

        protected void Sign(string url, string tokenSecret1, string tokenSecret2, string requestType, string query)
        {
            Sign(url, tokenSecret1, tokenSecret2, requestType, ParseQueryString(query));
        }

        protected HashAlgorithm GetHash(string tokenSecret1, string tokenSecret2)
        {
            var keystring = $"{Uri.EscapeDataString(tokenSecret1)}&{Uri.EscapeDataString(tokenSecret2)}";

            var hmacsha1 = new HMACSHA1
            {
                Key = Encoding.ASCII.GetBytes(keystring)
            };
            return hmacsha1;
        }

        protected async Task<string> PostRequest(string url)
        {
            var oauthString = string.Empty;
            if (AuthorizationParameters != null)
            {
                oauthString = string.Join(", ",
                                               AuthorizationParameters.Select(
                                                   key =>
                                                   key.Key +
                                                   (string.IsNullOrEmpty(key.Value) ? string.Empty : "=\"" + Uri.EscapeDataString(key.Value) + "\"")));
                AuthorizationParameters.Clear();
            }


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", oauthString);
                var response = await client.PostAsync(new Uri(url, UriKind.Absolute), null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return content;
                }

                throw new HttpRequestException($"{GetType().Name}: Error posting request. Url: '{url}'. Status code: {response.StatusCode} Content: {content}");
            }
        }

        protected async Task<string> GetRequest(string url, Dictionary<string, string> parameters)
        {
            var oauthString = string.Empty;
            if (AuthorizationParameters != null)
            {
                oauthString += string.Join(", ",
                                               AuthorizationParameters.Select(
                                                   key =>
                                                   key.Key + 
                                                   (string.IsNullOrEmpty(key.Value) ? string.Empty : "=\"" + Uri.EscapeDataString(key.Value) + "\"")));
                AuthorizationParameters.Clear();
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", oauthString);

                var queryString = string.Join("&", parameters.Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value)));
                var response = await client.GetAsync(new Uri(url + "?" + queryString, UriKind.Absolute));
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return content;
                }

                throw new HttpRequestException($"{GetType().Name}: Error getting request. Url: '{url}?{queryString}'. Status code: {response.StatusCode} Content: {content}");
            }
        }

        protected OauthToken ParseReponse(string response)
        {
            var token = new OauthToken();
            if (string.IsNullOrEmpty(response)) return token;

            //  "oauth_token=qWnfRDBG47T57ud3peQeYtWHjoGSlbYtAwboQZuD&oauth_token_secret=iZJG3SKMA8BJexk4iEtQTwZmbstfR1TqoY1yKBo6&oauth_callback_confirmed=true"    string
            var keyValPairs = response.Split('&');
            foreach (var splits in keyValPairs.Select(t => t.Split('=')))
            {
                switch (splits[0])
                {
                    case "oauth_token":
                        token.Token = splits[1];
                        break;
                    case "oauth_token_secret":
                        token.Secret = splits[1];
                        break;
                    case "oauth_expires_in":
                        int seconds;
                        if (int.TryParse(splits[1], out seconds))
                            token.Expires = DateTime.Now.AddSeconds(seconds);
                        break;
                    case "oauth_session_handle":
                        token.SessionHandle = splits[1];
                        break;
                }
            }

            return token;
        }
        #endregion
        
        #region Public Methods 
        public async Task<OauthToken> GetRequestToken()
        {
            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthCallback, _keys.Value.CallbackUrl},
                {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthVersion, OAuthVersion}
            };

            Sign(RequestTokenUrl, _keys.Value.ConsumerSecret, string.Empty, "POST", "");
            var response = await PostRequest(RequestTokenUrl);
            return ParseReponse(response);
        }

        public async Task<OauthToken> GetAccessToken(OauthToken token)
        {
            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthToken, token.Token},
                {OauthParameter.OauthVerifier, token.Verifier},
                {OauthParameter.OauthVersion, OAuthVersion}
             };

            Sign(AccessTokenUrl, _keys.Value.ConsumerSecret, token.Secret, "POST", "");
            var response = await PostRequest(AccessTokenUrl);
            return ParseReponse(response);
        }
        
        /// <summary>
        /// A callback is needed to get back oauth_token and oauth_verifier
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetAuthorizationUrl(OauthToken token)
        {
            return AuthorizeUrl + "?oauth_token=" + token.Token;
        }
        #endregion
    }
}
