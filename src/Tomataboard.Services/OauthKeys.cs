namespace Tomataboard.Services
{
    public class OauthKeys
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class Api500pxKeys : OauthKeys { }

    public class FlickrServiceKeys : OauthKeys { }

    public class YahooWeatherServiceKeys : OauthKeys { }
}