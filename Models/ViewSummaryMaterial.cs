using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewSummaryMaterial
    {
        public string? revision { get; set; }
        public int? total { get; set; }
        public int? stock { get; set; }
        public int? belum_pr {  get; set; }
        public int? sudah_pr { get; set; }
        public int? outstanding_pr {get; set; }
        public int? inquiry_harga {  get; set; }
        public int? hps_oe {  get; set; }
        public int? proses_tender { get; set; }
        public int? penetapan_pemenang {  get; set; }
        public int? sudah_po {  get; set; }
        public int? belum_tiba {  get; set; }
        public int? sudah_tiba { get; set; }

    }
}
