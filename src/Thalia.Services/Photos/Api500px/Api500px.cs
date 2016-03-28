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
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Thalia.Data.Entities;
using Thalia.Services.Photos.Api500px.Contracts;
using Thalia.Services.Photos.Api500px.Models;
using Thalia.Services.Extensions;

// ReSharper disable InconsistentNaming

namespace Thalia.Services.Photos.Api500px
{
    /// <summary>
    /// https://github.com/500px/api-documentation
    /// https://apigee.com/vova/embed/console/api500px
    /// OAuth 1.0a
    ///
    /// Good ideas here: http://www.rahulpnath.com/blog/exploring-oauth-c-and-500px/
    /// To check the signature: http://oauth.googlecode.com/svn/code/javascript/example/signature.html
    /// </summary>
    public class Api500px  : OauthService, IApi500px
    {
        /// <summary>
        /// https://github.com/500px/api-documentation
        /// 1,000,000 requesters per month 
        /// </summary>
        public Quota Quota => new Quota() { Requests = 1000000 / 30, Time = TimeSpan.FromDays(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(6);
        
        #region Constructors
        public Api500px(ILogger<Api500px> logger, IOptions<Api500pxKeys> keys)
            :base(logger)
        {
            _logger = logger;
            _keys = keys;
            AccessTokenUrl = "https://api.500px.com/v1/oauth/access_token";
            AuthorizeUrl = "https://api.500px.com/v1/oauth/authorize";
            RequestTokenUrl = "https://api.500px.com/v1/oauth/request_token";
            AlwaysEscapeSignature = true;
        }
        #endregion

        private static async Task<T> DeserializeContent<T>(string content) where T : Response, new()
        {
            var response = JsonConvert.DeserializeObject<T>(content) ?? new T();
            response.Content = content;
            return response;
        }

        private static async Task<T> DeserializeResponse<T>(HttpResponseMessage httpResponse) where T : Response, new()
        {
            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<T>(content) ?? new T();

            response.Content = content;
            response.IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
            response.StatusCode = httpResponse.StatusCode;

            if (!httpResponse.IsSuccessStatusCode)
            {
                //   Debug.WriteLine("HttpResponseMessage failed: " + httpResponse + "\r\n -- Content: " + content + ((!string.IsNullOrWhiteSpace(response.Error)) ? "\r\n -- Error: " + response.Error : string.Empty));
            }

            return response;
        }

        #region Public Methods

        /// <summary>
        /// All subsequent request to any protected resource needs the AccessToken and should be 
        /// signed using ConsumerKey and the access token's secret code.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<GetPhotosResponse> Photos(string parameters)
        {
            var url = "https://api.500px.com/v1/photos";

            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthToken, AccessToken.Token},
                {OauthParameter.OauthVersion, OAuthVersion}
            };

            Sign(url, _keys.Value.ConsumerSecret, AccessToken.Secret, "GET", parameters);
            var content = await GetRequest(url, ParseQueryString(parameters));
            return await DeserializeContent<GetPhotosResponse>(content);
        }

        public async Task<GetPhotosResponse> Search(string parameters)
        {
            AuthorizationParameters = new Dictionary<string, string>()
            {
                {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                {OauthParameter.OauthNonce, GetNonce()},
                {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                {OauthParameter.OauthTimestamp, GetTimeStamp()},
                {OauthParameter.OauthToken, AccessToken.Token},
                {OauthParameter.OauthVersion, OAuthVersion}
            };

            var url = "https://api.500px.com/v1/photos/search";

            Sign(url, _keys.Value.ConsumerSecret, AccessToken.Secret, "GET", parameters);
            var content = await GetRequest(url, ParseQueryString(parameters));
            return await DeserializeContent<GetPhotosResponse>(content);
        }

        public async Task<List<Photo>> Execute(string parameters)
        {
            // "term=inspire&rpp=30
            try
            {
                AuthorizationParameters = new Dictionary<string, string>()
                {
                    {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                    {OauthParameter.OauthNonce, GetNonce()},
                    {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                    {OauthParameter.OauthTimestamp, GetTimeStamp()},
                    {OauthParameter.OauthToken, AccessToken.Token},
                    {OauthParameter.OauthVersion, OAuthVersion}
                };

                var queryParams = new Dictionary<string, string>()
                {
                    {"term", parameters},
                    {"rpp", "30" }
                };

                var url = "https://api.500px.com/v1/photos/search";
                Sign(url, _keys.Value.ConsumerSecret, AccessToken.Secret, "GET", queryParams);
                var content = await GetRequest(url, queryParams);
                return GetResult(content);

                //_logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
                //return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }

        private List<Photo> GetResult(string json)
        {
            var photosResponse = JsonConvert.DeserializeObject<GetPhotosResponse>(json);
            if (photosResponse?.Photos == null) return null;

            var photos = new List<Photo>();
            foreach (var item in photosResponse.Photos)
            {
                var url = item.GetPhotoUrl();
                if (string.IsNullOrEmpty(url)) continue;

                var photo = new Photo()
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Description = item.Description,
                    Created = item.Created,
                    AuthorName = item.User.FullName,
                    AuthorCountry = item.User.County,
                    AuthorId = item.User.Id,
                    AuthorUsername = item.User.UserName,
                    Location = item.Location,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Favorites = item.Favorites,
                    Likes = item.Likes,
                    Rating = item.Rating,
                    Views = item.TimesViewed,
                    Url = url,
                    AuthorUrl = item.GetAuthorUrl(),
                    Service = "500px"
                };
                photos.Add(photo);
            }
            return photos;
        }
        #endregion
    }
}
