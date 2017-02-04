using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tomataboard.Models.EmailViewModels
{
    public class MessageViewModel
    {
        public string Title = "Tomataboard";
        public string CallbackUrl { get; set; }
    }
}
