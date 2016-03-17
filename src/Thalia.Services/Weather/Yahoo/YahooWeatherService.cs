using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Thalia.Data.Entities;
using Thalia.Services.Extensions;

namespace Thalia.Services.Weather.Yahoo
{
    /// <summary>
    /// https://developer.yahoo.com/weather/
    /// https://developer.yahoo.com/apps/
    /// https://developer.yahoo.com/oauth/guide/
    /// </summary>
    public class YahooWeatherService : OauthService, IYahooWeatherService
    {
        #region Private Fields
        private readonly IOptions<YahooWeatherKeys> _keys;
        private readonly OauthToken _accessToken;
        #endregion

        /// <summary>
        /// access is limited to 2,000 signed calls per day  
        /// </summary>
        public Quota Quota => new Quota() { Requests = 2000, Time = TimeSpan.FromDays(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public YahooWeatherService(ILogger<YahooWeatherService> logger, IOptions<YahooWeatherKeys> keys)
            :base(logger)
        {
            _logger = logger;
            _keys = keys;
            _accessToken = new OauthToken() { Token = _keys.Value.AccessToken, Secret = _keys.Value.AccessSecret };
            AccessUrl = "https://api.login.yahoo.com/oauth/v2/get_token";
            AuthorizeUrl = "https://api.login.yahoo.com/oauth/v2/request_auth";
            RequestTokenUrl = "https://api.login.yahoo.com/oauth/v2/get_request_token";

        }
        public YahooWeatherService(ILogger<YahooWeatherService> logger, IOptions<YahooWeatherKeys> keys, AccessToken accessToken) : this(logger, keys)
        {
            _accessToken = new OauthToken() { Token = accessToken.Token, Secret = accessToken.Secret };
        }
        #endregion

        public async Task<WeatherConditions> Execute(string parameters)
        {
            try
            {
                var queryString = $"q=select item.condition from weather.forecast where woeid in (select woeid from geo.places(1) where text='{parameters}')&format=json";
                //var url = "https://query.yahooapis.com/v1/public/yql";
                var url = "https://query.yahooapis.com/v1/yql";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url + "?" + queryString, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var photosResponse = 0;// JsonConvert.DeserializeObject<GetPhotosResponse>(content);
                        if (photosResponse == null) return null;

                        //if (photosResponse.Stat == "ok")
                        //{
                            //return GetResult(content);
                        //}

                        //_logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Stat: {photosResponse.Stat}, Code: {photosResponse.Code}, Message: {photosResponse.Message}");
                    }

                    //_logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }
    }
}
