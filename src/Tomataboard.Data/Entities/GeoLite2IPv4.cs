using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tomataboard.Data.Entities
{
    [Table("GeoLite2IPv4")]
    public class GeoLite2IPv4
    { 

        [Column("network")]
        [Key]
        public string Network { get; set; }

        [Column("geoname_id")]
        public int? GeonameId { get; set; }

        [Column("registered_country_geoname_id")]
        public int? RegisteredCountryGeonameId { get; set; }

        [Column("represented_country_geoname_id")]
        public int? RepresentedCountryGeonameId { get; set; }

        [Column("is_anonymous_proxy")]
        public int? IsAnonymousProxy { get; set; }

        [Column("is_satellite_provider")]
        public int? IsSatelliteProvider { get; set; }

        [Column("postal_code")]
        public string PostalCode { get; set; }

        [Column("latitude")]
        public decimal? Latitude { get; set; }

        [Column("longitude")]
        public decimal? Longitude { get; set; }

        [Column("startIpNum")]
        public long StartIpNum { get; set; }

        [Column("endIpNum")]
        public long EndIpNum { get; set; }
    }
}
