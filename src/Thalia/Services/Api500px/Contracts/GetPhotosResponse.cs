using System.Collections.Generic;
using System.Runtime.Serialization;
using Thalia.Services.Api500.Models;

namespace Thalia.Services.Api500.Contracts
{
    [DataContract]
    public class GetPhotosResponse: PagedResponse
    {
        [DataMember(Name = "photos")]
        public List<Photo> Photos { get; set; }
    }
}
