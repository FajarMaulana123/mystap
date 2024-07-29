using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("requestor")]
    public class Requestor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? fungsi { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int? deleted { get; set; }
        public long? deletedBy { get; set; }
        public int? updated { get; set; }
        public long? updatedBy { get; set; }
        public long? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }

        public Users users { get; set; }

    }
}
