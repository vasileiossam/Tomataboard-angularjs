using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Thalia.Services.Photos.Flickr
{
	[DataContract]
	public class PagedResponse  
	{
        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "pages")]
        public int Pages { get; set; }

        [DataMember(Name = "perpage")]
        public int PerPage { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "photo")]
        public PhotoDto[] Photos { get; set; }
    }
}
