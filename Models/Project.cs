using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? projectNo { get; set; }
        public string? description { get; set; }
        public string? revision { get; set; }
        public string? month { get; set; }
        public string? year { get; set; }
        public int? active { get; set; }
        public int? deleted { get; set; }
        public int? updated { get; set; }
        [Column(TypeName = "date")]
        public DateTime? tglTA { get; set; }
        [Column(TypeName = "date")]
        public DateTime? tglSelesaiTA { get; set; }
        public long? deletedBy { get; set; }
        public long? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }
        public long? plansID { get; set; }
        public int? durasiTABrick { get; set; }
        public string? finalDate { get; set; }
        public string? additional1Date { get; set; }
        public string? additional2Date { get; set; }
        public string? taoh { get; set; }

        [ForeignKey("createdBy")]
        public Users? users { get; set; }
    }
}
