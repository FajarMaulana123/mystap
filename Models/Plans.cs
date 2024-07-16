using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Plans
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? plans { get; set; }
        public string? planDesc { get; set; }
        public long? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }
        public int? deleted { get; set; }


        [ForeignKey("createdBy")]
        public Users users { get; set; }
    }
}
