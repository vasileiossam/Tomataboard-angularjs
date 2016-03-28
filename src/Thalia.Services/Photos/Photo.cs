using System;

namespace Thalia.Services.Photos
{
    public class Photo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Created { get; set; }
        public string AuthorName { get; set; }
        public string AuthorCountry { get; set; }
        public long AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string AuthorUrl { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Favorites { get; set; }
        public int Likes { get; set; }
        public double Rating { get; set; }
        public int Views { get; set; }
        public string Url { get; set; }
        public string Service { get; set; }
    }
}
