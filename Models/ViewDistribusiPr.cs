using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewDistribusiPr
    {
        public long? id { get; set; }
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? qty_pr { get; set; }
        public string? pg { get; set; }
        public string? reqmt_date { get; set; }
        public string? order { get; set; }
        public string? material { get; set; }
        public string? material_description { get; set; }
        public string? lld { get; set; }
        public string? status_pr { get; set; }
        public string? doc_pr { get; set; }
        public int? dt_ta { get; set; }
        public int? dt_iv { get; set; }
        public int? dt_purch { get; set; }
        public string? status_pengadaan { get; set; }
        public string? buyer { get; set; }
        public string? keterangan { get; set; }
        public string? po { get; set; }
        public string? po_item { get; set; }
        public string? qty_po { get; set; }
        public string? equipment { get; set; }

    }
}
