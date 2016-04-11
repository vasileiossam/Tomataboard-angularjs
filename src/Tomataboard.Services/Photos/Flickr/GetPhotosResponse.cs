using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tomataboard.Services.Photos.Flickr
{
    [DataContract]
    public class GetPhotosResponse: Response
    {
        [DataMember(Name = "photos")]
        public PagedResponse Page { get; set; }
    }
}
