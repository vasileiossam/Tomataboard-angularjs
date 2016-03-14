using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Thalia.Services
{
    /// <summary>
    /// OAuth 1.0
    /// http://oauth.googlecode.com/svn/code/javascript/example/signature.html
    /// </summary>
    public class OauthService
    {
        #region Protected Fields
        protected Dictionary<string, string> AuthorizationParameters;
        protected ILogger<OauthService> _logger;
        protected OauthToken _accessToken;
        protected const string OAuthSignatureMethod = "HMAC-SHA1";
        protected const string OAuthVersion = "1.0";
        protected string AccessUrl;
        protected string AuthorizeUrl;
        protected string RequestTokenUrl;
        #endregion

        #region Constructors
        public OauthService(ILogger<OauthService> logger)
        {
            _logger = logger;
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

        protected static Dictionary<string, string> ParseQueryString(String query)
        {
            Dictionary<string, string> queryDict = new Dictionary<string, string>();
            foreach (var token in query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = token.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    queryDict[parts[0].Trim()] = parts[1].Trim();
                }
            }
            return queryDict;
        }

        protected void Sign(string url, string tokenSecret1, string tokenSecret2, string requestType, string query)
        {
            // join the Oauth and query parameters in one dictionary

            // get Oauth params first
            var parameters = new Dictionary<string, string>(AuthorizationParameters);

            //  add the query params
            var queryParams = ParseQueryString(query);
            foreach (var param in queryParams)
            {
                if (!parameters.ContainsKey(param.Key)) parameters.Add(param.Key, param.Value);
            }

            // sorting all params is important
            var signatureParams = string.Join("&", parameters.OrderBy(x => x.Key).Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value ?? string.Empty)));
            var signatureBase = requestType + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(signatureParams);

            var hash = GetHash(tokenSecret1, tokenSecret2);
            var dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            var hashBytes = hash.ComputeHash(dataBuffer);

           // AuthorizationParameters.Add(OauthParameter.OauthSignature, Uri.EscapeDataString(Convert.ToBase64String(hashBytes)));
            AuthorizationParameters.Add(OauthParameter.OauthSignature,  Convert.ToBase64String(hashBytes));
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
                                                   (string.IsNullOrEmpty(key.Value)
                                                        ? string.Empty
                                                        : "=\"" + Uri.EscapeDataString(key.Value) + "\"")));
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

                _logger.LogError($"{GetType().Name}: Error posting request. Url: '{url}'. Status code: {response.StatusCode} Content: {content}");
            }

            return string.Empty;
        }

        protected async Task<HttpResponseMessage> GetRequest(string url, string parameters)
        {
            var oauthString = string.Empty;
            if (AuthorizationParameters != null)
            {
                //oauthString = "realm=\"" + url + "\", "; 
                oauthString += string.Join(", ",
                                               AuthorizationParameters.Select(
                                                   key =>
                                                   key.Key +
                                                   (string.IsNullOrEmpty(key.Value)
                                                        ? string.Empty
                                                        : "=\"" + key.Value + "\"")));
                AuthorizationParameters.Clear();
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", oauthString);
                return await client.GetAsync(new Uri(url + "?" + parameters, UriKind.Absolute));
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
                }
            }

            return token;
        }
        #endregion
        
        #region Public Methods 
        public async Task<OauthToken> GetRequestToken(string consumerKey, string consumerSecret, string callbackUrl)
        {
            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthCallback, callbackUrl},
                {OauthParameter.OauthConsumerKey, consumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthVersion, OAuthVersion}
            };

            Sign(RequestTokenUrl, consumerSecret, string.Empty, "POST", "");
            var response = await PostRequest(RequestTokenUrl);
            return ParseReponse(response);
        }

        public async Task<OauthToken> GetAccessToken(OauthToken token, string consumerKey, string consumerSecret)
        {
            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthConsumerKey, consumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthToken, token.Token},
                {OauthParameter.OauthVerifier, token.Verifier},
                {OauthParameter.OauthVersion, OAuthVersion}
             };

            Sign(AccessUrl, consumerSecret, token.Secret, "POST", "");
            var response = await PostRequest(AccessUrl);
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
