using System.Collections.Generic;
using Thalia.Services.Photos.Api500px.Intefaces;

namespace Thalia.Services.Photos.Api500px.Models
{
    public class PhotoFilter : IPhotoFilter
    {
        #region Constructor
        public PhotoFilter()
        {
            Feature = Feature.FreshToday;
            Sizes = ImageCollection.GetAllSizeIds();
        }

        public PhotoFilter(long userId)
        {
            Feature = Feature.User;
            Sizes = ImageCollection.GetAllSizeIds();
            UserId = userId;
        }

        public PhotoFilter(Feature feature, Categories exclude)
        {
            Categories = exclude;
            Feature = feature;
            Sizes = ImageCollection.GetAllSizeIds();
        }
        #endregion
        
        #region Public Properties
        public Categories Categories { get; set; }
        public Feature Feature { get; set; }
        public FilterMode FilterMode { get; set; }
        public List<int> Sizes { get; set; }
        public Sort Sort { get; set; }
        public SortDirection SortDirection { get; set; }
        public long UserId { get; set; }
        #endregion

        #region Public Properties
        public override string ToString()
        {
            //return UrlBuilder.GetPhotos(this);
            return null;
        }
        #endregion
    }
}
