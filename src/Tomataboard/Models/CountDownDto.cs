using System;

namespace Tomataboard.Models
{
    public class CountDownDto
    {
        public string EventPlaceholder { get; set;}
        public string EventDescription { get; set; }
        public DateTime EndDate { get; set; }
        public bool Started { get; set; }
    }
}
