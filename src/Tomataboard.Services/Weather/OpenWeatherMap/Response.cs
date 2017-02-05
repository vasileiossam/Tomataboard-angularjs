using System.Runtime.Serialization;

namespace Tomataboard.Services.Weather.OpenWeatherMap
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "cod")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}