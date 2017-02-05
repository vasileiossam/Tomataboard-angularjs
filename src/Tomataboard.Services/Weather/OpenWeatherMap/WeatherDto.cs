using System.Linq;
using System.Runtime.Serialization;

namespace Tomataboard.Services.Weather.OpenWeatherMap
{
    [DataContract]
    public class WeatherDto : Response
    {
        [DataMember(Name = "weather")]
        public Weather[] Weather { get; set; }

        [DataMember(Name = "main")]
        public Main Main { get; set; }

        [DataMember(Name = "wind")]
        public Wind Wind { get; set; }

        [DataMember(Name = "clouds")]
        public Clouds Clouds { get; set; }

        public string Title
        {
            get
            {
                var weather = Weather.FirstOrDefault();
                return weather == null ? string.Empty : weather.Main;
            }
        }

        public string Description
        {
            get
            {
                var weather = Weather.FirstOrDefault();
                return weather == null ? string.Empty : weather.Description;
            }
        }

        public string IconCode
        {
            get
            {
                var weather = Weather.FirstOrDefault();
                return weather == null ? string.Empty : weather.Icon;
            }
        }
    }

    [DataContract]
    public class Weather
    {
        /// <summary>
        /// Weather condition id
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Group of weather parameters (Rain, Snow, Extreme etc.)
        /// </summary>
        [DataMember(Name = "main")]
        public string Main { get; set; }

        /// <summary>
        /// Weather condition within the group
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Weather icon id
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; }
    }

    [DataContract]
    public class Main
    {
        /// <summary>
        /// Temperature. Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [DataMember(Name = "temp")]
        public float Temperature { get; set; }

        /// <summary>
        /// Humidity, %
        /// </summary>
        [DataMember(Name = "humidity")]
        public int Humidity { get; set; }

        /// <summary>
        /// Minimum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [DataMember(Name = "temp_min")]
        public float MinTemperature { get; set; }

        /// <summary>
        /// Maximum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [DataMember(Name = "temp_max")]
        public float MaxTemperature { get; set; }
    }

    [DataContract]
    public class Wind
    {
        /// <summary>
        ///  Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
        /// </summary>
        [DataMember(Name = "speed")]
        public float Speed { get; set; }

        /// <summary>
        /// Wind direction, degrees (meteorological)
        /// </summary>
        [DataMember(Name = "deg")]
        public float Deg { get; set; }
    }

    /// <summary>
    ///  Cloudiness, %
    /// </summary>
    [DataContract]
    public class Clouds
    {
        [DataMember(Name = "all")]
        public int? All { get; set; }
    }
}