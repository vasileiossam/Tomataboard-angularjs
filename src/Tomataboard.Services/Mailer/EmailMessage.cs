using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tomataboard.Services.Mailer
{
    public class EmailMessage
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
    }
}
