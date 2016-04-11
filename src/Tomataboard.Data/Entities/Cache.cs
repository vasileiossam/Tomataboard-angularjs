using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tomataboard.Data.Entities
{
    [Table("Cache")]
    public class Cache
    {
        [Key]
        public long Id { get; set; }
        public string Service { get; set; }
        public string Operation { get; set; }
        public string Params { get; set; }
        public string Result { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public bool HasErrored { get; set; }
    }
}
