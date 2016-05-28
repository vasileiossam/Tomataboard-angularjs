using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tomataboard.Services;
using Tomataboard.Services.Extensions;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Photos.Api500px;
using Tomataboard.Services.Photos.Api500px.Contracts;
using Tomataboard.Services.AccessTokens;
using Microsoft.Extensions.Options;

// ReSharper disable InconsistentNaming

namespace Tomataboard.Services.Photos.Api500px
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

        private OauthToken _accessToken;
        private OauthToken AccessToken => _accessToken ?? (_accessToken = _accessTokensRepository.Find(GetType().Name));
        private readonly Random _rnd = new Random();

        #region Constructors
        public Api500px(
            ILogger<Api500px> logger,
            IOptions<Api500pxKeys> keys,
            IAccessTokensRepository accessTokensRepository)
            : base(logger, accessTokensRepository)
        {
            _logger = logger;
            _keys = keys;
            AccessTokenUrl = "https://api.500px.com/v1/oauth/access_token";
            AuthorizeUrl = "https://api.500px.com/v1/oauth/authorize";
            RequestTokenUrl = "https://api.500px.com/v1/oauth/request_token";
            AlwaysEscapeSignature = true;
        }
        #endregion

        #region Public Methods

        private async Task<int> GetTotalPages(Dictionary<string, string> oauthParams, Dictionary<string, string> queryParams, string url)
        {
            AuthorizationParameters = new Dictionary<string, string>(oauthParams);
            Sign(url, _keys.Value.ConsumerSecret, AccessToken.Secret, "GET", queryParams);
            var json = await GetRequest(url, queryParams);
            var photosResponse = JsonConvert.DeserializeObject<GetPhotosResponse>(json);
            return photosResponse?.Photos == null ? 0 : photosResponse.TotalPages;
        }

        /// <summary>
        /// https://github.com/500px/api-documentation/blob/master/endpoints/photo/GET_photos.md
        /// </summary>
        /// <param name="category">Case sensitive, separate multiple values with a comma.</param>
        /// <returns></returns>
        private async Task<List<Photo>> GetPopularPhotos(string category)
        {
            var url = "https://api.500px.com/v1/photos";

            var oauthParams = new Dictionary<string, string>
            {
                    {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                    {OauthParameter.OauthNonce, ""},
                    {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
                    {OauthParameter.OauthTimestamp, GetTimeStamp()},
                    {OauthParameter.OauthToken, AccessToken.Token},
                    {OauthParameter.OauthVersion, OAuthVersion}
            };

            var queryParams = new Dictionary<string, string>
            {
                { "feature", "popular"},
                { "only", category },
                { "page", "" },
                { "rpp", "100" },
                { "image_size", "1080,1600,2048" }
            };

            oauthParams[OauthParameter.OauthNonce] = GetNonce();
            queryParams["page"] = "1";
            var totalPages = await GetTotalPages(oauthParams, queryParams, url);

            oauthParams[OauthParameter.OauthNonce] = GetNonce();
            queryParams["page"] = GetRandomPageNumber(totalPages).ToString();
            AuthorizationParameters = oauthParams;

            Sign(url, _keys.Value.ConsumerSecret, AccessToken.Secret, "GET", queryParams);
            var json = await GetRequest(url, queryParams);
            return GetResult(json);
        }
        
        public async Task<List<Photo>> Execute(string parameters)
        {
            try
            {
                if (AccessToken == null)
                {
                    throw new Exception("Where is the AccessToken?");
                }

                // ignore the parameters for now and always return Landscapes
                return await GetPopularPhotos("Landscapes");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }

        private int GetRandomPageNumber(int totalPages)
        {
            if (totalPages == 0) return 1;
            return _rnd.Next(1, totalPages + 1);
        }

        private List<Photo> GetResult(string json)
        {
            var photosResponse = JsonConvert.DeserializeObject<GetPhotosResponse>(json);
            if (photosResponse?.Photos == null) return null;

            var photos = new List<Photo>();
            foreach (var item in photosResponse.Photos)
            {
                var url = item.GetUrl();
                if (string.IsNullOrEmpty(url)) continue;
                if (item.Views == 0) continue;

                var photo = new Photo
                {
                    Service = "500px",
                    Name = item.Name.LimitTo(PhotoConstants.MaxNameLength),
                    AuthorName = item.User.FullName,
                    //AuthorProfilePage = item.GetAuthorProfilePage(),
                    PhotoPage = item.GetPhotoPage(),
                    Url = url
                };

                photos.Add(photo);
            }

            return photos.Count == 0 ? null : photos;
        }
        #endregion
    }
}
