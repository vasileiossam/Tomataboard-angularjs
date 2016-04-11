using System.Collections.Generic;
using Tomataboard.Services.Photos.Api500px.Models;

namespace Tomataboard.Services.Photos.Api500px.Intefaces
{
    public interface IPhotoFilter
    {
        #region Properties
        Categories Categories { get; set; }
        Feature Feature { get; set; }
        FilterMode FilterMode { get; set; }
        List<int> Sizes { get; set; }
        Sort Sort { get; set; }
        SortDirection SortDirection { get; set; }
        long UserId { get; set; }
        #endregion

        #region Methods
        string ToString();
        #endregion
    }
}