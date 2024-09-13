using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("paket_joblist")]
    public class PaketJoblist
    {
        [Key]
        public long id_paket {  get; set; }
        public long? projectID { get; set; }
        public long? no_paket { get; set; }
        public string? tag_no { get; set; }
        public string? no_memo { get; set; }
        public string? disiplin { get; set; }
        public int? additional { get; set; }
        public DateTime? created_date { get; set; }
    }
}
