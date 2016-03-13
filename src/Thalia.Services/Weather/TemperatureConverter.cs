namespace Thalia.Services.Weather
{
    public static class TemperatureConverter
    {
        public static double CelsiusToFahrenheit(double c)
        {
            return ((9.0 / 5.0) * c) + 32;
        }

        public static double FahrenheitToCelsius(double f)
        {
            return (5.0 / 9.0) * (f - 32);
        }

        public static double KelvinToCelsius(double k)
        {
            return k - 273.15;
        }

        public static double KelvinToFahrenheit(double k)
        {
            return k * (9.0 / 5.0) - 459.67;
        }
    }
}
