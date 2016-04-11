namespace Tomataboard.Services.Photos.Api500px.Intefaces
{
    public interface IPagedResponse
    {
        int CurrentPage { get; set; }
        int TotalItems { get; set; }
        int TotalPages { get; set; }
    }
}