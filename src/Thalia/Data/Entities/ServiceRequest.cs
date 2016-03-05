using System;
using Microsoft.Data.Entity.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thalia.Data.Entities
{
    [Table("ServiceRequests")]
    public class ServiceRequest
    {
        [Key]
        public long Id { get; set; }
        public string Api { get; set; }
        public string Operation { get; set; }
        public string Ip { get; set; }
        public DateTime Created { get; set; }
        public string Response { get; set; }
    }
}
