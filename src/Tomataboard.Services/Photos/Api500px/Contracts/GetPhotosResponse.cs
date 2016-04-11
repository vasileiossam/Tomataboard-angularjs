using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tomataboard.Services.Photos.Api500px.Contracts
{
    [DataContract]
    public class GetPhotosResponse: PagedResponse
    {
        [DataMember(Name = "photos")]
        public List<Models.PhotoDto> Photos { get; set; }
    }
}
