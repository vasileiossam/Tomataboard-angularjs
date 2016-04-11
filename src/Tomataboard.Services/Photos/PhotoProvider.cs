using System.Collections.Generic;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Photos.Api500px;
using Tomataboard.Services.Photos.Flickr;
using Tomataboard.Services.Cache;

namespace Tomataboard.Services.Photos
{
    public class PhotoProvider : Provider<List<Photo>>, IPhotoProvider
    {
        public PhotoProvider(ILogger<PhotoProvider> logger, ICacheRepository<List<Photo>> cacheRepository,
            // ReSharper disable once InconsistentNaming
            IApi500px api500px,
            IFlickrService flickrService) 
            : base(logger, cacheRepository)
        {
            _operations.Add(api500px);
            _operations.Add(flickrService);
        }
    }
}
