using System.Runtime.Serialization;

namespace Tomataboard.Services.Photos.Flickr
{
    [DataContract]
    public class PhotoDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "secret")]
        public string Secret { get; set; }

        [DataMember(Name = "server")]
        public string Server { get; set; }

        [DataMember(Name = "farm")]
        public int Farm { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "ispublic")]
        public int IsPublic { get; set; }

        [DataMember(Name = "isfriend")]
        public int IsFriend { get; set; }

        [DataMember(Name = "isfamily")]
        public int IsFamily { get; set; }

        [DataMember(Name = "ownername")]
        public string AuthorName { get; set; }

        [DataMember(Name = "dateupload")]
        public string DateUpload { get; set; }

        [DataMember(Name = "views")]
        public int Views { get; set; }

        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }

        // l large
        [DataMember(Name = "url_l")]
        public string UrlL { get; set; }

        [DataMember(Name = "height_l")]
        public int HeightL { get; set; }

        [DataMember(Name = "width_l")]
        public int WidthL { get; set; }

        //h large 1600, 1600 on longest side
        [DataMember(Name = "url_h")]
        public string UrlH { get; set; }

        [DataMember(Name = "height_h")]
        public int HeightH { get; set; }

        [DataMember(Name = "width_h")]
        public int WidthH { get; set; }

        //k large 2048, 2048 on longest side
        [DataMember(Name = "url_k")]
        public string UrlK { get; set; }

        [DataMember(Name = "height_k")]
        public int HeightK { get; set; }

        [DataMember(Name = "width_k")]
        public int WidthK { get; set; }

        #region Public Methods

        public string GetUrl()
        {
            // https://www.flickr.com/services/api/misc.urls.html
            if (!string.IsNullOrEmpty(UrlK)) return UrlK;
            return !string.IsNullOrEmpty(UrlH) ? UrlH : UrlL;
        }

        public string GetAuthorProfilePage()
        {
            // https://www.flickr.com/services/api/misc.urls.html
            return $"https://www.flickr.com/people/{Owner}/";
        }

        public string GetPhotoPage()
        {
            return $"https://www.flickr.com/photos/{Owner}/{Id}";
        }

        #endregion Public Methods
    }
}