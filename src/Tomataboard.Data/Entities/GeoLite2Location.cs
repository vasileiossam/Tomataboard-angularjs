using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tomataboard.Data.Entities
{
    [Table("GeoLite2Locations")]
    public class GeoLite2Location
    {
        [Key]
        [Column("geoname_id")]
        public int GeonameId { get; set; }

        [Column("locale_code")]
        public string LocaleCode { get; set; }

        [Column("continent_code")]
        public string ContinentCode { get; set; }

        [Column("continent_name")]
        public string ContinentName { get; set; }

        [Column("country_iso_code")]
        public string CountryIsoCode { get; set; }

        [Column("country_name")]
        public string CountryName { get; set; }

        [Column("subdivision_1_iso_code")]
        public string Subdivision1IsoCode { get; set; }

        [Column("subdivision_1_name")]
        public string Subdivision1Name { get; set; }

        [Column("subdivision_2_iso_code")]
        public string Subdivision2IsoCode { get; set; }

        [Column("subdivision_2_name")]
        public string Subdivision2Name { get; set; }

        [Column("city_name")]
        public string CityName { get; set; }

        [Column("metro_code")]
        public string MetroCode { get; set; }

        [Column("time_zone")]
        public string TimeZone { get; set; }
    }
}