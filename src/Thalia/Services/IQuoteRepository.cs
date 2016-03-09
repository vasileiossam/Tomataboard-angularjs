using Thalia.Data.Entities;

namespace Thalia.Services
{
    public interface IQuoteRepository
    {
        Quote GetQuoteOfTheDay();
    }
}
