using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thalia.Data.Entities
{
    [Table("AccessTokens")]
    public class AccessToken
    {
        [Key]
        public int Id { get; set; }
        public string Service { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public string SessionHandle { get; set; }
    }
}
