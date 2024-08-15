using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Joblist
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? jobNo { get; set; }
        public string? projectNo { get; set; } 
        public long? unitCode { get; set; }
        public string? eqTagNo { get; set; }
        public long? status { get; set; }
        public string? criteriaMI { get; set; }
        public string? criteriaPI { get; set; }
        public string? criteriaOPT { get; set; }
        public string? userSection { get; set; }
        public int? deleted { get; set; }
        public long? projectID { get; set; }
        public string? remarks { get; set; }
        public string? keterangan { get; set; }
        public long? createBy { get; set; }
        public long? deletedBy { get; set; }
        public long? updateBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }


        [ForeignKey("createBy")]
        public Users users { get; set; }

        public Project project { get; set; }

        [ForeignKey("unitCode")]
        public Unit unit { get; set; }

    }
}
