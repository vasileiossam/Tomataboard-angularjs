using System.Runtime.Serialization;

namespace Thalia.Services.Locations.IpGeolocation
{
    [DataContract]
    public class LocationDto
    {
        [DataMember(Name = "as")]
        public string As { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }
        [DataMember(Name = "isp")]
        public string Isp { get; set; }
        [DataMember(Name = "lat")]
        public string Latitude { get; set; }
        [DataMember(Name = "lon")]
        public string Longitude { get; set; }
        [DataMember(Name = "org")]
        public string Org { get; set; }
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "regionName")]
        public string RegionName { get; set; }
        [DataMember(Name = "timezone")]
        public string TimeZone { get; set; }
        [DataMember(Name = "zip")]
        public string PostCode { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "query")]
        public string Ip { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
