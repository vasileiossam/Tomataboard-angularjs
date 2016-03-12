using Thalia.Data.Entities;

namespace Thalia.Services.Quotes
{
    public interface IQuoteRepository
    {
        Quote GetQuoteOfTheDay();
    }
}
