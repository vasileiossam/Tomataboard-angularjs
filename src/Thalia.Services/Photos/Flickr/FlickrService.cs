using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Random _rnd = new Random();
        private const string ServiceUrl = "https://api.flickr.com/services/rest/";
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
                var year = GetRandomYear();
                var page = GetRandomPageNumber(await GetTotalPages(parameters, year));
                var queryString = GetQueryString(parameters, page, year);

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(ServiceUrl + "?" + queryString, UriKind.Absolute));
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

        #region private

        private int GetRandomYear()
        {
            return _rnd.Next(2013, DateTime.Now.Year + 1);
        }

        private int GetRandomPageNumber(int totalPages)
        {
            if (totalPages == 0) return 1;
            
            // to compensate for the 4000 results limit which is actually 2000 (per_page = 100)
            if (totalPages > 20) totalPages = 20;

            return _rnd.Next(1, totalPages + 1);
        }

        /// <summary>
        /// https://www.flickr.com/services/api/flickr.photos.search.html
        /// https://www.flickr.com/services/api/explore/flickr.photos.search
        /// 
        /// from the documentation page: flickr.photos.search will return at most the first 4,000 results 
        ///                              for any given search query. If this is an issue, we recommend 
        ///                              trying a more specific query. 
        ///
        /// This method is very buggy. The limit is not 4000 but 2000 results. 
        /// Pages after the limit can bring results from Page 1 or return empty.
        /// Also, pages in the range 1 - 20 might returned empty in successive calls.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="page"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private string GetQueryString(string tags, int page, int year)
        {
            var queryParams = new Dictionary<string, string>
            {
                    // no OAuth authentication is required for this method.
                    { "method", "flickr.photos.search" },
                    { "api_key", _keys.Value.ConsumerKey },

                    { "format", "json" },
                    { "nojsoncallback", "1" },
                    { "content_type", "1" }, // 1 = photos only
                    { "media", "photos" },
                    { "safe_search", "1" }, // 1 = safe
                    { "extras", "date_upload,owner_name,geo,o_dims,views,url_l,url_h,url_k" },
                    
                    // possible values are: date-posted-asc, date-posted-desc, date-taken-asc, date-taken-desc, interestingness-desc, interestingness-asc, and relevance.
                    { "sort", "interestingness-desc" },
                    
                    // use the upload date to build a more specific query
                    //{ "min_upload_date", new DateTime(year, 1, 1).GetUnixTimestamp().ToString() },
                    //{ "max_upload_date", new DateTime(year, 12, 31).GetUnixTimestamp().ToString() },
                    { "min_upload_date", new DateTime(year, 1, 1).ToString("yyyy-MM-dd")},
                    { "max_upload_date", new DateTime(year, 12, 31).ToString("yyyy-MM-dd")},
                    
                    { "tags", tags },
                    { "tag_mode", "any" },
                    { "page", page.ToString() },
                    { "per_page", "100" }
                };

            return string.Join("&", queryParams.Select(key => key.Key + "=" + Uri.EscapeDataString(key.Value)));
        }

        private async Task<int> GetTotalPages(string tags, int year)
        {
            var queryString = GetQueryString(tags, 1, year);

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri(ServiceUrl + "?" + queryString, UriKind.Absolute));
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var photosResponse = JsonConvert.DeserializeObject<GetPhotosResponse>(content);
                    if (photosResponse == null) return 0;

                    if (photosResponse.Stat == "ok")
                    {
                        return photosResponse.Page.Pages;
                    }
                }
            }

            return 0;
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
                    //Id = item.Id,
                    Name = item.Title.LimitTo(PhotoConstants.MaxNameLength),
                    //Created = item.DateUpload.GetDateTimeFromUnixString(),
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

            return photos.Count == 0 ? null : photos;
        }
        #endregion
    }
}
