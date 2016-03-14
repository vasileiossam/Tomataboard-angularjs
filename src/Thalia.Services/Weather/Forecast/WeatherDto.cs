using System.Linq;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace Thalia.Services.Weather.Forecast
{
    [DataContract]
    public class WeatherDto : Response
    {
        [DataMember]
        public float latitude { get; set; }
        [DataMember]
        public float longitude { get; set; }
        [DataMember]
        public string timezone { get; set; }
        [DataMember]
        public int offset { get; set; }
        [DataMember]
        public Currently currently { get; set; }
        [DataMember]
        public Hourly hourly { get; set; }
        [DataMember]
        public Daily daily { get; set; }
        [DataMember]
        public Flags flags { get; set; }
    }

    [DataContract]
    public class Currently
    {
        [DataMember]
        public int time { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public float precipIntensity { get; set; }
        [DataMember]
        public float precipProbability { get; set; }
        [DataMember]
        public string precipType { get; set; }
        [DataMember]
        public float temperature { get; set; }
        [DataMember]
        public float apparentTemperature { get; set; }
        [DataMember]
        public float dewPoint { get; set; }
        [DataMember]
        public float humidity { get; set; }
        [DataMember]
        public float windSpeed { get; set; }
        [DataMember]
        public int windBearing { get; set; }
        [DataMember]
        public float cloudCover { get; set; }
        [DataMember]
        public float pressure { get; set; }
        [DataMember]
        public float ozone { get; set; }
    }

    [DataContract]
    public class Hourly
    {
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public Datum[] data { get; set; }
    }

    [DataContract]
    public class Datum
    {
        [DataMember]
        public int time { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public float precipIntensity { get; set; }
        [DataMember]
        public float precipProbability { get; set; }
        [DataMember]
        public string precipType { get; set; }
        [DataMember]
        public float temperature { get; set; }
        [DataMember]
        public float apparentTemperature { get; set; }
        [DataMember]
        public float dewPoint { get; set; }
        [DataMember]
        public float humidity { get; set; }
        [DataMember]
        public float windSpeed { get; set; }
        [DataMember]
        public int windBearing { get; set; }
        [DataMember]
        public float cloudCover { get; set; }
        [DataMember]
        public float pressure { get; set; }
        [DataMember]
        public float ozone { get; set; }
        [DataMember]
        public float precipAccumulation { get; set; }
    }

    [DataContract]
    public class Daily
    {
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public Datum1[] data { get; set; }
    }

    [DataContract]
    public class Datum1
    {
        [DataMember]
        public int time { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public int sunriseTime { get; set; }
        [DataMember]
        public int sunsetTime { get; set; }
        [DataMember]
        public float moonPhase { get; set; }
        [DataMember]
        public float precipIntensity { get; set; }
        [DataMember]
        public float precipIntensityMax { get; set; }
        [DataMember]
        public int precipIntensityMaxTime { get; set; }
        [DataMember]
        public float precipProbability { get; set; }
        [DataMember]
        public string precipType { get; set; }
        [DataMember]
        public float temperatureMin { get; set; }
        [DataMember]
        public int temperatureMinTime { get; set; }
        [DataMember]
        public float temperatureMax { get; set; }
        [DataMember]
        public int temperatureMaxTime { get; set; }
        [DataMember]
        public float apparentTemperatureMin { get; set; }
        [DataMember]
        public int apparentTemperatureMinTime { get; set; }
        [DataMember]
        public float apparentTemperatureMax { get; set; }
        [DataMember]
        public int apparentTemperatureMaxTime { get; set; }
        [DataMember]
        public float dewPoint { get; set; }
        [DataMember]
        public float humidity { get; set; }
        [DataMember]
        public float windSpeed { get; set; }
        [DataMember]
        public int windBearing { get; set; }
        [DataMember]
        public float cloudCover { get; set; }
        [DataMember]
        public float pressure { get; set; }
        [DataMember]
        public float ozone { get; set; }
        [DataMember]
        public float precipAccumulation { get; set; }
    }

    [DataContract]
    public class Flags
    {
        [DataMember]
        public string[] sources { get; set; }
        [DataMember]
        public string[] isdstations { get; set; }
        [DataMember]
        public string[] madisstations { get; set; }
        [DataMember]
        public string units { get; set; }
    }

}
