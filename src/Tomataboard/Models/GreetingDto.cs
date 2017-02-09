using System;

namespace Tomataboard.Models
{
    public class GreetingDto
    {
        public bool Show { get; set;}
        public string DefaultName { get; set; }
        public string Name { get; set; }
        public bool RandomName { get; set; }
        public string Names { get; set; }
        public string NamesPlaceholder { get; set; }
    }
}
