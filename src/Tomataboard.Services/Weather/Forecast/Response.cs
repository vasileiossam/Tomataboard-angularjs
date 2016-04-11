using System.Runtime.Serialization;

namespace Tomataboard.Services.Weather.Forecast
{
    [DataContract]
    public class Response
    {

        [DataMember(Name = "code")]
        public int? Code { get; set; }

        [DataMember(Name = "error")]
        public string Message { get; set; }
    }
}