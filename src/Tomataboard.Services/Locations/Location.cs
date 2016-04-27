using System;

namespace Tomataboard.Services.Locations
{
    public class Location
    {
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public bool UsesFahrenheit
        {
            get
            {
                if (string.IsNullOrEmpty(CountryCode))
                {
                    return false;
                }
                var code = CountryCode.Trim();
                return (code.IndexOf("US", StringComparison.OrdinalIgnoreCase) == 0);
            }
        }
    }
}
