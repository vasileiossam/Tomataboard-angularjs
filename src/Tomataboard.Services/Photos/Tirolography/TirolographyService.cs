using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tomataboard.Services.Photos.Tirolography
{
    public class TirolographyService : ITirolographyService
    {
        // no quota since we have the tirolography photos as static files
        public Quota Quota => null;

        public TimeSpan? Expiration { get; }

        public const int MaxPhotoNum = 36;

        public TirolographyService()
        {
        }

        public async Task<List<Photo>> Execute(string parameters)
        {
            var list = new List<Photo>();
            for (var i = 1; i <= MaxPhotoNum; i++)
            {
                var num = i.ToString("D3");

                var photo = new Photo()
                {
                    Service = "Tirolography",
                    Name = "Tirolography",
                    AuthorName = "Dimitrios Barbatsoulis",
                    //AuthorProfilePage = item.GetAuthorProfilePage(),
                    PhotoPage = "http://www.tirolography.com",
                    Url = $"images/tirolography/{num}.jpg"
                };
                list.Add(photo);
            }
            return list;
        }
    }
}