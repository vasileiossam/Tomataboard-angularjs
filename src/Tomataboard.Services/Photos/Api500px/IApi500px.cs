using System.Collections.Generic;
using Tomataboard.Services;

namespace Tomataboard.Services.Photos.Api500px
{
    public interface IApi500px : IOauthService, IServiceOperation<List<Photo>>
    {

    }
}
