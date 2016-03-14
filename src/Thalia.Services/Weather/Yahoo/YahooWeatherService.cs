using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Thalia.Services.Extensions;

namespace Thalia.Services.Weather.Yahoo
{
    /// <summary>
    /// https://developer.yahoo.com/weather/
    /// </summary>
    public class YahooWeatherService : IYahooWeatherService
    {
        #region Private Fields
        private readonly IOptions<YahooWeatherKeys> _keys;
        private readonly ILogger<YahooWeatherService> _logger;
        #endregion

        /// <summary>
        /// access is limited to 2,000 signed calls per day  
        /// </summary>
        public Quota Quota => new Quota() { Requests = 2000, Time = TimeSpan.FromDays(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public YahooWeatherService(ILogger<YahooWeatherService> logger, IOptions<YahooWeatherKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        //public async Task<List<Photo>> Execute(string parameters)
        //{
        //    // "term=inspire&rpp=30
        //    try
        //    {
        //        AuthorizationParameters = new Dictionary<string, string>()
        //        {
        //            {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
        //            {OauthParameter.OauthNonce, GetNonce()},
        //            {OauthParameter.OauthSignatureMethod, OAuthSignatureMethod},
        //            {OauthParameter.OauthTimestamp, GetTimeStamp()},
        //            {OauthParameter.OauthToken, _accessToken.Token},
        //            {OauthParameter.OauthVersion, OAuthVersion}
        //        };

        //        var queryParams = new Dictionary<string, string>()
        //        {
        //            {"term", parameters},
        //            {"rpp", "30" }
        //        };
        //        var queryString = string.Join("&", queryParams.Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value)));

        //        var url = "https://api.500px.com/v1/photos/search";
        //        Sign(url, _keys.Value.ConsumerSecret, _accessToken.Secret, "GET", queryString);
        //        var response = await GetRequest(url, queryString);
        //        var content = await response.Content.ReadAsStringAsync();

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return GetResult(content);
        //        }

        //        _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Exception: " + ex.GetError());
        //    }

        //    return null;
        //}


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

        private WeatherConditions GetResult(string json)
        {
            throw new NotImplementedException();
        }

        Task<WeatherConditions> IServiceOperation<WeatherConditions>.Execute(string parameters)
        {
            throw new NotImplementedException();
        }
    }
}
