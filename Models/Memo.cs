using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("request")]
    public class Memo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id {  get; set; }
        public int? projectID { get; set; }
        public string? reqNo { get; set; }
        public string? reqDate { get; set; }
        public string? reqDesc { get; set; }
        public string? reqYear { get; set; }
        public string? attach { get; set; }
        public long? requestor { get; set; }
        public int? showing { get; set; }
        public int? deleted { get; set; }
        public int? deletedBy { get; set; }
        public int? updated { get; set; }
        public int? updatedBy { get; set; }
        public long? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }

        [ForeignKey("createdBy")]
        public Users users { get; set; }
        public Project project { get; set; }

        [ForeignKey("requestor")]
        public Requestor requestors { get; set; }

    }
}
