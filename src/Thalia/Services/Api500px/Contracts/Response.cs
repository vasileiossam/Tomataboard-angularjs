using System.Net;
using System.Runtime.Serialization;

namespace Thalia.Services.Api500.Contracts
{
    [DataContract]
    public class Response
    {
        public string Content { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
