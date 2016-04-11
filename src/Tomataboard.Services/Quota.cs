using System;

namespace Tomataboard.Services
{
    public class Quota
    {
        public int Requests { get; set; }
        public TimeSpan Time { get; set; }
    }
}
