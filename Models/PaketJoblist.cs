using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class PaketJoblist
    {
        public long id_paket {  get; set; }
        public long? projectID { get; set; }
        public long? noPaket { get; set; }
        public string? tag_no { get; set; }
        public string? no_memo { get; set; }
        public string? disiplin { get; set; }
        public int? additional { get; set; }
        public DateTime? created_date { get; set; }
    }
}
