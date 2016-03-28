using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Thalia.Services.Extensions;

namespace Thalia.Services.Photos.Flickr
{
    /// <summary>
    /// https://www.flickr.com/services/api/
    /// https://www.flickr.com/services/developer/api/
    /// </summary>
    public class FlickrService : IFlickrService
    {
        #region Private Fields
        private readonly IOptions<FlickrServiceKeys> _keys;
        private readonly ILogger<FlickrService> _logger;
        #endregion

        // 3600 per hour
        public Quota Quota => new Quota() { Requests = 3600, Time = TimeSpan.FromHours(1) };
        public TimeSpan? Expiration => TimeSpan.FromHours(6);
        
        #region Constructors
        public FlickrService(ILogger<FlickrService> logger, IOptions<FlickrServiceKeys> keys)
        {
            _logger = logger;
            _keys = keys;
        }
        #endregion

        /// <summary>
        /// https://www.flickr.com/services/api/flickr.photos.search.html
        /// https://www.flickr.com/services/api/explore/flickr.photos.search
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<Photo>> Execute(string parameters)
        {
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"method", "flickr.photos.search"},
                    {"api_key", _keys.Value.ConsumerKey},
                    {"format", "json" },
                    {"nojsoncallback", "1" },
                    // The order in which to sort returned photos. Deafults to date-posted-desc 
                    // (unless you are doing a radial geo query, in which case the default sorting is 
                    // by ascending distance from the point specified). 
                    // The possible values are: date-posted-asc, date-posted-desc, date-taken-asc, 
                    // date-taken-desc, interestingness-desc, interestingness-asc, and relevance.
                    {"sort", "interestingness-desc"},
                    {"content_type", "1"}, // 1 = photos only
                    {"media", "photos"},
                    //{"min_upload_date", new DateTime(2013, 1, 1).GetUnixTimestamp().ToString()},
                    {"tags", parameters},
                    {"extras", "date_upload,owner_name,geo,o_dims,views,url_l,url_h,url_k" }
                };
                var queryString = string.Join("&", queryParams.Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value)));
                var url = "https://api.flickr.com/services/rest/";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url + "?" + queryString, UriKind.Absolute));
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var photosResponse = JsonConvert.DeserializeObject<GetPhotosResponse>(content);
                        if (photosResponse == null) return null;

                        if (photosResponse.Stat == "ok")
                        {
                            return GetResult(content);
                        }

                        _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Stat: {photosResponse.Stat}, Code: {photosResponse.Code}, Message: {photosResponse.Message}");
                    }

                    _logger.LogError($"{GetType().Name}: Cannot get photos for '{parameters}'. Status code: {response.StatusCode} Content: {content}");
                }
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
            if (photosResponse?.Page == null) return null;

            var photos = new List<Photo>();
            foreach (var item in photosResponse.Page.Photos)
            {
                var url = item.GetPhotoUrl();
                if (string.IsNullOrEmpty(url)) continue;
                
                var photo = new Photo()
                {
                       
                    Id = item.Id,
                    Name = item.Title,
                    Created = item.DateUpload.GetDateTimeFromUnixString(),
                    AuthorName = item.AuthorName,
                    //Location = item.Location,
                    //Latitude = item.Latitude,
                    //Longitude = item.Longitude,
                    //Favorites = item.Favorites,
                    //Likes = item.Likes,
                    //Rating = item.Rating,
                    Views = item.Views,
                    Url = url,
                    AuthorUrl = item.GetAuthorUrl(),
                    Service = "Flickr"
                };

                photos.Add(photo);
            }
            return photos;
        }
    }
}
