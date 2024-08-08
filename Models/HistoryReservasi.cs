using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("history_reservasi")]
    public class HistoryReservasi
    {
        public long id {  get; set; }
        public string? revision { get; set; }
        public int? total_item { get; set; }
        public int? stock { get; set; }
        public int? create_pr { get; set; }
        public int? outstanding_pr { get; set; }
        public int? inquiry_harga { get; set; }
        public int? hps_oe { get; set; }
        public int? proses_tender { get; set; }
        public int? penetapan_pemenang { get; set; }
        public int? tunggu_onsite { get; set; }
        public int? onsite { get; set; }
        public DateTime created_date { get; set; }
    }
}
