using System;

namespace Thalia.Services
{
    public class Quota
    {
        public int Requests { get; set; }
        public TimeSpan Time { get; set; }
    }
}
