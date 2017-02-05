using System;

namespace Tomataboard.Services.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetUnixTimestamp(this DateTime date)
        {
            return (int)(date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static DateTime? GetDateTimeFromUnixString(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            long seconds = 0;
            if (long.TryParse(str, out seconds))
            {
                return date.AddSeconds(seconds).ToLocalTime();
            }

            return null;
        }
    }
}