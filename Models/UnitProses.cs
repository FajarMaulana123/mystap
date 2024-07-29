using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("unit_proses")]
    public class UnitProses
    {
        public long id { get; set; }
        public string? inisial { get; set; }
        public string? description { get; set; }
        public int? deleted { get; set; }
        public long? createdBy { get; set; }
        [ForeignKey("createdBy")]
        public Users users { get; set; }
    }
}
