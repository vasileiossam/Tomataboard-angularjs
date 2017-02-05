using System.Collections.Generic;

namespace Tomataboard.Services.Photos.Api500px
{
    public interface IApi500px : IOauthService, IServiceOperation<List<Photo>>
    {
    }
}