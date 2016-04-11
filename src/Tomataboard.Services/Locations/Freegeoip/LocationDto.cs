using System.Runtime.Serialization;

namespace Tomataboard.Services.Locations.Freegeoip
{
    [DataContract]
    public class LocationDto
    {
        [DataMember(Name = "ip")]
        public string Ip { get; set; }
        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }
        [DataMember(Name = "country_name")]
        public string CountryName { get; set; }
        [DataMember(Name = "region_code")]
        public string RegionCode { get; set; }
        [DataMember(Name = "region_name")]
        public string RegionName { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "zip_code")]
        public string PostCode { get; set; }
        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }
        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }
        [DataMember(Name = "metro_code")]
        public string MetroCode { get; set; }
    }
}
