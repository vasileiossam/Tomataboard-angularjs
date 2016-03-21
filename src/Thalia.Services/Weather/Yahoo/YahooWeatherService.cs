using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Thalia.Services.Extensions;
using Thalia.Services.Locations;
using System.Linq;

namespace Thalia.Services.Weather.Yahoo
{
    /// <summary>
    /// https://developer.yahoo.com/weather/
    /// https://developer.yahoo.com/apps/
    /// https://developer.yahoo.com/oauth/guide/
    /// </summary>
    public class YahooWeatherService : OauthService, IYahooWeatherService
    {
        /// <summary>
        /// access is limited to 2,000 signed calls per day  
        /// </summary>
        public Quota Quota => new Quota() { Requests = 2000, Time = TimeSpan.FromDays(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(1);

        #region Constructors
        public YahooWeatherService(
            ILogger<YahooWeatherService> logger, 
            IOptions<YahooWeatherServiceKeys> keys)
            :base(logger)
        {
            _logger = logger;
            _keys = keys;

            RequestTokenUrl = "https://api.login.yahoo.com/oauth/v2/get_request_token";
            AuthorizeUrl = "https://api.login.yahoo.com/oauth/v2/request_auth";
            AccessTokenUrl = "https://api.login.yahoo.com/oauth/v2/get_token";
        }
        #endregion

        private Dictionary<string, string> GetQueryParameters(Location location)
        {
            // use city and country names e.g "Melbourne, AUS";
            var country = string.IsNullOrEmpty(location.CountryCode) ? location.Country : location.CountryCode;
            var cityName = $"{location.City},{location.StateCode},{country}".Replace(",,", "");
            return new Dictionary<string, string>
            {
                {"q", $"select item from weather.forecast where woeid in (select woeid from geo.places(1) where text='{cityName}')"},
                {"format", "json" }
            };
        }

        public async Task<WeatherConditions> Execute(string parameters)
        {
            var location = JsonConvert.DeserializeObject<Location>(parameters);
            if (location == null)
            {
                _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Cannot deserialize parameters");
                return null;
            }

            try
            {
                // this is the only way that I made YQL to work; the consumer secret is used as signature,
                // the signature method is PLAINTEXT and an & is appended in the end of the signature,
                // access token is not needed in the oauth parameters.
                AuthorizationParameters = new Dictionary<string, string>()
                {
                    {OauthParameter.OauthConsumerKey, _keys.Value.ConsumerKey},
                    {OauthParameter.OauthNonce, GetNonce()},
                    {OauthParameter.OauthSignatureMethod, "PLAINTEXT"},
                    {OauthParameter.OauthTimestamp, GetTimeStamp()},
                    {OauthParameter.OauthVersion, OAuthVersion},
                    {OauthParameter.OauthSignature, _keys.Value.ConsumerSecret + "&"}
                };

                var url = "https://query.yahooapis.com/v1/yql";
                var content = await GetRequest(url, GetQueryParameters(location));

                var weatherDto = GetResult(content);
                if (weatherDto != null)
                {
                    return weatherDto;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Cannot get weather for '{parameters}'. Exception: " + ex.GetError());
            }

            return null;
        }
    

        private WeatherConditions GetResult(string json)
        {
            var weatherDto = JsonConvert.DeserializeObject<WeatherDto>(json);
            if (weatherDto?.query?.results?.channel?.item?.condition == null) return null;

            var condition = weatherDto.query.results.channel.item.condition;
            
            // Icon.Code = 3200: not available
            if (condition.code == "3200") return null;

            double temperatureF;
            double.TryParse(condition.temp, out temperatureF);

            var weatherConditions = new WeatherConditions()
            {
                Title = condition.text,
                Description = condition.text,
                TemperatureC = (int)Math.Ceiling(TemperatureConverter.FahrenheitToCelsius(temperatureF)),
                TemperatureF = (int) temperatureF,
                Icon = Icons.GetCssClass(condition.code)
            };
            
            return weatherConditions;
        }
    }
}
