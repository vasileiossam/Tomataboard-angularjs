using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tomataboard.Data.Entities
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public string Wording { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
    }
}
