using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thalia.Data.Entities
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]
        public long Id { get; set; }
        public string Quote { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
    }
}
