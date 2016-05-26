using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Tomataboard.Services.Photos.Api500px;
using Tomataboard.Services.Photos.Flickr;
using Tomataboard.Services.Cache;
using Tomataboard.Services.Photos.Tirolography;

namespace Tomataboard.Services.Photos
{
    public class PhotoProvider : Provider<List<Photo>>, IPhotoProvider
    {
        public PhotoProvider(ILogger<PhotoProvider> logger, ICacheRepository<List<Photo>> cacheRepository,
            // ReSharper disable once InconsistentNaming
            IApi500px api500px,
            IFlickrService flickrService,
            ITirolographyService tirolographyService) 
            : base(logger, cacheRepository)
        {
            _operations.Add(api500px);
            _operations.Add(flickrService);
            _operations.Add(tirolographyService);
        }
    }
}
