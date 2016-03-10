using System.Collections.Generic;
using Thalia.Services.Photos.Api500px.Models;

namespace Thalia.Services.Photos.Api500px.Intefaces
{
    public interface IPhotoFilter
    {
        #region Properties
        CategoriesDto Categories { get; set; }
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