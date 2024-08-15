using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("disiplin")]
    public class Disiplin
    {
        public long id { get; set; }
        public string disiplins {  get; set; }
        public int? deleted { get; set; }
        public string? inisial { get; set; }

    }
}
