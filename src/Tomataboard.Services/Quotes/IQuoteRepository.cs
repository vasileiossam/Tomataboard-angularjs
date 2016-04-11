using System.Collections.Generic;
using Tomataboard.Data.Entities;

namespace Tomataboard.Services.Quotes
{
    public interface IQuoteRepository
    {
        Quote GetQuoteOfTheDay();
        List<Quote> GetQuotes(string tags);
    }
}
