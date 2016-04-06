using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Thalia.Services.Cache;
using Thalia.Services.Photos.Api500px;
using Thalia.Services.Photos.Flickr;

namespace Thalia.Services.Photos
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
