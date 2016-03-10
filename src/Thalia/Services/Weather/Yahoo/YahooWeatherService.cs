using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Thalia.Extensions;

namespace Thalia.Services.Weather.Yahoo
{
    /// <summary>
    /// https://developer.yahoo.com/weather/
    /// OAuth 2.0
    /// </summary>
    public class YahooWeatherService
    {
        #region Private Fields
        private readonly IOptions<YahooWeatherKeys> _keys;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public YahooWeatherService(ILogger logger, IOptions<YahooWeatherKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        public async Task<Weather> Execute(string parameters)
        {
           // Parameters = parameters;

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
