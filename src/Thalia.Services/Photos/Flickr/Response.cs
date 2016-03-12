using System.Net;
using System.Runtime.Serialization;

namespace Thalia.Services.Photos.Flickr
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "stat")]
        public string Stat { get; set; }

        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}