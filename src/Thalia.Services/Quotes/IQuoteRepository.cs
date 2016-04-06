using System.Collections.Generic;
using Thalia.Data.Entities;

namespace Thalia.Services.Quotes
{
    public interface IQuoteRepository
    {
        Quote GetQuoteOfTheDay();
        List<Quote> GetQuotes(string tags);
    }
}
