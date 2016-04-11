using System.Collections.Generic;
using Tomataboard.Services;

namespace Tomataboard.Services.Photos.Flickr
{
    public interface IFlickrService : IServiceOperation<List<Photo>>
    {
    }
}
