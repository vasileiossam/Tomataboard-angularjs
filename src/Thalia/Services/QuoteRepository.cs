using Thalia.Data;
using Thalia.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Thalia.Services
{
    #region tags
    //age
    //alone
    //amazing
    //anger
    //anniversary
    //architecture
    //art
    //attitude
    //beauty
    //best
    //birthday
    //business
    //car
    //change
    //christmas
    //communication
    //computers
    //cool
    //courage
    //dad
    //dating
    //death
    //design
    //diet
    //dreams
    //easter
    //education
    //environmental
    //equality
    //experience
    //failure
    //faith
    //family
    //famous
    //fathersday
    //fear
    //finance
    //fitness
    //food
    //forgiveness
    //freedom
    //friendship
    //funny
    //future
    //gardening
    //god
    //good
    //government
    //graduation
    //great
    //happiness
    //health
    //history
    //home
    //hope
    //humor
    //imagination
    //inspirational
    //intelligence
    //jealousy
    //knowledge
    //leadership
    //learning
    //legal
    //life
    //love
    //marriage
    //medical
    //memorialday
    //men
    //mom
    //money
    //morning
    //mothersday
    //motivational
    //movies
    //movingon
    //music
    //nature
    //newyears
    //parenting
    //patience
    //patriotism
    //peace
    //pet
    //poetry
    //politics
    //positive
    //power
    //relationship
    //religion
    //respect
    //romantic
    //sad
    //saintpatricksday
    //science
    //smile
    //society
    //sports
    //strength
    //success
    //sympathy
    //teacher
    //technology
    //teen
    //thankful
    //thanksgiving
    //time
    //travel
    //trust
    //truth
    //valentinesday
    //war
    //wedding
    //wisdom
    //women
    //work
    #endregion

    public class QuoteRepository : IQuoteRepository
    {
        private ThaliaContext _context;
        private Random _rnd = new Random();

        public QuoteRepository(ThaliaContext context)
        {
            _context = context;
        }

        private List<Quote> GetRandomQuotes(string tags)
        {
            var quotes = from quote in GetQuotes(tags)
                         select new { Quote = quote, Rnd = _rnd.Next() };

            var randomQuotes = new List<Quote>();
            foreach(var quote in quotes.OrderBy(x => x.Rnd)) randomQuotes.Add(quote.Quote);

            return randomQuotes;
        }

        private List<Quote> GetQuotes(string tags)
        {
            var quotes = new List<Quote>();
            var tagArray = tags.Split(',');
            foreach(var tag in tagArray)
            {
                var q = _context.Quotes.Where(x => x.Tag == tag).ToList();
                quotes.AddRange(q);
            }
            return quotes;
        }

        public Quote GetQuoteOfTheDay()
        {
            var quote = GetRandomQuotes("inspirational,intelligence,motivational").FirstOrDefault();
             return quote;
        }
    }
}
