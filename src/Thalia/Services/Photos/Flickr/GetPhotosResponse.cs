using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Thalia.Services.Photos.Flickr
{
    [DataContract]
    public class GetPhotosResponse: Response
    {
        [DataMember(Name = "photos")]
        public PagedResponse Page { get; set; }
    }
}
