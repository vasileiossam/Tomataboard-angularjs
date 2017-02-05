using System;
using System.Collections.Generic;
using System.Linq;

namespace Tomataboard.Services.Greetings
{
    [Obsolete("Use javascript instead")]
    public class GreetingsService : IGreetingsService
    {
        private class Greeting
        {
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
            public string Text { get; set; }
        }

        private readonly List<Greeting> _greetings = new List<Greeting>
        {
            new Greeting {Start = new TimeSpan(5, 0, 0), End = new TimeSpan(12, 0, 0), Text = "Good morning"},
            new Greeting {Start = new TimeSpan(12, 1, 0), End = new TimeSpan(17, 0, 0), Text = "Good afternoon"},
            new Greeting {Start = new TimeSpan(17, 1, 0), End = new TimeSpan(24, 0, 0), Text = "Good evening"}
        };

        public string GetGreeting(long milliseconds)
        {
            var time = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(milliseconds)).TimeOfDay;
            var greeting = _greetings.FirstOrDefault(x => (time >= x.Start) && (time <= x.End)) ?? _greetings[0];
            return greeting.Text;
        }
    }
}