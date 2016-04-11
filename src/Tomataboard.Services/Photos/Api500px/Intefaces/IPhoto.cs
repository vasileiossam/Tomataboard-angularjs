using System;
using Tomataboard.Services.Photos.Api500px.Models;

namespace Tomataboard.Services.Photos.Api500px.Intefaces
{
    public interface IPhoto
    {
        string Apeture { get; set; }
        string Camera { get; set; }
        int Category { get; set; }
        DateTime Created { get; set; }
        string Description { get; set; }
        bool Favorited { get; set; }
        int Favorites { get; set; }
        string FocalLength { get; set; }
        double HighestRating { get; set; }
        long Id { get; set; }
        ImageCollection Images { get; set; }
        string Iso { get; set; }
        double? Latitude { get; set; }
        string Lens { get; set; }
        string Location { get; set; }
        double? Longitude { get; set; }
        bool Liked { get; set; }
        int Likes { get; set; }
        string Name { get; set; }
        double Rating { get; set; }
        string ShutterSpeed { get; set; }
        DateTime? Taken { get; set; }
        int Views { get; set; }
        User User { get; set; }
    }
}