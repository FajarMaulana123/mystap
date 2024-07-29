using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("sow_group")]
    public class SowGroup
    {
       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long id {  get; set; }
        public string? groups_ {  get; set; }
        public string? inisial { get; set; }
        public string? sub_group { get; set; }
        public int? urut { get; set; }
        public int? deleted { get; set; }

       
    }
}
